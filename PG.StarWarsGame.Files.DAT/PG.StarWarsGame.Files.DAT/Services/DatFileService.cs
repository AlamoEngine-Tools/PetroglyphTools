// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PG.Commons.Common.Exceptions;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

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

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatBinaryConverter)}."));
        
        factory.GetWriter().WriteBinary(absoluteFilePath, factory.GetConverter().ModelToBinary(datModel));
    }

    public IDatFile LoadDatFile(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatBinaryConverter)}."));

        var fileInfo = new DatFileInformation { FilePath = filePath };

        var datModel =  factory.GetConverter()
            .BinaryToModel(factory.GetReader().ReadBinary(FileSystem.FileStream.New(filePath, FileMode.Open)));

        return new DatFileHolder(datModel, fileInfo, Services);
    }

    public DatFileType PeekDatFileType(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatBinaryConverter)}."));
        var reader = (DatFileReader)factory.GetReader();
        return reader.PeekFileType(FileSystem.FileStream.New(filePath, FileMode.Open));
    }
}