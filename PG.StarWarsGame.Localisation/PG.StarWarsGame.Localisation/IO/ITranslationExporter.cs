// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO
{
    /// <summary>
    /// Exports single-language translation data from a database to <typeparamref name="TResult"/>.
    /// </summary>
    public interface ITranslationExporter<TResult>
    {
        /// <summary>
        /// Serialises the translations for <paramref name="language"/> from <paramref name="source"/>
        /// into <typeparamref name="TResult"/>.
        /// </summary>
        TResult Export(ITranslationDatabase source, IAlamoLanguageDefinition language);
    }

    /// <summary>
    /// Exports multi-language translation data from a database to <typeparamref name="TResult"/>.
    /// </summary>
    public interface IMultiLanguageTranslationExporter<TResult>
    {
        /// <summary>
        /// Serialises all languages and entries from <paramref name="source"/> into <typeparamref name="TResult"/>.
        /// </summary>
        TResult Export(ITranslationDatabase source);
    }
}
