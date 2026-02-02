// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in a way that path separators are unified to the current system's default separator and upper-cases the path.
/// </summary>
public sealed class DefaultDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    /// <summary>
    /// Returns a singleton instance of the <see cref="DefaultDataEntryPathNormalizer"/>.
    /// </summary>
    public static readonly DefaultDataEntryPathNormalizer Instance = new();

    private static readonly PathNormalizeOptions DefaultNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        TreatBackslashAsSeparator = true,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    private DefaultDataEntryPathNormalizer()
    {
    }

    /// <inheritdoc />
    public override string Normalize(ReadOnlySpan<char> entryPath)
    {
        if (entryPath.Length == 0)
            return string.Empty;

        char[]? pooledCharArray = null;
        try
        {
            var buffer = entryPath.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(entryPath.Length)
                : stackalloc char[entryPath.Length];

            var normalizedLength = PathNormalizer.Normalize(entryPath, buffer, DefaultNormalizeOptions);
            return buffer.Slice(0, normalizedLength).ToString();
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }

    /// <inheritdoc />
    protected override int Normalize(ReadOnlySpan<char> entryPath, Span<char> destination)
    {
        return entryPath.Length == 0 ? 0 : PathNormalizer.Normalize(entryPath, destination, DefaultNormalizeOptions);
    }
}