// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Drawing;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Binary;

internal class MtdBinaryConverter(IServiceProvider serviceProvider): ServiceBase(serviceProvider), IMtdBinaryConverter
{
    private readonly ICrc32HashingService _hashingService = serviceProvider.GetRequiredService<ICrc32HashingService>();

    public MtdBinaryFile ModelToBinary(IMegaTextureDirectory model)
    {
        if (model == null) 
            throw new ArgumentNullException(nameof(model));

        var header = new MtdHeader((uint)model.Count);
        
        var entries = model
            .OrderBy(x => x.Crc32) // The game sorts by CRC anyway, so we to it a favour
            .Select(x =>
            new MtdBinaryFileInfo(
                x.FileName, 
                (uint)x.Area.X, 
                (uint)x.Area.Y, 
                (uint)x.Area.Width,
                (uint)x.Area.Height,
                x.HasAlpha)).ToList();


        return new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(entries));
    }

    public IMegaTextureDirectory BinaryToModel(MtdBinaryFile binary)
    {
        if (binary == null) 
            throw new ArgumentNullException(nameof(binary));
        var entries = binary.Items.Select(CreateEntryFromBinary);
        try
        {
            return new MegaTextureDirectory(entries);
        }
        catch (DuplicateMtdEntryException e)
        {
            throw new BinaryCorruptedException(e.Message, e);
        }
    }

    private MegaTextureFileIndex CreateEntryFromBinary(MtdBinaryFileInfo x)
    {
        var name = x.Name;
        var crc = _hashingService.GetCrc32(name, MtdFileConstants.NameEncoding);
        return new MegaTextureFileIndex(x.Name, crc, new Rectangle((int)x.X, (int)x.Y, (int)x.Width, (int)x.Height), x.Alpha);
    }
}