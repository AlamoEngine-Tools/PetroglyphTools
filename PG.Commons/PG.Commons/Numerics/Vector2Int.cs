using System;

namespace PG.Commons.Numerics;

/// <summary>
/// Represents a vector with two integer values.
/// </summary>
public readonly struct Vector2Int : IEquatable<Vector2Int>
{
    /// <summary>
    /// The first component of the vector.
    /// </summary>
    public int First { get; }

    /// <summary>
    /// The second component of the vector.
    /// </summary>
    public int Second { get; }

    /// <summary>
    /// Constructs a vector from the given <see cref="ReadOnlySpan{T}"/>. If the span does not contain enough elements,
    /// the default integer value 0 is used to initialize the respecting component.
    /// </summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector2Int(ReadOnlySpan<int> values)
    {
        var length = values.Length;
        if (length >= 1)
            First = values[0];
        if (length >= 2)
            Second = values[1];
    }

    /// <summary>
    /// Creates a vector whose elements have the specified values.
    /// </summary>
    /// <param name="first">The value to assign to the <see cref="First"/> field.</param>
    /// <param name="second">The value to assign to the <see cref="Second"/> field.</param>
    public Vector2Int(int first, int second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// Returns a value that indicates whether this instance and another vector are equal.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Vector2Int other)
    {
        return First == other.First && Second == other.Second;
    }

    /// <summary>
    /// Returns a value that indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the current instance and <paramref name="obj"/> are equal; otherwise, <see langword="false"/>. If obj is <see langword="null"/>, the method returns <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(First, Second);
    }
}
