using System;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using Xunit;

namespace PG.Commons.Test.DataTypes;

public readonly struct HasCrcStruct : IHasCrc32, IEquatable<HasCrcStruct>
{
    public HasCrcStruct(int crc32)
    {
        Crc32 = new Crc32(crc32);
        Object = new object();
    }

    public Crc32 Crc32 { get; }

    public object Object { get; } = new();

    public bool Equals(HasCrcStruct other)
    {
        return Crc32.Equals(other.Crc32) && Object.Equals(other.Object);
    }

    public override bool Equals(object? obj)
    {
        return obj is HasCrcStruct other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Crc32, Object);
    }
}

public class HasCrcClass(int crc32) : IHasCrc32, IEquatable<HasCrcClass>
{
    public Crc32 Crc32 { get; } = new(crc32);

    public object Object { get; } = new();

    public bool Equals(HasCrcClass? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Crc32.Equals(other.Crc32) && Object.Equals(other.Object);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((HasCrcClass)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Crc32, Object);
    }
}

public class CrcBasedEqualityComparerTest_Class : CrcBasedEqualityComparerTest<HasCrcClass>
{
    protected override HasCrcClass CreateT(int crc)
    {
        return new HasCrcClass(crc);
    }
}

public class CrcBasedEqualityComparerTest_Struct : CrcBasedEqualityComparerTest<HasCrcStruct>
{
    protected override HasCrcStruct CreateT(int crc)
    {
        return new HasCrcStruct(crc);
    }
}

public abstract class CrcBasedEqualityComparerTest<T> where T : IHasCrc32, IEquatable<T>
{
    protected abstract T CreateT(int crc);

    [Fact]
    public void Test_Equals()
    {
        var comparer = CrcBasedEqualityComparer<T>.Instance;
        Assert.True(comparer.Equals(CreateT(0), CreateT(0)));
        Assert.False(comparer.Equals(CreateT(1), CreateT(2)));
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var comparer = CrcBasedEqualityComparer<T>.Instance;

        Assert.Equal(comparer.GetHashCode(CreateT(1)), comparer.GetHashCode(CreateT(1)));
        Assert.NotEqual(comparer.GetHashCode(CreateT(1)), comparer.GetHashCode(CreateT(2)));
    }
}