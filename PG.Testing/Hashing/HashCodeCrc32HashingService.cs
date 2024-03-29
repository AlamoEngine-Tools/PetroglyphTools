using System;
using System.IO;
using System.Linq;
using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

public class HashCodeCrc32HashingService : ICrc32HashingService
{
    public Crc32 GetCrc32(string value, Encoding encoding)
    {
        return new Crc32(value.GetHashCode());
    }

    public Crc32 GetCrc32(ReadOnlySpan<byte> data)
    {
        var sum = data.ToArray().Sum(x => x);
        return new Crc32(sum.GetHashCode());
    }

    public Crc32 GetCrc32(Stream data)
    {
        var bytes = new byte[data.Length];
        return GetCrc32(bytes.AsSpan());
    }
}