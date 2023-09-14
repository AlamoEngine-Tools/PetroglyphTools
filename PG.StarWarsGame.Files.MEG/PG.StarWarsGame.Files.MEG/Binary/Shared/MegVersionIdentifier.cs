// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegVersionIdentifier : IMegVersionIdentifier
{

    // This method is optimized in a way to retrieve the MEGs file version as efficient 
    public MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead || !stream.CanSeek)
            throw new ArgumentException("Stream must be readable and seekable.", nameof(stream));

        encrypted = false;

        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        var flags = reader.ReadUInt32();
        var id = reader.ReadUInt32();


        // In V2 and V3 id and flags are never equal, where they are in V1.
        // (However this is an assumption my Mike.NL)
        // Note: In V1 we *could* have the situation where we store as many files in the meg to coincidentally match the magic number.
        // Thus we don't check for the magic number as that it would not gain us anything.
        if (flags == id)
            return MegFileVersion.V1;


        if (id != MegFileConstants.MegFileMagicNumber)
            throw new BinaryCorruptedException("Unrecognized .MEG file version");

        // This file is encrypted, thus it can only be V3
        if (flags == MegFileConstants.MegFileEncryptedFlag)
        {
            encrypted = true;
            return MegFileVersion.V3;
        }


        var dataStart = reader.ReadUInt32();
        var numFilenames = reader.ReadUInt32();
        var numFiles = reader.ReadUInt32();

        Debug.Assert(numFiles == numFilenames);

        // So far the file could be either V2 or V3. 
        // Efficient approach to check the meg version: 
        // For non empty meg files, we know the FileTable always starts at
        //
        //      fileTableStart = dataStart - n * 20     where n stands for the number of files. 
        //                                              The record size of the FileTable for V2 and V3 (unencrypted)
        //                                              is always (5 * 4 bytes) = 20 bytes long.
        //
        // So we simply assume we are dealing with a V3 meg file.
        // In that case, reading the next uint32 of the stream should yield us the filenamesSize.
        // There are several condition to check now.
        //
        // 1)   If that read operation failed:
        //          a) For non-empty MEGs, the archive is corrupted
        //          b) for emtpy MEGs, it must be V2
        //
        // 2)   If that read operation passed, we still could have either version. We now check whether:
        //          a) numFiles is 0 AND filenamesSize is null
        //             -->  The archive must be V3
        //
        //          b) 24 bytes + filenamesSize       where 24 bytes is
        //                                            the size of the V3 header
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
        //      Note: While this also is no *proven* approach, the chances are towards 0 we end up having a false positive
        if (flags == MegFileConstants.MegFileUnencryptedFlag)
        {
            if (!TryReadUInt32(reader, out var filenamesSize))
            {
                if (numFiles == 0)
                    return MegFileVersion.V2;
                throw new BinaryCorruptedException("Unrecognized :MEG file version");
            }
            
            if (numFiles == 0 && filenamesSize == 0)
                return MegFileVersion.V3;

            // known start of the FileTable
            var fileTableOffset = dataStart - numFiles * 20;


            if (fileTableOffset != 24 + filenamesSize)
                return MegFileVersion.V2;

            // Now check for first and last element values

            reader.BaseStream.Position = fileTableOffset;

            if (!FileRecordIsV3(reader, 0))
                return MegFileVersion.V2;

            if (numFiles == 1)
                return MegFileVersion.V3;

            var lastFileRecordPosition = dataStart - 20;
            reader.BaseStream.Position = lastFileRecordPosition;
            var lastFileTableIndex = numFiles - 1;

            return FileRecordIsV3(reader, lastFileTableIndex) ? MegFileVersion.V3 : MegFileVersion.V2;
        }

        throw new BinaryCorruptedException("Unrecognized .MEG file version.");
    }

    private static bool FileRecordIsV3(BinaryReader reader, uint expectedIndex)
    {
        try
        {
            var flag = reader.ReadUInt16();
            if (flag != 0)
                return false;

            reader.ReadUInt32(); // CRC
            var index = reader.ReadUInt32();
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