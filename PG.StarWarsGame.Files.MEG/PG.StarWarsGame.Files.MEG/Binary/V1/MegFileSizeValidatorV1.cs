// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal class MegFileSizeValidatorV1 : MegFileSizeValidatorBase<MegMetadata>
{
    public MegFileSizeValidatorV1(IServiceProvider services) : base(services)
    {
    }

    protected internal override bool ValidateCore(long bytesRead, long archiveSize, MegMetadata metadata)
    {
        var headerSize = metadata.Size;
        if (bytesRead != headerSize)
            return false;

        var totalDataSize = ((IMegFileTable)metadata.FileTable).Sum(d => d.FileSize);
        var expectedArchiveSize = headerSize + totalDataSize;
        return expectedArchiveSize == archiveSize;
    }
}