// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks whether the data to validate is not <see langword="null"/>.
/// </summary>
public sealed class NotNullDataEntryValidator : IMegDataEntryValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullDataEntryValidator"/> class.
    /// </summary>
    public static readonly NotNullDataEntryValidator Instance = new();

    private NotNullDataEntryValidator()
    {
    }

    /// <inheritdoc />
    public bool Validate(MegFileDataEntryBuilderInfo? builderInfo)
    {
        return builderInfo is not null;
    }

    /// <inheritdoc />
    public bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    {
        return entryPath != ReadOnlySpan<char>.Empty;
    }
}