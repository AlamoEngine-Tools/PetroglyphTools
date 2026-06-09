// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Baseline
{
    /// <summary>
    /// Provides pre-loaded translation databases built from the official EaW and FoC game localisations.
    /// </summary>
    public interface IBaselineTranslationProvider
    {
        /// <summary>
        /// Returns a keyed translation database (MasterText) for the given game and single language.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="language"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="game"/> is not EaW or FoC.</exception>
        IKeyedTranslationDatabase GetMasterText(GameType game, IAlamoLanguageDefinition language);

        /// <summary>
        /// Returns a keyed translation database (MasterText) merged from the given game and languages.
        /// </summary>
        IKeyedTranslationDatabase GetMasterText(GameType game, IReadOnlyList<IAlamoLanguageDefinition> languages);

        /// <summary>
        /// Returns an ordered translation database (CreditsText) for the given game and single language.
        /// </summary>
        /// <exception cref="ArgumentNullException">When <paramref name="language"/> is null.</exception>
        /// <exception cref="ArgumentException">When <paramref name="game"/> is not EaW or FoC.</exception>
        IOrderedTranslationDatabase GetCreditsText(GameType game, IAlamoLanguageDefinition language);

        /// <summary>
        /// Returns an ordered translation database (CreditsText) merged from the given game and languages.
        /// </summary>
        IOrderedTranslationDatabase GetCreditsText(GameType game, IReadOnlyList<IAlamoLanguageDefinition> languages);
    }
}
