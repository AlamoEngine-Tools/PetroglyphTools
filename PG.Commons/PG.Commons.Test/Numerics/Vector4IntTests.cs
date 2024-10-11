using System;
using PG.Commons.Numerics;
using Xunit;

namespace PG.Commons.Test.Numerics;

public class Vector4IntTests
{
    [Fact]
    public void Constructor_WithFourIntegers_ShouldInitializeCorrectly()
    {
        const int first = 5;
        const int second = 10;
        const int third = 15;
        const int fourth = 20;

        var vector = new Vector4Int(first, second, third, fourth);

        Assert.Equal(first, vector.First);
        Assert.Equal(second, vector.Second);
        Assert.Equal(third, vector.Third);
        Assert.Equal(fourth, vector.Fourth);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenAllValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2, 3, 4];

        var vector = new Vector4Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
        Assert.Equal(3, vector.Third);
        Assert.Equal(4, vector.Fourth);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenThreeValuesProvided()
    {
        ReadOnlySpan<int> values = [3, 4, 5];

        var vector = new Vector4Int(values);

        Assert.Equal(3, vector.First);
        Assert.Equal(4, vector.Second);
        Assert.Equal(5, vector.Third);
        Assert.Equal(0, vector.Fourth);
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenTwoValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2];

        var vector = new Vector4Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
        Assert.Equal(0, vector.Third);   // Default value for the third component
        Assert.Equal(0, vector.Fourth);  // Default value for the fourth component
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeToZero_WhenNoValuesProvided()
    {
        var values = ReadOnlySpan<int>.Empty;

        var vector = new Vector4Int(values);

        Assert.Equal(0, vector.First);   // Default value for the first component
        Assert.Equal(0, vector.Second);  // Default value for the second component
        Assert.Equal(0, vector.Third);   // Default value for the third component
        Assert.Equal(0, vector.Fourth);  // Default value for the fourth component
    }

    [Fact]
    public void Constructor_WithSpan_ShouldInitializeCorrectly_WhenMoreValuesProvided()
    {
        ReadOnlySpan<int> values = [1, 2, 3, 4, 5];

        var vector = new Vector4Int(values);

        Assert.Equal(1, vector.First);
        Assert.Equal(2, vector.Second);
        Assert.Equal(3, vector.Third);
        Assert.Equal(4, vector.Fourth);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenVectorsAreEqual()
    {
        var vector1 = new Vector4Int(1, 2, 3, 4);
        var vector2 = new Vector4Int(1, 2, 3, 4);

        Assert.True(vector1.Equals(vector2));
        Assert.True(vector1.Equals((object)vector2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenVectorsAreNotEqual()
    {
        var vector1 = new Vector4Int(1, 2, 3, 4);
        var vector2 = new Vector4Int(5, 6, 7, 8);

        Assert.False(vector1.Equals(vector2));
        Assert.False(vector1.Equals((object)vector2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNullObject()
    {
        var vector = new Vector4Int(1, 2, 3, 4);

        Assert.False(vector.Equals(null));
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualVectors()
    {
        var vector1 = new Vector4Int(1, 2, 3, 4);
        var vector2 = new Vector4Int(1, 2, 3, 4);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentHashCode_ForDifferentVectors()
    {
        var vector1 = new Vector4Int(1, 2, 3, 4);
        var vector2 = new Vector4Int(5, 6, 7, 8);

        var hashCode1 = vector1.GetHashCode();
        var hashCode2 = vector2.GetHashCode();

        Assert.NotEqual(hashCode1, hashCode2);
    }
}