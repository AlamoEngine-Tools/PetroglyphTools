// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace PG.StarWarsGame.Files.DAT.Binary;

/// <summary>
///     Simple class wrapper around global defaults used for all DAT file definitions.
/// </summary>
public static class DatFileConstants
{
    /// <summary>
    ///     The text key encoding. <br /><b>WARNING: The text keys may never be null-terminated!</b>
    /// </summary>
    public static readonly Encoding TextKeyEncoding = Encoding.ASCII;


    // This encoding *only* gets used for reading binary DAT files to maintain compatibility if some arbitrary tool does not use 7-bit ASCII.
    // This way we can preserve the original key.
    internal static readonly Encoding TextKeyEncoding_Latin1 = Encoding.GetEncoding(28591);

    /// <summary>
    ///     the value key encoding.
    /// </summary>
    public static readonly Encoding TextValueEncoding = Encoding.Unicode;
}