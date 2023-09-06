// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation;
using FluentValidation.Results;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv;

/// <inheritdoc />
public record CsvFormatterProcessingInstructionsParam : ILocalisationFormatterProcessingInstructionsParam
{
    /// <inheritdoc />
    public string? Directory { get; init; }

    /// <inheritdoc />
    public string? FileName { get; init; }

    /// <inheritdoc />
    public string? FileExtension { get; init; } = "csv";

    /// <summary>
    ///     The separator used in the CSV file
    /// </summary>
    public char? Separator { get; init; } = ',';

    /// <summary>
    ///     The character used wo wrap a key, eg. "MY_KEY"
    /// </summary>
    public char? KeyWrapper { get; init; } = null;

    /// <summary>
    ///     The character used wo wrap a value, eg. "My new value!"
    /// </summary>
    public char? ValueWrapper { get; init; } = null;

    /// <summary>
    ///     Determines whether the output is alphabetized by key.
    /// </summary>
    public bool? AlphabetizeKeys { get; init; } = true;

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        return new CsvFormatterProcessingInstructionsParamValidator().Validate(this);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        new CsvFormatterProcessingInstructionsParamValidator().ValidateAndThrow(this);
    }

    private class CsvFormatterProcessingInstructionsParamValidator :
        AbstractValidator<CsvFormatterProcessingInstructionsParam>
    {
        internal CsvFormatterProcessingInstructionsParamValidator()
        {
            RuleFor(param => param.Directory)
                .NotNull()
                .NotEmpty();
            RuleFor(param => param.FileName)
                .NotNull()
                .NotEmpty();
            RuleFor(param => param.AlphabetizeKeys)
                .NotNull();
        }
    }
}