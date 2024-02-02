using PG.Commons.Utilities.Validation;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that checks the passed <see cref="MegFileDataEntryBuilderInfo"/> is not <see langword="null"/>.
/// </summary>
public sealed class NotNullDataEntryValidator : NullableAbstractValidator<MegFileDataEntryBuilderInfo>, IBuilderInfoValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullDataEntryValidator"/> class.
    /// </summary>
    public static readonly NotNullDataEntryValidator Instance = new();

    /// <inheritdoc />
    protected override bool IsValueNullable => false;
}