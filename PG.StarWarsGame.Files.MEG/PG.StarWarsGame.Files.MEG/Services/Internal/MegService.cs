// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegService" />
internal sealed class MegService(IServiceProvider services) : ServiceBase(services), IMegService
{
    private IMegBinaryServiceFactory BinaryServiceFactory { get; } = services.GetRequiredService<IMegBinaryServiceFactory>();

    public void CreateMegArchive(
        FileSystemStream fileStream,
        MegFileVersion fileVersion,
        MegEncryptionData? encryptionData,
        IEnumerable<MegDataEntryBuilderInfo> builderInformation)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        if (builderInformation == null)
            throw new ArgumentNullException(nameof(builderInformation));

        var constructionArchive = BinaryServiceFactory.GetConstructionBuilder(fileVersion)
            .BuildConstructingMegArchive(builderInformation);

        if (constructionArchive.Encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        if (constructionArchive.Encrypted)
        {
            if (encryptionData is null)
                throw new NotSupportedException("Creating an encrypted MEG archive requires encryption key.");
            if (fileVersion == MegFileVersion.V3)
                throw new NotSupportedException("Creating an encrypted MEG archive requires the MEG version to be V3.");
        }


        var metadata = BinaryServiceFactory.GetConverter(constructionArchive.MegVersion)
            .ModelToBinary(constructionArchive.Archive);

        metadata.WriteTo(fileStream);

        long dataBytesWritten = metadata.Size;

        foreach (var file in constructionArchive)
        {
            using var dataStream = file.Location.GetDataStream();

            // TODO: Test in encryption case
            if (dataStream.Length != file.DataEntry.Location.Size)
                throw new InvalidOperationException(
                    $"Actual data entry size '{dataStream.Length}' does not match expected value: {file.DataEntry.Location.Size}");

            if (fileStream.Position != file.DataEntry.Location.Offset)
                throw new InvalidOperationException(
                    $"Actual file position '{fileStream.Position}' does not match expected entry offset: {file.DataEntry.Location.Offset}");

            dataStream.CopyTo(fileStream);

            dataBytesWritten += dataStream.Length;
        }

        Debug.Assert(dataBytesWritten == constructionArchive.ExpectedFileSize);
    }

    public IMegFile Load(string filePath)
    {
        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        var fullPath = FileSystem.Path.GetFullPath(filePath);
        using var fs = FileSystem.FileStream.New(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Load(fs);
    }

    public IMegFile Load(FileSystemStream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        return LoadMegFromFile(stream, stream.Name);
    }

    public IMegDataSource LoadArchive(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        
        if (TryGetFileName(stream, out var fileName))
            return LoadMegFromFile(stream, fileName);

        using var buffer = new MemoryStream(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
        stream.CopyTo(buffer);
        buffer.Position = 0;

        var (_, archive) = ReadArchive(buffer);
        return new InMemoryMeg(archive, buffer.ToArray());
    }

    public IMegDataSource LoadArchive(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        return LoadArchive(data.AsSpan());
    }

    public IMegDataSource LoadArchive(ReadOnlySpan<byte> data)
    {
        return LoadMegFromMemory(data);
    }

    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(file);

        using var fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegFileVersion(fs, out encrypted);
    }

    public MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(stream, out encrypted);
    }

    public MegFileVersion GetMegFileVersion(byte[] data, out bool encrypted)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using var stream = new MemoryStream(data, writable: false);
        return GetMegFileVersion(stream, out encrypted);
    }

    public MegFileVersion GetMegFileVersion(ReadOnlySpan<byte> data, out bool encrypted)
    {
        return GetMegFileVersion(data.ToArray(), out encrypted);
    }

    private MegFile LoadMegFromFile(Stream stream, string name)
    {
        var (version, archive) = ReadArchive(stream);
        using var megFileInfo = new MegFileInformation(FileSystem.Path.GetFullPath(name), version);
        return new MegFile(archive, megFileInfo, Services);
    }

    private InMemoryMeg LoadMegFromMemory(ReadOnlySpan<byte> megData)
    {
        var copiedMegData = megData.ToArray();
        using var dataStream = new MemoryStream(copiedMegData, writable: false);
        var (_, archive) = ReadArchive(dataStream);
        return new InMemoryMeg(archive, copiedMegData);
    }

    // Reads the version and the archive model from a seekable stream positioned at the start of the MEG.
    private (MegFileVersion Version, IMegArchive Archive) ReadArchive(Stream stream)
    {
        var startPosition = stream.Position;
        var megVersion = GetMegFileVersion(stream, out var encrypted);

        if (encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        stream.Seek(startPosition, SeekOrigin.Begin);

        using var binaryReader = BinaryServiceFactory.GetReader(megVersion);
        var metadata = binaryReader.ReadBinary(stream);

        var archive = BinaryServiceFactory.GetConverter(megVersion).BinaryToModel(metadata);
        return (megVersion, archive);
    }

    private static bool TryGetFileName(Stream stream, [NotNullWhen(true)] out string? fileName)
    {
        // NB: We cannot use stream.TryGetFileName extension cause that would
        // include MegDataStream which we do not support here.
        fileName = stream switch
        {
            FileSystemStream fileSystemStream => fileSystemStream.Name,
            FileStream fileStream => fileStream.Name,
            _ => null
        };
        return fileName is not null;
    }
}
