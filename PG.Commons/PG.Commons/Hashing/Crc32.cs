// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PG.Commons.Hashing;

/// <summary>
/// Represents a 32-bit Cyclic Redundancy Check (CRC32) checksum.
/// </summary>
[DebuggerDisplay("CRC: {_checksum}")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Crc32 : IEquatable<Crc32>, IComparable<Crc32>
{
    // Important: By design, this must be the only field of this struct!
    // This way we can assure that sizeof(CRC32) == sizeof(uint)
    private readonly uint _checksum;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Crc32"/> struct with the specified checksum value.
    /// </summary>
    /// <param name="checksum">The CRC32 checksum value.</param>
    public Crc32(uint checksum)
    {
        _checksum = checksum;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Crc32"/> struct with the specified checksum value.
    /// </summary>
    /// <param name="checksum">The CRC32 checksum value.</param>
    public Crc32(int checksum)
    {
        unchecked
        {
            _checksum = (uint)checksum;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Crc32"/> struct with the specified checksum data.
    /// </summary>
    /// <param name="checksum">Checksum data in little endian format.</param>
    internal Crc32(Span<byte> checksum)
    {
        _checksum = BinaryPrimitives.ReadUInt32LittleEndian(checksum);
    }

    /// <inheritdoc cref="object.ToString()"/>
    public override string ToString()
    {
        return ToString(false);
    }

    /// <inheritdoc cref="object.ToString()"/>
    /// <param name="asSignedInteger">When <see langword="true"/>, the checksum is represented by a signed integer; unsigned otherwise.</param>
    public string ToString(bool asSignedInteger)
    {
        var sb = new StringBuilder("CRC: ");
        if (asSignedInteger)
            sb.Append((int)_checksum);
        else
            sb.Append(_checksum);
        return sb.ToString();
    }

    /// <inheritdoc cref="IComparable{T}"/>
    public int CompareTo(Crc32 other)
    {
        return _checksum.CompareTo(other._checksum);
    }

    /// <inheritdoc cref="IEquatable{T}"/>
    public bool Equals(Crc32 other)
    {
        return _checksum == other._checksum;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Crc32 other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (int)_checksum;
    }

    /// <summary>
    /// Returns the CRC32 checksum as a byte array.
    /// </summary>
    /// <returns>The CRC32 checksum as a byte array.</returns>
    public unsafe byte[] GetBytes()
    {
        Span<byte> data = stackalloc byte[sizeof(Crc32)];
        GetBytes(data);
        return data.ToArray();
    }

    /// <summary>
    /// Writes the CRC32 checksum into a span of bytes in little endian.
    /// </summary>
    public void GetBytes(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, _checksum);
    }

    /// <summary>
    /// Determines whether two CRC32 checksums are equal.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if the two checksums are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Crc32 a, Crc32 b)
    {
        return Equals(a, b);
    }

    /// <summary>
    /// Determines whether two CRC32 checksums are not equal.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if the two checksums are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Crc32 a, Crc32 b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Determines whether one CRC32 checksum is greater than another.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >(Crc32 a, Crc32 b)
    {
        return a._checksum > b._checksum;
    }

    /// <summary>
    /// Determines whether one CRC32 checksum is less than another.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <(Crc32 a, Crc32 b)
    {
        return a._checksum < b._checksum;
    }

    /// <summary>
    /// Determines whether one CRC32 checksum is greater than or equal to another.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >=(Crc32 a, Crc32 b)
    {
        return a._checksum >= b._checksum;
    }

    /// <summary>
    /// Determines whether one CRC32 checksum is less than or equal to another.
    /// </summary>
    /// <param name="a">The first CRC32 checksum to compare.</param>
    /// <param name="b">The second CRC32 checksum to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <=(Crc32 a, Crc32 b)
    {
        return a._checksum <= b._checksum;
    }

    /// <summary>
    /// Defines an implicit conversion of an CRC32 checksum to an <see cref="uint"/>.
    /// </summary>
    /// <param name="crc">The checksum data.</param>
    public static explicit operator uint(Crc32 crc) => crc._checksum;

    /// <summary>
    /// Defines an implicit conversion of an CRC32 checksum to an <see cref="int"/>, which might be negative.
    /// </summary>
    /// <param name="crc">The checksum data.</param>
    public static explicit operator int(Crc32 crc)
    {
        unchecked
        {
            return (int)crc._checksum;
        }
    }
}