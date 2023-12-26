// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatFileWriter(IServiceProvider services) : ServiceBase(services), IDatFileWriter
{
    public void WriteBinary(string absoluteTargetFilePath, IDatFileMetadata model)
    {
        string directory = FileSystem.Path.GetDirectoryName(absoluteTargetFilePath) ??
                           throw new ArgumentException(
                               $"No valid directory could be extracted from the provided path {absoluteTargetFilePath}",
                               nameof(absoluteTargetFilePath));
        FileSystem.Directory.CreateDirectory(directory);
        using FileSystemStream stream = FileSystem.File.Create(absoluteTargetFilePath);
        byte[] data = model.Bytes;
        stream.Write(data, 0, data.Length);
    }
}