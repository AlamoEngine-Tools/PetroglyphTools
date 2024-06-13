using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class GetStringFromBinary
{
    private static readonly Random Random = new();

    [Params(267, 10)]
    public int N;

    private readonly Encoding _encoding = Encoding.ASCII;

    private BinaryReader _binaryReader;

    [GlobalSetup]
    public void Setup()
    {
        var randomString = RandomString(N);
        var ms = new MemoryStream(_encoding.GetBytes(randomString));
        _binaryReader = new BinaryReader(ms, _encoding, true);
    }


    [Benchmark(Baseline = true)]
    public string ReadBytesAndGetString()
    {
        _binaryReader.BaseStream.Position = 0;

        var bytes = _binaryReader.ReadBytes(N);
        if (bytes.Length != N)
            throw new Exception();
        return _encoding.GetString(bytes);
    }

    [Benchmark]
    public string ReadBytesAndGetString_ButWithSpanInShortCases()
    {
        _binaryReader.BaseStream.Position = 0;
        
#if NET

        // This is the only perf optimization i could get
        if (N <= 256)
        {
            Span<byte> buffer = stackalloc byte[N];
            var br = _binaryReader.Read(buffer);
            if (br != N)
                throw new Exception();
            return _encoding.GetString(buffer);
        }
#endif

        var bytes = _binaryReader.ReadBytes(N);
        if (bytes.Length != N)
            throw new Exception();
        return _encoding.GetString(bytes);
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_- öäü";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}