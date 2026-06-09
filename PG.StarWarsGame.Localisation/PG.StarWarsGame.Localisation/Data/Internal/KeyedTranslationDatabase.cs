// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class KeyedTranslationDatabase : IKeyedTranslationDatabase
    {
        private readonly Dictionary<string, TranslationEntry> _entries =
            new Dictionary<string, TranslationEntry>(StringComparer.Ordinal);

        private IAlamoLanguageDefinition? _activeLanguage;

        public IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }

        public int Count => _entries.Count;

        public ITranslationEntry this[int index] => _entries.Values.ElementAt(index);

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

        internal KeyedTranslationDatabase(IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            Languages = languages;
        }

        public bool SetTranslation(string key, IAlamoLanguageDefinition language, string value)
        {
            if (key is null) throw new ArgumentNullException(nameof(key));
            if (language is null) throw new ArgumentNullException(nameof(language));

            if (_entries.TryGetValue(key, out var existing))
            {
                existing.SetTranslation(language, value);
                return false;
            }

            var entry = new TranslationEntry(key);
            entry.SetTranslation(language, value);
            _entries[key] = entry;
            return true;
        }

        public bool RemoveEntry(string key) => _entries.Remove(key);

        public void Clear() => _entries.Clear();

        public bool ContainsKey(string key) => _entries.ContainsKey(key);

        public bool TryGetEntry(string key, out ITranslationEntry? entry)
        {
            if (_entries.TryGetValue(key, out var e))
            {
                entry = e;
                return true;
            }
            entry = null;
            return false;
        }

        public bool TryGetTranslation(string key, out string? value)
        {
            if (_activeLanguage is null)
                throw new InvalidOperationException(
                    "ActiveLanguage must be set before calling TryGetTranslation(string, out string?).");
            if (!TryGetEntry(key, out var entry))
            {
                value = null;
                return false;
            }
            return entry!.TryGetTranslation(_activeLanguage, out value);
        }

        public IEnumerator<ITranslationEntry> GetEnumerator() =>
            _entries.Values.Cast<ITranslationEntry>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
