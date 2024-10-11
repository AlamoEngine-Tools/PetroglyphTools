using System;
using PG.Commons.Numerics;
using Xunit;

namespace PG.Commons.Test.Numerics;

public class Vector3IntTests
{
    [Fact]
    public void Constructor_WithThreeIntegers_ShouldInitializeCorrectly()
    {
        const int first = 5;
        const int second = 10;
        const int third = 15;

        var vector = new Vector3Int(first, second, third);

        Assert.Equal(first, vector.First);
        Assert.Equal(second, vector.Second);
        Assert.Equal(third, vector.Third);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenAllValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2, 3];

        var vector = new Vector3Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
        Assert.Equal(3, vector.Third);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenTwoValuesProvided()
    {
        ReadOnlySpan<int> values = [3, 4];

        var vector = new Vector3Int(values);

        Assert.Equal(3, vector.First);
        Assert.Equal(4, vector.Second);
        Assert.Equal(0, vector.Third);  // Default value for the third component
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenOneValueProvided()
    {
        ReadOnlySpan<int> values = [5];

        var vector = new Vector3Int(values);

        Assert.Equal(5, vector.First);
        Assert.Equal(0, vector.Second);  // Default value for the second component
        Assert.Equal(0, vector.Third);   // Default value for the third component
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenMoreValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2, 3, 4];

        var vector = new Vector3Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
        Assert.Equal(3, vector.Third); 
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeToZero_WhenNoValuesProvided()
    {
        var values = ReadOnlySpan<int>.Empty;

        var vector = new Vector3Int(values);

        Assert.Equal(0, vector.First);   // Default value for the first component
        Assert.Equal(0, vector.Second);  // Default value for the second component
        Assert.Equal(0, vector.Third);   // Default value for the third component
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenVectorsAreEqual()
    {
        var vector1 = new Vector3Int(1, 2, 3);
        var vector2 = new Vector3Int(1, 2, 3);

        Assert.True(vector1.Equals(vector2));
        Assert.True(vector1.Equals((object)vector2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNullObject()
    {
        var vector = new Vector3Int(1, 2, 3);

        Assert.False(vector.Equals(null));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenVectorsAreNotEqual()
    {
        var vector1 = new Vector3Int(1, 2, 3);
        var vector2 = new Vector3Int(4, 5, 6);
        
        Assert.False(vector1.Equals(vector2));
        Assert.False(vector1.Equals((object)vector2));
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualVectors()
    {
        var vector1 = new Vector3Int(1, 2, 3);
        var vector2 = new Vector3Int(1, 2, 3);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentHashCode_ForDifferentVectors()
    {
        var vector1 = new Vector3Int(1, 2, 3);
        var vector2 = new Vector3Int(4, 5, 6);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.NotEqual(hashCode1, hashCode2);
    }
}