// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Binary;

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
            if (!_size.HasValue)
            {
                _size = GetSizeCore();
                if (!_size.HasValue)
                    throw new InvalidOperationException("Size value must not be null");
            }
            return _size.Value;
        }
    }

    /// <summary>
    /// Calculates the size in bytes of this instance
    /// </summary>
    /// <returns>The size in bytes.</returns>
    protected abstract int GetSizeCore();

    /// <summary>
    /// Converts this instance into an byte array.
    /// </summary>
    /// <returns>Byte representation.</returns>
    protected abstract byte[] ToBytesCore();
}
