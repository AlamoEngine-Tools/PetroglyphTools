using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Binary.File.Builder;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MEG.Commons.Exceptions;
using PG.StarWarsGame.Files.MEG.Holder;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Builder
{
    internal class MegFileBuilder : IBinaryFileBuilder<MegFile, MegFileHolder>
    {
        private const int HEADER_STARTING_OFFSET = 0;
        private const int HEADER_NUMBER_OF_FILES_OFFSET = 4;
        private const int FILE_NAME_TABLE_STARTING_OFFSET = 8;
        private readonly Encoding m_fileNameTableEncoding = Encoding.ASCII;
        private readonly IFileSystem m_fileSystem;
        private int m_currentOffset = 0;
        private uint m_numberOfFiles = 0;

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
            List<MegFileNameTableRecord> fileNameTable = new List<MegFileNameTableRecord>();
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
            List<MegFileContentTableRecord> megFileContentTableRecords = new List<MegFileContentTableRecord>();

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

        public MegFile FromHolder(MegFileHolder holder)
        {
            List<string> filePaths =
                holder.Content.Select(megFileDataEntry => megFileDataEntry.RelativeFilePath).ToList();
            List<MegFileNameTableRecord> megFileNameTableRecords = holder.Content
                .Select(megFileDataEntry => new MegFileNameTableRecord(megFileDataEntry.RelativeFilePath)).ToList();
            MegFileNameTable megFileNameTable = new MegFileNameTable(megFileNameTableRecords);
            uint currentOffset = (uint) new MegHeader(0, 0).Size;
            currentOffset += (uint) megFileNameTable.Size;
            List<MegFileContentTableRecord> megFileContentList = new List<MegFileContentTableRecord>();
            for (int i = 0; i < megFileNameTable.MegFileNameTableRecords.Count; i++)
            {
                uint crc32 = ChecksumUtility.GetChecksum(megFileNameTable.MegFileNameTableRecords[i].FileName);
                uint fileSizeInBytes = Convert.ToUInt32(m_fileSystem.FileInfo.FromFileName(filePaths[i]).Length);
                uint fileNameTableIndex = Convert.ToUInt32(i);
                MegFileContentTableRecord megFileContentTableRecord =
                    new MegFileContentTableRecord(crc32, 0, fileSizeInBytes, 0, fileNameTableIndex);
                megFileContentList.Add(megFileContentTableRecord);
                currentOffset += (uint) megFileContentTableRecord.Size;
            }

            megFileContentList.Sort();
            for (int i = 0; i < megFileContentList.Count; i++)
            {
                megFileContentList[i].FileTableRecordIndex = Convert.ToUInt32(i);
                megFileContentList[i].FileStartOffsetInBytes = currentOffset;
                currentOffset += megFileContentList[i].FileSizeInBytes;
            }

            MegFileContentTable megFileContentTable = new MegFileContentTable(megFileContentList);
            return new MegFile(
                new MegHeader((uint) megFileNameTable.MegFileNameTableRecords.Count,
                    (uint) megFileNameTable.MegFileNameTableRecords.Count), megFileNameTable, megFileContentTable);
        }
    }
}