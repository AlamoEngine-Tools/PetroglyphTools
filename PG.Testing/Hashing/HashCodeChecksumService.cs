using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

public class HashCodeChecksumService : IChecksumService
{
    public Crc32 GetChecksum(string s, Encoding encoding)
    {
        return new Crc32(s.GetHashCode());
    }
}