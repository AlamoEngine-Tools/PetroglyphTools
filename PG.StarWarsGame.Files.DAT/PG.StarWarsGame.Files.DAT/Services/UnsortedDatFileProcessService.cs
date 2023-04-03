// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Core.Attributes;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    /// <summary>
    /// Base implementation of the <see cref="IUnsortedDatFileProcessService"/> interface. 
    /// </summary>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public sealed class UnsortedDatFileProcessService : AbstractService<UnsortedDatFileProcessService>,
        IUnsortedDatFileProcessService
    {
        public UnsortedDatFileProcessService(IServiceProvider services) : base(services)
        {
        }

        public UnsortedDatFileHolder LoadFromFile(string filePath)
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
            return new UnsortedDatFileHolder(directoryPath, fileName, file);
        }

        public void SaveToFile(UnsortedDatFileHolder unsortedDatFileHolder)
        {
            using BinaryWriter writer = new BinaryWriter(FileSystem.File.Open(
                FileSystem.Path.Combine(unsortedDatFileHolder.FilePath, unsortedDatFileHolder.FileName),
                FileMode.CreateNew));
            DatFile datFile = new UnsortedDatFileBuilder().FromHolder(unsortedDatFileHolder);
            writer.Write(datFile.ToBytes());
        }
    }
}
