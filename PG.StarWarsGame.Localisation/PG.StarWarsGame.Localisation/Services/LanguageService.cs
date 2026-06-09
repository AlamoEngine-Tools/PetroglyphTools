// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Services
{
    /// <summary>
    /// Default implementation of <see cref="ILanguageService"/>.
    /// </summary>
    public sealed class LanguageService : ILanguageService
    {
        private readonly IReadOnlyList<IAlamoLanguageDefinition> _all;
        private readonly IReadOnlyList<IAlamoLanguageDefinition> _officiallySupported;
        private readonly Dictionary<string, IAlamoLanguageDefinition> _byIdentifier;

        /// <inheritdoc/>
        public IReadOnlyList<IAlamoLanguageDefinition> AllLanguages => _all;

        /// <inheritdoc/>
        public IReadOnlyList<IAlamoLanguageDefinition> OfficiallySupported => _officiallySupported;

        /// <inheritdoc/>
        public IAlamoLanguageDefinition Default =>
            _all.FirstOrDefault(l => l.IsDefault)
            ?? throw new InvalidOperationException("No default language definition is registered.");

        /// <summary>
        /// Initialises the service with the given language definitions.
        /// </summary>
        public LanguageService(IEnumerable<IAlamoLanguageDefinition> languages)
        {
            if (languages is null)
                throw new ArgumentNullException(nameof(languages));

            var list = languages.ToList();
            _all = list.AsReadOnly();
            _officiallySupported = list.Where(l => l.IsOfficiallySupported).ToList().AsReadOnly();
            _byIdentifier = list.ToDictionary(
                l => l.LanguageIdentifier,
                l => l,
                StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool IsOfficiallySupported(IAlamoLanguageDefinition language)
        {
            if (language is null)
                throw new ArgumentNullException(nameof(language));
            return _byIdentifier.TryGetValue(language.LanguageIdentifier, out var found)
                   && found.IsOfficiallySupported;
        }

        /// <inheritdoc/>
        public bool TryGetByIdentifier(string identifier, out IAlamoLanguageDefinition? language)
        {
            return _byIdentifier.TryGetValue(identifier, out language);
        }
    }
}
