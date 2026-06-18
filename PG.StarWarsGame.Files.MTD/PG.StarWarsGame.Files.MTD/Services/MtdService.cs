// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using AnakinRaW.CommonUtilities;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Data;
using PG.StarWarsGame.Files.MTD.Files;

namespace PG.StarWarsGame.Files.MTD.Services;

internal class MtdService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IMtdService
{
    private readonly IMtdBinaryConverter _binaryConverter = serviceProvider.GetRequiredService<IMtdBinaryConverter>();
    private readonly IMtdFileReader _fileReader = serviceProvider.GetRequiredService<IMtdFileReader>();

    public IMtdFile Load(string filePath)
    {
        ThrowHelper.ThrowIfNullOrEmpty(filePath);
        var fullPath = FileSystem.Path.GetFullPath(filePath);
        using var fs = FileSystem.FileStream.New(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Load(fs);
    }

    public IMtdFile Load(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));

        var model = ReadModel(stream);

        var filePath = stream.GetFilePath(out var isInMeg);

        // If the .MTD file is not embedded in a MEG, we want the absolute path.
        if (!isInMeg)
            filePath = FileSystem.Path.GetFullPath(filePath);

        var megFileInfo = new MtdFileInformation { FilePath = filePath, IsInsideMeg = isInMeg};

        return new MtdFile(model, megFileInfo, Services);
    }

    public IMegaTextureDirectory LoadModel(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException(nameof(stream));
        return ReadModel(stream);
    }

    public IMegaTextureDirectory LoadModel(byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        using var stream = new MemoryStream(data, writable: false);
        return LoadModel(stream);
    }

    public IMegaTextureDirectory LoadModel(ReadOnlySpan<byte> data)
    {
        using var stream = new MemoryStream(data.ToArray(), writable: false);
        return LoadModel(stream);
    }

    private IMegaTextureDirectory ReadModel(Stream stream)
    {
        var binaryModel = _fileReader.ReadBinary(stream);
        return _binaryConverter.BinaryToModel(binaryModel);
    }
}
