// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.Data.Config.v2
{
    /// <summary>
    /// Specifies the file format used to store translation resources.
    /// </summary>
    public enum TranslationResourceType
    {
        /// <summary>Binary Petroglyph DAT format.</summary>
        Dat,
        /// <summary>XML format conforming to the eaw-translation schema.</summary>
        Xml,
        /// <summary>CSV format with a header row of language identifiers.</summary>
        Csv,
        /// <summary>Java-style .properties format (one file per language).</summary>
        Nls
    }
}
