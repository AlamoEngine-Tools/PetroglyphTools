// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal class MegFileBinaryReaderV1(IServiceProvider services) : MegFileBinaryReaderBase<MegMetadata, MegHeader, MegFileTable>(services)
{
    protected internal override MegMetadata CreateMegMetadata(MegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable, MegFileTable fileTable)
    {
        return new MegMetadata(header, fileNameTable, fileTable);
    }

    protected internal override MegHeader BuildMegHeader(BinaryReader binaryReader)
    {
        var numFileNames = binaryReader.ReadUInt32();
        var numFiles = binaryReader.ReadUInt32();

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

    protected internal override MegFileTable BuildFileTable(BinaryReader binaryReader, MegHeader header)
    {
        var fileNumber = header.FileNumber;
        var megFileContentTableRecords = new List<MegFileTableRecord>(fileNumber);

        for (var i = 0; i < fileNumber; i++)
        {
            var record = BuildFileTableRecord(binaryReader);
            Debug.Assert(record.FileTableRecordIndex == i);
            megFileContentTableRecords.Add(record);
        }

        return new MegFileTable(megFileContentTableRecords);
    }

    private static MegFileTableRecord BuildFileTableRecord(BinaryReader binaryReader)
    {
        var crc32 = new Crc32(binaryReader.ReadUInt32());
        var fileTableRecordIndex = binaryReader.ReadUInt32();

        if (fileTableRecordIndex > int.MaxValue)
        {
            throw new NotSupportedException(
                ".MEG files with a file number greater than int32.MaxValue are not supported.");
        }

        var fileSizeInBytes = binaryReader.ReadUInt32();
        var fileStartOffsetInBytes = binaryReader.ReadUInt32();

        var fileNameTableIndex = binaryReader.ReadUInt32();
        if (fileNameTableIndex > int.MaxValue)
        {
            throw new NotSupportedException(
                ".MEG files with a file number greater than int32.MaxValue are not supported.");
        }

        return new MegFileTableRecord(
            crc32,
            fileTableRecordIndex,
            fileSizeInBytes,
            fileStartOffsetInBytes,
            fileNameTableIndex);
    }
}