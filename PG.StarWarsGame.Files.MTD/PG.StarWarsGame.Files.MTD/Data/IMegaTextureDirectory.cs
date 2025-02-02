// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MTD.Data;

/// <summary>
/// Represents a Petroglyph Mega Texture Directory.
/// </summary>
public interface IMegaTextureDirectory : IReadOnlyCollection<MegaTextureFileIndex>
{
    /// <summary>
    /// Determines whether an entry of the specified <see cref="Crc32"/> checksum is in the directory.
    /// </summary>
    /// <param name="crc32">The checksum of the entry to locate in the directory.</param>
    /// <returns><see langword="true"/> if item is found in the <see cref="IMegaTextureDirectory"/>; otherwise, <see langword="false"/>.</returns>
    bool Contains(Crc32 crc32);

    /// <summary>
    /// Gets the entry associated with the specified checksum.
    /// </summary>
    /// <param name="crc32">The checksum of the entry to get.</param>
    /// <param name="entry">
    /// When this method returns, the entry associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="entry"/> parameter.
    /// This parameter is passed uninitialized.</param>
    /// <returns></returns>
    bool TryGetEntry(Crc32 crc32, [NotNullWhen(true)] out MegaTextureFileIndex? entry);
}