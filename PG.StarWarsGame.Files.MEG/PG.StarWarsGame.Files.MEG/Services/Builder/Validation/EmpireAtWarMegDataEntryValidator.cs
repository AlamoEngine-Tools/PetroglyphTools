// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.FileSystem;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a MEG data entry whether it is compliant to the Petroglyph game Empire at War. 
/// </summary>
public sealed class EmpireAtWarMegDataEntryValidator : PetroglyphMegDataEntryValidator
{
    private static readonly PathNormalizeOptions EaWPathNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        TrailingDirectorySeparatorBehavior = TrailingDirectorySeparatorBehavior.Trim
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegDataEntryValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }


    /// <inheritdoc />
    public override bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    {
        if (!base.Validate(entryPath, encrypted, size))
            return false;

        if (encrypted)
            return false;

        if (entryPath.IndexOf('/') != -1)
            return false;

        Span<char> pathBuffer = stackalloc char[PetroglyphMaxMegFilePathLength];
        var length = entryPath.ToUpperInvariant(pathBuffer);
        var upper = pathBuffer.Slice(0, length);

        if (upper.Length != entryPath.Length || !entryPath.Equals(upper, StringComparison.Ordinal))
            return false;

        // On EaW we enforce '\\' as directory separator. Thus, on linux checking the file name would cause false-positives,
        // as '\\' is a valid file name character there and Path.GetFileName would yield us a false result.
        var systemLength = PathNormalizer.Normalize(entryPath, pathBuffer, EaWPathNormalizeOptions);
        if (systemLength != length)
            throw new InvalidOperationException();

        var normalized = pathBuffer.Slice(0, length);

        var fileName = FileSystem.Path.GetFileName(normalized);
        return FileNameUtilities.IsValidFileName(fileName, out _);
    }
}