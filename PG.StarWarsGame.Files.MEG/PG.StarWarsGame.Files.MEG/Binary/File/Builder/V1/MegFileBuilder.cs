// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary.File.Builder;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1;
using PG.StarWarsGame.Files.MEG.Commons.Exceptions;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace PG.StarWarsGame.Files.MEG.Binary.File.Builder.V1
{
    internal class MegFileBuilder : IBinaryFileBuilder<MegFile, MegFileHolder>
    {
        private const int HEADER_STARTING_OFFSET = 0;
        private const int HEADER_NUMBER_OF_FILES_OFFSET = 4;
        private const int FILE_NAME_TABLE_STARTING_OFFSET = 8;
        private readonly Encoding m_fileNameTableEncoding = Encoding.ASCII;
        private readonly IFileSystem m_fileSystem;
        private int m_currentOffset;
        private uint m_numberOfFiles;

        internal MegFileBuilder(IFileSystem fs = null)
        {
            m_fileSystem = fs ?? new FileSystem();
        }

        public MegFile FromBytes(byte[] byteStream)
        {
            if (byteStream == null || byteStream.Length < 1)
            {
                throw new ArgumentNullException(nameof(byteStream), "The provided file is empty.");
            }

            MegHeader header = BuildMegHeaderInternal(byteStream);
            MegFileNameTable megFileNameTable = BuildFileNameTableInternal(byteStream);
            MegFileContentTable megFileContentTable = BuildFileContentTableInternal(byteStream);
            return new MegFile(header, megFileNameTable, megFileContentTable);
        }

        public MegFile FromHolder(MegFileHolder holder, out IList<string> filesToStream)
        {
            List<string> files = holder.Content.Select(megFileDataEntry => megFileDataEntry.RelativeFilePath).ToList();
            List<MegFileNameTableRecord> megFileNameTableRecords =
                files.Select(file => new MegFileNameTableRecord(file)).ToList();
            megFileNameTableRecords.Sort();
            filesToStream = new List<string>();
            foreach (MegFileNameTableRecord megFileNameTableRecord in megFileNameTableRecords)
            {
                foreach (MegFileDataEntry megFileDataEntry in CollectSortedMegFileDataEntries(holder, megFileNameTableRecord))
                {
                    filesToStream.Add(megFileDataEntry.AbsoluteFilePath);
                    break;
                }
            }

            List<MegFileContentTableRecord> megFileContentTableRecords = new();
            for (int i = 0; i < megFileNameTableRecords.Count; i++)
            {
                uint crc32 = ChecksumUtility.GetChecksum(megFileNameTableRecords[i].FileName);
                uint fileTableRecordIndex = Convert.ToUInt32(i);
                uint fileSizeInBytes = Convert.ToUInt32(m_fileSystem.FileInfo.New(filesToStream[i]).Length);
                uint fileNameTableIndex = Convert.ToUInt32(i);
                megFileContentTableRecords.Add(new MegFileContentTableRecord(crc32, fileTableRecordIndex,
                    fileSizeInBytes, 0, fileNameTableIndex));
            }

            MegHeader header = new(Convert.ToUInt32(megFileContentTableRecords.Count),
                Convert.ToUInt32(megFileContentTableRecords.Count));
            MegFileNameTable megFileNameTable = new(megFileNameTableRecords);
            uint currentOffset = Convert.ToUInt32(header.Size);
            currentOffset += Convert.ToUInt32(megFileNameTable.Size);
            MegFileContentTable t = new(megFileContentTableRecords);
            currentOffset += Convert.ToUInt32(t.Size);
            foreach (MegFileContentTableRecord megFileContentTableRecord in megFileContentTableRecords)
            {
                megFileContentTableRecord.FileStartOffsetInBytes = currentOffset;
                currentOffset += Convert.ToUInt32(megFileContentTableRecord.FileSizeInBytes);
            }

            MegFileContentTable megFileContentTable = new(megFileContentTableRecords);
            return new MegFile(header, megFileNameTable, megFileContentTable);
        }

        private static IEnumerable<MegFileDataEntry> CollectSortedMegFileDataEntries(MegFileHolder holder, MegFileNameTableRecord megFileNameTableRecord)
        {
            return holder.Content.Where(megFileDataEntry =>
                megFileNameTableRecord.FileName.Equals(
                    megFileDataEntry.RelativeFilePath.Replace("\\", "/").Replace("\0", string.Empty),
                    StringComparison.InvariantCultureIgnoreCase));
        }

        public MegFile FromHolder(MegFileHolder holder)
        {
            return FromHolder(holder, out IList<string> _);
        }

        private MegHeader BuildMegHeaderInternal(byte[] byteStream)
        {
            uint numFileNames = BitConverter.ToUInt32(byteStream, HEADER_STARTING_OFFSET);
            uint numFiles = BitConverter.ToUInt32(byteStream, HEADER_NUMBER_OF_FILES_OFFSET);

            if (numFiles != numFileNames)
            {
                throw new MegFileCorruptedException(
                    $"The provided meg file is corrupt. The number of file names ({numFileNames}) has to be equal to the number of files ({numFiles})");
            }

            m_numberOfFiles = numFiles;
            return new MegHeader(numFileNames, numFiles);
        }

        private MegFileNameTable BuildFileNameTableInternal(byte[] byteStream)
        {
            List<MegFileNameTableRecord> fileNameTable = new();
            m_currentOffset = FILE_NAME_TABLE_STARTING_OFFSET;
            for (uint i = 0; i < m_numberOfFiles; i++)
            {
                ushort fileNameLength = BitConverter.ToUInt16(byteStream, m_currentOffset);
                string fileName =
                    m_fileNameTableEncoding.GetString(byteStream, m_currentOffset + sizeof(ushort), fileNameLength);
                m_currentOffset = m_currentOffset + sizeof(ushort) + fileNameLength;
                fileNameTable.Add(new MegFileNameTableRecord(fileName));
            }

            return new MegFileNameTable(fileNameTable);
        }

        private MegFileContentTable BuildFileContentTableInternal(byte[] byteStream)
        {
            List<MegFileContentTableRecord> megFileContentTableRecords = new();

            for (uint i = 0; i < m_numberOfFiles; i++)
            {
                uint crc32 = BitConverter.ToUInt32(byteStream, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint fileTableRecordIndex = BitConverter.ToUInt32(byteStream, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint fileSizeInBytes = BitConverter.ToUInt32(byteStream, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint fileStartOffsetInBytes = BitConverter.ToUInt32(byteStream, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint fileNameTableIndex = BitConverter.ToUInt32(byteStream, m_currentOffset);
                m_currentOffset += sizeof(uint);
                megFileContentTableRecords.Add(new MegFileContentTableRecord(
                    crc32,
                    fileTableRecordIndex,
                    fileSizeInBytes,
                    fileStartOffsetInBytes,
                    fileNameTableIndex));
            }

            return new MegFileContentTable(megFileContentTableRecords);
        }
    }
}
