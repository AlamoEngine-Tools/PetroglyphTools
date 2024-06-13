// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services.Builder.Normalization;
using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Provides methods to normalize a data entry's file path to store it into a MEG archive.
/// </summary>
public interface IMegDataEntryPathNormalizer : IBuilderEntryNormalizer<string>
{
    /// <summary>
    /// Normalizes a specified MEG data entry path and writes it into a span of characters.
    /// </summary>
    /// <remarks>
    /// This method never writes more characters into <paramref name="destination"/>
    /// than there are in <paramref name="filePath"/> but it can be less.
    /// </remarks>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <param name="destination">The span to write the normalized path into.</param>
    /// <returns>The number of chars written to <paramref name="destination"/>.</returns>
    int Normalize(ReadOnlySpan<char> filePath, Span<char> destination);

    /// <summary>
    /// Tries to normalize a specified MEG data entry path and writes it into a span of characters.
    /// </summary>
    /// <remarks>
    /// This method never writes more characters into <paramref name="destination"/>
    /// than there are in <paramref name="filePath"/> but it can be less.
    /// </remarks>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <param name="destination">The span to write the normalized path into.</param>
    /// <param name="charsWritten">The number of chars written to <paramref name="destination"/>.</param>
    /// <param name="notNormalizedMessage">An optional reason why the normalization failed or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the normalization was successful, otherwise, <see langword="false"/>.</returns>
    bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten, out string? notNormalizedMessage);
}