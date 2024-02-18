using System;
using System.IO.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Base class for a Petroglyph MEG file information validator.
/// </summary>
public abstract class PetroglyphMegFileInformationValidator : AbstractValidator<MegBuilderFileInformationValidationData>, IMegFileInformationValidator
{
    /// <summary>
    /// Gets the file system.
    /// </summary>
    protected IFileSystem FileSystem { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected PetroglyphMegFileInformationValidator(IServiceProvider serviceProvider)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();

        RuleFor(x => x).SetValidator(DefaultMegFileInformationValidator.Instance);

        // As we cannot know the actual path on the target system where the game will be installed,
        // it does not make sense to check the full path. Instead, we just check for the file name whether that's valid.
        RuleFor(x => x.FileInformation.FilePath)
            .Must(path =>
            {
                try
                {
                    var fileName = FileSystem.Path.GetFileName(path);
                    return fileName.Length <= 260;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }
}