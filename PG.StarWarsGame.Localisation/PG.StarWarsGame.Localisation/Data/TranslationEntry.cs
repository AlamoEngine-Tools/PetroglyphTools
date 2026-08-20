// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// A single translation entry: a string key with zero or more language-specific values.
    /// </summary>
    public sealed class TranslationEntry
    {
        private readonly Dictionary<IAlamoLanguageDefinition, string> _translations;

        /// <summary>Gets the translation key.</summary>
        public string Key { get; }

        /// <summary>Gets all available translations, keyed by language definition.</summary>
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

        /// <summary>
        /// Attempts to retrieve the translation value for <paramref name="language"/>.
        /// </summary>
        public bool TryGetTranslation(IAlamoLanguageDefinition language, [MaybeNullWhen(false)] out string? value)
        {
            return _translations.TryGetValue(language, out value);
        }
    }
}
