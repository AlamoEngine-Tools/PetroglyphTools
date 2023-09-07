using System.Text;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegFileConstants
{
    public const uint MegFileMagicNumber = 0x3F7D70A4;
    public const uint MegFileUnencryptedFlag = 0xFFFFFFFF;
    public const uint MegFileEncryptedFlag = 0x8FFFFFFF;

    // The game only supports ASCII. While other encoding, especially single-byte encodings like Latin1, might be possible
    // we do not support that here. Using Latin1 for example causes ambiguous files names.
    // e.g.:
    //      'ß.txt' --> '?.txt'
    //      'ä.txt' --> '?.txt'
    // Note: Mike.NL's Meg tool actually uses Latin1 (Windows-1251) causing this exact situation.
    public static readonly Encoding MegContentFileNameEncoding = Encoding.ASCII;
}