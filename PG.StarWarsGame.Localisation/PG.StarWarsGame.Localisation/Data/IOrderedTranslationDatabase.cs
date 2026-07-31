// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// A translation database where insertion order is preserved and duplicate keys are allowed
    /// (Credits / creditstext semantics).
    /// <see cref="ITranslationDatabase.SetTranslation"/> always appends a new entry.
    /// </summary>
    public interface IOrderedTranslationDatabase : ITranslationDatabase
    {
        /// <summary>
        /// Inserts a new entry at <paramref name="index"/> with an initial translation.
        /// </summary>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="language"/> is not present in <see cref="ITranslationDatabase.Languages"/>.
        /// </exception>
        void InsertAt(int index, string key, IAlamoLanguageDefinition language, string value);

        /// <summary>
        /// Sets the <paramref name="language"/> value on the existing entry at <paramref name="index"/>,
        /// rather than appending a new one. This is the only way to build a row carrying more than one
        /// language, since <see cref="ITranslationDatabase.SetTranslation"/> always appends.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is outside the bounds of the database.
        /// </exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="language"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="language"/> is not present in <see cref="ITranslationDatabase.Languages"/>.
        /// </exception>
        void SetTranslationAt(int index, IAlamoLanguageDefinition language, string value);

        /// <summary>Returns all entries whose key equals <paramref name="key"/>.</summary>
        IReadOnlyList<TranslationEntry> GetAllEntriesForKey(string key);
    }
}
