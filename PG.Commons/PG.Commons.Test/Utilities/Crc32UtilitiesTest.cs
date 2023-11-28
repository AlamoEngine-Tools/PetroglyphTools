using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class Crc32UtilitiesTest
{
    [TestMethod]
    [DataRow(1,2,3,4)]
    [DataRow(1,1,1,1,1)]
    [DataRow(1,2,2,2,3,5,100)]
    public void Test_EnsureSortedByCrc32(params int[] checksums)
    {
        var list = checksums.Select(checksum => new CrcHolder(checksum)).Cast<IHasCrc32>().ToList(); 
        ExceptionUtilities.AssertDoesNotThrowException(() => Crc32Utilities.EnsureSortedByCrc32(list));
    }

    [TestMethod]
    public void Test_EnsureSortedByCrc32_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => Crc32Utilities.EnsureSortedByCrc32<IHasCrc32>(null!));
    }

    [TestMethod]
    [DataRow(4, 3, 2, 1)]
    [DataRow(1, 1, 2, 4, 3, 5)]
    public void Test_EnsureSortedByCrc32_ThrowsUnsorted(params int[] checksums)
    {
        var list = checksums.Select(checksum => new CrcHolder(checksum)).ToList();
        Assert.ThrowsException<ArgumentException>(() => Crc32Utilities.EnsureSortedByCrc32(list));
    }


    private static IEnumerable<object[]> SortingTestData()
    {
        return new[]
        {
            new object[] {
                new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3) }, // Already sorted
                new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3) }
            },

            new object[] {
                new[] { ("a", 2), ("b", 1), ("c", 3), ("d", 1) },
                new[] { ("b", 1), ("d", 1), ("a", 2), ("c", 3) } // Ensure ("b", 1) is always before ("d", 1)
            },
        };
    }


    [DataTestMethod]
    [DynamicData(nameof(SortingTestData), DynamicDataSourceType.Method)]
    public void Test_SortByCrc32((string Id, int Crc)[] data, (string Id, int Crc)[] expectedList)
    {
        var list = data.Select(d=> new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();
        var expectedTransformed = expectedList.Select(d=> new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();

        var result = Crc32Utilities.SortByCrc32(list).ToList();

        CollectionAssert.AreEqual(expectedTransformed, result);
    }

    [TestMethod]
    public void Test_SortByCrc32_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => Crc32Utilities.SortByCrc32<IHasCrc32>(null!));
    }


    [TestMethod]
    public void Test_ListToCrcIndexRangeTable_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => Crc32Utilities.ListToCrcIndexRangeTable<IHasCrc32>(null!));
        Assert.ThrowsException<ArgumentException>(() => Crc32Utilities.ListToCrcIndexRangeTable(new List<CrcHolder>
        {
            new(1), 
            new(0)
        }));
    }


    private static IEnumerable<object[]> SortedTestDataForIndexTable()
    {
        return new[]
        {
            new object[] {
                new[] { ("a", 1), ("b", 1), ("c", 2), ("d", 3), ("e", 3), ("f", 4) }
            },
        };
    }

    //[DataTestMethod]
    //[DynamicData(nameof(SortedTestDataForIndexTable), DynamicDataSourceType.Method)]
    //public void Test_ListToCrcIndexRangeTable((string Id, int Crc)[] data, IDictionary<int, (int, int)> expectedDictionary)
    //{
    //    var list = data.Select(d => new CrcHolderWithIdentity(d.Id, d.Crc)).ToList();
        
    //    var expectedTransformed = new Dictionary<Crc32, IndexRange>();

    //    foreach (var tuple in expectedDictionary)
    //        expectedTransformed[new Crc32(tuple.Key)] = new IndexRange(tuple.Value.Item1, tuple.Value.Item2);

    //    var result = Crc32Utilities.ListToCrcIndexRangeTable(list);

    //}



    private class CrcHolder : IHasCrc32
    {
        public Crc32 Crc32 { get; }

        public CrcHolder(int number)
        {
            Crc32 = new Crc32(number);
        }
    }

    private class CrcHolderWithIdentity : IHasCrc32, IEquatable<CrcHolderWithIdentity>
    {
        public string Id { get; }

        public Crc32 Crc32 { get; }

        public CrcHolderWithIdentity(string id, int number)
        {
            Id = id;
            Crc32 = new Crc32(number);
        }

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
    }
}