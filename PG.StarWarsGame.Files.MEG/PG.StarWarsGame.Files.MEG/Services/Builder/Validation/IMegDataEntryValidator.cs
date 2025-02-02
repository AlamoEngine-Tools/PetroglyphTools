// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
    /// <param name="builderInfo">The data entry information to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="builderInfo"/> is valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(MegFileDataEntryBuilderInfo builderInfo);

    /// <summary>
    /// Validates the specified MEG data entry information whether they are compliant to the rules of the <see cref="IMegDataEntryValidator"/>.
    /// </summary>
    /// <param name="entryPath">A readonly character span containing the data entry's path.</param>
    /// <param name="encrypted">Information whether the data entry is encrypted</param>
    /// <param name="size">The file size of the data entry or <see langword="null"/> if the file size is unknown.</param>
    /// <returns><see langword="true"/> if the specified parameters are valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size);
}