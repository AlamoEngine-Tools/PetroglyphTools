using System;
using System.IO;
using System.Linq;
using System.Text;
using PG.Commons.Hashing;

namespace PG.Testing.Hashing;

/// <summary>
/// Represents a test <see cref="ICrc32HashingService"/> that derives the CRC32 value by parsing the input as an integer.
/// </summary>
public class ParseIntCrc32HashingService : ICrc32HashingService
{
    /// <inheritdoc/>
    public Crc32 GetCrc32(string value, Encoding encoding)
    {
        var intValue = int.Parse(value);
        return new Crc32(intValue);
    }

    /// <inheritdoc/>
    public Crc32 GetCrc32(ReadOnlySpan<byte> data)
    {
        var sum = data.ToArray().Sum(x => x);
        return new Crc32(sum);
    }

    /// <inheritdoc/>
    public Crc32 GetCrc32(Stream data)
    {
        var bytes = new byte[data.Length];
        return GetCrc32(bytes.AsSpan());
    }

    /// <inheritdoc/>
    public Crc32 GetCrc32(ReadOnlySpan<char> value, Encoding encoding)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public Crc32 GetCrc32Upper(ReadOnlySpan<char> value, Encoding encoding)
    {
        throw new NotImplementedException();
    }
}