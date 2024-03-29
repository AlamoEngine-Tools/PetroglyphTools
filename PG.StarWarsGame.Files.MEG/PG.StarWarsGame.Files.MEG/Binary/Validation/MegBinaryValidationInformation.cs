// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal class MegBinaryValidationInformation : IMegBinaryValidationInformation
{
    public long BytesRead { get; init; }

    public long FileSize { get; init; }

    public required IMegFileMetadata Metadata { get; init; }
}