//// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
//// Licensed under the MIT license. See LICENSE file in the project root for details.

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Microsoft.Extensions.Logging;
//using PG.Commons;
//using PG.StarWarsGame.Files.MEG.Binary.V1.Builder;
//using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
//using PG.StarWarsGame.Files.MEG.Data;
//using PG.StarWarsGame.Files.MEG.Files;

//namespace PG.StarWarsGame.Files.MEG.Services;

///// <summary>
/////     Default implementation of <see cref="IMegFileProcessService" /> for
/////     <a href="https://modtools.petrolution.net/docs/MegFileFormat">v1 .MEG files</a>.
///// </summary>
//public sealed class MegFileProcessService : AbstractService
//{
//    private const int BUFFER_SIZE = 4096;

//    /// <summary>
//    /// Initializes a new instance of the <see cref="AbstractService"/> class.
//    /// </summary>
//    /// <param name="services">The service provider for this instance.</param>
//    public MegFileProcessService(IServiceProvider services) : base(services)
//    {
//    }

//    /// <inheritdoc/>
//    /// <exception cref="ArgumentException"></exception>
//    public void PackFilesAsMegArchive(string megArchiveName, IDictionary<string, string> packedFileNameToAbsoluteFilePathsMap, string targetDirectory)
//    {
//        if (!StringUtility.HasText(megArchiveName))
//        {
//            throw new ArgumentException(
//                "The provided argument is null, an empty string or only consists of whitespace.",
//                nameof(megArchiveName));
//        }

//        if (!packedFileNameToAbsoluteFilePathsMap.Any())
//        {
//            throw new ArgumentException(
//                "The provided argument does not contain any elements.",
//                nameof(packedFileNameToAbsoluteFilePathsMap));
//        }

//        if (!StringUtility.HasText(targetDirectory))
//        {
//            throw new ArgumentException(
//                "The provided argument is null, an empty string or only consists of whitespace.",
//                nameof(targetDirectory));
//        }

//        string actualName = StringUtility.RemoveFileExtension(megArchiveName);
//        var megFileHolder = new MegFileHolder(targetDirectory, actualName);
//        foreach (var entry in packedFileNameToAbsoluteFilePathsMap)
//        {
//            megFileHolder.Content.Add(new MegDataEntry(entry.Key, entry.Value));
//        }

//        var builder = new MegFileBinaryReaderV1(FileSystem);
//        var megFile = builder.FileToBinary(megFileHolder, out var filesToStream);
//        var writePath = FileSystem.Path.Combine(megFileHolder.Directory, megFileHolder.FilePath);
//        CreateTargetDirectoryIfNotExists(megFileHolder.Directory);
//        using (var writer =
//               new BinaryWriter(FileSystem.FileStream.Create(writePath, FileMode.Create, FileAccess.Write,
//                   FileShare.None)))
//        {
//            writer.Write(megFile.ToBytes());
//        }

//        foreach (var file in filesToStream)
//        {
//            using Stream readStream = FileSystem.File.OpenRead(file);
//            using var writeStream =
//                FileSystem.FileStream.Create(writePath, FileMode.Append, FileAccess.Write, FileShare.None);
//            var buffer = new byte[BUFFER_SIZE];
//            int bytesRead;
//            while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
//            {
//                writeStream.Write(buffer, 0, bytesRead);
//            }
//        }
//    }

//    /// <inheritdoc/>
//    public void UnpackMegFile(string filePath, string targetDirectory)
//    {
//        var holder = Load(filePath);
//        UnpackMegFile(holder, targetDirectory);
//    }

//    /// <inheritdoc/>
//    public void UnpackMegFile(MegFileHolder holder, string targetDirectory)
//    {
//        CreateTargetDirectoryIfNotExists(targetDirectory);

//        using var readStream = GetBinaryReaderForFileHolder(holder);
//        foreach (var dataEntry in holder.Content)
//        {
//            var filePath = FileSystem.Path.Combine(targetDirectory, dataEntry.FilePath);
//            var path = FileSystem.FileInfo.FromFileName(filePath).Directory.FullName;
//            CreateTargetDirectoryIfNotExists(path);
//            ExtractFileFromMegArchive(readStream, dataEntry, filePath);
//        }
//    }

//    /// <inheritdoc/>
//    /// <exception cref="ArgumentException"></exception>
//    /// <exception cref="FileNotInMegException"></exception>
//    /// <exception cref="MultipleFilesWithMatchingNameInArchiveException"></exception>
//    public void UnpackSingleFileFromMegFile(MegFileHolder holder, string targetDirectory, string fileName,
//        bool preserveDirectoryHierarchy = true)
//    {
//        if (holder.Content == null || !holder.Content.Any())
//        {
//            throw new ArgumentException($"{typeof(MegFileHolder)}:{nameof(holder)} is not valid or contains no data.");
//        }

//        if (!StringUtility.HasText(targetDirectory))
//        {
//            throw new ArgumentException($"{typeof(string)}:{nameof(targetDirectory)} is not valid or contains no data.");
//        }

//        if (!StringUtility.HasText(fileName))
//        {
//            throw new ArgumentException($"{typeof(string)}:{nameof(fileName)} is not valid or contains no data.");
//        }

//        if (!holder.TryGetAllMegFileDataEntriesWithMatchingName(fileName, out var megFileDataEntries))
//        {
//            throw new FileNotInMegException(fileName, holder.FilePath);
//        }

