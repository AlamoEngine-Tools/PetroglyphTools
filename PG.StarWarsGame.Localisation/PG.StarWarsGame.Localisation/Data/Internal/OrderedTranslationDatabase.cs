// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class OrderedTranslationDatabase : IOrderedTranslationDatabase
    {
        private readonly List<TranslationEntry> _entries = new List<TranslationEntry>();

        private IAlamoLanguageDefinition? _activeLanguage;

        public IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }

        public int Count => _entries.Count;

        public ITranslationEntry this[int index] => _entries[index];

        public IAlamoLanguageDefinition? ActiveLanguage
        {
            get => _activeLanguage;
            set
            {
                if (value is not null && !Languages.Contains(value))
                    throw new ArgumentException(
                        $"Language '{value.LanguageIdentifier}' is not registered in this database.", nameof(value));
                _activeLanguage = value;
            }
        }

        internal OrderedTranslationDatabase(IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            Languages = languages;
        }

        public bool SetTranslation(string key, IAlamoLanguageDefinition language, string value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (language is null) throw new ArgumentNullException(nameof(language));

            var entry = new TranslationEntry(key);
            entry.SetTranslation(language, value);
            _entries.Add(entry);
            return true;
        }

        public bool RemoveEntry(string key)
        {
            var idx = _entries.FindIndex(e => e.Key == key);
            if (idx < 0) return false;
            _entries.RemoveAt(idx);
            return true;
        }

        public void Clear() => _entries.Clear();

        public void InsertAt(int index, string key, IAlamoLanguageDefinition language, string value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (language is null) throw new ArgumentNullException(nameof(language));

            var entry = new TranslationEntry(key);
            entry.SetTranslation(language, value);
            _entries.Insert(index, entry);
        }

        public IReadOnlyList<ITranslationEntry> GetAllEntriesForKey(string key) =>
            _entries.Where(e => e.Key == key).Cast<ITranslationEntry>().ToList().AsReadOnly();

        public IEnumerator<ITranslationEntry> GetEnumerator() =>
            _entries.Cast<ITranslationEntry>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
