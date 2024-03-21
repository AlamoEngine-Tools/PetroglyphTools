// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using AnakinRaW.CommonUtilities;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Binary;

namespace PG.StarWarsGame.Files.DAT.Services;

internal class DatFileService(IServiceProvider services) : ServiceBase(services), IDatFileService
{
    public void StoreDatFile(string datFileName, string targetDirectory, IEnumerable<DatFileEntry> entries, DatFileType datFileType)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(datFileName);
        ThrowHelper.ThrowIfNullOrWhiteSpace(targetDirectory);

        IList<DatFileEntry> entryList = entries.ToList();

        if (!entryList.Any())
        {
            throw new ArgumentException($"No valid dat file entries have been provided.", nameof(entries));
        }

        if (!FileSystem.Directory.Exists(targetDirectory))
        {
            FileSystem.Directory.CreateDirectory(targetDirectory);
        }

        var absoluteFilePath = FileSystem.Path.Combine(targetDirectory, datFileName);

        if (datFileType == DatFileType.OrderedByCrc32)
        {
            entryList = Crc32Utilities.SortByCrc32(entryList);
        }

        var datModel = new DatModel(entryList, datFileType);

        var datBinary = Services.GetRequiredService<IDatBinaryConverter>().ModelToBinary(datModel);

        throw new NotImplementedException();
    }

    public IDatFile LoadDatFile(string filePath)
    {
        var datBinary = Services.GetRequiredService<IDatFileReader>()
            .ReadBinary(FileSystem.FileStream.New(filePath, FileMode.Open));

        var converter = Services.GetRequiredService<IDatBinaryConverter>();
        var datModel = converter.BinaryToModel(datBinary);

        var fileInfo = new DatFileInformation { FilePath = filePath };

        return new DatFile(datModel, fileInfo, Services);
    }

    public DatFileType PeekDatFileType(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));

        var reader = Services.GetRequiredService<IDatFileReader>();

        return reader.PeekFileType(FileSystem.FileStream.New(filePath, FileMode.Open));
    }
}