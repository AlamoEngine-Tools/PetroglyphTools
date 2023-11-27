// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc cref="IMegFileService" />
public class MegFileService : ServiceBase, IMegFileService
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
        using FileSystemStream fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);

        MegFileVersion megVersion = GetMegFileVersion(fs, out bool encrypted);

        if (encrypted)
        {
            throw new NotSupportedException("Cannot load an encrypted .MEG archive without encryption key.\r\n" +
                                            "Use Load(string, ReadOnlySpan<byte>, ReadOnlySpan<byte>) instead.");
        }

        IBinaryFileReader<IMegFileMetadata> reader = Services.GetRequiredService<IMegBinaryServiceFactory>()
            .GetReader(megVersion);

        fs.Seek(0, SeekOrigin.Begin);
        return CreateHolderFromMetadata(reader, fs, filePath, megVersion);
    }

    /// <inheritdoc />
    public IMegFile Load(string filePath, ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        using FileSystemStream fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);

        MegFileVersion version = GetMegFileVersion(fs, out bool encrypted);

        if (!encrypted)
        {
            throw new NotSupportedException("The given .MEG archive is not encrypted.\r\n" +
                                            "Use Load(string) instead.");
        }

        IBinaryFileReader<IMegFileMetadata> reader = Services.GetRequiredService<IMegBinaryServiceFactory>()
            .GetReader(key, iv);

        fs.Seek(0, SeekOrigin.Begin);
        return CreateHolderFromMetadata(reader, fs, filePath, version);
    }

    private IMegFile CreateHolderFromMetadata(IBinaryFileReader<IMegFileMetadata> binaryBuilder, Stream fileStream,
        string filePath, MegFileVersion version)
    {
        IMegFileMetadata megMetadata = binaryBuilder.ReadBinary(fileStream);

        if (megMetadata.FileNumber == 0)
        {
            throw new NotSupportedException("Empty .MEG archives are not supported.");
        }

        var files = new List<MegFileDataEntry>(megMetadata.FileNumber);

        // According to the specification: 
        //  - The Meg's FileTable is sorted by CRC32.
        //  - It's not specified how or whether the FileNameTable is sorted.
        //  - The game merges FileTable entries (and takes the last entry for duplicates)
        //  --> In theory:  For file entries with the same file name (and thus same CRC32),
        //                  the game should use the last file.
        // 
        // Since an IMegFile expects a List<>, not a Collection<>, we have to preserve the order of the FileTable
        for (var i = 0; i < megMetadata.FileNumber; i++)
        {
            IMegFileDescriptor fileDescriptor = megMetadata.FileTable[i];
            Crc32 crc = fileDescriptor.Crc32;
            uint fileOffset = fileDescriptor.FileOffset;
            uint fileSize = fileDescriptor.FileSize;
            int fileNameIndex = fileDescriptor.FileNameIndex;
            string fileName = megMetadata.FileNameTable[fileNameIndex];
            files.Add(new MegFileDataEntry(crc, fileName, fileOffset, fileSize));
        }

        return new MegFileHolder(files, new MegFileHolderParam { FilePath = filePath, FileVersion = version },
            Services);
    }

    /// <inheritdoc />
    public MegFileVersion GetMegFileVersion(string file, out bool encrypted)
    {
        if (string.IsNullOrWhiteSpace(file))
        {
            throw new ArgumentNullException(nameof(file));
        }

        using FileSystemStream fs = FileSystem.FileStream.New(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        return GetMegFileVersion(fs, out encrypted);
    }

    /// <inheritdoc />
    public MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead || !stream.CanSeek)
        {
            throw new ArgumentException("Stream must be readable and seekable.", nameof(stream));
        }

        encrypted = false;

        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        uint flags = reader.ReadUInt32();
        uint id = reader.ReadUInt32();


        // In V2 and V3 id and flags are never equal, where they are in V1.
        // (However this is an assumption my Mike.NL)
        // Note: In V1 we *could* have the situation where we store as many files in the meg to coincidentally match the magic number.
        // Thus we don't check for the magic number as that it would not gain us anything.
        if (flags == id)
        {
            if (flags == 0)
            {
                throw new InvalidOperationException("Empty .MEG files re not supported");
            }

            return MegFileVersion.V1;
        }


        if (id != MegFileConstants.MegFileMagicNumber)
        {
            throw new BinaryCorruptedException("Unrecognized .MEG file version");
        }

        // This file is encrypted, thus it can only be V3
        if (flags == MegFileConstants.MegFileEncryptedFlag)
        {
            encrypted = true;
            return MegFileVersion.V3;
        }


        uint dataStart = reader.ReadUInt32();
        uint numFilenames = reader.ReadUInt32();
        uint numFiles = reader.ReadUInt32();

        Debug.Assert(numFiles == numFilenames);

        if (numFiles == 0)
        {
            throw new InvalidOperationException("Empty .MEG files re not supported");
        }

        // So far the file could be either V2 or V3. 
        // Efficient approach to check the meg version: 
        // For non empty meg files, we know the FileTable always starts at
        //
        //      fileTableStart = dataStart - n * 20     where n stands for the number of files. 
        //                                              The record size of the FileTable for V2 and V3 (unencrypted)
        //                                              is always (5 * 4 bytes) = 20 bytes long.
        //
        // So we simply assume we are dealing with a V3 meg file.
        // In that case, reading the next uint32 should yield us the filenamesSize.
        // There are several condition to check now.
        //
        // 1)   If that read operation failed
        //          --> we must have a V2 meg file. 
        //
        // 2)   If that read operation passed, we still could have either version. We now check whether
        //
        //          24 bytes + filenamesSize       where 24 bytes is
        //                                         the size of the V3 header
        //
        //      equals the value of fileTableStart.
        //      If the values do not match we are a V2 case where the reading filenamesSize gave us some garbage.
        //      However, we could end up in a case where "the garbage" randomly caused the equation to match but we still have a V2 input file. 
        //      Thus we parse the first and the last element of the FileTable and check its data for plausibility where
        //
        //          flags       uint16      0
        //          crc32       uint32      (omitted)
        //          index       uint32      0 OR N - 1
        //      Only if the check passes we return a V3 version.
        //
        //      Note: While this also is no *proven* approach the chances are towards 0 we end up having a false positive
        if (flags == MegFileConstants.MegFileUnencryptedFlag)
        {
            uint filenamesSize;

            if (!TryReadUInt32(reader, out filenamesSize))
            {
                return MegFileVersion.V2;
            }

            // known start of the FileTable
            uint fileTableOffset = dataStart - numFiles * 20;


            if (fileTableOffset != 24 + filenamesSize)
            {
                return MegFileVersion.V2;
            }

            // Now check for first and last element values

            reader.BaseStream.Position = fileTableOffset;

            if (!FileRecordIsV3(reader, 0))
            {
                return MegFileVersion.V2;
            }

            if (numFiles == 1)
            {
                return MegFileVersion.V3;
            }

            uint lastFileRecordPosition = dataStart - 20;
            reader.BaseStream.Position = lastFileRecordPosition;
            uint lastFileTableIndex = numFiles - 1;

            return FileRecordIsV3(reader, lastFileTableIndex) ? MegFileVersion.V3 : MegFileVersion.V2;
        }

        throw new BinaryCorruptedException("Unrecognized .MEG file version");
    }

    private static bool FileRecordIsV3(BinaryReader reader, uint expectedIndex)
    {
        try
        {
            ushort flag = reader.ReadUInt16();
            if (flag != 0)
            {
                return false;
            }

            reader.ReadUInt32(); // CRC
            uint index = reader.ReadUInt32();
            return index == expectedIndex;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryReadUInt32(BinaryReader reader, out uint value)
    {
        try
        {
            value = reader.ReadUInt32();
            return true;
        }
        catch (EndOfStreamException)
        {
            value = 0;
            return false;
        }
    }
}