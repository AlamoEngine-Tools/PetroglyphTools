using System;
using System.Diagnostics;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PG.Benchmarks;

[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class EncodeString
{
    [Params(
       "string value with non-ascii chars öäü",
        "very short",
        "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore " +
        "et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. " +
        "Stet clita kasd gubergren, no sea takimata sanctus est Lo"
        )]
    public string StringValue;

    private readonly Encoding _encoding = Encoding.ASCII;
    private int _byteCount;

    [GlobalSetup]
    public void GetMaxByteCount()
    {
        _byteCount = _encoding.GetMaxByteCount(StringValue.Length);
    }
    
    [Benchmark(Baseline = true)]
    public string Simplest()
    {
        var bytes = _encoding.GetBytes(StringValue);
        return _encoding.GetString(bytes);
    }

    [Benchmark]
    public unsafe string New()
    {

#if NET
        if (_byteCount <= 256)
        {
            Span<byte> buffer = stackalloc byte[_byteCount];
            var bytesWritten = _encoding.GetBytes(StringValue.AsSpan(), buffer);
            return _encoding.GetString(buffer.Slice(0, bytesWritten));
        }
#endif

        var bytes = new byte[_byteCount];
        fixed (char* pFileName = StringValue)
        fixed (byte* pBuffer = bytes)
        {
            var bytesWritten = _encoding.GetBytes(pFileName, StringValue.Length, pBuffer, _byteCount);
            Debug.Assert(bytesWritten <= _byteCount);
            return _encoding.GetString(pBuffer, bytesWritten);
        }
    }

    [Benchmark]
    public unsafe string New2()
    {

#if NET
        if (_byteCount <= 256)
        {
            Span<byte> buffer = stackalloc byte[_byteCount];
            var bytesWritten = _encoding.GetBytes(StringValue.AsSpan(), buffer);
            return _encoding.GetString(buffer.Slice(0, bytesWritten));
        }
#endif

        var bytes = _encoding.GetBytes(StringValue);
        fixed (byte* pBuffer = bytes)
            return _encoding.GetString(pBuffer, bytes.Length);
    }
}