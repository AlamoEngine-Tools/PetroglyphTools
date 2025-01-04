// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// A validator that checks whether a <see cref="string"/> can be used as a key for DAT files.
/// </summary>
public interface IDatKeyValidator
{
    /// <summary>
    /// Validates the specified key.
    /// </summary>
    /// <param name="key">The key to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="key"/> is valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(string? key);

    /// <summary>
    /// Validates the specified character span whether it represents a valid key.
    /// </summary>
    /// <param name="key">The span representing the key to validate.</param>
    /// <returns><see langword="true"/> if <paramref name="key"/> is valid; otherwise, <see langword="false"/>.</returns>
    bool Validate(ReadOnlySpan<char> key);
}