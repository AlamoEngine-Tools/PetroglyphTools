using System;
using System.Linq;
using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

public class HashCodeChecksumService : IChecksumService
{
    public Crc32 GetChecksum(string value, Encoding encoding)
    {
        return new Crc32(value.GetHashCode());
    }

    public Crc32 GetChecksum(ReadOnlySpan<byte> data)
    {
        var sum = data.ToArray().Sum(x => x);
        return new Crc32(sum.GetHashCode());
    }
}