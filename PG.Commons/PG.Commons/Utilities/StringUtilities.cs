﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.CompilerServices;
using System.Text;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.Extensions;
#endif


namespace PG.Commons.Utilities;

/// <summary>
/// Provides primitive helper methods for strings.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Checks whether a given string, when converted to bytes, is not longer than the max value of an <see cref="ushort"/>.
    /// Throws an <see cref="ArgumentException"/> if the string is longer.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="encoding">The encoding that shall be used to get the string length.</param>
    /// <returns>The actual length of the value in bytes.</returns>
    /// <exception cref="ArgumentException">The string is too long.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ValidateStringByteSizeUInt16(ReadOnlySpan<char> value, Encoding encoding)
    {
        if (value == ReadOnlySpan<char>.Empty)
            throw new ArgumentNullException(nameof(value));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        var size = encoding.GetByteCount(value);   

        if (size is < 0 or > ushort.MaxValue)
            throw new ArgumentException($"The value is longer than the expected {ushort.MaxValue} characters.", nameof(value));

        return (ushort)size;
    }

    /// <summary>
    /// Checks whether a given character sequence has no more characters than the max value of an <see cref="ushort"/>.
    /// Throws an <see cref="ArgumentException"/> if the string is longer.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>The actual length of the value in characters.</returns>
    /// <exception cref="ArgumentException">The string is too long.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ValidateStringCharLengthUInt16(ReadOnlySpan<char> value)
    {
        if (value == ReadOnlySpan<char>.Empty)
            throw new ArgumentNullException(nameof(value));

        var length = value.Length;
        if (length is < 0 or > ushort.MaxValue)
            throw new ArgumentException($"The value is longer that the expected {ushort.MaxValue} characters.", nameof(value));

        return (ushort)length;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the given character sequence contains non-ASCII characters.
    /// </summary>
    /// <param name="value">The character sequence to validate.</param>
    /// <exception cref="ArgumentException">The character sequence contains non-ASCII characters.</exception>
    public static void ValidateIsAsciiOnly(ReadOnlySpan<char> value)
    {
        if (value == ReadOnlySpan<char>.Empty)
            throw new ArgumentNullException(nameof(value));
        if (!IsAsciiOnly(value))
            throw new ArgumentException("Value contains non-ASCII characters.", nameof(value));
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the given character sequence contains non-ASCII characters.
    /// </summary>
    /// <param name="value">The character sequence to validate.</param>
    /// <exception cref="ArgumentException">The character sequence contains non-ASCII characters.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAsciiOnly(ReadOnlySpan<char> value)
    {
        if (value == ReadOnlySpan<char>.Empty)
            throw new ArgumentNullException(nameof(value));

        foreach (var ch in value)
        {
            if ((uint)ch > '\x007f')
                return false;
        }
        return true;
    }
}