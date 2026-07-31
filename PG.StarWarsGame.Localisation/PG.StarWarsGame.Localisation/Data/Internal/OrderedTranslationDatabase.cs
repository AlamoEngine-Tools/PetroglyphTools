// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class OrderedTranslationDatabase : TranslationDatabaseBase, IOrderedTranslationDatabase
    {
        private readonly List<TranslationEntry> _entries = new();

        public override int Count => _entries.Count;

        public override TranslationEntry this[int index] => _entries[index];

        internal OrderedTranslationDatabase(IReadOnlyList<IAlamoLanguageDefinition> languages)
            : base(languages) { }

        public override bool SetTranslation(string key, IAlamoLanguageDefinition language, string value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            EnsureLanguageRegistered(language);

            var entry = new TranslationEntry(key);
            entry.SetTranslation(language, value);
            _entries.Add(entry);
            return true;
        }

        public override bool RemoveEntry(string key)
        {
            var idx = _entries.FindIndex(e => e.Key == key);
            if (idx < 0) return false;
            _entries.RemoveAt(idx);
            return true;
        }

        public override void Clear() => _entries.Clear();

        public void InsertAt(int index, string key, IAlamoLanguageDefinition language, string value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            EnsureLanguageRegistered(language);

            var entry = new TranslationEntry(key);
            entry.SetTranslation(language, value);
            _entries.Insert(index, entry);
        }

        public void SetTranslationAt(int index, IAlamoLanguageDefinition language, string value)
        {
            EnsureLanguageRegistered(language);
            if (index < 0 || index >= _entries.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _entries[index].SetTranslation(language, value);
        }

        public IReadOnlyList<TranslationEntry> GetAllEntriesForKey(string key) =>
            _entries.Where(e => e.Key == key).ToList().AsReadOnly();

        public override IEnumerator<TranslationEntry> GetEnumerator() =>
            _entries.GetEnumerator();
    }
}
