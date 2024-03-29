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
            .NotNull()
            .NotEmpty()
            .Length(1, 260)
            .Must(path =>
            {
                if (FileSystem.Path.HasTrailingDirectorySeparator(path))
                    return false;

                // On Linux this is ':' which conveniently also forbids things like "C:/" too.
                // On Windows this is ';'
                if (path.IndexOf(FileSystem.Path.PathSeparator) != -1)
                    return false;
                
                try
                {
                    var normalized = PathNormalizer
                        .Normalize(path, new PathNormalizeOptions
                        {
                            UnifyDirectorySeparators = true,
                            TrailingDirectorySeparatorBehavior = TrailingDirectorySeparatorBehavior.Trim
                        });

                    if (FileSystem.Path.IsPathRooted(normalized))
                        return false;


                    var fullNormalized = FileSystem.Path.GetFullPath(normalized);

                    var currentDirectory = FileSystem.Path.GetFullPath(FileSystem.Directory.GetCurrentDirectory());
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