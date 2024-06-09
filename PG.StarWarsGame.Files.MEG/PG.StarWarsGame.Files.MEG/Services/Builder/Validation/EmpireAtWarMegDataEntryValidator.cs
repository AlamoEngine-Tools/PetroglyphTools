using System;
using AnakinRaW.CommonUtilities.FileSystem;
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
            var pathSpan = path.AsSpan();

            if (pathSpan.IndexOf('/') != -1)
                return false;

            try
            {
                var fileName = FileSystem.Path.GetFileName(pathSpan);
                return FileNameUtilities.IsValidFileName(fileName, out _);
            }
            catch (Exception)
            {
                return false;
            }
        });
    }
}