// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a <see cref="MegFileDataEntryBuilderInfo"/> whether it is compliant to a Petroglyph game.
/// </summary>
public abstract class PetroglyphMegDataEntryValidator : IBuilderInfoValidator
{
    /// <summary>
    /// The max number of characters allowed in a PG game for entry paths.
    /// </summary>
    protected const int PetroglyphMaxMegFilePathLength = 259;

    private static readonly PathNormalizeOptions PetroglyphPathNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true,
        TrailingDirectorySeparatorBehavior = TrailingDirectorySeparatorBehavior.Trim
    };

    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphMegDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
    {
       FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
    }


    /// <inheritdoc />
    public bool Validate(MegFileDataEntryBuilderInfo? builderInfo)
    {
        if (builderInfo is null)
            return false;

        return Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size);
    }

    /// <inheritdoc />
    public virtual bool Validate(ReadOnlySpan<char> entryPath, bool encrypted, uint? size)
    { 
        if (entryPath.Length is 0 or > PetroglyphMaxMegFilePathLength)
            return false;

        try
        {
            Span<char> buffer = stackalloc char[PetroglyphMaxMegFilePathLength];
            var length = PathNormalizer.Normalize(entryPath, buffer, PetroglyphPathNormalizeOptions);

            // We trimmed a trailing separator.
            if (length < entryPath.Length)
                return false;

            var normalized = buffer.Slice(0, length);
            
            if (normalized.IndexOf(' ') != -1)
                return false;
            if (IsRootedOrStartsWithCurrent(normalized))
                return false;

            var fullNormalized = FileSystem.Path.GetFullPath(normalized);

            var currentDirectory = FileSystem.Directory.GetCurrentDirectory();
            var combined = FileSystem.Path.Combine(currentDirectory, normalized);

            return combined.Equals(fullNormalized, StringComparison.Ordinal);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool IsRootedOrStartsWithCurrent(ReadOnlySpan<char> path)
    {
        if (FileSystem.Path.IsPathRooted(path))
            return true;

        if (path.Length > 2) 
            return false;
        if (path[0] == '.' && path[1] is '/' or '\\')
            return true;

        return false;
    }
}