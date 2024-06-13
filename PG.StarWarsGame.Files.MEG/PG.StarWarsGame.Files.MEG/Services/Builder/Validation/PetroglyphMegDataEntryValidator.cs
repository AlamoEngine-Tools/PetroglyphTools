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
    protected const int PetroglyphMaxFilePathLength = 260;

    // Normalization must not trim trailing directory separators, as otherwise we can't check for that constraint.
    private static readonly PathNormalizeOptions PetroglyphPathNormalizeOptions = new()
    {
        UnifyDirectorySeparators = true
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
        if (entryPath.Length is 0 or > PetroglyphMaxFilePathLength)
            return false;
        
        // We do not allow spaces, as for XML parsing, as they are also used as delimiters in lists (e.g, SFX Samples)
        // Also, we exclude path separator which is ':' on Linux which conveniently also forbids things like "C:/" on linux systems too.
        // On Windows this is ';'
        if (entryPath.IndexOfAny(' ', FileSystem.Path.PathSeparator) != -1)
            return false;

        try
        {
            Span<char> buffer = stackalloc char[PetroglyphMaxFilePathLength];
            var length = PathNormalizer.Normalize(entryPath, buffer, PetroglyphPathNormalizeOptions);
            
            var normalized = buffer.Slice(0, length).ToString();

            // We need to perform the following checks after system-dependent normalization
            if (FileSystem.Path.IsPathRooted(normalized))
                return false;
            if (FileSystem.Path.HasTrailingDirectorySeparator(normalized))
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
}