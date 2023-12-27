using System;
using System.Linq;
using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

public class ParseIntChecksumService : IChecksumService
{
    public Crc32 GetChecksum(string value, Encoding encoding)
    {
        var intValue = int.Parse(value);
        return new Crc32(intValue);
    }

    public Crc32 GetChecksum(ReadOnlySpan<byte> data)
    {
        var sum = data.ToArray().Sum(x => x);
        return new Crc32(sum);
    }
}