// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

internal interface IMegFileSizeValidator
{
    bool Validate(MegBinaryValidationInformation info);
}