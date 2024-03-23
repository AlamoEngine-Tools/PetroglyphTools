// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Binary;

namespace PG.StarWarsGame.Files.DAT.Services;

internal class DatFileService(IServiceProvider services) : ServiceBase(services), IDatFileService
{
    public void CreateDatFile(FileSystemStream fileStream, IEnumerable<DatStringEntry> entries, DatFileType datFileType)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        var datModel = new ConstructingDatModel(entries, datFileType);

        var datBinary = Services.GetRequiredService<IDatBinaryConverter>().ModelToBinary(datModel);

#if NETSTANDARD2_1_OR_GREATER || NET
        fileStream.Write(datBinary.Bytes);
#else
        fileStream.Write(datBinary.Bytes, 0, datBinary.Size);
#endif
    }

    public IDatFile Load(string filePath)
    {
        var datBinary = Services.GetRequiredService<IDatFileReader>()
            .ReadBinary(FileSystem.FileStream.New(filePath, FileMode.Open));

        var converter = Services.GetRequiredService<IDatBinaryConverter>();
        var datModel = converter.BinaryToModel(datBinary);

        var fileInfo = new DatFileInformation { FilePath = filePath };

        return new DatFile(datModel, fileInfo, Services);
    }

    public DatFileType GetDatFileType(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));

        var reader = Services.GetRequiredService<IDatFileReader>();

        return reader.PeekFileType(FileSystem.FileStream.New(filePath, FileMode.Open));
    }
}