using System;

namespace PG.Commons.Numerics;

/// <summary>
/// Represents a vector with three integer values.
/// </summary>
public readonly struct Vector3Int : IEquatable<Vector3Int>
{
    /// <summary>
    /// Gets the first component of the vector.
    /// </summary>
    public int First { get; }

    /// <summary>
    /// Gets the second component of the vector.
    /// </summary>
    public int Second { get; }

    /// <summary>
    /// Gets the third component of the vector.
    /// </summary>
    public int Third { get; }

    /// <summary>
    /// Constructs a vector from the given <see cref="ReadOnlySpan{T}"/>. If the span does not contain enough elements,
    /// the default integer value 0 is used to initialize the respecting component.
    /// </summary>
    /// <param name="values">The span of elements to assign to the vector.</param>
    public Vector3Int(ReadOnlySpan<int> values)
    {
        var length = values.Length;
        if (length >= 1)
            First = values[0];
        if (length >= 2)
            Second = values[1];
        if (length >= 3)
            Third = values[2];
    }

    /// <summary>
    /// Creates a vector whose elements have the specified values.
    /// </summary>
    /// <param name="first">The value to assign to the <see cref="First"/> field.</param>
    /// <param name="second">The value to assign to the <see cref="Second"/> field.</param>
    /// <param name="third">The value to assign to the <see cref="Third"/> field.</param>
    public Vector3Int(int first, int second, int third)
    {
        First = first;
        Second = second;
        Third = third;
    }

    /// <summary>
    /// Returns a value that indicates whether this instance and another vector are equal.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Vector3Int other)
    {
        return First == other.First && Second == other.Second && Third == other.Third;
    }

    /// <summary>
    /// Returns a value that indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if the current instance and <paramref name="obj"/> are equal; otherwise, <see langword="false"/>. If obj is <see langword="null"/>, the method returns <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Vector3Int other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(First, Second, Third);
    }
}
