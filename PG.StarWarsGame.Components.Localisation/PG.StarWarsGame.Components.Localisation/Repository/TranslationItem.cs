// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using FluentValidation;
using FluentValidation.Results;

namespace PG.StarWarsGame.Components.Localisation.Repository;

/// <inheritdoc cref="ITranslationItem" />
public sealed class TranslationItem : ITranslationItem
{
    internal TranslationItem()
    {
    }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="key">The string key.</param>
    /// <param name="value">The translated text.</param>
    /// <exception cref="ArgumentNullException"></exception>
    internal TranslationItem(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        Key = key ?? throw new ArgumentNullException(nameof(key));
        Value = value;
    }


    /// <inheritdoc />
    public string Key { get; private set; } = null!;

    /// <inheritdoc />
    public string Value { get; private set; } = null!;

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        return new TranslationItemValidator().Validate(this);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        new TranslationItemValidator().ValidateAndThrow(this);
    }

    /// <inheritdoc />
    private class TranslationItemValidator : AbstractValidator<TranslationItem>
    {
        internal TranslationItemValidator()
        {
            RuleFor(i => i.Key).NotNull().NotEmpty();
            RuleFor(i => i.Value).NotNull();
        }
    }
}