// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal class MegFileSizeValidatorV1 : MegFileSizeValidatorBase<MegMetadata>
{
    public MegFileSizeValidatorV1(IServiceProvider services) : base(services)
    {
    }

    protected override bool ValidateCore(long bytesRead, MegMetadata metadata)
    {
        throw new NotImplementedException();
    }
}