using System.Linq;
using FluentValidation;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a specified <see cref="MegFileInformation"/> is compliant to the MEG specification.
/// </summary>
public sealed class DefaultMegFileInformationValidator : AbstractValidator<MegBuilderFileInformationValidationData>, IMegFileInformationValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="DefaultMegFileInformationValidator"/> class.
    /// </summary>
    public static readonly DefaultMegFileInformationValidator Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMegFileInformationValidator"/> class with rules for ensuring MEG specification
    /// compliant <see cref="MegFileInformation"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="MegFileInformation"/> are considered <b>not</b> to be compliant if the MEG is encrypted but the version is not V3.
    /// </remarks>
    private DefaultMegFileInformationValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FileInformation).NotNull();
        RuleFor(x => x.DataEntries).NotNull();
        RuleFor(x => x)
            .Custom((data, context) =>
            {
                var isEncrypted = data.DataEntries.Any(e => e.Encrypted);
                var hasEncryptionData = data.FileInformation.HasEncryption;

                if (isEncrypted && !hasEncryptionData) 
                    context.AddFailure("Encryption data must be provided for encrypted MEG archives.");
                if (!isEncrypted && hasEncryptionData)
                    context.AddFailure("No encryption data must be provided for non-encrypted MEG archives.");
            });
    }
}