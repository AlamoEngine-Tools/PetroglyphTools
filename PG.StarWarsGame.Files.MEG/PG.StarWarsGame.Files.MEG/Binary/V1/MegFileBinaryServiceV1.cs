// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal class MegFileBinaryServiceV1 : MegFileBinaryServiceBase<MegMetadata, MegHeader, MegFileTable>
{
    public MegFileBinaryServiceV1(IServiceProvider services) : base(services)
    {
    }

    protected internal override MegMetadata CreateMegMetadata(MegHeader header, MegFileNameTable fileNameTable, MegFileTable fileTable)
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
        var megFileContentTableRecords = new List<MegFileContentTableRecord>(fileNumber);

        for (var i = 0; i < fileNumber; i++)
        {
            var crc32 = new Crc32(binaryReader.ReadUInt32());
            var fileTableRecordIndex = binaryReader.ReadUInt32();

            if (fileTableRecordIndex > int.MaxValue)
            {
                throw new NotSupportedException(
                    ".MEG files with a file number greater than int32.MaxValue are not supported.");
            }

            Debug.Assert(fileTableRecordIndex == i);

            var fileSizeInBytes = binaryReader.ReadUInt32();
            var fileStartOffsetInBytes = binaryReader.ReadUInt32();

            var fileNameTableIndex = binaryReader.ReadUInt32();
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

    //private IEnumerable<MegFileDataEntry> CollectSortedMegFileDataEntries(IMegFile holder,
    //    MegFileNameTableRecord megFileNameTableRecord)
    //{
    //    return holder.Content.Where(megFileDataEntry =>
    //        megFileNameTableRecord.FileName.Equals(
    //            megFileDataEntry.RelativeFilePath.Replace("\\", "/").Replace("\0", string.Empty),
    //            StringComparison.InvariantCultureIgnoreCase));
    //}

    //public MegMetadata FromHolder(IMegFile holder)
    //{
    //    return FromHolder(holder, out var _);
    //}

    //public IMegFile ToHolder(MegFileHolderParam param, MegMetadata model)
    //{
    //    throw new NotImplementedException();
    //}
}