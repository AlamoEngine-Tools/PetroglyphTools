// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileService" />
internal sealed class MegFileService(IServiceProvider services) : ServiceBase(services), IMegFileService
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

        var streamFactory = Services.GetRequiredService<IMegDataStreamFactory>();

        foreach (var file in constructionArchive)
        {
            using var dataStream = streamFactory.GetStream(file.Location);

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

        var startPosition = stream.Position;
        var megVersion = GetMegFileVersion(stream, out var encrypted);

        if (encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        using var megFileInfo = new MegFileInformation(FileSystem.Path.GetFullPath(stream.Name), megVersion);

        stream.Seek(startPosition, SeekOrigin.Begin);

        var megMetadata = Load(stream, megFileInfo);

        var converter = BinaryServiceFactory.GetConverter(megVersion);
        var megArchive = converter.BinaryToModel(megMetadata);
        return new MegFile(megArchive, megFileInfo, Services);
    }

    private IMegFileMetadata Load(Stream megStream, MegFileInformation megFileInfo)
    {
        using var binaryReader = BinaryServiceFactory.GetReader(megFileInfo.FileVersion);
        return binaryReader.ReadBinary(megStream);
    }

    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(file);

        using var fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegFileVersion(fs, out encrypted);
    }

    private MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(stream, out encrypted);
    }
}