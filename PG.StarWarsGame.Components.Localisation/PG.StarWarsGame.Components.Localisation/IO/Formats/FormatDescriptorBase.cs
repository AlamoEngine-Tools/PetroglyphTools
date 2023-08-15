// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Repository;
using PG.Commons.Services;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats;

/// <inheritdoc cref="ILocalisationFormatDescriptor{TProcessingInstructions,TRepository}" />
public abstract class FormatDescriptorBase<TProcessingInstructions, TRepository> : ServiceBase,
    ILocalisationFormatDescriptor<TProcessingInstructions, TRepository>
    where TProcessingInstructions : ILocalisationFormatterProcessingInstructionsParam
    where TRepository : IRepository
{
    /// <summary>
    ///     Fallback file name base.
    /// </summary>
    protected string FallbackFileNameBase => "translation";

    /// <inheritdoc />
    protected FormatDescriptorBase(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public abstract string ToFileName(ILocalisationFormatter<TProcessingInstructions, TRepository> formatter,
        IAlamoLanguageDefinition definition);

    /// <inheritdoc />
    public abstract ILocalisationFormatter<TProcessingInstructions, TRepository> CreateFormatter(
        TProcessingInstructions param);
}