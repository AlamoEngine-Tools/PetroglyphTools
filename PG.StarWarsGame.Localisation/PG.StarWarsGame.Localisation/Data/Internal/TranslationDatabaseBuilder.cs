// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class TranslationDatabaseBuilder : ITranslationDatabaseBuilder
    {
        private readonly List<IAlamoLanguageDefinition> _languages = new List<IAlamoLanguageDefinition>();
        private IAlamoLanguageDefinition? _activeLanguage;

        public ITranslationDatabaseBuilder WithLanguage(IAlamoLanguageDefinition language)
        {
            if (language is null) throw new ArgumentNullException(nameof(language));
            _languages.Add(language);
            return this;
        }

        public ITranslationDatabaseBuilder WithLanguages(IEnumerable<IAlamoLanguageDefinition> languages)
        {
            if (languages is null) throw new ArgumentNullException(nameof(languages));
            _languages.AddRange(languages);
            return this;
        }

        public ITranslationDatabaseBuilder SetActiveLanguage(IAlamoLanguageDefinition language)
        {
            _activeLanguage = language;
            return this;
        }

        public IKeyedTranslationDatabase BuildKeyed()
        {
            var langs = _languages.ToList().AsReadOnly();
            var db = new KeyedTranslationDatabase(langs);
            ApplyActiveLanguage(db);
            return db;
        }

        public IOrderedTranslationDatabase BuildOrdered()
        {
            var langs = _languages.ToList().AsReadOnly();
            var db = new OrderedTranslationDatabase(langs);
            ApplyActiveLanguage(db);
            return db;
        }

        private void ApplyActiveLanguage(ITranslationDatabase db)
        {
            if (_activeLanguage is null) return;
            if (!db.Languages.Contains(_activeLanguage))
                throw new ArgumentException(
                    $"Active language '{_activeLanguage.LanguageIdentifier}' was not added to this builder.",
                    nameof(_activeLanguage));
            db.ActiveLanguage = _activeLanguage;
        }
    }
}
