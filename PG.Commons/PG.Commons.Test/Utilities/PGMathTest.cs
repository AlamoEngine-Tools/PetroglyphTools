using System;
using System.Collections.Generic;
using PG.Commons.Utilities;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class PGMathTest
{
    public static IEnumerable<object[]> Clamp_UnsignedInt_TestData()
    {
        yield return [1, 1, 3, 1];
        yield return [2, 1, 3, 2];
        yield return [3, 1, 3, 3];
        yield return [1, 1, 1, 1];

        yield return [0, 1, 3, 1];
        yield return [4, 1, 3, 3];
    }

    public static IEnumerable<object[]> Clamp_SignedInt_TestData()
    {
        yield return [-1, -1, 1, -1];
        yield return [0, -1, 1, 0];
        yield return [1, -1, 1, 1];
        yield return [1, -1, 1, 1];

        yield return [-2, -1, 1, -1];
        yield return [2, -1, 1, 1];
    }

    [Theory]
    [MemberData(nameof(Clamp_SignedInt_TestData))]
    public static void Clamp_Int(int value, int min, int max, int expected)
    {
        Assert.Equal(expected, PGMath.Clamp(value, min, max));
#if NET
        Assert.Equal(expected, Math.Clamp(value, min, max));
#endif

    }

    [Theory]
    [MemberData(nameof(Clamp_UnsignedInt_TestData))]
    public static void Clamp_Byte(byte value, byte min, byte max, byte expected)
    {
        Assert.Equal(expected, PGMath.Clamp(value, min, max));
#if NET
        Assert.Equal(expected, Math.Clamp(value, min, max));
#endif
    }
}