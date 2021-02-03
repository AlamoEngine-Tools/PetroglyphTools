// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Composition;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    [Export(nameof(IUnsortedDatFileProcessService))]
    internal class UnsortedDatFileProcessService : IUnsortedDatFileProcessService
    {
        [CanBeNull] private readonly ILogger m_logger;
        [NotNull] private readonly IFileSystem m_fileSystem;

        public UnsortedDatFileProcessService([CanBeNull] IFileSystem fileSystem,
            [CanBeNull] ILoggerFactory loggerFactory = null)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            m_logger = loggerFactory?.CreateLogger<UnsortedDatFileProcessService>();
        }

        public UnsortedDatFileHolder LoadFromFile(string filePath)
        {
            string path = m_fileSystem.Path.GetFullPath(filePath);
            m_logger?.LogInformation($"Attempting to load a {nameof(SortedDatFileHolder)} instance from {filePath}");
            if (!m_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException($"The file at {filePath} was could not be found.");
            }

            string fileName = m_fileSystem.Path.GetFileName(path);
            string directoryPath = m_fileSystem.Path.GetDirectoryName(path);
            Debug.Assert(m_fileSystem.File != null, "m_fileSystem.File != null");
            DatFile file = new SortedDatFileBuilder().FromBytes(m_fileSystem.File.ReadAllBytes(filePath));
            return new UnsortedDatFileHolder(directoryPath, fileName, file);
        }

        public void SaveToFile(UnsortedDatFileHolder unsortedDatFileHolder)
        {
            using BinaryWriter writer = new BinaryWriter(m_fileSystem.File.Open(
                m_fileSystem.Path.Combine(unsortedDatFileHolder.FilePath, unsortedDatFileHolder.FileName),
                FileMode.CreateNew));
            DatFile datFile = new UnsortedDatFileBuilder().FromHolder(unsortedDatFileHolder);
            writer.Write(datFile.ToBytes());
        }
    }
}
