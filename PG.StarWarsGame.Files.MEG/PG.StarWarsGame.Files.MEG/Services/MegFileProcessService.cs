using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Binary.File.Builder;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MEG.Holder;

namespace PG.StarWarsGame.Files.MEG.Services
{
    [Export(nameof(IMegFileProcessService))]
    internal class MegFileProcessService : IMegFileProcessService
    {
        private readonly ILogger m_logger;
        [NotNull] private readonly IFileSystem m_fileSystem;

        public MegFileProcessService(IFileSystem fileSystem, ILogger logger = null)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            m_logger = logger;
        }

        public void PackFilesAsMegArchive(string megArchiveName, string baseDirectoryPath,
            List<string> absoluteFilePaths,
            string targetDirectory)
        {
            throw new System.NotImplementedException();
        }

        public void UnpackMegFile(string filePath, string targetDirectory)
        {
            MegFileHolder holder = Load(filePath);
            UnpackMegFile(holder, targetDirectory);
        }

        public void UnpackMegFile(MegFileHolder holder, string targetDirectory)
        {
            if (!m_fileSystem.Directory.Exists(targetDirectory))
            {
                m_logger?.LogWarning($"The given directory does not exist. Trying to create it.");
                m_fileSystem.Directory.CreateDirectory(targetDirectory);
            }

            using BinaryReader reader = new BinaryReader(m_fileSystem.FileStream.Create(
                m_fileSystem.Path.Combine(holder.FilePath, $"{holder.FileName}.{holder.FileType.FileExtension}"),
                FileMode.Open));
            foreach (MegFileDataEntry megFileDataEntry in holder.Content)
            {
                string filePath = m_fileSystem.Path.Combine(targetDirectory, megFileDataEntry.RelativeFilePath);
                string path = m_fileSystem.FileInfo.FromFileName(filePath).Directory.FullName;
                if (!m_fileSystem.Directory.Exists(path))
                {
                    m_logger?.LogWarning($"The given directory does not exist. Trying to create it.");
                    m_fileSystem.Directory.CreateDirectory(path);
                }
                byte[] file = new byte[megFileDataEntry.Size];
                reader.BaseStream.Seek(megFileDataEntry.Offset, SeekOrigin.Begin);
                reader.Read(file, 0, file.Length);
                m_fileSystem.File.WriteAllBytes(filePath, file);
            }
        }

        public void UnpackMegFile(MegFileHolder holder, string targetDirectory, string fileName, bool preserveDirectoryHierarchy = true)
        {
            throw new System.NotImplementedException();
        }

        public MegFileHolder Load(string filePath)
        {
            if (null == filePath || !StringUtility.HasText(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!m_fileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist.");
            }

            uint headerSize = GetMegFileHeaderSize(filePath);
            byte[] megFileHeader = new byte[headerSize];
            //TODO [gruenwaldlu, 2020-10-06-10:47:46+2]: Update to IFIleSystem!
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                reader.Read(megFileHeader, 0, megFileHeader.Length);
            }

            MegFileBuilder builder = new MegFileBuilder();
            MegFile megFile = builder.FromBytes(megFileHeader);
            MegFileHolder holder = new MegFileHolder(m_fileSystem.Path.GetDirectoryName(filePath),
                m_fileSystem.Path.GetFileNameWithoutExtension(filePath));
            for (int i = 0; i < megFile.Header.NumFiles; i++)
            {
                string fileName = megFile.FileNameTable.MegFileNameTableRecords[i].FileName;
                uint fileOffset = megFile.FileContentTable.MegFileContentTableRecords[i].FileStartOffsetInBytes;
                int fileSize = megFile.FileNameTable.MegFileNameTableRecords[i].Size;
                holder.Content.Add(new MegFileDataEntry(fileName, Convert.ToInt32(fileOffset), fileSize));
            }

            return holder;
        }

        private uint GetMegFileHeaderSize([NotNull] string path)
        {
            uint headerSize = 0;
            using BinaryReader reader = new BinaryReader(m_fileSystem.FileStream.Create(path, FileMode.Open));
            uint containedFiles = reader.ReadUInt32();
            uint currentOffset = sizeof(uint) * 2;
            for (uint i = 0; i < containedFiles; i++)
            {
                reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
                ushort fileNameLenght = reader.ReadUInt16();
                currentOffset += Convert.ToUInt32(sizeof(ushort) + fileNameLenght);
            }

            headerSize += currentOffset;
            uint megContentTableRecordSize = Convert.ToUInt32(new MegFileContentTableRecord(0, 0, 0, 0, 0).Size);
            headerSize += megContentTableRecordSize * containedFiles;
            return headerSize;
        }
    }
}