using PG.Commons.Hashing;
using System.Collections.Generic;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Test;

public static class DatTestData
{
    public const string SmallCrcValue = "small";   // 0x7545EA13
    public static readonly Crc32 SmallCrc = new(0x7545EA13);

    public const string BigCrcValue = "big";       // 0xD3FBE249
    public static readonly Crc32 BigCrc = new(0xD3FBE249);


    internal static DatBinaryFile CreateUnsortedBinary()
    {
        return new DatBinaryFile(new DatHeader(2),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                CreateIndexRecord(false),
                CreateIndexRecord(true),
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("b"),
                new("a"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                CreateKeyRecord(BigCrcValue),
                CreateKeyRecord(SmallCrcValue),
            }));
    }

    internal static IReadOnlyList<DatStringEntry> CreateUnsortedModel()
    {
        return [
            CreateEntry(false, "b"),
            CreateEntry(true, "a"),
        ];
    }

    internal static IReadOnlyList<DatStringEntry> CreateSortedModel()
    {
        return [
            CreateEntry(true, "a"),
            CreateEntry(false, "b"),
        ];
    }

    internal static DatBinaryFile CreateSortedBinary()
    {
        return new DatBinaryFile(new DatHeader(2),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                CreateIndexRecord(true),
                CreateIndexRecord(false),
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("a"),
                new("b"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                CreateKeyRecord(SmallCrcValue),
                CreateKeyRecord(BigCrcValue),
            }));
    }

    private static KeyTableRecord CreateKeyRecord(string key, string? alternateKey = null)
    {
        return new KeyTableRecord(key, alternateKey ?? key);
    }

    private static IndexTableRecord CreateIndexRecord(bool small, uint valueLength = 1)
    {
        return small
            ? new IndexTableRecord(SmallCrc, (uint)SmallCrcValue.Length, valueLength)
            : new IndexTableRecord(BigCrc, (uint)BigCrcValue.Length, valueLength);
    }

    private static DatStringEntry CreateEntry(bool small, string value)
    {
        return small
            ? new DatStringEntry(SmallCrcValue, SmallCrc, value)
            : new DatStringEntry(BigCrcValue, BigCrc, value);
    }
}