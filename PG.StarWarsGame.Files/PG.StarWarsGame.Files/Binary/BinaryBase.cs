// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
/// Represents a readonly binary data structure with lazy bytes and size calculation.
/// </summary>
public abstract class BinaryBase : IBinary
{
    private byte[]? _bytes;
    private int? _size;

    /// <inheritdoc/>
    public byte[] Bytes
    {
        get
        {
            _bytes ??= ToBytesCore() ?? throw new InvalidOperationException("Bytes data must not be null");
            return (byte[])_bytes.Clone();
        }
    }

    /// <inheritdoc/>
    public int Size
    {
        get
        {
            _size ??= GetSizeCore();
            return _size.Value;
        }
    }

    /// <summary>
    /// Calculates the size in bytes of this instance
    /// </summary>
    /// <returns>The size in bytes.</returns>
    protected abstract int GetSizeCore();

    /// <summary>
    /// Converts the binary model into an array of bytes.
    /// </summary>
    /// <returns>An array of bytes of the binary model.</returns>
    protected abstract byte[] ToBytesCore();
}
