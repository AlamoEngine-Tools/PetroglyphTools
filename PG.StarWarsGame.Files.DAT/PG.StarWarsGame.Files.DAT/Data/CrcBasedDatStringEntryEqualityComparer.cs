// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// Compares two <see cref="DatStringEntry"/> by their CRC32 checksum.
/// </summary>
public class CrcBasedDatStringEntryEqualityComparer : IEqualityComparer<DatStringEntry>
{
    /// <summary>
    /// Gets a shared instance of the <see cref="CrcBasedDatStringEntryEqualityComparer"/> class.
    /// </summary>
    public static readonly CrcBasedDatStringEntryEqualityComparer Instance = new();


    /// <inheritdoc />
    public bool Equals(DatStringEntry x, DatStringEntry y)
    {
        return x.Crc32.Equals(y.Crc32);
    }

    /// <inheritdoc />
    public int GetHashCode(DatStringEntry obj)
    {
        return obj.Crc32.GetHashCode();
    }
}