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
        if (entryPath.Length is 0 or > 260)
            return false;

        if (FileSystem.Path.HasTrailingDirectorySeparator(entryPath))
            return false;

        // We do not allow spaces, as for XML parsing, spaces are also used as delimiters in lists (e.g, SFX Samples)
        // Also, on Linux this is ':' which conveniently also forbids things like "C:/" too. On Windows this is ';'
        if (entryPath.IndexOfAny(' ', FileSystem.Path.PathSeparator) != -1)
            return false;

        try
        {
            if (FileSystem.Path.IsPathRooted(entryPath))
                return false;

            Span<char> buffer = stackalloc char[260];
            var length = PathNormalizer.Normalize(entryPath, buffer, new PathNormalizeOptions
            {
                UnifyDirectorySeparators = true,
                TrailingDirectorySeparatorBehavior = TrailingDirectorySeparatorBehavior.Trim
            });

            var normalized = buffer.Slice(0, length).ToString();

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