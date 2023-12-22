﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileService" />
/// <summary>
///     Initializes a new <see cref="MegFileService" /> class.
/// </summary>
/// <param name="services">The service provider for this instance.</param>
public sealed class MegFileService(IServiceProvider services) : ServiceBase(services), IMegFileService
{
    private IMegBinaryServiceFactory BinaryServiceFactory { get; } = services.GetRequiredService<IMegBinaryServiceFactory>();

    /// <inheritdoc />   
    public void CreateMegArchive(MegFileHolderParam megFileParameters,
        IEnumerable<MegFileDataEntryBuilderInfo> builderInformation, bool overwrite)
    {
        if (megFileParameters == null)
            throw new ArgumentNullException(nameof(megFileParameters));

        if (builderInformation == null)
            throw new ArgumentNullException(nameof(builderInformation));

        var megFilePath = FileSystem.Path.GetFullPath(megFileParameters.FilePath);

        if (!overwrite && FileSystem.File.Exists(megFilePath))
            throw new IOException($"The file '{megFilePath}' already exists.");

        var constructionArchive = BinaryServiceFactory.GetConstructionBuilder(megFileParameters.FileVersion)
            .BuildConstructingMegArchive(builderInformation);

        if (constructionArchive.Encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        var metadata = BinaryServiceFactory.GetConverter(constructionArchive.MegVersion)
            .ModelToBinary(constructionArchive.Archive);

        // TODO: Use helper service, Create random file next to actual location so that we don't end up copying files across drives
        var tempFile = FileSystem.Path.Combine(FileSystem.Path.GetTempPath(), FileSystem.Path.GetRandomFileName());
        try
        {
            WriteMegFile(tempFile, metadata, constructionArchive);

            var directory = FileSystem.Path.GetDirectoryName(megFilePath);
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("File location does not point to a directory", nameof(megFilePath));
            FileSystem.Directory.CreateDirectory(directory!);

            FileSystem.File.Copy(tempFile, megFilePath, overwrite);
        }
        finally
        {
            try
            {
                FileSystem.File.Delete(tempFile);
            }
            catch (Exception e)
            {
                Logger.LogWarning(e, $"Unable to delete temporary MEG file '{tempFile}'. Reason: {e.Message}");
            }
        }
    }

    private void WriteMegFile(string tempFile, IMegFileMetadata metadata, IConstructingMegArchive constructingArchive)
    {
        using var fileStream = FileSystem.FileStream.New(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);

#if NETSTANDARD2_1_OR_GREATER || NET
        fileStream.Write(metadata.Bytes);
#else
        fileStream.Write(metadata.Bytes, 0, metadata.Size);
#endif
        long dataBytesWritten = metadata.Size;

        var streamFactory = Services.GetRequiredService<IMegDataStreamFactory>();

        foreach (var file in constructingArchive)
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

    /// <inheritdoc />
    public IMegFile Load(string filePath)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var megVersion = Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(fs, out var encrypted);

        if (encrypted)
            throw new InvalidOperationException("Cannot load an encrypted .MEG archive without encryption key.\r\n" +
                                                "Use Load(string, ReadOnlySpan<byte>, ReadOnlySpan<byte>) instead.");

        fs.Seek(0, SeekOrigin.Begin);

        using var reader = BinaryServiceFactory.GetReader(megVersion);
        using var param = new MegFileHolderParam
        {
            FilePath = filePath, // Is there a valid reason not to use an absolute path here?
            FileVersion = megVersion,
        };

        return Load(reader, fs, megVersion, param);
    }

    /// <inheritdoc />
    public IMegFile Load(string filePath, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var megVersion = GetMegFileVersion(fs, out var encrypted);

        if (!encrypted)
            throw new InvalidOperationException("The given .MEG archive is not encrypted.\r\n" +
                                                "Use Load(string) instead.");
        
        Debug.Assert(megVersion == MegFileVersion.V3);

        throw new NotImplementedException("Encrypted archives are currently not supported");

        //fs.Seek(0, SeekOrigin.Begin);

        //using var reader = BinaryServiceFactory.GetReader(key, iv);
        //using var param = new MegFileHolderParam
        //{
        //    FilePath = filePath, // Is there a valid reason not to use an absolute path here?
        //    FileVersion = megVersion,
        //    Key = key.ToArray(),
        //    IV = iv.ToArray()
        //};
        //return Load(reader, fs, megVersion, param);
    }

    private MegFileHolder Load(IMegFileBinaryReader binaryReader, Stream megStream, MegFileVersion megVersion, MegFileHolderParam param)
    {
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

        var converter = BinaryServiceFactory.GetConverter(megVersion);
        var megArchive = converter.BinaryToModel(megMetadata);
        return new MegFileHolder(megArchive, param, Services);
    }

    /// <inheritdoc />
    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(file);

        using var fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegFileVersion(fs, out encrypted);
    }

    /// <inheritdoc />
    public MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));
        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(stream, out encrypted);
    }
}