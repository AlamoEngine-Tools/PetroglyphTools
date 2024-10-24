﻿using System;
using System.Runtime.CompilerServices;

namespace PG.Commons.Utilities;

// NB: This class is meant only to bring support for Math functions which are not available for all targeted .NET versions.
// PG specific implementations go into the engine library. The name PGMath is chosen because it's short. 
// TODO: Once C# supports extending static classes (System.Math in this case) this class should use that feature. 

/// <summary>
/// Provides constants and static methods for common mathematical functions not provided by all target versions of .NET.
/// </summary>
public static class PGMath
{
    /// <summary>
    /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The value to be clamped.</param>
    /// <param name="min">The lower bound of the result.</param>
    /// <param name="max">The upper bound of the result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max)
    {
#if NETSTANDARD2_1 || NET
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            throw new ArgumentException($"'{min}' cannot be greater than {max}");
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
#endif

    }

    /// <summary>
    /// Returns <paramref name="value"/> clamped to the inclusive range of <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="value">The value to be clamped.</param>
    /// <param name="min">The lower bound of the result.</param>
    /// <param name="max">The upper bound of the result.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(byte value, byte min, byte max)
    {
#if NETSTANDARD2_1 || NET
        return Math.Clamp(value, min, max);
#else
        if (min > max)
            throw new ArgumentException($"'{min}' cannot be greater than {max}");
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
#endif
    }
}