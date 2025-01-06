using System;
using PG.Commons.Data;
using Xunit;

namespace PG.Commons.Test.DataTypes;

public class IndexRangeTest
{
    [Fact]
    public void Ctor_SetsProperties()
    {
        var range = new IndexRange(1, 2);
        Assert.Equal(1, range.Start);
        Assert.Equal(2, range.Length);
    }

    [Fact]
    public void Ctor_InvalidArgs_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new IndexRange(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new IndexRange(0, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new IndexRange(0, 0));
        Assert.Throws<OverflowException>(() => new IndexRange(int.MaxValue, 2));
    }

    [Theory]
    [InlineData(0, 1, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(0, 2, 1)]
    [InlineData(1, 2, 2)]
    public void End(int start, int length, int expectedEnd)
    {
        Assert.Equal(expectedEnd, new IndexRange(start, length).End);
    }

    [Fact]
    public void Equals_HashCode()
    {
        var range1 = new IndexRange(1, 2);
        var range2 = new IndexRange(1, 2);

        var range3 = new IndexRange(1, 3);
        var range4 = new IndexRange(2, 2);
        
        Assert.True(range1.Equals(range2));
        Assert.True(range1.Equals((object)range2));

        Assert.False(range1.Equals(null!));
        Assert.False(range1.Equals(default));
        Assert.False(range1.Equals(range3));
        Assert.False(range1.Equals(range4));

        Assert.Equal(range1.GetHashCode(), range2.GetHashCode());
        Assert.NotEqual(range1.GetHashCode(), range3.GetHashCode());
        Assert.NotEqual(range1.GetHashCode(), range4.GetHashCode());

        Assert.True(range1 == range2);
        Assert.True(range1 != range3);

        Assert.False(range1 == range4);
        Assert.False(range1 != range2);
    }
}