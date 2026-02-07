// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Security.Cryptography;
using System.Text;

namespace PG.StarWarsGame.Files.MEG.Binary;

/// <summary>
/// Defines constants for handling MEG files.
/// </summary>
public class MegFileConstants
{
    internal const uint MegFileMagicNumber = 0x3F7D70A4;
    internal const uint MegFileUnencryptedFlag = 0xFFFFFFFF;
    internal const uint MegFileEncryptedFlag = 0x8FFFFFFF;

    internal const ushort MegMaxEntryPathLength = ushort.MaxValue;
    internal const uint MegMaxEntrySize = uint.MaxValue;

    // There is no 'real' max file size, but there are a couple of scenarios to consider.
    // 1) Max file size could be interpreted as (2^33) - 2, which would represent a MEG file with two data entries. 
    // The size of Entry1 is just large enough so that the data start index of Entry2 starts at uint.MaxValue (2^32 - 1).
    // Entry2 itself is uint.MaxValue bytes large.
    // This makes MaxFileSize = (2^32 - 1) + (2^32 - 1) = 2^33 - 2 = 0x1FFFFFFFE bytes (~8GB)
    // 2) If we consider a MEG file with only metadata (entry size is 0),
    // we could theoretically create a filename table that is (2 + ushort.MaxValue) * int.MaxValue large, which is ~140TB.
    //
    // NB: Even 2) not the real max file size, but it does not make sense to actually allow any of these cases.
    // We limit this library 4GB in order to prevent consumers writing absurd big MEG files. 
    // Additionally, having the limit to be in bounds of uint simplifies things.
    internal const uint MegMaxFileSize = uint.MaxValue;
    
    /// <summary>
    /// The max number of characters allowed in Empire at War game for MEG entry paths.
    /// </summary>
    public const int EawMaxEntryPathLength = 259;

    // The Eaw/Foc engines have an implementation limitation that limits the capabilities of a MEG size under certain circumstances.
    // To quote Mike.NL here:
    //      MegaFileClass reads the index, and stores unsigned ints.
    //      MegaFileClass::Subfile_Read reads an individual file by calling FileClass::Seek and FileClass::Read.
    //      The former takes a int for the offset.It ends up calling Win32's SetFilePointer with a negative offset and FILE_BEGIN,
    //      which fails and returns INVALID_SET_FILE_POINTER (0xFFFFFFFF).
    //      FileClass::Seek ignores this failure and just continues with the new offset of UINT_MAX.
    //
    // For that situation that an entry overlaps the 2GB boundary:
    //      The Read operation itself works, so if the entire sub-file is read with just Read calls, it works.
    //      If, however, Seek is called on the sub-file while reading it (e.g. to skip past unsupported chunks in a ChunkFile),
    //      then it doesn't, because MegaFileClass::Subfile_Seek calculates the new offset,
    //      which might lie above 2GB, and calls FileClass::Seek again with a signed integer. 
    //
    // Therefore, we limit the max entry size, and thus the max file size to int.MaxValue (2GB) for Eaw/Foc MEG files.
    /// <summary>
    /// The max size of a MEG entry for Empire at War / Forces of Corruption.
    /// </summary>
    public const int EawMegMaxEntrySize = int.MaxValue;
    
    /// <summary>
    /// The max file size of a MEG file for Empire at War / Forces of Corruption.
    /// </summary>
    public const int EawMegMaxFileSize = EawMegMaxEntrySize;

    // However, the specification does not state which encoding is required but instead relies on the number of characters of a string.
    // Implicitly, a 1:1 ratio for bytes - # chars is required though, which limits the possible encodings to a single-byte encoding,
    // such as ASCII, ISO 8859, Windows 1252, etc.
    //
    // For file names such as MEG files or MEG data entries, the game only supports ASCII.
    // 
    // Note: Mike.NL's MEG tool actually uses an extended ASCII encoding, probably ISO 8859-1.
    // This can cause situations where arbitrary MEG files might have data entries causing ambiguities:
    // e.g.:
    //      'ß.txt' --> '?.txt'
    //      'ä.txt' --> '?.txt'
    /// <summary>
    /// ASCII encoding is used for MEG entries.
    /// </summary>
    public static readonly Encoding MegDataEntryPathEncoding = Encoding.ASCII;

    // This encoding *only* gets used for reading binary MEG files to maintain compatibility with Mike.NL's tool.
    // This way we can preserve the original file name so that consumers of this library can handle non-ASCII named files.
    internal static readonly Encoding ExtendedMegEntryPathEncoding = Encoding.GetEncoding(28591);
    
    
    // Encryption Config
    internal const CipherMode AesMode = CipherMode.CBC;
    internal const PaddingMode AesPadding = PaddingMode.Zeros;
    internal const uint AesBlockSize = 128 / 8; // AES128 = 16bytes block size
}