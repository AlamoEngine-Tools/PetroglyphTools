// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Util;
using PG.Core.Attributes;
using PG.Core.Services;
using PG.StarWarsGame.Files.MEG.Binary.File.Builder.V1;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1;
using PG.StarWarsGame.Files.MEG.Commons.Exceptions;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Services.V1
{
    /// <summary>
    ///     Default implementation of <see cref="IMegFileProcessService" /> for
    ///     <a href="https://modtools.petrolution.net/docs/MegFileFormat">v1 <code>*.MEG</code> files</a>.
    ///     When requesting the default implementation via an IoC Container or registering via injection, you may pass
    ///     a file system as argument implementing <see cref="System.IO.Abstractions.IFileSystem" /> and a logger factory
    ///     implementing <see cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    /// </summary>
    [Export(nameof(IMegFileProcessService))]
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public sealed class MegFileProcessService : AbstractService<MegFileProcessService>, IMegFileProcessService
    {
        private const int BUFFER_SIZE = 4096;

        public MegFileProcessService(IServiceProvider services) : base(services)
        {
        }

        public void PackFilesAsMegArchive(string megArchiveName,
            IDictionary<string, string> packedFileNameToAbsoluteFilePathsMap, string targetDirectory)
        {
            if (!StringUtility.HasText(megArchiveName))
            {
                throw new ArgumentException(
                    "The provided argument is null, an empty string or only consists of whitespace.",
                    nameof(megArchiveName));
            }

            if (!packedFileNameToAbsoluteFilePathsMap.Any())
            {
                throw new ArgumentException(
                    "The provided argument does not contain any elements.",
                    nameof(packedFileNameToAbsoluteFilePathsMap));
            }

            if (!StringUtility.HasText(targetDirectory))
            {
                throw new ArgumentException(
                    "The provided argument is null, an empty string or only consists of whitespace.",
                    nameof(targetDirectory));
            }

            string actualName = StringUtility.RemoveFileExtension(megArchiveName);
            MegFileHolder megFileHolder = new(targetDirectory, actualName);
            foreach ((string key, string value) in packedFileNameToAbsoluteFilePathsMap)
            {
                megFileHolder.Content.Add(new MegFileDataEntry(key, value));
            }

            MegFileBuilder builder = new(FileSystem);
            MegFile megFile = builder.FromHolder(megFileHolder, out IList<string> filesToStream);
            string writePath = FileSystem.Path.Combine(megFileHolder.FilePath, megFileHolder.FullyQualifiedName);
            CreateTargetDirectoryIfNotExists(megFileHolder.FilePath);
            using (BinaryWriter writer =
                new(FileSystem.FileStream.Create(writePath, FileMode.Create, FileAccess.Write,
                    FileShare.None)))
            {
                writer.Write(megFile.ToBytes());
            }

            foreach (string file in filesToStream)
            {
                using Stream readStream = FileSystem.File.OpenRead(file);
                using Stream writeStream =
                    FileSystem.FileStream.Create(writePath, FileMode.Append, FileAccess.Write, FileShare.None);
                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesRead;
                while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                }
            }
        }

        public void UnpackMegFile(string filePath, string targetDirectory)
        {
            MegFileHolder holder = Load(filePath);
            UnpackMegFile(holder, targetDirectory);
        }

        public void UnpackMegFile(MegFileHolder holder, string targetDirectory)
        {
            CreateTargetDirectoryIfNotExists(targetDirectory);

            using BinaryReader readStream = GetBinaryReaderForFileHolder(holder);
            foreach (MegFileDataEntry megFileDataEntry in holder.Content)
            {
                string filePath = FileSystem.Path.Combine(targetDirectory, megFileDataEntry.RelativeFilePath);
                string path = FileSystem.FileInfo.FromFileName(filePath).Directory.FullName;
                CreateTargetDirectoryIfNotExists(path);
                ExtractFileFromMegArchive(readStream, megFileDataEntry, filePath);
            }
        }

        public void UnpackSingleFileFromMegFile(MegFileHolder holder, string targetDirectory, string fileName,
            bool preserveDirectoryHierarchy = true)
        {
            if (holder.Content == null || !holder.Content.Any())
            {
                throw new ArgumentException(
                    $"{typeof(MegFileHolder)}:{nameof(holder)} is not valid or contains no data.");
            }

            if (!StringUtility.HasText(targetDirectory))
            {
                throw new ArgumentException(
                    $"{typeof(string)}:{nameof(targetDirectory)} is not valid or contains no data.");
            }

            if (!StringUtility.HasText(fileName))
            {
                throw new ArgumentException(
                    $"{typeof(string)}:{nameof(fileName)} is not valid or contains no data.");
            }

            if (!holder.TryGetAllMegFileDataEntriesWithMatchingName(fileName,
                out IList<MegFileDataEntry> megFileDataEntries))
            {
                throw new FileNotContainedInArchiveException(
                    $"The file \"{fileName}\" is not contained in the currently loaded MEG archive \"{holder.FullyQualifiedName}\"");
            }

            if (megFileDataEntries.Count > 1)
            {
                throw new MultipleFilesWithMatchingNameInArchiveException(
                    $"There are multiple files matching the search filer \"{fileName}\" in the currently loaded MEG archive \"{holder.FullyQualifiedName}\". The files cannot be extracted without conflicting overwrites." +
                    $"\nFiles in question: {megFileDataEntries}");
            }

            CreateTargetDirectoryIfNotExists(targetDirectory);
            if (preserveDirectoryHierarchy)
            {
                UnpackMegFilePreservingDirectoryHierarchy(holder, targetDirectory, megFileDataEntries[0]);
            }
            else
            {
                UnpackMegFileFlatDirectoryHierarchy(holder, targetDirectory, megFileDataEntries[0]);
            }
        }

        public MegFileHolder Load(string filePath)
        {
            if (null == filePath || !StringUtility.HasText(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!FileSystem.File.Exists(filePath))
            {
                throw new FileNotFoundException($"The file {filePath} does not exist.");
            }

            uint headerSize = GetMegFileHeaderSize(filePath);
            byte[] megFileHeader = new byte[headerSize];
            using (BinaryReader reader = new(FileSystem.FileStream.Create(filePath, FileMode.Open)))
            {
                reader.Read(megFileHeader, 0, megFileHeader.Length);
            }

            MegFileBuilder builder = new();
            MegFile megFile = builder.FromBytes(megFileHeader);
            MegFileHolder holder = new(FileSystem.Path.GetDirectoryName(filePath),
                FileSystem.Path.GetFileNameWithoutExtension(filePath));
            for (int i = 0; i < megFile.Header.NumFiles; i++)
            {
                string fileName = megFile.FileNameTable[i].FileName;
                uint fileOffset = megFile.FileContentTable[i].FileStartOffsetInBytes;
                uint fileSize = megFile.FileContentTable[i].FileSizeInBytes;
                holder.Content.Add(new MegFileDataEntry(fileName, Convert.ToInt32(fileOffset), fileSize));
            }

            return holder;
        }

        private void ExtractFileFromMegArchive(BinaryReader readStream, MegFileDataEntry megFileDataEntry,
            string filePath)
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            readStream.BaseStream.Seek(megFileDataEntry.Offset, SeekOrigin.Begin);
            using Stream writeStream =
                FileSystem.FileStream.Create(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
            int bytesRead;
            int bytesToWrite = Convert.ToInt32(megFileDataEntry.Size);
            while ((bytesRead = readStream.Read(buffer, 0, Math.Min(buffer.Length, bytesToWrite))) > 0)
            {
                bytesToWrite -= bytesRead;
                writeStream.Write(buffer, 0, bytesRead);
            }
        }

        private void CreateTargetDirectoryIfNotExists(string targetDirectory)
        {
            if (FileSystem.Directory.Exists(targetDirectory))
            {
                return;
            }

            Logger.LogWarning("The given directory \"{0}\" does not exist. Trying to create it.", targetDirectory);
            FileSystem.Directory.CreateDirectory(targetDirectory);
        }

        private void UnpackMegFilePreservingDirectoryHierarchy(MegFileHolder holder, string targetDirectory,
            MegFileDataEntry megFileDataEntry)
        {
            string filePath = FileSystem.Path.Combine(targetDirectory, megFileDataEntry.RelativeFilePath);
            string path = FileSystem.FileInfo.FromFileName(filePath).Directory.FullName;
            CreateTargetDirectoryIfNotExists(path);
            using BinaryReader reader = GetBinaryReaderForFileHolder(holder);
            ExtractFileFromMegArchive(reader, megFileDataEntry, filePath);
        }

        private void UnpackMegFileFlatDirectoryHierarchy(MegFileHolder holder, string targetDirectory,
            MegFileDataEntry megFileDataEntry)
        {
            string filePath = FileSystem.Path.Combine(targetDirectory, megFileDataEntry.RelativeFilePath);
            filePath = FileSystem.Path.Combine(targetDirectory, FileSystem.Path.GetFileName(filePath));
            using BinaryReader reader = GetBinaryReaderForFileHolder(holder);
            ExtractFileFromMegArchive(reader, megFileDataEntry, filePath);
        }

        private BinaryReader GetBinaryReaderForFileHolder(MegFileHolder holder)
        {
            return new BinaryReader(FileSystem.FileStream.Create(
                FileSystem.Path.Combine(holder.FilePath, $"{holder.FileName}.{holder.FileType.FileExtension}"),
                FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        private uint GetMegFileHeaderSize([NotNull] string path)
        {
            uint headerSize = 0;
            using BinaryReader reader = new(FileSystem.FileStream.Create(path, FileMode.Open));
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
