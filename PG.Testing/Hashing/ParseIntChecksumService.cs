using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

public class ParseIntChecksumService : IChecksumService
{
    public Crc32 GetChecksum(string s, Encoding encoding)
    {
        var value = int.Parse(s);
        return new Crc32(value);
    }
}