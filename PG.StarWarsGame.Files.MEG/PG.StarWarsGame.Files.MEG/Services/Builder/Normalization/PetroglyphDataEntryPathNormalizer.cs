// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in a way that path separators are unified to the backslash separator and upper-cases the path.
/// </summary>
public sealed class PetroglyphDataEntryPathNormalizer : MegDataEntryPathNormalizerBase
{
    private static readonly PathNormalizeOptions PetroglyphNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        UnifySeparatorKind = DirectorySeparatorKind.Windows,
        UnifyCase = UnifyCasingKind.UpperCaseForce
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphDataEntryPathNormalizer"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public PetroglyphDataEntryPathNormalizer(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override int Normalize(ReadOnlySpan<char> filePath, Span<char> destination)
    {
        return PathNormalizer.Normalize(filePath, destination, PetroglyphNormalizeOptions);
    }
}