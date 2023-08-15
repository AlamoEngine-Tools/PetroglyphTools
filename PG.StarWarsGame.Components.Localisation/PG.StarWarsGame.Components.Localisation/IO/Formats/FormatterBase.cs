// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using FluentValidation;
using FluentValidation.Results;
using PG.Commons.Repository;
using PG.Commons.Services;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats;

/// <inheritdoc cref="ILocalisationFormatter{TProcessingInstructions,TData}" />
public abstract class FormatterBase<TProcessingInstructions, TRepository> : ServiceBase,
    ILocalisationFormatter<TProcessingInstructions, TRepository>
    where TRepository : IRepository
    where TProcessingInstructions : ILocalisationFormatterProcessingInstructionsParam
{
    private TProcessingInstructions _param = default;

    /// <inheritdoc />
    public TProcessingInstructions Param
    {
        get => _param;
        set
        {
            value.ValidateAndThrow();
            _param = value;
        }
    }

    /// <inheritdoc />
    public ValidationResult Validate()
    {
        return new FormatterBaseValidator().Validate(this);
    }

    /// <inheritdoc />
    public void ValidateAndThrow()
    {
        new FormatterBaseValidator().ValidateAndThrow(this);
    }

    /// <inheritdoc />
    public ILocalisationFormatDescriptor<TProcessingInstructions, TRepository> Descriptor { get; }

    /// <inheritdoc />
    public abstract void Export(TRepository data);

    /// <inheritdoc />
    public abstract TRepository Import();

    /// <inheritdoc />
    public ILocalisationFormatter<TProcessingInstructions, TRepository> WithProcessingInstructions(
        TProcessingInstructions param)
    {
        Param = param;
        return this;
    }


    internal class FormatterBaseValidator : AbstractValidator<FormatterBase<TProcessingInstructions, TRepository>>
    {
        internal FormatterBaseValidator()
        {
            RuleFor(p => p.Param)
                .NotNull()
                .Must((_, p) => p.Validate().IsValid);
        }
    }

    /// <inheritdoc />
    protected FormatterBase(ILocalisationFormatDescriptor<TProcessingInstructions, TRepository> descriptor,
        IServiceProvider services) : base(services)
    {
        Descriptor = descriptor;
    }
}