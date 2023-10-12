// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Repository;
using PG.StarWarsGame.Components.Localisation.Languages;

namespace PG.StarWarsGame.Components.Localisation.IO.Formats;

/// <summary>
///     The base contract for a file format.
/// </summary>
/// <typeparam name="TProcessingInstructions">
///     The processing instructions for the associated
///     <see cref="ILocalisationFormatter{TProcessingInstructions,TData}" />
/// </typeparam>
/// <typeparam name="TRepository">The repository to perform actions on.</typeparam>
public interface ILocalisationFormatDescriptor<TProcessingInstructions, TRepository>
    where TProcessingInstructions : ILocalisationFormatterProcessingInstructionsParam
    where TRepository : IRepository
{
    /// <summary>
    ///     Generates the final file name for the given format.
    /// </summary>
    /// <param name="formatter">The current formatter.</param>
    /// <param name="definition">The used <see cref="IAlamoLanguageDefinition" /></param>
    /// <returns></returns>
    string ToFileName(ILocalisationFormatter<TProcessingInstructions, TRepository> formatter,
        IAlamoLanguageDefinition definition);

    /// <summary>
    ///     Factory method for the associated <see cref="ILocalisationFormatter{TProcessingInstructions,TData}" />
    /// </summary>
    /// <param name="param">The formatter param.</param>
    /// <returns>The constructed formatter.</returns>
    ILocalisationFormatter<TProcessingInstructions, TRepository> CreateFormatter(TProcessingInstructions param);
}