// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary.File;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegBinaryServiceFactory : IMegBinaryServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MegBinaryServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBinaryFileReader<IMegFileMetadata> GetReader(MegFileVersion megVersion)
    {
        if (megVersion == MegFileVersion.V1)
            return new V1.MegFileBinaryServiceV1();
        throw new NotImplementedException();
    }

    public IBinaryFileReader<IMegFileMetadata> GetReader(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        throw new NotImplementedException();
    }
}