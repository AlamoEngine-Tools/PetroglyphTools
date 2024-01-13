using FluentValidation;
using FluentValidation.Results;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks the passed <see cref="MegFileDataEntryBuilderInfo"/> is not <see langword="null"/>.
/// </summary>
public class NotNullDataEntryValidator : AbstractValidator<MegFileDataEntryBuilderInfo>, IBuilderInfoValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullDataEntryValidator"/> class.
    /// </summary>
    public static readonly NotNullDataEntryValidator Instance = new();

    /// <inheritdoc/>
    public sealed override ValidationResult Validate(ValidationContext<MegFileDataEntryBuilderInfo> context)
    {
        if (context.InstanceToValidate is null)
            return new ValidationResult(new[] { new ValidationFailure("MegFileDataEntryBuilderInfo", "MegFileDataEntryBuilderInfo cannot be null.") });
        return base.Validate(context);
    }
}