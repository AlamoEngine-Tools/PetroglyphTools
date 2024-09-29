// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

/// <summary>
/// Normalizes a path in the same way the Empire at War Alamo engine normalizes meg entry paths (e.g, for file lookups).
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
        // The general idea of this algorithm is first normalizing slashes to back-slashes and uppercasing the whole input.
        // Second removing unnecessary leading path section, [ ('.' '\\') || '\\' ]
        // However, the second part of the algorithm seems a little broken for absolute paths as
        // for example 'c:\\test.meg' results in ':\\test.meg'.
        var ln = PathNormalizer.Normalize(filePath, destination, PetroglyphNormalizeOptions);

        var normalized = destination.Slice(0, ln);

        var fileStartIndex = normalized.LastIndexOf('\\');
        if (fileStartIndex == -1)
            return ln;

        var pathStart = 0;
        var colon = normalized.IndexOf(':');
        if (colon != -1)
            pathStart = colon;

        if (normalized[pathStart] == '.')
            ++pathStart;
        if (normalized[pathStart] == '\\')
            ++pathStart;

        var pathSection = normalized.Slice(pathStart, fileStartIndex - pathStart);

        if (pathSection.Length == 0)
        {
            // +1 because we don't want a leading separator
            var fileSection = normalized.Slice(fileStartIndex + 1);
            fileSection.CopyTo(destination);
            return fileSection.Length;
        }

        var fullSection = normalized.Slice(pathStart);
        fullSection.CopyTo(destination);
        return fullSection.Length;
    }
}