// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal sealed class MegBinaryConverterV1 : MegBinaryConverterBase<MegMetadata>
{
    public MegBinaryConverterV1(IServiceProvider services) : base(services)
    {
    }

    protected override MegMetadata ModelToBinaryCore(IMegArchive model)
    {
        throw new NotImplementedException();
    }
}