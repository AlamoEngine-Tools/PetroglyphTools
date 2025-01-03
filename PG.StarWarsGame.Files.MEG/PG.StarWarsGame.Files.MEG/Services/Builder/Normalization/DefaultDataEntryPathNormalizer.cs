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
    private static readonly PathNormalizeOptions DefaultNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDataEntryPathNormalizer"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public DefaultDataEntryPathNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override string Normalize(ReadOnlySpan<char> filePath)
    {
        if (filePath.Length == 0)
            return string.Empty;

        char[]? pooledCharArray = null;
        try
        {
            var buffer = filePath.Length > 265
                ? pooledCharArray = ArrayPool<char>.Shared.Rent(filePath.Length)
                : stackalloc char[filePath.Length];

            var normalizedLength = PathNormalizer.Normalize(filePath, buffer, DefaultNormalizeOptions);
            return buffer.Slice(0, normalizedLength).ToString();
        }
        finally
        {
            if (pooledCharArray is not null)
                ArrayPool<char>.Shared.Return(pooledCharArray);
        }
    }

    /// <inheritdoc />
    protected override int Normalize(ReadOnlySpan<char> filePath, Span<char> destination)
    {
        return filePath.Length == 0 ? 0 : PathNormalizer.Normalize(filePath, destination, DefaultNormalizeOptions);
    }
}