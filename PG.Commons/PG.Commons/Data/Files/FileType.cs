// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Data.Files
{
    /// <summary>
    ///     The file data encoding on disc.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        ///     A binary file type.
        /// </summary>
        Binary,

        /// <summary>
        ///     A plain-text file type.
        /// </summary>
        Text,

        /// <summary>
        ///     An Alamo Engine specific file type, the Mega Texture Directory (*.MTD) consisting of a binary file holding the meta
        ///     information about a linked texture atlas.
        /// </summary>
        TextureAtlasWithMetaInformation
    }
}
