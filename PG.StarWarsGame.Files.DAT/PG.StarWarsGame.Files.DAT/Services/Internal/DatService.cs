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

internal class DatService(IServiceProvider services) : ServiceBase(services), IDatService
{
    public void CreateDatFile(FileSystemStream fileStream, IEnumerable<DatStringEntry> entries, DatFileType datFileType)
    {
        if (fileStream is null)
            throw new ArgumentNullException(nameof(fileStream));
        if (entries is null)
            throw new ArgumentNullException(nameof(entries));

        var datModel = new ConstructingDatModel(entries, datFileType);

        var datBinary = Services.GetRequiredService<IDatBinaryConverter>().ModelToBinary(datModel);

        datBinary.WriteTo(fileStream);
    }

    public IDatFile Load(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        var fileType = GetDatFileType(fs);
        fs.Seek(0, SeekOrigin.Begin);
        return LoadAs(fs, fileType);
    }

    public IDatFile Load(FileSystemStream fileStream)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));
        var currentPos = fileStream.Position;
        var fileType = GetDatFileType(fileStream);
        fileStream.Seek(currentPos, SeekOrigin.Begin);
        return LoadAs(fileStream, fileType);
    }

    public IDatFile LoadAs(string filePath, DatFileType requestedFileType)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return LoadAs(fs, requestedFileType);
    }

    public IDatFile LoadAs(FileSystemStream fileStream, DatFileType requestedFileType)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        var datModel = ReadModel(fileStream, requestedFileType);

        var filePath = FileSystem.Path.GetFullPath(fileStream.Name);
        var fileInfo = new DatFileInformation { FilePath = filePath };

        return new DatFile(datModel, fileInfo, Services);
    }

    public IDatModel LoadModel(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return LoadModelFromStream(stream, null);
    }

    public IDatModel LoadModel(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return LoadModel(stream);
    }

    public IDatModel LoadModel(ReadOnlySpan<byte> data)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return LoadModel(stream);
    }

    public IDatModel LoadModelAs(Stream stream, DatFileType requestedFileType)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return LoadModelFromStream(stream, requestedFileType);
    }

    public IDatModel LoadModelAs(byte[] data, DatFileType requestedFileType)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return LoadModelAs(stream, requestedFileType);
    }

    public IDatModel LoadModelAs(ReadOnlySpan<byte> data, DatFileType requestedFileType)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return LoadModelAs(stream, requestedFileType);
    }

    public DatFileType GetDatFileType(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return GetDatFileType(fs);
    }

    public DatFileType GetDatFileType(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return Services.GetRequiredService<IDatFileReader>().PeekFileType(stream);
    }

    public DatFileType GetDatFileType(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return GetDatFileType(stream);
    }

    public DatFileType GetDatFileType(ReadOnlySpan<byte> data)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return GetDatFileType(stream);
    }

    private IDatModel LoadModelFromStream(Stream stream, DatFileType? requestedFileType)
    {
        // Reading the model requires seeking back to the start after peeking the file type. If the source
        // stream cannot seek, copy it into memory first so the in-memory load path can re-read it.
        if (!stream.CanSeek)
        {
            using var seekableCopy = new MemoryStream();
            stream.CopyTo(seekableCopy);
            seekableCopy.Position = 0;
            return ReadModelFromSeekableStream(seekableCopy, requestedFileType);
        }

        return ReadModelFromSeekableStream(stream, requestedFileType);
    }

    private IDatModel ReadModelFromSeekableStream(Stream stream, DatFileType? requestedFileType)
    {
        DatFileType fileType;
        if (requestedFileType.HasValue)
        {
            fileType = requestedFileType.Value;
        }
        else
        {
            var startPosition = stream.Position;
            fileType = GetDatFileType(stream);
            stream.Seek(startPosition, SeekOrigin.Begin);
        }

        return ReadModel(stream, fileType);
    }

    private IDatModel ReadModel(Stream stream, DatFileType requestedFileType)
    {
        var reader = Services.GetRequiredService<IDatFileReader>();
        var datBinary = reader.ReadBinary(stream);

        var converter = Services.GetRequiredService<IDatBinaryConverter>();
        var datModel = converter.BinaryToModel(datBinary);

        if (requestedFileType == DatFileType.NotOrdered && datModel is ISortedDatModel sorted)
            datModel = sorted.ToUnsortedModel();

        if (requestedFileType == DatFileType.OrderedByCrc32 && datModel is IUnsortedDatModel)
            throw new InvalidOperationException("Unsorted DAT file cannot be loaded as sorted DAT file");

        return datModel;
    }
}
