// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that check an instance of a <see cref="MegBuilderFileInformationValidationData"/>
/// </summary>
public interface IMegFileInformationValidator
{
    /// <summary>
    /// Validates a specified MEG file information to the rules of this instance.
    /// </summary>
    /// <param name="infoValidationData">The file information to validate</param>
    /// <returns><see langword="true"/> if <paramref name="infoValidationData"/> is valid; otherwise, <see langword="false"/>.</returns>
    MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData);
}