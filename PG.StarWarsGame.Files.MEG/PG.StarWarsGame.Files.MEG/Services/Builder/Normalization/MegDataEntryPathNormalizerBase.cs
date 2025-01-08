// Copyright (c) Alamo Engine Tools- and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Base class for an <see cref="IMegDataEntryPathNormalizer"/>.
/// </summary>
public abstract class MegDataEntryPathNormalizerBase : IMegDataEntryPathNormalizer
{
    private protected MegDataEntryPathNormalizerBase()
    {
    }

    /// <inheritdoc />
    public abstract string Normalize(ReadOnlySpan<char> filePath);

    /// <inheritdoc />
    public string Normalize(string entry)
    {
        return Normalize(entry.AsSpan());
    }

    /// <inheritdoc />
    public bool TryNormalize(ReadOnlySpan<char> filePath, Span<char> destination, out int charsWritten)
    {
        if (filePath.Length == 0)
        {
            charsWritten = 0;
            return true;
        }
        try
        {
            charsWritten = Normalize(filePath, destination);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            charsWritten = 0;
            return false;
        }
    }

    /// <summary>
    /// Normalizes the specified span containing a MEG data entry path to a preallocated character span, and returns the number of written characters.
    /// </summary>
    /// <remarks>
    /// This method may require more characters for <paramref name="destination"/> than there are in <paramref name="filePath"/>.
    /// </remarks>
    /// <param name="filePath">The entry's file path to normalize.</param>
    /// <param name="destination">The span to write the normalized path into.</param>
    /// <returns><see langword="true"/>The number of chars written to <paramref name="destination"/> are stored to this variable.<see langword="false"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="destination"/> is too short.</exception>
    protected abstract int Normalize(ReadOnlySpan<char> filePath, Span<char> destination);
}