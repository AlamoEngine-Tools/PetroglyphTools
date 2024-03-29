using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.Testing;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class Crc32UtilitiesTest
{
    [Theory]
    [InlineData(1, 2, 3, 4)]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 2, 2, 2, 3, 5, 100, -1)]
    public void Test_EnsureSortedByCrc32(params int[] checksums)
    {
        var list = checksums.Select(checksum => new CrcHolder(checksum)).Cast<IHasCrc32>().ToList();
        ExceptionUtilities.AssertDoesNotThrowException(() => Crc32Utilities.EnsureSortedByCrc32(list));
    }

    [Fact]
    public void Test_EnsureSortedByCrc32_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Crc32Utilities.EnsureSortedByCrc32<IHasCrc32>(null!));
    }

    [Theory]
    [InlineData(4, 3, 2, 1)]
    [InlineData(1, 1, 2, 4, 3, 5)]
    [InlineData(-1, 0, 1, 2, 3)]
    public void Test_EnsureSortedByCrc32_ThrowsUnsorted(params int[] checksums)
    {
        var list = checksums.Select(checksum => new CrcHolder(checksum)).ToList();
        Assert.Throws<ArgumentException>(() => Crc32Utilities.EnsureSortedByCrc32(list));
    }


    public static IEnumerable<object[]> SortingTestData()
    {
        return new[]
        {
            [
                new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3) }, // Already sorted
                new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3) }
            ],
            [
                new[] { ("a", -1), ("b", 1), ("c", 2), ("d", 3) }, 
                new[] { ("b", 1), ("c", 2), ("d", 3), ("a", -1) }
            ],
            new object[]
            {
                new[] { ("a", 2), ("b", 1), ("c", 3), ("d", 1) },
                new[] { ("b", 1), ("d", 1), ("a", 2), ("c", 3) } // Ensure ("b", 1) is always before ("d", 1)
            },
        };
    }


    [Theory]
    [MemberData(nameof(SortingTestData))]
    public void Test_SortByCrc32((string Id, int Crc)[] data, (string Id, int Crc)[] expectedList)
    {
        var list = data.Select(d=> new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();
        var expectedTransformed = expectedList.Select(d=> new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();

        var result = Crc32Utilities.SortByCrc32(list).ToList();

        Assert.Equal(expectedTransformed, result);
    }

    [Fact]
    public void Test_SortByCrc32_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Crc32Utilities.SortByCrc32<IHasCrc32>(null!));
    }


    [Fact]
    public void Test_ListToCrcIndexRangeTable_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Crc32Utilities.ListToCrcIndexRangeTable<IHasCrc32>(null!));
        Assert.Throws<ArgumentException>(() => Crc32Utilities.ListToCrcIndexRangeTable(new List<CrcHolder>
        {
            new(1), 
            new(0)
        }));
    }


    public static IEnumerable<object[]> SortedTestDataForIndexTable()
    {
        return new[]
        {
            [
                new[] { 1, 1, 2, 3, 3, 4 },
                new Dictionary<int, (int, int)> { { 1, (0, 2) }, { 2, (2, 1) }, { 3, (3, 2) }, { 4, (5, 1) } }
            ],
            [
                new int[] { },
                new Dictionary<int, (int, int)>()
            ],
            [
                new[] { 1, 2, 3, 3 },
                new Dictionary<int, (int, int)> { { 1, (0, 1) }, { 2, (1, 1) }, { 3, (2, 2) } }
            ],
            new object[]
            {
                // (uint)-1 is larger than 1
                new[] { 1, -1 },
                new Dictionary<int, (int, int)> { { 1, (0, 1) }, { -1, (1, 1) } }
            },
        };
    }

    [Theory]
    [MemberData(nameof(SortedTestDataForIndexTable))]
    public void Test_ListToCrcIndexRangeTable(int[] inputData, Dictionary<int, (int, int)> expectedData)
    {
        var list = inputData.Select(d => new CrcHolder(d)).ToList();
        var expectedTransformed = new Dictionary<Crc32, IndexRange>();

        foreach (var tuple in expectedData)
            expectedTransformed[new Crc32(tuple.Key)] = new IndexRange(tuple.Value.Item1, tuple.Value.Item2);

        var result = Crc32Utilities.ListToCrcIndexRangeTable(list);

        Assert.Equal(expectedTransformed, result);
    }

    [Fact]
    public void Test_ItemsWithCrc_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Crc32Utilities.ItemsWithCrc(default, new Dictionary<Crc32, IndexRange>(), (IList<CrcHolder>)null!));
        Assert.Throws<ArgumentNullException>(() => Crc32Utilities.ItemsWithCrc(default, null!, new List<CrcHolder>()));
    }

    [Theory]
    [MemberData(nameof(SortedTestDataForItemsWithCrc))]
    public void Test_ItemsWithCrc((string Id, int Crc)[] data, ICollection<ItemsWithCrcQueryData> queries)
    {
        var list = data.Select(d => new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();

        var map = Crc32Utilities.ListToCrcIndexRangeTable(list);

        foreach (var queryData in queries)
        {
            var items = Crc32Utilities.ItemsWithCrc(queryData.Crc, map, list);
            Assert.Equal(queryData.ExpectedItems, items);
        }
    }

    public static IEnumerable<object[]> SortedTestDataForItemsWithCrc()
    {
        yield return
        [
            Array.Empty<(string, int)>(),
            new List<ItemsWithCrcQueryData>
            {
                new()
                {
                    Crc = new Crc32(0),
                    ExpectedItems = []
                }
            }
        ];
        yield return
        [
            new[] { ("a", 1) },
            new List<ItemsWithCrcQueryData>
            {
                new()
                {
                    Crc = new Crc32(0),
                    ExpectedItems = []
                },
                new()
                {
                    Crc = new Crc32(1),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("a", 1)
                    ]
                },
            }
        ];
        yield return
        [
            new[] { ("a", 1), ("b", 1) },
            new List<ItemsWithCrcQueryData>
            {
                new()
                {
                    Crc = new Crc32(0),
                    ExpectedItems = []
                },
                new()
                {
                    Crc = new Crc32(1),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("a", 1),
                        new CrcHolderWithIdentity("b", 1),
                    ]
                },
            }
        ];
        yield return
        [
            new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3), ("e", 3), ("f", 4) },
            new List<ItemsWithCrcQueryData>
            {
                new()
                {
                    Crc = new Crc32(0),
                    ExpectedItems = []
                },
                new()
                {
                    Crc = new Crc32(1),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("a", 1), 
                        new CrcHolderWithIdentity("b", 1)
                    ]
                },
                new()
                {
                    Crc = new Crc32(2),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("c", 2),
                    ]
                },
                new()
                {
                    Crc = new Crc32(3),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("d", 3),
                        new CrcHolderWithIdentity("e", 3)
                    ]
                },
                new()
                {
                    Crc = new Crc32(4),
                    ExpectedItems = [
                        new CrcHolderWithIdentity("f", 4),
                    ]
                },
            }
        ];
    }

    public struct ItemsWithCrcQueryData
    {
        public Crc32 Crc { get; init; }

        public IList<CrcHolderWithIdentity> ExpectedItems { get; init; }
    }


    public class CrcHolder(int number) : IHasCrc32
    {
        public Crc32 Crc32 { get; } = new(number);
    }

    public class CrcHolderWithIdentity(string id, int number) : IHasCrc32, IEquatable<CrcHolderWithIdentity>
    {
        public string Id { get; } = id;

        public Crc32 Crc32 { get; } = new(number);

        public bool Equals(CrcHolderWithIdentity? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Crc32.Equals(other.Crc32);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CrcHolderWithIdentity)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Crc32);
        }

        public override string ToString()
        {
            return $"{Id}:{Crc32}";
        }
    }
}