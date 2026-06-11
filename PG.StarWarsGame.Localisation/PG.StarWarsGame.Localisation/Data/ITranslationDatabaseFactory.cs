// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data.Config;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// Creates empty translation databases for a given set of languages.
    /// </summary>
    public interface ITranslationDatabaseFactory
    {
        /// <summary>Creates a keyed database (unique keys, MasterText semantics).</summary>
        IKeyedTranslationDatabase CreateKeyed(IReadOnlyList<IAlamoLanguageDefinition> languages);

        /// <summary>Creates a keyed database using the languages from <paramref name="descriptor"/>.</summary>
        IKeyedTranslationDatabase CreateKeyed(TranslationProjectDescriptor descriptor);

        /// <summary>Creates an ordered database (duplicate keys allowed, Credits semantics).</summary>
        IOrderedTranslationDatabase CreateOrdered(IReadOnlyList<IAlamoLanguageDefinition> languages);

        /// <summary>Creates an ordered database using the languages from <paramref name="descriptor"/>.</summary>
        IOrderedTranslationDatabase CreateOrdered(TranslationProjectDescriptor descriptor);

        /// <summary>Returns a fluent builder for constructing a translation database.</summary>
        ITranslationDatabaseBuilder CreateDatabase();
    }
}
