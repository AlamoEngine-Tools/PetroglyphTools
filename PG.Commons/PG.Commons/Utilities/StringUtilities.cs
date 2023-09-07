// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides primitive helper methods for strings.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Checks whether a given string, when converted to bytes, is not longer than the max value of an <see cref="ushort"/>.
    /// Throws an <see cref="OverflowException"/> if the string is longer.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="encoding">The encoding that shall be used to get the string length.</param>
    /// <returns>The actual length of the value in bytes.</returns>
    /// <exception cref="OverflowException">When the string was too long.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ValidateStringByteSizeUInt16(string value, Encoding encoding)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        var length = encoding.GetByteCount(value);
        try
        {
            return Convert.ToUInt16(length);
        }
        catch (OverflowException)
        {
            throw new OverflowException($"The value {value} is longer that the expected {ushort.MaxValue} characters.");
        }
    }
}