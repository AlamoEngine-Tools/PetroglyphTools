// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using PG.Commons;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Default implementation of the <see cref="ISortedDatFileProcessService"/> interface.
/// </summary>
public sealed class SortedDatFileProcessService : AbstractService<SortedDatFileProcessService>,
    ISortedDatFileProcessService
{
    public SortedDatFileProcessService(IServiceProvider services) : base(services)
    {
    }

    public SortedDatFileHolder LoadFromFile(string filePath)
    {
        var path = FileSystem.Path.GetFullPath(filePath);
        Logger.LogInformation("Attempting to load a {0} instance from {1}", nameof(SortedDatFileHolder), filePath);
        if (!FileSystem.File.Exists(path))
        {
            throw new FileNotFoundException($"The file at {filePath} was could not be found.");
        }

        var fileName = FileSystem.Path.GetFileName(path);
        var directoryPath = FileSystem.Path.GetDirectoryName(path);
        Debug.Assert(FileSystem.File != null, "FileSystem.File != null");
        var file = new SortedDatFileBuilder().FromBytes(FileSystem.File.ReadAllBytes(filePath));
        return new SortedDatFileHolder(directoryPath, fileName, file);
    }

    public void SaveToFile(SortedDatFileHolder sortedDatFileHolder)
    {
        using var writer = new BinaryWriter(FileSystem.File.Open(
            FileSystem.Path.Combine(sortedDatFileHolder.Directory, sortedDatFileHolder.FileName), FileMode.CreateNew));
        var datFile = new SortedDatFileBuilder().FromHolder(sortedDatFileHolder);
        writer.Write(datFile.ToBytes());
    }
}
