using FluentValidation;
using PG.Commons.Utilities.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// A validator that checks keys used for DAT files.
/// </summary>
public interface IDatKeyValidator : IValidator<string>;

/// <summary>
/// A validator that checks the passed key is not <see langword="null"/>.
/// </summary>
public sealed class NotNullKeyValidator : NullableAbstractValidator<string>, IDatKeyValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullKeyValidator"/> class.
    /// </summary>
    public static readonly NotNullKeyValidator Instance = new();

    /// <inheritdoc />
    protected override bool IsValueNullable => false;
}