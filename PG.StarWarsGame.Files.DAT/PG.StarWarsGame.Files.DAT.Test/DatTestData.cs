using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.DAT.Test;

public static class DatTestData
{
    public const string SmallCrcValue = "small";   // 0x7545EA13
    public static readonly Crc32 SmallCrc = new(0x7545EA13);

    public const string BigCrcValue = "big";       // 0xD3FBE249
    public static readonly Crc32 BigCrc = new(0xD3FBE249);
}