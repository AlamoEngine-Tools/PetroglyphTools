// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// 
/// </summary>
public class CrcBasedDatStringEntryEqualityComparer : IEqualityComparer<DatStringEntry>
{
    internal static readonly CrcBasedDatStringEntryEqualityComparer Instance = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(DatStringEntry x, DatStringEntry y)
    {
        return x.Crc32.Equals(y.Crc32);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(DatStringEntry obj)
    {
        return obj.Crc32.GetHashCode();
    }
}