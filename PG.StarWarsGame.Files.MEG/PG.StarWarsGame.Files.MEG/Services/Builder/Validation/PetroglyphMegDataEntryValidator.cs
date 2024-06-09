using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Utilities.Validation;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a <see cref="MegFileDataEntryBuilderInfo"/> whether it is compliant to a Petroglyph game.
/// </summary>
public abstract class PetroglyphMegDataEntryValidator : NullableAbstractValidator<MegFileDataEntryBuilderInfo>, IBuilderInfoValidator
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <inheritdoc />
    protected sealed override bool IsValueNullable => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphMegDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        RuleFor(e => e.FilePath)
            .Must(path =>
            {
                var pathSpan = path.AsSpan();

                if (pathSpan.Length is 0 or > 260)
                    return false;

                if (FileSystem.Path.HasTrailingDirectorySeparator(pathSpan))
                    return false;

                // We do not allow spaces, as for XML parsing, spaces are also used as delimiters in lists (e.g, SFX Samples)
                // Also, on Linux this is ':' which conveniently also forbids things like "C:/" too. On Windows this is ';'
                if (pathSpan.IndexOfAny(' ', FileSystem.Path.PathSeparator) != -1)
                    return false;

                try
                {
                    if (FileSystem.Path.IsPathRooted(pathSpan))
                        return false;

                    Span<char> buffer = stackalloc char[260];
                    var length = PathNormalizer.Normalize(pathSpan, buffer, new PathNormalizeOptions
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
            });
    }
}