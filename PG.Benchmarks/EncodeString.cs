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
        var count = _byteCount;
        var encoding = _encoding;
        var value = StringValue.AsSpan();

        var buffer = count <= 256 ? stackalloc byte[count] : new byte[count];

#if NET
        var bytesWritten = encoding.GetBytes(value, buffer);
        return encoding.GetString(buffer.Slice(0, bytesWritten));
#else

        fixed (char* pFileName = value)
        fixed (byte* pBuffer = buffer)
        {
            var bytesWritten = encoding.GetBytes(pFileName, value.Length, pBuffer, count);
            Debug.Assert(bytesWritten <= count);
            return encoding.GetString(pBuffer, bytesWritten);
        }
#endif
    }
}