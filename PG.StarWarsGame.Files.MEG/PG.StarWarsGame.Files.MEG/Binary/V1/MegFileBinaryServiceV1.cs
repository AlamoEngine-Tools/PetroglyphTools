// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PG.Commons.Binary;
using PG.Commons.Binary.File;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal class MegFileBinaryServiceV1 : IBinaryFileConverter<MegMetadata, IMegFile, MegFileHolderParam>,
    IBinaryFileReader<IMegFileMetadata>
{
    public IMegFileMetadata ReadBinary(Stream byteStream)
    {
        if (byteStream.Length < 1)
        {
            throw new InvalidOperationException("Cannot read empty data");
        }

        using var binaryReader = new BinaryReader(byteStream, MegFileConstants.MegContentFileNameEncoding, true);

        MegHeader header = BuildMegHeaderInternal(binaryReader);
        MegFileNameTable fileNameTable = BuildFileNameTableInternal(binaryReader, (int)header.NumFiles);
        MegFileTable fileTable = BuildFileContentTableInternal(binaryReader, (int)header.NumFiles);

        return new MegMetadata(header, fileNameTable, fileTable);
    }

    private MegHeader BuildMegHeaderInternal(BinaryReader reader)
    {
        uint numFileNames = reader.ReadUInt32();
        uint numFiles = reader.ReadUInt32();

        if (numFiles != numFileNames)
        {
            throw new BinaryCorruptedException(
                $"The provided meg file is corrupt. The number of file names ({numFileNames}) has to be equal to the number of files ({numFiles})");
        }

        if (numFiles > int.MaxValue)
        {
            throw new NotSupportedException(
                ".MEG files with a file number greater than int32.MaxValue are not supported.");
        }

        return new MegHeader(numFileNames, numFiles);
    }

    private MegFileNameTable BuildFileNameTableInternal(BinaryReader reader, int fileNumber)
    {
        var fileNameTable = new List<MegFileNameTableRecord>();
        Encoding encoding = MegFileConstants.MegContentFileNameEncoding;
        for (uint i = 0; i < fileNumber; i++)
        {
            ushort fileNameLength = reader.ReadUInt16();
            string fileName = GetString(reader, fileNameLength, encoding);
            fileNameTable.Add(new MegFileNameTableRecord(fileName, encoding));
        }

        return new MegFileNameTable(fileNameTable);
    }

    private static string GetString(BinaryReader reader, ushort length, Encoding encoding)
    {
        byte[]? rentedFromPool = null;

        Span<byte> buffer =
#if NETSTANDARD2_0
            rentedFromPool = ArrayPool<byte>.Shared.Rent(length);
#else
            length > 256
                ? rentedFromPool = ArrayPool<byte>.Shared.Rent(length)
                : stackalloc byte[length];
#endif

        int bytesRead = reader.Read();
        if (bytesRead != length)
        {
            throw new BinaryCorruptedException($"Unable to read string value of given length '{length}'.");
        }

        string @string = encoding.GetString(buffer.ToArray());

        if (rentedFromPool is not null)
        {
            ArrayPool<byte>.Shared.Return(rentedFromPool);
        }

        return @string;
    }

    private MegFileTable BuildFileContentTableInternal(BinaryReader reader, int fileNumber)
    {
        var megFileContentTableRecords = new List<MegFileContentTableRecord>(fileNumber);

        for (var i = 0; i < fileNumber; i++)
        {
            var crc32 = new Crc32(reader.ReadUInt32());
            uint fileTableRecordIndex = reader.ReadUInt32();

            if (fileTableRecordIndex > int.MaxValue)
            {
                throw new NotSupportedException(
                    ".MEG files with a file number greater than int32.MaxValue are not supported.");
            }

            Debug.Assert(fileTableRecordIndex == i);

            uint fileSizeInBytes = reader.ReadUInt32();
            uint fileStartOffsetInBytes = reader.ReadUInt32();

            uint fileNameTableIndex = reader.ReadUInt32();
            if (fileNameTableIndex > int.MaxValue)
            {
                throw new NotSupportedException(
                    ".MEG files with a file number greater than int32.MaxValue are not supported.");
            }

            megFileContentTableRecords.Add(new MegFileContentTableRecord(
                crc32,
                fileTableRecordIndex,
                fileSizeInBytes,
                fileStartOffsetInBytes,
                fileNameTableIndex));
        }

        return new MegFileTable(megFileContentTableRecords);
    }


    public MegMetadata FromHolder(IMegFile holder, out IList<string> filesToStream)
    {
        //var files = holder.Content.Select(megFileDataEntry => megFileDataEntry.RelativeFilePath).ToList();
        //var megFileNameTableRecords =
        //    files.Select(file => new MegFileNameTableRecord(file)).ToList();
        //megFileNameTableRecords.Sort();
        //filesToStream = new List<string>();
        //foreach (var megFileNameTableRecord in megFileNameTableRecords)
        //{
        //    foreach (var megFileDataEntry in CollectSortedMegFileDataEntries(holder, megFileNameTableRecord))
        //    {
        //        filesToStream.Add(megFileDataEntry.AbsoluteFilePath);
        //        break;
        //    }
        //}

        //var megFileContentTableRecords = new List<MegFileContentTableRecord>();
        //for (var i = 0; i < megFileNameTableRecords.Count; i++)
        //{
        //    var crc32 = ChecksumService.GetChecksum(megFileNameTableRecords[i].FileName);
        //    var fileTableRecordIndex = Convert.ToUInt32(i);
        //    var fileSizeInBytes = Convert.ToUInt32(m_fileSystem.FileInfo.FromFileName(filesToStream[i]).Length);
        //    var fileNameTableIndex = Convert.ToUInt32(i);
        //    megFileContentTableRecords.Add(new MegFileContentTableRecord(crc32, fileTableRecordIndex,
        //        fileSizeInBytes, 0, fileNameTableIndex));
        //}

        //var header = new MegHeader(Convert.ToUInt32(megFileContentTableRecords.Count),
        //    Convert.ToUInt32(megFileContentTableRecords.Count));
        //var megFileNameTable = new MegFileNameTable(megFileNameTableRecords);
        //var currentOffset = Convert.ToUInt32(header.Size);
        //currentOffset += Convert.ToUInt32(megFileNameTable.Size);
        //var t = new MegFileContentTable(megFileContentTableRecords);
        //currentOffset += Convert.ToUInt32(t.Size);
        //foreach (var megFileContentTableRecord in megFileContentTableRecords)
        //{
        //    megFileContentTableRecord.FileStartOffsetInBytes = currentOffset;
        //    currentOffset += Convert.ToUInt32(megFileContentTableRecord.FileSizeInBytes);
        //}

        //var megFileContentTable = new MegFileContentTable(megFileContentTableRecords);
        //return new MegMetadata(header, megFileNameTable, megFileContentTable);

        throw new NotImplementedException();
    }

    private IEnumerable<MegFileDataEntry> CollectSortedMegFileDataEntries(IMegFile holder,
        MegFileNameTableRecord megFileNameTableRecord)
    {
        return holder.Content.Where(megFileDataEntry =>
            megFileNameTableRecord.FileName.Equals(
                megFileDataEntry.RelativeFilePath.Replace("\\", "/").Replace("\0", string.Empty),
                StringComparison.InvariantCultureIgnoreCase));
    }

    public MegMetadata FromHolder(IMegFile holder)
    {
        return FromHolder(holder, out IList<string> _);
    }

    public IMegFile ToHolder(MegFileHolderParam param, MegMetadata model)
    {
        throw new NotImplementedException();
    }
}