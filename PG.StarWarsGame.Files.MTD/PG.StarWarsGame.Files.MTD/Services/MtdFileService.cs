// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using AnakinRaW.CommonUtilities;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Files;

namespace PG.StarWarsGame.Files.MTD.Services;

internal class MtdFileService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IMtdFileService
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
        
        var binaryModel = _fileReader.ReadBinary(stream);
        var model = _binaryConverter.BinaryToModel(binaryModel);

        var filePath = stream.GetFilePath(out var isInMeg);

        // If the .MTD file is not embedded in a MEG, we want the absolute path.
        if (!isInMeg)
            filePath = FileSystem.Path.GetFullPath(filePath);

        var megFileInfo = new MtdFileInformation { FilePath = filePath, IsInsideMeg = isInMeg};

        return new MtdFile(model, megFileInfo, Services);
    }
}