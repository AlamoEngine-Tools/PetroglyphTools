// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// A fluent builder for constructing <see cref="ITranslationDatabase"/> instances.
    /// </summary>
    public interface ITranslationDatabaseBuilder
    {
        /// <summary>Adds a single language to the database.</summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="language"/> is <see langword="null"/>.</exception>
        ITranslationDatabaseBuilder WithLanguage(IAlamoLanguageDefinition language);

        /// <summary>Adds multiple languages to the database.</summary>
        /// <exception cref="System.ArgumentNullException"><paramref name="languages"/> is <see langword="null"/>.</exception>
        ITranslationDatabaseBuilder WithLanguages(IEnumerable<IAlamoLanguageDefinition> languages);

        /// <summary>
        /// Sets the active language for the database.
        /// Validation that the language is present in the database's language list occurs at build time.
        /// </summary>
        ITranslationDatabaseBuilder WithActiveLanguage(IAlamoLanguageDefinition language);

        /// <summary>
        /// Builds a keyed translation database (unique keys, MasterText semantics).
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// The active language (if set) is not in the accumulated language list.
        /// </exception>
        IKeyedTranslationDatabase BuildKeyed();

        /// <summary>
        /// Builds an ordered translation database (duplicate keys allowed, Credits semantics).
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// The active language (if set) is not in the accumulated language list.
        /// </exception>
        IOrderedTranslationDatabase BuildOrdered();
    }
}
