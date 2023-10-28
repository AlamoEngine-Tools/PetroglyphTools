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
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileService" />
public sealed class MegFileService : ServiceBase, IMegFileService
{
    /// <summary>
    ///     Initializes a new <see cref="MegFileService" /> class.
    /// </summary>
    /// <param name="services">The service provider for this instance.</param>
    public MegFileService(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public void CreateMegArchive(string filePath, IEnumerable<MegFileDataEntryBuilderInfo> builderInformation, MegFileVersion megFileVersion)
    {
        var constructionArchive = Services.GetRequiredService<IMegConstructionArchiveService>().Build(builderInformation);

        var metadata = Services.GetRequiredService<IMegBinaryServiceFactory>()
            .GetConverter(megFileVersion)
            .ModelToBinary(constructionArchive.Archive);

        var bytes = metadata.Bytes;

        using var fs = FileSystem.FileStream.New(filePath, FileMode.Create, FileAccess.Write);

        fs.Write(bytes, 0, bytes.Length);

        var streamFactory = Services.GetRequiredService<IMegDataStreamFactory>();

        foreach (var file in constructionArchive)
        {
            using var dataStream = streamFactory.GetDataStream(file.OriginInfo);

            // TODO: Test in encryption case
            if (dataStream.Length != file.FileEntry.Location.Size)
                throw new InvalidOperationException(); // TODO: InvalidModelException

            if (fs.Position != file.FileEntry.Location.Offset)
                throw new InvalidOperationException(); // TODO: InvalidModelException

            dataStream.CopyTo(fs);
        }
    }

    /// <inheritdoc />
    public void CreateMegArchive(string filePath, IEnumerable<MegFileDataEntryBuilderInfo> builderInformation, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    { 
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IMegFile Load(string filePath)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var megVersion = Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(fs, out var encrypted);

        if (encrypted)
        {
            throw new NotSupportedException("Cannot load an encrypted .MEG archive without encryption key.\r\n" +
                                            "Use Load(string, ReadOnlySpan<byte>, ReadOnlySpan<byte>) instead.");
        }

        fs.Seek(0, SeekOrigin.Begin);

        using var reader = Services.GetRequiredService<IMegBinaryServiceFactory>().GetReader(megVersion);
        using var param = new MegFileHolderParam
        {
            FilePath = filePath,
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
        {
            throw new NotSupportedException("The given .MEG archive is not encrypted.\r\n" +
                                            "Use Load(string) instead.");
        }

        Debug.Assert(megVersion == MegFileVersion.V3);

        fs.Seek(0, SeekOrigin.Begin);

        using var reader = Services.GetRequiredService<IMegBinaryServiceFactory>().GetReader(key, iv);
        using var param = new MegFileHolderParam
        {
            FilePath = filePath,
            FileVersion = megVersion,
            Key = key.ToArray(),
            IV = iv.ToArray()
        };
        return Load(reader, fs, megVersion, param);
    }

    private IMegFile Load(IMegFileBinaryReader binaryReader, Stream megStream, MegFileVersion megVersion, MegFileHolderParam param)
    {
        IMegFileMetadata megMetadata;
        try
        {
            var startPosition = megStream.Position;
            megMetadata = binaryReader.ReadBinary(megStream);
            var endPosition = megStream.Position;

            var bytesRead = endPosition - startPosition;

            // These is no reason to validate the archive's size if we cannot access the whole stream size. 
            // We also don't want to read the whole stream if this is a "lazy" stream (such as a pipe)
            if (!megStream.CanSeek)
                throw new NotSupportedException("Non-seekable streams are currently not supported.");

            var actualMegSize = megStream.Length - startPosition;
            var validator = Services.GetRequiredService<IMegBinaryValidator>();

            var validationResult = validator.Validate(new MegBinaryValidationInformation
            {
                Metadata = megMetadata,
                FileSize = actualMegSize,
                BytesRead = bytesRead
            });

            if (!validationResult.IsValid)
                throw new BinaryCorruptedException($"Unable to read .MEG archive: {validationResult.Errors.First().ErrorMessage}");
        }
        catch (BinaryCorruptedException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new BinaryCorruptedException($"Unable to read .MEG archive: {e.Message}", e);
        }

        var converter = Services.GetRequiredService<IMegBinaryServiceFactory>().GetConverter(megVersion);
        var megArchive = converter.BinaryToModel(megMetadata);
        return new MegFileHolder(megArchive, param, Services);
    }

    /// <inheritdoc />
    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        if (string.IsNullOrWhiteSpace(file))
            throw new ArgumentNullException(nameof(file));

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