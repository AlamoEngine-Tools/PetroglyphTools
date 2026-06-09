// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Services
{
    /// <summary>
    /// Provides lookup and validation operations over the registered Alamo language definitions.
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>Gets all registered language definitions.</summary>
        IReadOnlyList<IAlamoLanguageDefinition> AllLanguages { get; }

        /// <summary>Gets only the officially supported language definitions.</summary>
        IReadOnlyList<IAlamoLanguageDefinition> OfficiallySupported { get; }

        /// <summary>
        /// Gets the default game language (English).
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when no language definition is marked as default.
        /// </exception>
        IAlamoLanguageDefinition Default { get; }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="language"/> is in the officially supported list.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="language"/> is <see langword="null"/>.
        /// </exception>
        bool IsOfficiallySupported(IAlamoLanguageDefinition language);

        /// <summary>
        /// Attempts to find a language definition whose <see cref="IAlamoLanguageDefinition.LanguageIdentifier"/>
        /// matches <paramref name="identifier"/> (case-insensitive).
        /// </summary>
        bool TryGetByIdentifier(string identifier, out IAlamoLanguageDefinition? language);
    }
}
