// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Data.Files
{
    /// <summary>
    /// Minimal meta-information for an Alamo file.
    /// </summary>
    public interface IAlamoFileType
    {
        /// <summary>
        /// Describes how the file content is represented on disc.
        /// </summary>
        FileType Type { get; }

        /// <summary>
        /// The expected file extension, e.g. xml, exe, alo, ala, ...
        /// </summary>
        string FileExtension { get; }
    }
}
