// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using PG.Commons.Utilities;

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
    public override void Normalize(ReadOnlySpan<char> filePath, ref ValueStringBuilder stringBuilder)
    {
        if (filePath.Length == 0)
            return;
        stringBuilder.EnsureCapacity(filePath.Length);
        var length = PathNormalizer.Normalize(filePath, stringBuilder.RawChars, DefaultNormalizeOptions);
        stringBuilder.Length = length;
    }
}