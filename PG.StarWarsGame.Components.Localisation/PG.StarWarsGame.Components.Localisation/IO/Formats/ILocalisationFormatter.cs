// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Repository;
using PG.Commons.Validation;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats;

/// <summary>
///     A formatter for a given <see cref="IRepository" />
/// </summary>
/// <typeparam name="TProcessingInstructions">Additional processing instructions for the formatter.</typeparam>
/// <typeparam name="TData">The repository to perform the action on.</typeparam>
public interface ILocalisationFormatter<TProcessingInstructions, TData> : IValidatable
    where TProcessingInstructions : ILocalisationFormatterProcessingInstructionsParam
    where TData : IRepository
{
    /// <summary>
    ///     The current param.
    /// </summary>
    TProcessingInstructions Param { get; }

    /// <summary>
    ///     The corresponding descriptor.
    /// </summary>
    ILocalisationFormatDescriptor<TProcessingInstructions, TData> Descriptor { get; }

    /// <summary>
    ///     Exports the given <see cref="IRepository" /> according to the given
    ///     <see cref="ILocalisationFormatterProcessingInstructionsParam" />.
    /// </summary>
    void Export(TData repository);

    /// <summary>
    ///     Imports a given <see cref="IRepository" /> according to the given
    ///     <see cref="ILocalisationFormatterProcessingInstructionsParam" />.
    /// </summary>
    /// <returns></returns>
    TData Import();

    /// <summary>
    ///     Update the formatter with given <see cref="ILocalisationFormatterProcessingInstructionsParam" />.
    /// </summary>
    /// <param name="param">
    ///     The <see cref="ILocalisationFormatterProcessingInstructionsParam" /> param to update the formatter
    ///     with.
    /// </param>
    /// <returns></returns>
    ILocalisationFormatter<TProcessingInstructions, TData> WithProcessingInstructions(TProcessingInstructions param);
}