using System.Text;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegFileConstants
{
    public const uint MegFileMagicNumber = 0x3F7D70A4;
    public const uint MegFileUnencryptedFlag = 0xFFFFFFFF;
    public const uint MegFileEncryptedFlag = 0x8FFFFFFF;

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
    public static readonly Encoding MegDataEntryPathEncoding = Encoding.ASCII;

    // This encoding *only* gets used for reading binary MEG files to maintain compatibility with Mike.NL's tool.
    // This way we can preserve the original file name so that consumers of this library can handle non-ASCII named files.
    public static readonly Encoding ExtendedMegEntryPathEncoding = Encoding.GetEncoding(28591);
}