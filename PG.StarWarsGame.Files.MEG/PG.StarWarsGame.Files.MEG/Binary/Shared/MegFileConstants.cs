using System.Text;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegFileConstants
{
    public const uint MegFileMagicNumber = 0x3F7D70A4;
    public const uint MegFileUnencryptedFlag = 0xFFFFFFFF;
    public const uint MegFileEncryptedFlag = 0x8FFFFFFF;

    // The game only supports ASCII.
    // Some additional thoughts though, why we should try Unicode at some point:
    //
    // a) The binary file itself (probably) does not care for the actual byte size of a single character,
    //    cause reading the filename is controlled by the length info. So as long as we use the same encoding for string and length processing,
    //    we should always be on the safe side.
    // b) Unknown chars get converted into '?' which means that 'ä' and 'ö' suddenly get transformed into the same char which causes ambiguous files. 
    //    For this binary model we should not care, since files are processed by the table index value.
    //    It just makes things more convenient for this model. The processing services then can decide how to handle broken engine constraints.
    //    In fact, even Mike.nl uses Windows-1252 in his tool (it's a 1 byte encoding, though).
    //
    // TODO: Test this with Unicode/Default encoding at some point.
    //  Expected result is that the game loads the file just fine and also all embedded files, as long as their file names are engine supported, get processed.
    public static readonly Encoding MegContentFileNameEncoding = Encoding.ASCII;
}