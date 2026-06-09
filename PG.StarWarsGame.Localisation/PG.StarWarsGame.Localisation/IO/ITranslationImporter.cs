// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO
{
    /// <summary>
    /// Imports single-language translation data from <typeparamref name="TSource"/> into a database.
    /// </summary>
    public interface ITranslationImporter<TSource>
    {
        /// <summary>
        /// Reads <paramref name="source"/> and adds all entries for <paramref name="language"/>
        /// into <paramref name="target"/>.
        /// </summary>
        void Import(TSource source, IAlamoLanguageDefinition language, ITranslationDatabase target);
    }

    /// <summary>
    /// Imports multi-language translation data from <typeparamref name="TSource"/> into a database.
    /// </summary>
    public interface IMultiLanguageTranslationImporter<TSource>
    {
        /// <summary>
        /// Reads <paramref name="source"/> and adds all languages and entries into <paramref name="target"/>.
        /// Languages not tracked by <paramref name="target"/> are skipped.
        /// </summary>
        void Import(TSource source, ITranslationDatabase target);
    }
}
