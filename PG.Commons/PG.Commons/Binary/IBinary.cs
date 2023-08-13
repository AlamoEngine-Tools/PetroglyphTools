// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Binary;

/// <summary>
/// Represents a Petroglyph binary file part. This can be a table record, chunks, etc.
/// </summary>
public interface IBinary
{
    /// <summary>
    /// Gets the byte array representation of this instance.
    /// </summary>
    byte[] Bytes { get; }
    
    /// <summary>
    /// Gets the size in bytes of this instance
    /// </summary>
    int Size { get; }
}