//        if (megFileDataEntries.Count > 1)
//        {
//            throw new AmbiguousFileException(
//                $"There are multiple files matching the search filer \"{fileName}\" in the currently loaded MEG archive \"{holder.FilePath}\". The files cannot be extracted without conflicting overwrites." +
//                $"\nFiles in question: {megFileDataEntries}");
//        }

//        CreateTargetDirectoryIfNotExists(targetDirectory);
//        if (preserveDirectoryHierarchy)
//        {
//            UnpackMegFilePreservingDirectoryHierarchy(holder, targetDirectory, megFileDataEntries[0]);
//        }
//        else
//        {
//            UnpackMegFileFlatDirectoryHierarchy(holder, targetDirectory, megFileDataEntries[0]);
//        }
//    }

//    /// <inheritdoc/>
//    /// <exception cref="ArgumentNullException"></exception>
//    /// <exception cref="FileNotFoundException"></exception>
//    public MegFileHolder Load(string filePath)
//    {
//        if (null == filePath || !StringUtility.HasText(filePath))
//        {
//            throw new ArgumentNullException(nameof(filePath));
//        }

//        if (!FileSystem.File.Exists(filePath))
//        {
//            throw new FileNotFoundException($"The file {filePath} does not exist.");
//        }

//        var headerSize = GetMegFileHeaderSize(filePath);
//        var megFileHeader = new byte[headerSize];
//        using (var reader = new BinaryReader(FileSystem.FileStream.Create(filePath, FileMode.Open)))
//        {
//            reader.Read(megFileHeader, 0, megFileHeader.Length);
//        }

//        var builder = new MegFileBinaryReaderV1();
//        var megFile = builder.FromBytes(megFileHeader);
//        var holder = new MegFileHolder(FileSystem.Path.GetDirectoryName(filePath),
//            FileSystem.Path.GetFileNameWithoutExtension(filePath));
//        for (var i = 0; i < megFile.Header.NumFiles; i++)
//        {
//            var fileName = megFile.FileNameTable[i].FileName;
//            var fileOffset = megFile.FileTable[i].FileStartOffsetInBytes;
//            var fileSize = megFile.FileTable[i].FileSizeInBytes;
//            holder.Content.Add(new MegDataEntry(fileName, Convert.ToInt32(fileOffset), fileSize));
//        }

//        return holder;
//    }

//    private void ExtractFileFromMegArchive(BinaryReader readStream, MegDataEntry dataEntry,
//        string filePath)
//    {
//        var buffer = new byte[BUFFER_SIZE];
//        readStream.BaseStream.Seek(dataEntry.Offset, SeekOrigin.Begin);
//        using var writeStream =
//            FileSystem.FileStream.Create(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
//        int bytesRead;
//        var bytesToWrite = Convert.ToInt32(dataEntry.Size);
//        while ((bytesRead = readStream.Read(buffer, 0, Math.Min(buffer.Length, bytesToWrite))) > 0)
//        {
//            bytesToWrite -= bytesRead;
//            writeStream.Write(buffer, 0, bytesRead);
//        }
//    }

//    private void CreateTargetDirectoryIfNotExists(string targetDirectory)
//    {
//        if (FileSystem.Directory.Exists(targetDirectory))
//        {
//            return;
//        }

//        Logger.LogWarning("The given directory \"{0}\" does not exist. Trying to create it.", targetDirectory);
//        FileSystem.Directory.CreateDirectory(targetDirectory);
//    }

//    private void UnpackMegFilePreservingDirectoryHierarchy(MegFileHolder holder, string targetDirectory,
//        MegDataEntry dataEntry)
//    {
//        var filePath = FileSystem.Path.Combine(targetDirectory, dataEntry.FilePath);
//        var path = FileSystem.FileInfo.FromFileName(filePath).Directory.FullName;
//        CreateTargetDirectoryIfNotExists(path);
//        using var reader = GetBinaryReaderForFileHolder(holder);
//        ExtractFileFromMegArchive(reader, dataEntry, filePath);
//    }

//    private void UnpackMegFileFlatDirectoryHierarchy(MegFileHolder holder, string targetDirectory,
//        MegDataEntry dataEntry)
//    {
//        var filePath = FileSystem.Path.Combine(targetDirectory, dataEntry.FilePath);
//        filePath = FileSystem.Path.Combine(targetDirectory, FileSystem.Path.GetFileName(filePath));
//        using var reader = GetBinaryReaderForFileHolder(holder);
//        ExtractFileFromMegArchive(reader, dataEntry, filePath);
//    }

//    private BinaryReader GetBinaryReaderForFileHolder(MegFileHolder holder)
//    {
//        return new BinaryReader(FileSystem.FileStream.Create(
//            FileSystem.Path.Combine(holder.Directory, $"{holder.FileName}.{holder.FileType.FileExtension}"),
//            FileMode.Open, FileAccess.Read, FileShare.Read));
//    }

//    private uint GetMegFileHeaderSize(string path)
//    {
//        uint headerSize = 0;
//        using var reader = new BinaryReader(FileSystem.FileStream.Create(path, FileMode.Open));
//        var containedFiles = reader.ReadUInt32();
//        uint currentOffset = sizeof(uint) * 2;
//        for (uint i = 0; i < containedFiles; i++)
//        {
//            reader.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
//            var fileNameLenght = reader.ReadUInt16();
//            currentOffset += Convert.ToUInt32(sizeof(ushort) + fileNameLenght);
//        }

//        headerSize += currentOffset;
//        var megContentTableRecordSize = Convert.ToUInt32(new MegFileTableRecord(0, 0, 0, 0, 0).Size);
//        headerSize += megContentTableRecordSize * containedFiles;
//        return headerSize;
//    }
//}
