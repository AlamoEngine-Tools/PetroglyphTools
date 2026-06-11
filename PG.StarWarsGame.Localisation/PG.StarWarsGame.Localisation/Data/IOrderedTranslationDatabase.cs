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
        void InsertAt(int index, string key, IAlamoLanguageDefinition language, string value);

        /// <summary>Returns all entries whose key equals <paramref name="key"/>.</summary>
        IReadOnlyList<TranslationEntry> GetAllEntriesForKey(string key);
    }
}
