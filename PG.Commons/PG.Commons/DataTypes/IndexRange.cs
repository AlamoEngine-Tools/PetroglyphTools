// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.DataTypes;

/// <summary>
/// Represent a range has a start index and a length.
/// </summary>
public readonly struct IndexRange
{
    /// <summary>
    /// Gets the inclusive start index of the Range.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// Gets the length of the index range.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the last index, inclusive, of the range.
    /// </summary>
    public int End => Start + Length - 1;

    /// <summary>Construct a Range object using the start and length.</summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="length">The length of range.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> is negative.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than 1.</exception>
    /// <exception cref="OverflowException">The range is too long.</exception>
    public IndexRange(int start, int length)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start));
        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(start));
        Start = start;
        Length = length;
        if (End < start)
            throw new OverflowException("Range exceeds the maximum allowed size.");
    }

    /// <summary>
    /// Indicates whether the current Range object is equal to another object of the same type.
    /// </summary>
    /// <param name="value">An object to compare with this object</param>
    public override bool Equals(object? value)
    {
        if (value is not IndexRange other)
            return false;
        return other.Start.Equals(Start) && other.Length.Equals(Length);
    }

    /// <summary>
    /// Indicates whether the current Range object is equal to another Range object.
    /// </summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals(IndexRange other)
    {
        return other.Start.Equals(Start) && other.Length.Equals(Length);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Start, Length);
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"{Start}..{End}";
    }

    /// <summary>
    /// Determines whether two index ranges are equal.
    /// </summary>
    /// <param name="a">The first index range to compare.</param>
    /// <param name="b">The second index range to compare.</param>
    /// <returns><see langword="true"/> if the two index ranges are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(IndexRange a, IndexRange b)
    {
        return Equals(a, b);
    }

    /// <summary>
    /// Determines whether two index ranges are not equal.
    /// </summary>
    /// <param name="a">The first index range to compare.</param>
    /// <param name="b">The second index range to compare.</param>
    /// <returns><see langword="true"/> if the two index ranges are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(IndexRange a, IndexRange b)
    {
        return !(a == b);
    }
}