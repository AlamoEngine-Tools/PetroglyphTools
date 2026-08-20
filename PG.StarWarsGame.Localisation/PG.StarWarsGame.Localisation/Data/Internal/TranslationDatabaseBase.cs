// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal abstract class TranslationDatabaseBase : ITranslationDatabase
    {
        private IAlamoLanguageDefinition? _activeLanguage;

        public IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }

        public IAlamoLanguageDefinition? ActiveLanguage
        {
            get => _activeLanguage;
            set
            {
                if (value is not null && !Languages.Any(l => l.Equals(value)))
                    throw new ArgumentException(
                        $"Language '{value.LanguageIdentifier}' is not registered in this database.", nameof(value));
                _activeLanguage = value;
            }
        }

        protected TranslationDatabaseBase(IReadOnlyList<IAlamoLanguageDefinition> languages)
        {
            Languages = languages;
        }

        /// <summary>
        /// Guards a write against a language the database was not constructed with. Without this the write
        /// would succeed into the entry's dictionary and then silently vanish on export, since the exporters
        /// only emit columns for <see cref="Languages"/>.
        /// </summary>
        protected void EnsureLanguageRegistered(IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            if (!Languages.Any(l => l.Equals(language)))
                throw new ArgumentException(
                    $"Language '{language.LanguageIdentifier}' is not registered in this database.", nameof(language));
        }

        public abstract int Count { get; }
        public abstract TranslationEntry this[int index] { get; }
        public abstract bool SetTranslation(string key, IAlamoLanguageDefinition language, string value);
        public abstract bool RemoveEntry(string key);
        public abstract void Clear();
        public abstract IEnumerator<TranslationEntry> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
