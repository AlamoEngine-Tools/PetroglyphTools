using System;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PG.Commons.Services;

[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class GetStringCrc32
{
    private static readonly Random Random = new();

    [Params(10, 257)]
    public int N;

    private readonly Encoding _encoding = Encoding.ASCII;
    private string _s1 = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s1 = RandomString(N);
    }

    [Benchmark(Baseline = true)]
    public Crc32 Old()
    {
        Span<byte> checksum = stackalloc byte[8];
        System.IO.Hashing.Crc32.Hash(_encoding.GetBytes(_s1), checksum);
        return new Crc32(checksum);
    }

    [Benchmark]
    public unsafe Crc32 Current()
    {
        Span<byte> buffer;

        if (_s1.Length > 256)
        {
#if NET
            var stringSpan = _s1.AsSpan();
            var maxByteSize = _encoding.GetMaxByteCount(stringSpan.Length);

            var b = new byte[maxByteSize].AsSpan();
            var nb = _encoding.GetBytes(stringSpan, b);
            buffer = b.Slice(0, nb);
#else
            buffer = _encoding.GetBytes(_s1).AsSpan();
#endif
        }
        else
        {
            var stringSpan = _s1.AsSpan();
            var maxByteSize = _encoding.GetMaxByteCount(stringSpan.Length);

            var buff = stackalloc byte[maxByteSize];
            fixed (char* sp = &stringSpan.GetPinnableReference())
            {
                var a = _encoding.GetBytes(sp, _s1.Length, buff, maxByteSize);
                buffer = new Span<byte>(buff, a);
            }
        }

        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        System.IO.Hashing.Crc32.Hash(buffer, checksum);
        return new Crc32(checksum);
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}