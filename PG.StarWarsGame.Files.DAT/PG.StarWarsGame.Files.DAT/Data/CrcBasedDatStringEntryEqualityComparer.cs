using System.Collections.Generic;

namespace PG.StarWarsGame.Files.DAT.Data;

internal class CrcBasedDatStringEntryEqualityComparer : IEqualityComparer<DatStringEntry>
{
    internal static readonly CrcBasedDatStringEntryEqualityComparer Instance = new();

    public bool Equals(DatStringEntry x, DatStringEntry y)
    {
        return x.Crc32.Equals(y.Crc32);
    }

    public int GetHashCode(DatStringEntry obj)
    {
        return obj.Crc32.GetHashCode();
    }
}