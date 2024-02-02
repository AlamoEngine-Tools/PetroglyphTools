using System;
using FluentValidation;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
///  Validates a MEG file information whether it is compliant to the Petroglyph game Empire at War. 
/// </summary>
public sealed class EmpireAtWarMegFileInformationValidator : PetroglyphMegFileInformationValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegFileInformationValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegFileInformationValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        RuleFor(x => x.FileInformation)
            .Must(x => !x.HasEncryption)
            .Must(x => x.FileVersion == MegFileVersion.V1);

        // As we cannot know the actual path on the target system where the game will be installed,
        // it does not make sense to check the full path. Instead, we just check for the file name whether that's valid.
        RuleFor(x => x.FileInformation.FilePath)
            .Must(path =>
            {
                try
                {
                    var fileName = FileSystem.Path.GetFileName(path);
                    if (!FileNameUtilities.IsValidFileName(fileName, out _))
                        return false;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
    }
}