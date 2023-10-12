// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Repository;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv;

/// <inheritdoc cref="ILocalisationFormatter{TProcessingInstructions,TData}" />
public abstract class
    CsvFormatterBase<TRepository> : FormatterBase<CsvFormatterProcessingInstructionsParam, TRepository>
    where TRepository : IRepository
{
    /// <inheritdoc />
    protected CsvFormatterBase(
        ILocalisationFormatDescriptor<CsvFormatterProcessingInstructionsParam, TRepository> descriptor,
        IServiceProvider services) : base(descriptor, services)
    {
    }
}