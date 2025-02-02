// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal class MegFileTableValidator : IFileTableValidator
{
    public bool Validate(IMegFileTable fileTable)
    {
        var lastCrc = new Crc32(0);
        for (var i = 0; i < fileTable.Count; i++)
        {
            var descriptor = fileTable[i];
            if (descriptor.Index != i)
                return false;
            if (descriptor.Crc32 < lastCrc)
                return false;
            lastCrc = descriptor.Crc32;
        }
        return true;
    }
}