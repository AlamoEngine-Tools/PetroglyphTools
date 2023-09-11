// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Net.Http.Headers;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegBinaryServiceFactory : IMegBinaryServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MegBinaryServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IMegFileBinaryReader GetReader(MegFileVersion megVersion)
    {
        if (megVersion == MegFileVersion.V1)
            return new V1.MegFileBinaryServiceV1(_serviceProvider);
        throw new NotImplementedException();
    }

    public IMegFileSizeValidator GetSizeValidator(MegFileVersion fileVersion, bool encrypted)
    {
        if (encrypted) 
            throw new NotImplementedException();
        switch (fileVersion)
        {
            case MegFileVersion.V1:
                return new MegFileSizeValidatorV1(_serviceProvider);
            case MegFileVersion.V2:
                break;
            case MegFileVersion.V3:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(fileVersion), fileVersion, null);
        }

        throw new NotImplementedException();
    }

    public IMegFileBinaryReader GetReader(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        throw new NotImplementedException();
    }
}