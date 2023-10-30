// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Utilities;

/// <summary>
/// Represent a range has a start index and a length.
/// </summary>
internal readonly struct IndexRange
{
    /// <summary>Represent the inclusive start index of the Range.</summary>
    public int Start { get; }

    /// <summary>
    /// <see cref="Range"/>
    /// </summary>
    public int Length { get; }

    /// <summary>Construct a Range object using the start and end indexes.</summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="length">Represent the exclusive end index of the range.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="length"/> is negative.</exception>
    public IndexRange(int start, int length)
    {
        if (start < 0 || length < 0)
            throw new ArgumentOutOfRangeException();
        Start = start;
        Length = length;
    }

    /// <summary>Indicates whether the current Range object is equal to another object of the same type.</summary>
    /// <param name="value">An object to compare with this object</param>
    public override bool Equals(object? value)
    {
        if (value is not IndexRange other)
            return false;
        return other.Start.Equals(Start) && other.Length.Equals(Length);
    }

    /// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
    /// <param name="other">An object to compare with this object</param>
    public bool Equals(IndexRange other)
    {
        return other.Start.Equals(Start) && other.Length.Equals(Length);
    }

    /// <summary>Returns the hash code for this instance.</summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Start, Length);
    }
}