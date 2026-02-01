// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator for a MEG data entries.
/// </summary>
/// <remarks>
/// Note: The games may consume MEG files, created by other tools than this library.
/// An instance of this <see cref="IMegDataEntryValidator"/> may be more restrictive than those other tools.
/// Therefore, this validator shall <em>only</em> be used when <b>creating</b> new MEG files.
/// </remarks>
public interface IMegDataEntryValidator
{
    /// <summary>
    /// Validates the specified MEG data entry information whether it is compliant to the rules of the <see cref="IMegDataEntryValidator"/>.
    /// </summary>
    /// <param name="entryInfo">The data entry information to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="entryInfo"/> is valid; otherwise, <see langword="false"/>.</returns>
    MegDataEntryValidationResult Validate(MegFileDataEntryBuilderInfo entryInfo);
}