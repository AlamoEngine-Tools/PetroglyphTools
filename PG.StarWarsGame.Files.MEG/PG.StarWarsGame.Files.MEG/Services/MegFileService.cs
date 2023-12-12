// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
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
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data;
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

        var megFilePath = megFileParameters.FilePath;

        Commons.Utilities.ThrowHelper.ThrowIfNullOrWhiteSpace(megFilePath, nameof(megFileParameters));

        var constructionArchive = BinaryServiceFactory.GetConstructionBuilder(megFileParameters.FileVersion)
            .BuildConstructingMegArchive(builderInformation);

        var metadata = BinaryServiceFactory.GetConverter(constructionArchive.MegVersion)
            .ModelToBinary(constructionArchive.Archive);

        var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;

        using var fs = FileSystem.FileStream.New(megFilePath, fileMode, FileAccess.Write, FileShare.None);

#if NETSTANDARD2_1_OR_GREATER || NET
        fs.Write(metadata.Bytes);
#else
        fs.Write(metadata.Bytes, 0, metadata.Size);
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

            if (fs.Position != file.DataEntry.Location.Offset)
                throw new InvalidOperationException(
                    $"Actual file position '{fs.Position}' does not match expected entry offset: {file.DataEntry.Location.Offset}");

            dataStream.CopyTo(fs);

            dataBytesWritten += dataStream.Length;
        }

        // Note: Technically, the specification does not disallow MEG files larger than 4GB. 
        // E.g, a MEG with one entry being exactly 4GB large.
        // The Archive itself is larger (Metadata + 4GB),
        // however the Metadata is still valid since no each part is within the uint32 range. 
        if (dataBytesWritten > uint.MaxValue)
            ThrowHelper.ThrowMegExceeds4GigabyteException(megFilePath);
        
        Debug.Assert(dataBytesWritten == fs.Position);
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

        fs.Seek(0, SeekOrigin.Begin);

        using var reader = BinaryServiceFactory.GetReader(key, iv);
        using var param = new MegFileHolderParam
        {
            FilePath = filePath, // Is there a valid reason not to use an absolute path here?
            FileVersion = megVersion,
            Key = key.ToArray(),
            IV = iv.ToArray()
        };
        return Load(reader, fs, megVersion, param);
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