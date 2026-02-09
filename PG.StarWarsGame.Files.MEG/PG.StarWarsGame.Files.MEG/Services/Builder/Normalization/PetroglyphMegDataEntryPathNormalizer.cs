// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Represents a path normalizer that normalizes MEG data entry paths according to Petroglyph game requirements.
/// </summary>
public class PetroglyphMegDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// Gets the options used to normalize MEG data entry paths according to Petroglyph game requirements.
    /// </summary>
    /// <remarks>
    /// These options ensure that the normalized paths are uppercase and use Windows-style directory separators (backslash).
    /// </remarks>
    protected internal static readonly PathNormalizeOptions PetroglyphNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        UnifySeparatorKind = DirectorySeparatorKind.Windows,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    /// <summary>
    /// Normalizes the specified MEG data entry path according to Petroglyph game requirements.
    /// </summary>
    /// <returns>The normalized entry path.</returns>
    /// <param name="entryPath">The read-only span containing the entry's file path to normalize.</param>
    /// <remarks>
    /// This method ensures that the path is converted to uppercase and uses Windows-style directory separators (backslash).
    /// </remarks>
    public override string Normalize(ReadOnlySpan<char> entryPath)
    {
        return entryPath.Length == 0 ? string.Empty : PathNormalizer.Normalize(entryPath, PetroglyphNormalizeOptions);
    }

    /// <summary>
    /// Normalizes the specified MEG data entry path and writes the normalized path to the destination buffer.
    /// </summary>
    /// <param name="entryPath">The read-only span containing the entry's file path to normalize.</param>
    /// <param name="destination">
    /// A buffer to store the normalized path. The buffer must be large enough to hold the normalized path.
    /// </param>
    /// <returns>
    /// The number of characters written to the destination buffer after normalization.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the destination buffer is not large enough to store the normalized path.
    /// </exception>
    /// <remarks>
    /// This method ensures that the path is converted to uppercase and uses Windows-style directory separators (backslash).
    /// </remarks>
    protected override int Normalize(ReadOnlySpan<char> entryPath, Span<char> destination)
    {
        return entryPath.Length == 0 ? 0 : PathNormalizer.Normalize(entryPath, destination, PetroglyphNormalizeOptions);
    }
}