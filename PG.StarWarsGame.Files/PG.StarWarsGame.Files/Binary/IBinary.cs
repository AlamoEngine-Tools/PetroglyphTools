// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
/// Represents a binary element or binary file.
/// </summary>
public interface IBinary
{
    /// <summary>
    /// Gets an array of bytes of the binary.
    /// </summary>
    byte[] Bytes { get; }
    
    /// <summary>
    /// Gets the size in bytes of the binary.
    /// </summary>
    int Size { get; }

    /// <summary>
    /// Fills the specified span with the bytes of the binary.
    /// </summary>
    /// <param name="bytes">The span of bites to fill.</param>
    void GetBytes(Span<byte> bytes);
}