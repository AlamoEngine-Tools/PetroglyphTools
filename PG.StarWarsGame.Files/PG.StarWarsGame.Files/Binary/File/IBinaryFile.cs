// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;

namespace PG.StarWarsGame.Files.Binary.File;

/// <summary>
/// Represents a Petroglyph binary file.
/// </summary>
public interface IBinaryFile : IBinary
{
    /// <summary>
    /// Writes the binary file to the specified stream.
    /// </summary>
    /// <param name="stream">The stream to write the binary data to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    void WriteTo(Stream stream);
}
