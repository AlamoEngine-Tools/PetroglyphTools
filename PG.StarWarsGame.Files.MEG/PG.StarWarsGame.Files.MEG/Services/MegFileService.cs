// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    public void CreateMegArchive(string megArchiveName, string targetDirectory,
        IEnumerable<MegFileDataEntryInfo> packedFileNameToAbsoluteFilePathsMap,
        MegFileVersion megFileVersion)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void CreateMegArchive(string megArchiveName, string targetDirectory,
        IEnumerable<MegFileDataEntryInfo> packedFileNameToAbsoluteFilePathsMap,
        ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IMegFile Load(string filePath)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);

        var megVersion = Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(fs, out var encrypted);

        if (encrypted)
        {
            throw new NotSupportedException("Cannot load an encrypted .MEG archive without encryption key.\r\n" +
                                            "Use Load(string, ReadOnlySpan<byte>, ReadOnlySpan<byte>) instead.");
        }

        using var reader = Services.GetRequiredService<IMegBinaryServiceFactory>().GetReader(megVersion);

        fs.Seek(0, SeekOrigin.Begin);

        var archive = Load(reader, fs, filePath);

        return new MegFileHolder(archive, new MegFileHolderParam { FilePath = filePath, FileVersion = megVersion },
            Services);
    }

    /// <inheritdoc />
    public IMegFile Load(string filePath, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);

        var megVersion = GetMegFileVersion(fs, out var encrypted);

        if (!encrypted)
        {
            throw new NotSupportedException("The given .MEG archive is not encrypted.\r\n" +
                                            "Use Load(string) instead.");
        }

        using var reader = Services.GetRequiredService<IMegBinaryServiceFactory>().GetReader(key, iv);

        fs.Seek(0, SeekOrigin.Begin);
        
        var archive = Load(reader, fs, filePath);

        return new MegFileHolder(archive, new MegFileHolderParam { FilePath = filePath, FileVersion = megVersion },
            Services);
    }

    private IMegArchive Load(IMegFileBinaryReader binaryBuilder, Stream fileStream, string filePath)
    {
        IMegFileMetadata megMetadata;
        try
        {
            var startPosition = fileStream.Position;
            megMetadata = binaryBuilder.ReadBinary(fileStream);
            var endPosition = fileStream.Position;

            var bytesRead = endPosition - startPosition;

            // These is no reason to validate the archive's size if we cannot access the whole stream size. 
            // We also don't want to read the whole stream if this is a "lazy" stream (such as a pipe)
            if (!fileStream.CanSeek)
                throw new NotSupportedException("Non-seekable streams are currently not supported.");

            var actualMegSize = fileStream.Length - startPosition;
            var validator = Services.GetRequiredService<IMegFileSizeValidator>();

            var validationResult = validator.Validate(new MegSizeValidationInformation
            {
                Metadata = megMetadata,
                ArchiveSize = actualMegSize,
                BytesRead = bytesRead
            });

            if (!validationResult.IsValid)
                throw new BinaryCorruptedException($"Unable to read .MEG archive: Read bytes do not match expected size: {validationResult}");
        }
        catch (BinaryCorruptedException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new BinaryCorruptedException($"Unable to read .MEG archive: {e.Message}", e);
        }

        var files = new List<MegFileDataEntry>(megMetadata.Header.FileNumber);

        // According to the specification: 
        //  - The Meg's FileTable is sorted by CRC32.
        //  - It's not specified how or whether the FileNameTable is sorted.
        //  - The game merges FileTable entries (and takes the last entry for duplicates)
        //  --> In theory:  For file entries with the same file name (and thus same CRC32),
        //                  the game should use the last file.
        // 
        // Since an IMegFile expects a List<>, not a Collection<>, we have to preserve the order of the FileTable
        var lastCrc = default(Crc32);
        for (var i = 0; i < megMetadata.Header.FileNumber; i++)
        {
            var fileDescriptor = megMetadata.FileTable[i];
            var crc = fileDescriptor.Crc32;

            if (lastCrc > crc) 
                Logger.LogWarning($"The MEG's file table is not sorted correctly by CRC32. MEG file path: '{filePath}'");

            var fileOffset = fileDescriptor.FileOffset;
            var fileSize = fileDescriptor.FileSize;
            var fileNameIndex = fileDescriptor.FileNameIndex;
            var fileName = megMetadata.FileNameTable[fileNameIndex];
            files.Add(new MegFileDataEntry(crc, fileName, fileOffset, fileSize));
        }

        return new MegArchive(files);
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
        return Services.GetRequiredService<IMegVersionIdentifier>().GetMegFileVersion(stream, out encrypted);
    }
}