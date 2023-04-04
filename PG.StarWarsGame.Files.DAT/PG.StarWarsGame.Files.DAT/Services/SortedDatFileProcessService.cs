// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.Logging;
using PG.Core.Attributes;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;
using System;
using System.Diagnostics;
using System.IO;

namespace PG.StarWarsGame.Files.DAT.Services
{
    /// <summary>
    /// Default implementation of the <see cref="ISortedDatFileProcessService"/> interface.
    /// </summary>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public sealed class SortedDatFileProcessService : AbstractService<SortedDatFileProcessService>,
        ISortedDatFileProcessService
    {
        public SortedDatFileProcessService(IServiceProvider services) : base(services)
        {
        }

        public SortedDatFileHolder LoadFromFile(string filePath)
        {
            string path = FileSystem.Path.GetFullPath(filePath);
            Logger.LogInformation("Attempting to load a {0} instance from {1}", nameof(SortedDatFileHolder), filePath);
            if (!FileSystem.File.Exists(path))
            {
                throw new FileNotFoundException($"The file at {filePath} was could not be found.");
            }

            string fileName = FileSystem.Path.GetFileName(path);
            string directoryPath = FileSystem.Path.GetDirectoryName(path);
            Debug.Assert(FileSystem.File != null, "FileSystem.File != null");
            DatFile file = new SortedDatFileBuilder().FromBytes(FileSystem.File.ReadAllBytes(filePath));
            return new SortedDatFileHolder(directoryPath, fileName, file);
        }

        public void SaveToFile(SortedDatFileHolder sortedDatFileHolder)
        {
            using BinaryWriter writer = new(FileSystem.File.Open(
                FileSystem.Path.Combine(sortedDatFileHolder.FilePath, sortedDatFileHolder.FileName),
                FileMode.CreateNew));
            DatFile datFile = new SortedDatFileBuilder().FromHolder(sortedDatFileHolder);
            writer.Write(datFile.ToBytes());
        }
    }
}