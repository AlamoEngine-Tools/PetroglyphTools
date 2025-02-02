// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
public interface IMegDataEntryPathNormalizer
{
    /// <summary>
    /// Normalizes the specified MEG data entry path.
    /// </summary>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <returns>The normalized path.</returns>
    string Normalize(string filePath);

    /// <summary>
    /// Normalizes the specified span containing a MEG data entry path.
    /// </summary>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <returns>The normalized path.</returns>
    string Normalize(ReadOnlySpan<char> filePath);

    /// <summary>
    /// Attempts to normalize the specified span containing a MEG data entry path to a preallocated character span, and returns a value that indicates whether the operation succeeded.
    /// </summary>
    /// <remarks>
    /// This method may require more characters for <paramref name="destination"/>
    /// than there are in <paramref name="filePath"/>.
    /// </remarks>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <param name="destination">The span to write the normalized path into.</param>
    /// <param name="charsWritten">The number of chars written to <paramref name="destination"/> are stored to this variable.</param>
    /// <returns><see langword="true"/> if the normalization was completed and copied to <paramref name="destination"/>; otherwise, <see langword="false"/>.</returns>
    bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten);
}