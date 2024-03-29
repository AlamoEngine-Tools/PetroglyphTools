// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

interface IMegBinaryValidationInformation
{
    public long BytesRead { get; }

    public long FileSize { get; }

    public IMegFileMetadata Metadata { get; }
}