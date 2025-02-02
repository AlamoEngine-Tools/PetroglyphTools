using System;
using PG.Commons.Numerics;
using Xunit;

namespace PG.Commons.Test.Numerics;

public class Vector2IntTests
{ 
    [Fact]
    public void Constructor_WithTwoIntegers_ShouldInitializeCorrectly()
    {
        const int first = 5;
        const int second = 10;

        var vector = new Vector2Int(first, second);

        Assert.Equal(first, vector.First);
        Assert.Equal(second, vector.Second);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenBothValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2];

        var vector = new Vector2Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenOneValueProvided()
    {
        ReadOnlySpan<int> values = [3];

        var vector = new Vector2Int(values);

        Assert.Equal(3, vector.First);
        Assert.Equal(0, vector.Second);  // Default value for the second component
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenMoreValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2, 3];

        var vector = new Vector2Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
    }


    [Fact]
    public void Constructor_WithSpan_ShouldInitializeToZero_WhenNoValuesProvided()
    {
        var values = ReadOnlySpan<int>.Empty;

        var vector = new Vector2Int(values);

        Assert.Equal(0, vector.First);   // Default value for the first component
        Assert.Equal(0, vector.Second);  // Default value for the second component
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenVectorsAreEqual()
    {
        var vector1 = new Vector2Int(1, 2);
        var vector2 = new Vector2Int(1, 2);

        var result = vector1.Equals(vector2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenVectorsAreNotEqual()
    {
        var vector1 = new Vector2Int(1, 2);
        var vector2 = new Vector2Int(3, 4);

        var result = vector1.Equals(vector2);

        Assert.False(result);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingWithEqualObject()
    {
        var vector1 = new Vector2Int(1, 2);
        object vector2 = new Vector2Int(1, 2);

        var result = vector1.Equals(vector2);

        Assert.True(result);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithDifferentObject()
    {
        var vector1 = new Vector2Int(1, 2);
        object vector2 = new Vector2Int(3, 4);

        Assert.False(vector1.Equals(vector2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNullObject()
    {
        var vector = new Vector2Int(1, 2);

        Assert.False(vector.Equals(null));
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualVectors()
    {
        var vector1 = new Vector2Int(1, 2);
        var vector2 = new Vector2Int(1, 2);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentHashCode_ForDifferentVectors()
    {
        var vector1 = new Vector2Int(1, 2);
        var vector2 = new Vector2Int(3, 4);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.NotEqual(hashCode1, hashCode2);
    }
}