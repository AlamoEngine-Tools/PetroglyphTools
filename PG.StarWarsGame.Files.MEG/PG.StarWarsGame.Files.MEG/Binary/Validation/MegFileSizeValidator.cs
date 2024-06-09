// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation.V1;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal sealed class MegFileSizeValidator : IMegFileSizeValidator
{
    public bool Validate(IMegBinaryValidationInformation info)
    {
        if (info == null) 
            throw new ArgumentNullException(nameof(info));
        if (info.Metadata is null || info.BytesRead <= 0 || info.FileSize <= 0)
            return false;

        if (info.FileSize < info.BytesRead)
            return false;
        if (info.Metadata is MegMetadata)
            return new V1SizeValidator().Validate(info);
        // TODO: Implement for other versions
        return true;
    }
}