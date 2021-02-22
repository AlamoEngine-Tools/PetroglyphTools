// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    [Export(nameof(ISortedDatFileProcessService))]
    internal class SortedDatFileProcessService : AService<SortedDatFileProcessService>, ISortedDatFileProcessService
    {
        public SortedDatFileProcessService([CanBeNull] IFileSystem fileSystem,
            [CanBeNull] ILoggerFactory loggerFactory = null) : base(fileSystem, loggerFactory)
        {
        }

        public SortedDatFileHolder LoadFromFile(string filePath)
        {
            string path = FileSystem.Path.GetFullPath(filePath);
            Logger?.LogInformation($"Attempting to load a {nameof(SortedDatFileHolder)} instance from {filePath}");
            if (!FileSystem.File.Exists(path))
            {
                throw new FileNotFoundException($"The file at {filePath} was could not be found.");
            }

            string fileName = FileSystem.Path.GetFileName(path);
            string directoryPath = FileSystem.Path.GetDirectoryName(path);
            Debug.Assert(FileSystem.File != null, "m_fileSystem.File != null");
            DatFile file = new SortedDatFileBuilder().FromBytes(FileSystem.File.ReadAllBytes(filePath));
            return new SortedDatFileHolder(directoryPath, fileName, file);
        }

        public void SaveToFile(SortedDatFileHolder sortedDatFileHolder)
        {
            using BinaryWriter writer = new BinaryWriter(FileSystem.File.Open(
                FileSystem.Path.Combine(sortedDatFileHolder.FilePath, sortedDatFileHolder.FileName),
                FileMode.CreateNew));
            DatFile datFile = new SortedDatFileBuilder().FromHolder(sortedDatFileHolder);
            writer.Write(datFile.ToBytes());
        }
    }
}
