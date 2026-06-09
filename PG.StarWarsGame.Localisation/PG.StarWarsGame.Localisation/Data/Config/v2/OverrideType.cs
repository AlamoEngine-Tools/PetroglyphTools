// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.Data.Config.v2
{
    /// <summary>
    /// Specifies whether a translation project provides core game text, expansion text, or mod text.
    /// </summary>
    public enum OverrideType
    {
        /// <summary>Core base-game translation resources.</summary>
        Core,
        /// <summary>Expansion translation resources.</summary>
        Expansion,
        /// <summary>Mod-specific translation resources.</summary>
        Mod
    }
}
