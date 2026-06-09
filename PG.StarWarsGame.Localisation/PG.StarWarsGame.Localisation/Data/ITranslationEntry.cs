// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data
{
    /// <summary>
    /// A single translation entry: a string key with zero or more language-specific values.
    /// </summary>
    public interface ITranslationEntry
    {
        /// <summary>Gets the translation key.</summary>
        string Key { get; }

        /// <summary>Gets all available translations, keyed by language definition.</summary>
        IReadOnlyDictionary<IAlamoLanguageDefinition, string> Translations { get; }

        /// <summary>
        /// Attempts to retrieve the translation value for <paramref name="language"/>.
        /// </summary>
        bool TryGetTranslation(IAlamoLanguageDefinition language, out string? value);
    }
}
