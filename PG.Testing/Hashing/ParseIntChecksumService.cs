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
}