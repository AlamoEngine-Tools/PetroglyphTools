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
    /// <summary>
    /// This method is optimized in a way to retrieve the MEGs file version as efficient as possible.
    /// If an invalid MEG archive is detected a <see cref="BinaryCorruptedException"/> is thrown.
    /// <b>However</b> this method does not completely verify whether the passed stream is a valid MEG archive or not.
    /// </summary>
    /// <param name="stream">The MEG archive stream</param>
    /// <param name="encrypted">Indicates whether the archive is encrypted or not.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is null.</exception>
    /// <exception cref="ArgumentException">The <paramref name="stream"/> is not readable or seekable.</exception>
    /// <exception cref="BinaryCorruptedException">The read data is not a valid MEG archive.</exception>
    public unsafe MegFileVersion GetMegFileVersion(Stream stream, out bool encrypted)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        if (!stream.CanSeek)
            throw new ArgumentException("Stream must seekable.", nameof(stream));

        if (stream.Length == 0)
            throw new ArgumentException("Stream must not be empty", nameof(stream));

        encrypted = false;

        using var reader = new BinaryReader(stream, Encoding.UTF8, true);

        try
        {
            var flags = reader.ReadUInt32();
            var id = reader.ReadUInt32();


            // In V2 and V3 id and flags are never equal, where they are in V1.
            // (However this is an assumption my Mike.NL)
            // Note: In V1 we *could* have the situation where we store as many files in the meg to coincidentally match the magic number.
            // Thus we don't check for the magic number as that it would not gain us anything.
            if (flags == id)
                return MegFileVersion.V1;


            if (id != MegFileConstants.MegFileMagicNumber)
                throw new BinaryCorruptedException("Unrecognized .MEG archive version");

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
            //          b) for empty MEGs, it must be V2
            //
            // 2)   If that read operation passed, we still could have either version. We now check whether:
            //          a)  numFiles is 0 AND filenamesSize is 0
            //                  -->  The archive must be V3
            //          b)  numFiles is 0 AND filenameSize is not 0
            //                  -->  The archive is V2 with some additional data (which we ignore)
            //
            //          c)  24 bytes + filenamesSize       where 24 bytes is
            //                                             the size of the V3 header
            //
            //              equals the value of fileTableStart.
            //
            //      If the values do not match we are a V2 case where the reading filenamesSize gave us some garbage.
            //      However, we could end up in a case where "the garbage" randomly caused the equation to match but we still have a V2 input file. 
            //      Thus we parse the first and the last element of the FileTable and check its data for plausibility where
            //          flags       uint16      0
            //          crc32       uint32      (omitted)
            //          index       uint32      0 OR N - 1
            //      Only if the check passes we return a V3 version.
            //
            //      Otherwise, if equation failed or if V3 record check failed, we perform a check similarly whether a valid V2 record is present.
            //          crc32       uint32      (omitted)
            //          index       uint32      0 OR N - 1
            //
            //      Note:   While this no *proven* approach, the chances are towards 0 we end up having a false positive. 
            //              The minimal possible value in an V2 archive interpreted as filenamesSize would be: 
            //              0x210001 (~2MB), which is massive and unseen for any PG game or even existing modification.
            //              (Such files would be artificial only made to crash this library - but we are prepared!)
            //              
            //              The value 0x20210001 is represents the following: 
            //              0x210001 <=> [0x01, 0x00, 0x21, 0x00] (Little Endian)
            //                  where [0x01, 0x00] is the # chars of the first filename in the table,
            //                  where [0x21], shortest allowed filename value "!" and
            //                  where [0x00] is the LSB part of the next file name length.
            if (flags == MegFileConstants.MegFileUnencryptedFlag)
            {
                if (!TryReadUInt32(reader, out var filenamesSize))
                {
                    if (numFiles == 0)
                        return MegFileVersion.V2;
                    throw new BinaryCorruptedException("Unrecognized :MEG file version");
                }

                if (numFiles == 0)
                {
                    if (filenamesSize == 0)
                        return MegFileVersion.V3;

                    // An empty V2 file that has some junk attached.
                    return MegFileVersion.V2;
                }

                delegate*<BinaryReader, uint, bool> method;
                MegFileVersion versionToCheck;

                // known start of the FileTable
                var fileTableOffset = checked(dataStart - numFiles * 20);


                // Now check for first and last element values
                
                if (fileTableOffset == 24 + filenamesSize)
                {
                    versionToCheck = MegFileVersion.V3;
                    method = &FileRecordIsV3;
                }
                else
                {
                    versionToCheck = MegFileVersion.V2;
                    method = &FileRecordIsV2;
                }

                reader.BaseStream.Position = fileTableOffset;

                if (CheckFirstAndLastRecord(reader, dataStart, numFiles, method))
                    return versionToCheck;

                if (versionToCheck == MegFileVersion.V2)
                    throw new BinaryCorruptedException("Unrecognized .MEG file version.");


                // The V3 check failed.
                // There is a chance > 0% (as explained above) the archive looked like V3 but actually is V2.
                // Thus we check again a V2 case.
                reader.BaseStream.Position = fileTableOffset;

                if (CheckFirstAndLastRecord(reader, dataStart, numFiles, &FileRecordIsV2))
                    return MegFileVersion.V2;
                throw new BinaryCorruptedException("Unrecognized .MEG file version.");
            }

            throw new BinaryCorruptedException("Unrecognized .MEG file version.");
        }
        catch (OverflowException e)
        {
            throw new BinaryCorruptedException($".MEG archive has invalid data: {e.Message}", e);
        }
        catch (EndOfStreamException e)
        {
            throw new BinaryCorruptedException($"Unable to read .MEG archive: {e.Message}", e);
        }
    }

    private static unsafe bool CheckFirstAndLastRecord(BinaryReader reader, uint dataStart, uint numFiles, delegate*<BinaryReader, uint, bool> isRecord)
    {
        if (!isRecord(reader, 0))
            return false;

        if (numFiles == 1)
            return true;

        // Constant value 20 applies to both V2 and V3 record size
        var lastFileRecordPosition = dataStart - 20;
        reader.BaseStream.Position = lastFileRecordPosition;
        var lastFileTableIndex = numFiles - 1;

        return isRecord(reader, lastFileTableIndex);
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

    private static bool FileRecordIsV2(BinaryReader reader, uint expectedIndex)
    {
        try
        {
            reader.ReadUInt32(); // CRC
            var index = reader.ReadUInt32();
            return index == expectedIndex;
        }
        catch
        {
            return false;
        }
    }
}