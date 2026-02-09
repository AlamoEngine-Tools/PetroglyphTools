// Copyright (c) Alamo Engine Tools- and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Represents a base implementation of <see cref="IMegDataEntryPathNormalizer"/>.
/// </summary>
public abstract class MegDataEntryPathNormalizerBase : IMegDataEntryPathNormalizer
{
    /// <inheritdoc />
    public abstract string Normalize(ReadOnlySpan<char> entryPath);

    /// <inheritdoc />
    public string Normalize(string entryPath)
    {
        return Normalize(entryPath.AsSpan());
    }

    /// <inheritdoc />
    public bool TryNormalize(ReadOnlySpan<char> entryPath, Span<char> destination, out int charsWritten)
    {
        try
        {
            charsWritten = Normalize(entryPath, destination);
            return true;
        }
        catch
        {
            charsWritten = 0;
            return false;
        }
    }

    /// <summary>
    /// Normalizes the specified span containing a MEG data entry path to a preallocated character span, and returns the number of written characters.
    /// </summary>
    /// <remarks>
    /// This method may require more characters for <paramref name="destination"/> than there are in <paramref name="entryPath"/>.
    /// </remarks>
    /// <param name="entryPath">The read-only span containing the entry's file path to normalize.</param>
    /// <param name="destination">The span to write the normalized path into.</param>
    /// <returns><see langword="true"/>The number of chars written to <paramref name="destination"/> are stored to this variable.<see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    protected abstract int Normalize(ReadOnlySpan<char> entryPath, Span<char> destination);
}