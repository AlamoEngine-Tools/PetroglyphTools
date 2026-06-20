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
    private const string LoadArchiveEncryptedMessage = "Loading an encrypted MEG archive via LoadArchive is not supported.";

    private IMegBinaryServiceFactory BinaryServiceFactory { get; } = services.GetRequiredService<IMegBinaryServiceFactory>();

    public void CreateMegArchive(
        Stream stream,
        MegVersion megVersion,
        MegEncryptionData? encryptionData,
        IEnumerable<MegDataEntryBuilderInfo> builderInformation)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (builderInformation == null)
            throw new ArgumentNullException(nameof(builderInformation));

        var constructionArchive = BinaryServiceFactory.GetConstructionBuilder(megVersion)
            .BuildConstructingMegArchive(builderInformation);

        if (constructionArchive.Encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        if (constructionArchive.Encrypted)
        {
            if (encryptionData is null)
                throw new NotSupportedException("Creating an encrypted MEG archive requires encryption key.");
            if (megVersion == MegVersion.V3)
                throw new NotSupportedException("Creating an encrypted MEG archive requires the MEG version to be V3.");
        }


        var metadata = BinaryServiceFactory.GetConverter(constructionArchive.MegVersion)
            .ModelToBinary(constructionArchive.Archive);

        metadata.WriteTo(stream);

        long dataBytesWritten = metadata.Size;

        foreach (var file in constructionArchive)
        {
            using var dataStream = file.Location.GetDataStream();

            // TODO: Test in encryption case
            if (dataStream.Length != file.DataEntry.Location.Size)
                throw new InvalidOperationException(
                    $"Actual data entry size '{dataStream.Length}' does not match expected value: {file.DataEntry.Location.Size}");

            if (stream.Position != file.DataEntry.Location.Offset)
                throw new InvalidOperationException(
                    $"Actual file position '{stream.Position}' does not match expected entry offset: {file.DataEntry.Location.Offset}");

            dataStream.CopyTo(stream);

            dataBytesWritten += dataStream.Length;
        }

        Debug.Assert(dataBytesWritten == constructionArchive.ExpectedFileSize);
    }

    public IMegFile LoadFile(string filePath)
    {
        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        var fullPath = FileSystem.Path.GetFullPath(filePath);
        using var fs = FileSystem.FileStream.New(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return LoadFile(fs);
    }

    public IMegFile LoadFile(FileSystemStream stream)
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
        {
            var (version, archive, encrypted) = ReadArchive(stream);
            if (encrypted)
                throw new NotSupportedException(LoadArchiveEncryptedMessage);
            using var megFileInfo = new MegFileInformation(FileSystem.Path.GetFullPath(fileName), version);
            return new MegFile(archive!, megFileInfo, Services);
        }

        using var buffer = new MemoryStream(stream.CanSeek ? (int)(stream.Length - stream.Position) : 0);
        stream.CopyTo(buffer);
        buffer.Position = 0;

        var (_, memArchive, memEncrypted) = ReadArchive(buffer);
        if (memEncrypted)
            throw new NotSupportedException(LoadArchiveEncryptedMessage);
        return new InMemoryMeg(memArchive!, buffer.ToArray());
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

    public MegVersion GetMegVersion(string file, out bool encrypted)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(file);

        using var fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegVersion(fs, out encrypted);
    }

    public MegVersion GetMegVersion(Stream stream, out bool encrypted)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegVersion(stream, out encrypted);
    }

    public MegVersion GetMegVersion(byte[] data, out bool encrypted)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using var stream = new MemoryStream(data, writable: false);
        return GetMegVersion(stream, out encrypted);
    }

    public MegVersion GetMegVersion(ReadOnlySpan<byte> data, out bool encrypted)
    {
        return GetMegVersion(data.ToArray(), out encrypted);
    }

    private MegFile LoadMegFromFile(Stream stream, string name)
    {
        var (version, archive, encrypted) = ReadArchive(stream);
        if (encrypted)
            throw new NotImplementedException("Loading an encrypted MEG archive is not yet implemented.");

        using var megFileInfo = new MegFileInformation(FileSystem.Path.GetFullPath(name), version);
        return new MegFile(archive!, megFileInfo, Services);
    }

    private InMemoryMeg LoadMegFromMemory(ReadOnlySpan<byte> megData)
    {
        var copiedMegData = megData.ToArray();
        using var dataStream = new MemoryStream(copiedMegData, writable: false);
        var (_, archive, encrypted) = ReadArchive(dataStream);
        if (encrypted)
            throw new NotSupportedException(LoadArchiveEncryptedMessage);

        return new InMemoryMeg(archive!, copiedMegData);
    }

    // Reads the version and the archive model from a seekable stream positioned at the start of the MEG.
    // Does not throw on encrypted input; the caller decides which exception is appropriate.
    private (MegVersion Version, IMegArchive? Archive, bool Encrypted) ReadArchive(Stream stream)
    {
        var startPosition = stream.Position;
        var megVersion = GetMegVersion(stream, out var encrypted);

        if (encrypted)
            return (megVersion, null, true);

        stream.Seek(startPosition, SeekOrigin.Begin);

        using var binaryReader = BinaryServiceFactory.GetReader(megVersion);
        var metadata = binaryReader.ReadBinary(stream);

        var archive = BinaryServiceFactory.GetConverter(megVersion).BinaryToModel(metadata);
        return (megVersion, archive, false);
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
