// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegBinaryServiceFactory(IServiceProvider serviceProvider) : IMegBinaryServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public IMegFileBinaryReader GetReader(MegFileVersion megVersion)
    {
        if (megVersion == MegFileVersion.V1)
            return new MegFileBinaryReaderV1(_serviceProvider);

        throw new NotImplementedException("MEGs other than V1 are currently not supported.");
    }

    public IMegBinaryConverter GetConverter(MegFileVersion megVersion)
    {
        if (megVersion == MegFileVersion.V1)
            return new MegBinaryConverterV1(_serviceProvider);

        throw new NotImplementedException("MEGs other than V1 are currently not supported.");
    }

    public IConstructingMegArchiveBuilder GetConstructionBuilder(MegFileVersion megVersion)
    {
        if (megVersion == MegFileVersion.V1)
            return new ConstructingMegArchiveBuilderV1(_serviceProvider);

        throw new NotImplementedException("MEGs other than V1 are currently not supported.");
    }

    public IMegSizeCalculator GetMegSizeCalculator(MegFileVersion megVersion)
    {
        return megVersion switch
        {
            MegFileVersion.V1 => new MegV1SizeCalculator(),
            _ => throw new NotImplementedException($"MEG version {megVersion} is currently not supported.")
        };
    }
}