// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace PG.StarWarsGame.Files.DAT.Binary;

/// <summary>
///     Defines global constants of the Petroglyph DAT file format.
/// </summary>
public static class DatFileConstants
{
    /// <summary>
    ///     Returns the text key encoding, which is ASCII.
    /// </summary>
    public static readonly Encoding TextKeyEncoding = Encoding.ASCII;


    // This encoding *only* gets used for reading binary DAT files to maintain compatibility if some arbitrary tool does not use 7-bit ASCII.
    // This way we can preserve the original key.
    internal static readonly Encoding TextKeyEncoding_Latin1 = Encoding.GetEncoding(28591);

    /// <summary>
    ///     Returns the text value encoding, which is 16bit UTF Little Endian.
    /// </summary>
    public static readonly Encoding TextValueEncoding = Encoding.Unicode;
}