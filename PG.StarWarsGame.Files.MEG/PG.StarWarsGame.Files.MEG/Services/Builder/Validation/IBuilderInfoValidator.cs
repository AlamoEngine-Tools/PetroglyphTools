// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks an instance of a <see cref="MegFileDataEntryBuilderInfo"/>
/// </summary>
public interface IBuilderInfoValidator
{
    /// <summary>
    /// Validates the specified MEG data entry information whether it is compliant to the rules of this instance.
    /// </summary>
    /// <param name="builderInfo">The data entry information to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="builderInfo"/> is valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(MegFileDataEntryBuilderInfo builderInfo);

    /// <summary>
    /// Validates the specified MEG data entry information whether they are compliant to the rules of this instance.
    /// </summary>
    /// <param name="entryPath">A readonly span containing the data entry's path.</param>
    /// <param name="encrypted">Information whether the data entry is encrypted</param>
    /// <param name="size">The file size of the data entry or <see langword="null"/> if the file size is unknown.</param>
    /// <returns><see langword="true"/> if the specified parameters are valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size);
}