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
    [Export(nameof(ISortedDatFileProcessService))]
    internal class SortedDatFileProcessService : ISortedDatFileProcessService
    {
        private readonly ILogger m_logger;
        [NotNull] private readonly IFileSystem m_fileSystem;

        public SortedDatFileProcessService(IFileSystem fileSystem, ILogger logger = null)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            m_logger = logger;
        }

        public SortedDatFileHolder LoadFromFile(string filePath)
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
            return new SortedDatFileHolder(directoryPath, fileName, file);
        }

        public void SaveToFile(SortedDatFileHolder sortedDatFileHolder)
        {
            using (BinaryWriter writer = new BinaryWriter(m_fileSystem.File.Open(
                m_fileSystem.Path.Combine(sortedDatFileHolder.FilePath, sortedDatFileHolder.FileName),
                FileMode.CreateNew)))
            {
                DatFile datFile = new SortedDatFileBuilder().FromHolder(sortedDatFileHolder);
                writer.Write(datFile.ToBytes());
            }
        }
    }
}
