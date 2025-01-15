// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
using System.IO;
using PG.StarWarsGame.Files.Binary.File;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
/// Base class for a Petroglyph binary file.
/// </summary>
public abstract class BinaryFile : BinaryBase, IBinaryFile
{
    /// <inheritdoc />
    public void WriteTo(Stream stream)
    {
        if (stream == null) 
            throw new ArgumentNullException(nameof(stream));

        var size = Size;
        var bytes = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            GetBytes(bytes);
            stream.Write(bytes, 0, size);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}