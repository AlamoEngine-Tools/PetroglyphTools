// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// A translation database where keys are unique (MasterText / mastertextfile semantics).
    /// <see cref="ITranslationDatabase.SetTranslation"/> upserts by key.
    /// </summary>
    public interface IKeyedTranslationDatabase : ITranslationDatabase
    {
        /// <summary>Returns <see langword="true"/> if an entry with <paramref name="key"/> exists.</summary>
        bool ContainsKey(string key);

        /// <summary>Attempts to retrieve the entry for <paramref name="key"/>.</summary>
        bool TryGetEntry(string key, out ITranslationEntry? entry);

        /// <summary>
        /// Attempts to retrieve the translation for <paramref name="key"/> using <see cref="ITranslationDatabase.ActiveLanguage"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="ITranslationDatabase.ActiveLanguage"/> is <see langword="null"/>.
        /// </exception>
        bool TryGetTranslation(string key, out string? value);
    }
}
