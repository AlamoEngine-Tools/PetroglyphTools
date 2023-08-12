// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using FluentValidation;
using FluentValidation.Results;
using PG.Commons.Validation;

namespace PG.StarWarsGame.Components.Localisation.Languages;

/// <summary>
///     Base definition for an <see cref="IAlamoLanguageDefinition" /> implementation.
/// </summary>
public abstract class AlamoLanguageDefinitionBase : IAlamoLanguageDefinition, IValidatable
{
    /// <summary>
    ///     A string that is being used to identify the language of the *.DAT file, e.g. a language identifier
    ///     "english" would produce the file "mastertextfile_english.dat"
    /// </summary>
    protected abstract string ConfiguredLanguageIdentifier { get; }

    /// <summary>
    ///     The .NET Culture that best describes the language. This culture can be used for spell checking,
    ///     auto-translation between languages, etc.
    /// </summary>
    protected abstract CultureInfo ConfiguredCulture { get; }

    /// <inheritdoc />
    public string LanguageIdentifier => ConfiguredLanguageIdentifier;

    /// <inheritdoc />
    public CultureInfo Culture => ConfiguredCulture;

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    protected virtual bool EqualsInternal(IAlamoLanguageDefinition other)
    {
        return GetHashCodeInternal().CompareTo(other.GetHashCode()) == 0;
    }

    /// <summary>
    ///     Serves as the default hash function.
    /// </summary>
    /// <remarks>
    ///     The default implementation does intentionally create hash conflicts between identical language definitions,
    ///     e.g. using the same <see cref="LanguageIdentifier" /> and <see cref="Culture" /> for two different derived classes.
    /// </remarks>
    /// <returns></returns>
    protected virtual int GetHashCodeInternal()
    {
        return HashCode.Combine(LanguageIdentifier.ToUpper().GetHashCode(), Culture.GetHashCode());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is IAlamoLanguageDefinition alamoLanguageDefinition && EqualsInternal(alamoLanguageDefinition);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetHashCodeInternal();
    }

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        return ValidateInternal();
    }

    /// <summary>
    ///     Base <see cref="Validate" /> implementation.
    /// </summary>
    protected virtual ValidationResult ValidateInternal()
    {
        AlamoLanguageDefinitionValidatorBase v = new();
        return v.Validate(this);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        ValidateAndThrowInternal();
    }

    /// <summary>
    ///     Base <see cref="ValidateAndThrow" /> implementation.
    /// </summary>
    protected virtual void ValidateAndThrowInternal()
    {
        AlamoLanguageDefinitionValidatorBase v = new();
        v.ValidateAndThrow(this);
    }

    /// <summary>
    ///     Base class for <see cref="IAlamoLanguageDefinition" /> validations.
    /// </summary>
    protected class AlamoLanguageDefinitionValidatorBase : AbstractValidator<AlamoLanguageDefinitionBase>
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public AlamoLanguageDefinitionValidatorBase()
        {
            RuleFor(languageDefinition => languageDefinition.ConfiguredCulture).NotNull();
            RuleFor(languageDefinition => languageDefinition.ConfiguredLanguageIdentifier).NotNull().NotEmpty();
        }
    }
}