// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using FluentValidation;
using FluentValidation.Results;
using PG.Commons.Validation;

namespace PG.StarWarsGame.Localisation.Languages;

public abstract class AlamoLanguageDefinitionBase : IAlamoLanguageDefinition, IComparable, IValidatable
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

    public string LanguageIdentifier => ConfiguredLanguageIdentifier;
    public CultureInfo Culture => ConfiguredCulture;

    /// <summary>
    ///     Compares the current instance with another object of the same type and returns an integer that indicates
    ///     whether the current instance precedes, follows, or occurs in the same position in the sort order as the
    ///     other object.
    /// </summary>
    /// <remarks>
    ///     The <see cref="IAlamoLanguageDefinition" /> specific override differs in its sort order in such a way,
    ///     that all elements are ordered by their <see cref="CultureInfo.TwoLetterISOLanguageName" />t.
    /// </remarks>
    /// <param name="other"></param>
    /// <returns></returns>
    protected virtual int CompareToInternal(IAlamoLanguageDefinition other)
    {
        return string.Compare(Culture.TwoLetterISOLanguageName, other.Culture.TwoLetterISOLanguageName,
            StringComparison.Ordinal);
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    protected virtual bool EqualsInternal(IAlamoLanguageDefinition other)
    {
        return CompareTo(other) == 0;
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
        unchecked
        {
            return (ConfiguredLanguageIdentifier.GetHashCode() * 397) ^ Culture.GetHashCode();
        }
    }

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

    public int CompareTo(object obj)
    {
        if (obj is not IAlamoLanguageDefinition other)
        {
            throw new ArgumentException(
                $"The type of {obj.GetType()} is not assignable to {typeof(IAlamoLanguageDefinition)}. The two objects cannot be compared.");
        }

        return CompareToInternal(other);
    }

    public override int GetHashCode()
    {
        return GetHashCodeInternal();
    }

    public ValidationResult Validate()
    {
        return ValidateInternal();
    }

    protected virtual ValidationResult ValidateInternal()
    {
        AlamoLanguageDefinitionValidatorBase v = new();
        return v.Validate(this);
    }

    public void ValidateAndThrow()
    {
        ValidateAndThrowInternal();
    }

    protected virtual void ValidateAndThrowInternal()
    {
        AlamoLanguageDefinitionValidatorBase v = new();
        v.ValidateAndThrow(this);
    }

    protected class AlamoLanguageDefinitionValidatorBase : AbstractValidator<AlamoLanguageDefinitionBase>
    {
        public AlamoLanguageDefinitionValidatorBase()
        {
            RuleFor(languageDefinition => languageDefinition.ConfiguredCulture).NotNull();
            RuleFor(languageDefinition => languageDefinition.ConfiguredLanguageIdentifier).NotNull().NotEmpty();
        }
    }
}