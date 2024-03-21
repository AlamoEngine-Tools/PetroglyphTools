// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.DAT.Services;

internal class DatFileService(IServiceProvider services) : ServiceBase(services), IDatFileService
{
    private IDatBinaryServiceFactory BinaryServiceFactory { get; } = 
        services.GetService<IDatBinaryServiceFactory>() ??
        throw new LibraryInitialisationException(typeof(IDatBinaryServiceFactory));

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

        BinaryServiceFactory.GetWriter().WriteBinary(absoluteFilePath, BinaryServiceFactory.GetConverter().ModelToBinary(datModel));
    }

    public IDatFile LoadDatFile(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var fileInfo = new DatFileInformation { FilePath = filePath };

        var datModel = BinaryServiceFactory.GetConverter()
            .BinaryToModel(BinaryServiceFactory.GetReader().ReadBinary(FileSystem.FileStream.New(filePath, FileMode.Open)));

        return new DatFileHolder(datModel, fileInfo, Services);
    }

    public DatFileType PeekDatFileType(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var reader = BinaryServiceFactory.GetReader();

        return reader.PeekFileType(FileSystem.FileStream.New(filePath, FileMode.Open));
    }
}