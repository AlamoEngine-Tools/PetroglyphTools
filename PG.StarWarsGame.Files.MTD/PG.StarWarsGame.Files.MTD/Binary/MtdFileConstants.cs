// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace PG.StarWarsGame.Files.MTD.Binary;

/// <summary>
///     Simple class wrapper around global defaults used for all MTD file definitions.
/// </summary>
public static class MtdFileConstants
{
    /// <summary>
    ///     The text key encoding. <br /><b>WARNING: When writing MTD files, file names are zero-padded to fill 64 bytes.</b>
    /// </summary>
    public static readonly Encoding NameEncoding = Encoding.ASCII;

    /// <summary>
    /// The maximum allowed length of a file name for MTD entries.
    /// </summary>
    /// <remarks>
    /// Max size is 63, because we need to reserve on byte for the zero terminator '\0' when converting to a binary file.
    /// </remarks>
    public const int MaxFileNameSize = 63;
}