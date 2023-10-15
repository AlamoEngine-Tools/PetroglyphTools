// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using PG.Commons.Common.Exceptions;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Services;

internal class DatFileService : ServiceBase, IDatFileService
{
    public DatFileService(IServiceProvider services) : base(services)
    {
    }

    public void StoreDatFile(string datFileName, string targetDirectory, IEnumerable<DatFileEntry> entries,
        DatFileType datFileType)
    {
        if (string.IsNullOrWhiteSpace(datFileName))
        {
            throw new ArgumentException($"No valid file name provided: {datFileName}", nameof(datFileName));
        }

        if (string.IsNullOrWhiteSpace(targetDirectory))
        {
            throw new ArgumentException($"No valid target director provided: {targetDirectory}",
                nameof(targetDirectory));
        }

        // ReSharper disable once PossibleMultipleEnumeration
        var entryList = entries.ToList();

        if (!entryList.Any())
        {
            throw new ArgumentException($"No valid dat file entries have been provided.", nameof(entries));
        }

        if (!FileSystem.Directory.Exists(targetDirectory))
        {
            FileSystem.Directory.CreateDirectory(targetDirectory);
        }

        var absoluteFilePath = FileSystem.Path.Combine(targetDirectory, datFileName);
        var fileInfo = FileSystem.FileInfo.New(absoluteFilePath);
        var extension = fileInfo.Extension;
        var fileType = new DatAlamoFileType();
        if (!$".{fileType.FileExtension}".ToLower().Equals(extension.ToLower()))
        {
            Logger.LogInformation(
                $"The provided file name {datFileName} does not end with the correct extension of \".{fileType.FileExtension}\".");
            if (fileInfo.Extension == string.Empty)
            {
                absoluteFilePath = absoluteFilePath + "." + fileType.FileExtension;
            }
        }

        if (datFileType == DatFileType.OrderedByCrc32)
        {
            entryList.Sort();
        }

        // ReSharper disable once PossibleMultipleEnumeration
        var datHolder = new DatFileHolder(entryList.AsReadOnly(),
            new DatFileHolderParam() { FilePath = absoluteFilePath, Order = datFileType }, Services);

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatFileConverter)}."));
        factory.GetWriter().WriteBinary(absoluteFilePath, factory.GetConverter().FileToBinary(datHolder));
    }

    public IDatFile LoadDatFile(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatFileConverter)}."));
        return factory.GetConverter()
            .BinaryToFile(new DatFileHolderParam() { FilePath = filePath },
                factory.GetReader().ReadBinary(FileSystem.FileStream.New(filePath, FileMode.Open)));
    }

    public DatFileType PeekDatFileType(string filePath)
    {
        if (!FileSystem.File.Exists(filePath))
        {
            throw new ArgumentException($"The file {filePath} does not exist.", nameof(filePath));
        }

        var factory = (IDatBinaryServiceFactory)(Services.GetService(typeof(IDatBinaryServiceFactory)) ??
                                                 throw new LibraryInitialisationException(
                                                     $"No implementation could be found for {nameof(IDatFileConverter)}."));
        var reader = (DatFileReader)factory.GetReader();
        return reader.PeekFileType(FileSystem.FileStream.New(filePath, FileMode.Open));
    }
}