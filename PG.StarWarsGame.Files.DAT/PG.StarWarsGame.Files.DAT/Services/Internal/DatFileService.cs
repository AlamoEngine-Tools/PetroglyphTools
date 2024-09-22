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
        if (fileStream is null)
            throw new ArgumentNullException(nameof(fileStream));
        if (entries is null)
            throw new ArgumentNullException(nameof(entries));

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
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        var fileType = GetDatFileType(fs);
        fs.Seek(0, SeekOrigin.Begin);
        return LoadAs(fs, fileType);
    }

    public IDatFile LoadAs(string filePath, DatFileType requestedFileType)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return LoadAs(fs, requestedFileType);
    }

    private IDatFile LoadAs(FileSystemStream fileStream, DatFileType requestedFileType)
    {
        var reader = Services.GetRequiredService<IDatFileReader>();
        var datBinary = reader.ReadBinary(fileStream);

        var converter = Services.GetRequiredService<IDatBinaryConverter>();
        var datModel = converter.BinaryToModel(datBinary);

        if (requestedFileType == DatFileType.NotOrdered && datModel is ISortedDatModel sorted)
            datModel = sorted.ToUnsortedModel();

        if (requestedFileType == DatFileType.OrderedByCrc32 && datModel is IUnsortedDatModel)
            throw new NotSupportedException("Unsorted DAT file cannot be loaded as sorted DAT file");

        var filePath = FileSystem.Path.GetFullPath(fileStream.Name);
        var fileInfo = new DatFileInformation { FilePath = filePath };

        return new DatFile(datModel, fileInfo, Services);
    }


    public DatFileType GetDatFileType(string filePath)
    {
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return GetDatFileType(fs);
    }

    private DatFileType GetDatFileType(Stream fileStream)
    {
        return Services.GetRequiredService<IDatFileReader>().PeekFileType(fileStream);
    }
}