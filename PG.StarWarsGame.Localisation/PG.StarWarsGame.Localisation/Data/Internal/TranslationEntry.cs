// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Internal
{
    internal sealed class TranslationEntry : ITranslationEntry
    {
        private readonly Dictionary<IAlamoLanguageDefinition, string> _translations;

        public string Key { get; }

        public IReadOnlyDictionary<IAlamoLanguageDefinition, string> Translations => _translations;

        internal TranslationEntry(string key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            _translations = new Dictionary<IAlamoLanguageDefinition, string>();
        }

        internal void SetTranslation(IAlamoLanguageDefinition language, string value)
        {
            _translations[language] = value;
        }

        public bool TryGetTranslation(IAlamoLanguageDefinition language, out string? value)
        {
            return _translations.TryGetValue(language, out value);
        }
    }
}
