using FluentValidation;
using FluentValidation.Results;

namespace PG.Commons.Utilities.Validation;

/// <summary>
/// Base class for object validators. Validators of this type do not throw if an object to validate is <see langword="null"/>,
/// but returns the appropriate <see cref="ValidationResult"/> based on the configuration of this instance.
/// </summary>
/// <typeparam name="T">The type of the object being validated</typeparam>
public abstract class NullableAbstractValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Gets a value indicating the objects validated by this <see cref="NullableAbstractValidator{T}"/> are allowed to be <see langword="null"/>.
    /// </summary>
    protected abstract bool IsValueNullable { get; }

    /// <inheritdoc />
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        if (context.InstanceToValidate is null)
        {
            if (IsValueNullable)
                return new ValidationResult();
            return new ValidationResult(new[] { new ValidationFailure(string.Empty, "Object must not be null.") });
        }
        return base.Validate(context);
    }
}