// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Data.Config
{
    /// <summary>
    /// Describes a translation project: which game it targets, its override level,
    /// the resource format it uses, and the languages it covers.
    /// </summary>
    public interface ITranslationProjectDescriptor
    {
        /// <summary>Gets the game this project provides translations for.</summary>
        GameType Game { get; }

        /// <summary>Gets whether this project provides core, expansion, or mod text.</summary>
        OverrideType OverrideType { get; }

        /// <summary>Gets the file format used to store translation resources.</summary>
        TranslationResourceType ResourceType { get; }

        /// <summary>Gets the languages covered by this translation project.</summary>
        IReadOnlyList<IAlamoLanguageDefinition> Languages { get; }
    }
}
