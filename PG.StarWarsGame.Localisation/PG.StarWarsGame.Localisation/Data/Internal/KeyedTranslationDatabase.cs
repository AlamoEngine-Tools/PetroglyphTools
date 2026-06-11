// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class KeyedTranslationDatabase : TranslationDatabaseBase, IKeyedTranslationDatabase
    {
        private readonly Dictionary<string, TranslationEntry> _entries = new(StringComparer.Ordinal);

        public override int Count => _entries.Count;

        public override TranslationEntry this[int index] => _entries.Values.ElementAt(index);

        internal KeyedTranslationDatabase(IReadOnlyList<IAlamoLanguageDefinition> languages)
            : base(languages) { }

        public override bool SetTranslation(string key, IAlamoLanguageDefinition language, string value)
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

        public override bool RemoveEntry(string key) => _entries.Remove(key);

        public override void Clear() => _entries.Clear();

        public bool ContainsKey(string key) => _entries.ContainsKey(key);

        public bool TryGetEntry(string key, [NotNullWhen(true)] out TranslationEntry? entry)
        {
            return _entries.TryGetValue(key, out entry);
        }

        public bool TryGetTranslation(string key, [MaybeNullWhen(false)] out string? value)
        {
            if (ActiveLanguage is null)
                throw new InvalidOperationException(
                    "ActiveLanguage must be set before calling TryGetTranslation(string, out string?).");
            if (!TryGetEntry(key, out var entry))
            {
                value = null;
                return false;
            }
            return entry.TryGetTranslation(ActiveLanguage, out value);
        }

        public override IEnumerator<TranslationEntry> GetEnumerator() =>
            _entries.Values.GetEnumerator();
    }
}
