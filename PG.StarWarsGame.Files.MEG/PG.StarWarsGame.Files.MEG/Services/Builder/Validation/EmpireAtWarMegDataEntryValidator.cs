using System;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using FluentValidation;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a MEG data entry whether it is compliant to the Petroglyph game Empire at War. 
/// </summary>
public sealed class EmpireAtWarMegDataEntryValidator : PetroglyphMegDataEntryValidator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarMegDataEntryValidator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public EmpireAtWarMegDataEntryValidator(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        RuleFor(info => info.Encrypted).Must(e => !e);

        RuleFor(info => info.FilePath).Must(path =>
        {
            var normalized = PathNormalizer.Normalize(path,
                new PathNormalizeOptions { UnifyDirectorySeparators = true, UnifySeparatorKind = DirectorySeparatorKind.Windows });
            if (!normalized.Equals(path))
                return false;

            try
            {
                var systemNormalized = PathNormalizer.Normalize(path,
                    new PathNormalizeOptions { UnifyDirectorySeparators = true });

                var fileName = FileSystem.Path.GetFileName(systemNormalized);
                return FileNameUtilities.IsValidFileName(fileName, out _);
            }
            catch (Exception )
            {
                return false;
            }
        });
    }
}