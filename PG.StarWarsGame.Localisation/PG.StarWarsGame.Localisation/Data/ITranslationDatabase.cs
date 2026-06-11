// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// An in-memory store of <see cref="TranslationEntry"/> objects for one or more languages.
    /// Changes are fully decoupled from any file format.
    /// </summary>
    public interface ITranslationDatabase : IReadOnlyList<TranslationEntry>
    {
        /// <summary>Gets the languages tracked by this database.</summary>
        IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }

        /// <summary>
        /// Adds or updates the translation value for <paramref name="key"/> in <paramref name="language"/>.
        /// </summary>
        /// <returns><see langword="true"/> if a new entry was created; <see langword="false"/> if an existing entry was updated.</returns>
        bool SetTranslation(string key, IAlamoLanguageDefinition language, string value);

        /// <summary>Removes the entry identified by <paramref name="key"/>.</summary>
        /// <returns><see langword="true"/> if an entry was removed; otherwise <see langword="false"/>.</returns>
        bool RemoveEntry(string key);

        /// <summary>Removes all entries from the database.</summary>
        void Clear();

        /// <summary>
        /// Gets or sets the language used by single-language convenience operations.
        /// Must be one of <see cref="Languages"/>, or <see langword="null"/> to clear.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// Set to a language not present in <see cref="Languages"/>.
        /// </exception>
        IAlamoLanguageDefinition? ActiveLanguage { get; set; }
    }
}
