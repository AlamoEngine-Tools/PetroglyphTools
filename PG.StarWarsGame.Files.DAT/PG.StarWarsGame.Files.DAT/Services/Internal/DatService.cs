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
    public void CreateDatBinary(Stream fileStream, IEnumerable<DatStringEntry> entries, DatLayoutKind datLayoutKind)
    {
        if (fileStream is null)
            throw new ArgumentNullException(nameof(fileStream));
        if (entries is null)
            throw new ArgumentNullException(nameof(entries));

        var datModel = new ConstructingDatModel(entries, datLayoutKind);

        var datBinary = Services.GetRequiredService<IDatBinaryConverter>().ModelToBinary(datModel);

        datBinary.WriteTo(fileStream);
    }

    public IDatFile LoadFile(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        var fileType = GetDatLayoutKind(fs);
        fs.Seek(0, SeekOrigin.Begin);
        return LoadFileAs(fs, fileType);
    }

    public IDatFile LoadFile(FileSystemStream fileStream)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));
        var currentPos = fileStream.Position;
        var fileType = GetDatLayoutKind(fileStream);
        fileStream.Seek(currentPos, SeekOrigin.Begin);
        return LoadFileAs(fileStream, fileType);
    }

    public IDatFile LoadFileAs(string filePath, DatLayoutKind requestedLayout)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return LoadFileAs(fs, requestedLayout);
    }

    public IDatFile LoadFileAs(FileSystemStream fileStream, DatLayoutKind requestedLayout)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        var datModel = ReadModel(fileStream, requestedLayout);

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

    public IDatModel LoadModelAs(Stream stream, DatLayoutKind requestedLayout)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return LoadModelFromStream(stream, requestedLayout);
    }

    public IDatModel LoadModelAs(byte[] data, DatLayoutKind requestedLayout)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return LoadModelAs(stream, requestedLayout);
    }

    public IDatModel LoadModelAs(ReadOnlySpan<byte> data, DatLayoutKind requestedLayout)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return LoadModelAs(stream, requestedLayout);
    }

    public DatLayoutKind GetDatLayoutKind(string filePath)
    {
        if (filePath == null)
            throw new ArgumentNullException(nameof(filePath));
        using var fs = FileSystem.FileStream.New(filePath, FileMode.Open, FileAccess.Read);
        return GetDatLayoutKind(fs);
    }

    public DatLayoutKind GetDatLayoutKind(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return Services.GetRequiredService<IDatFileReader>().PeekFileType(stream);
    }

    public DatLayoutKind GetDatLayoutKind(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return GetDatLayoutKind(stream);
    }

    public DatLayoutKind GetDatLayoutKind(ReadOnlySpan<byte> data)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return GetDatLayoutKind(stream);
    }

    private IDatModel LoadModelFromStream(Stream stream, DatLayoutKind? requestedFileType)
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

    private IDatModel ReadModelFromSeekableStream(Stream stream, DatLayoutKind? requestedFileType)
    {
        DatLayoutKind fileType;
        if (requestedFileType.HasValue)
        {
            fileType = requestedFileType.Value;
        }
        else
        {
            var startPosition = stream.Position;
            fileType = GetDatLayoutKind(stream);
            stream.Seek(startPosition, SeekOrigin.Begin);
        }

        return ReadModel(stream, fileType);
    }

    private IDatModel ReadModel(Stream stream, DatLayoutKind requestedFileType)
    {
        var reader = Services.GetRequiredService<IDatFileReader>();
        var datBinary = reader.ReadBinary(stream);

        var converter = Services.GetRequiredService<IDatBinaryConverter>();
        var datModel = converter.BinaryToModel(datBinary);

        if (requestedFileType == DatLayoutKind.NotOrdered && datModel is ISortedDatModel sorted)
            datModel = sorted.ToUnsortedModel();

        if (requestedFileType == DatLayoutKind.OrderedByCrc32 && datModel is IUnsortedDatModel)
            throw new InvalidOperationException("Unsorted DAT file cannot be loaded as sorted DAT file");

        return datModel;
    }
}
