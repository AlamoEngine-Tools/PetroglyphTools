// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation.V1;

internal class V1SizeValidator
{
    public bool Validate(IMegBinaryValidationInformation info)
    {
        if (info.BytesRead != info.Metadata.Size)
            return false;
        
        var totalDataSize = info.Metadata.FileTable.Sum(d => d.FileSize);
        var expectedArchiveSize = info.BytesRead + totalDataSize;
        return expectedArchiveSize == info.FileSize;
    }
}