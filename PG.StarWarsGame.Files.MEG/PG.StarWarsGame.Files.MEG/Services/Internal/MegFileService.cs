﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileService" />
/// <summary>
///     Initializes a new <see cref="MegFileService" /> class.
/// </summary>
/// <param name="services">The service provider for this instance.</param>
internal sealed class MegFileService(IServiceProvider services) : ServiceBase(services), IMegFileService
{
    private IMegBinaryServiceFactory BinaryServiceFactory { get; } = services.GetRequiredService<IMegBinaryServiceFactory>();

    public void CreateMegArchive(MegFileHolderParam megFileParameters, IEnumerable<MegFileDataEntryBuilderInfo> builderInformation)
    {
        if (megFileParameters == null)
            throw new ArgumentNullException(nameof(megFileParameters));

        if (builderInformation == null)
            throw new ArgumentNullException(nameof(builderInformation));

        if (megFileParameters.HasEncryption)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        var megFilePath = FileSystem.Path.GetFullPath(megFileParameters.FilePath);
        
        var constructionArchive = BinaryServiceFactory.GetConstructionBuilder(megFileParameters.FileVersion)
            .BuildConstructingMegArchive(builderInformation);

        if (constructionArchive.Encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        var metadata = BinaryServiceFactory.GetConverter(constructionArchive.MegVersion)
            .ModelToBinary(constructionArchive.Archive);
        
        // We only support creating a new file here, so that we don't accidentally overwrite a file which we will be reading from.
        // IMegBuilder supports overwriting files.
        using var fileStream = FileSystem.FileStream.New(megFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);

#if NETSTANDARD2_1_OR_GREATER || NET
        fileStream.Write(metadata.Bytes);
#else
        fileStream.Write(metadata.Bytes, 0, metadata.Size);
#endif
        long dataBytesWritten = metadata.Size;

        var streamFactory = Services.GetRequiredService<IMegDataStreamFactory>();

        foreach (var file in constructionArchive)
        {
            using var dataStream = streamFactory.GetDataStream(file.Location);

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

        // Note: Technically, the specification does not disallow MEG files larger than 4GB. 
        // E.g, a MEG with one entry being exactly 4GB large.
        // The Archive itself is larger (Metadata + 4GB),
        // however the Metadata is still valid since no each part is within the uint32 range. 
        if (dataBytesWritten > uint.MaxValue)
            ThrowHelper.ThrowMegExceeds4GigabyteException(fileStream.Name);

        Debug.Assert(dataBytesWritten == fileStream.Position);
    }

    public IMegFile Load(string filePath)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrEmpty(filePath);

        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var megVersion = GetMegFileVersion(fs, out var encrypted);

        if (encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        using var param = new MegFileHolderParam
        {
            FilePath = filePath, // Is there a valid reason not to use an absolute path here?
            FileVersion = megVersion
        };

        //param.Validate();

        fs.Seek(0, SeekOrigin.Begin);
        var megMetadata = LoadAndValidateMetadata(fs, param);

        var converter = BinaryServiceFactory.GetConverter(megVersion);
        var megArchive = converter.BinaryToModel(megMetadata);
        return new MegFile(megArchive, param, Services);
    }

    private IMegFileMetadata LoadAndValidateMetadata(Stream megStream, MegFileHolderParam param)
    {
        using var binaryReader = BinaryServiceFactory.GetReader(param.FileVersion);

        var startPosition = megStream.Position;
        var megMetadata = binaryReader.ReadBinary(megStream);
        var endPosition = megStream.Position;

        var bytesRead = endPosition - startPosition;

        // There is no reason to validate the archive's size if we cannot access the whole stream size. 
        // We also don't want to read the whole stream if this is a "lazy" stream (such as a pipe)
        if (!megStream.CanSeek)
            throw new NotSupportedException("Non-seekable streams are currently not supported.");

        var actualMegSize = megStream.Length - startPosition;

        // Note: Technically, the specification does not disallow MEG files larger than 4GB. 
        // E.g, a MEG with one entry being exactly 4GB large.
        // The Archive itself is larger (Metadata + 4GB),
        // however the Metadata is still valid since no each part is within the uint32 range. 
        if (actualMegSize > uint.MaxValue)
            ThrowHelper.ThrowMegExceeds4GigabyteException(param.FilePath);

        var validator = Services.GetRequiredService<IMegBinaryValidator>();

        var validationResult = validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = megMetadata,
            FileSize = actualMegSize,
            BytesRead = bytesRead
        });

        if (!validationResult.IsValid)
            throw new BinaryCorruptedException($"Unable to read .MEG archive: {validationResult.Errors.First().ErrorMessage}");

        return megMetadata;
    }

    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(file);

        using var fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegFileVersion(fs, out encrypted);
    }

    private MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(stream, out encrypted);
    }
}