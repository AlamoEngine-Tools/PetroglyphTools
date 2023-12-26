using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
#if NETFRAMEWORK
using System.Runtime.InteropServices;
#endif

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class EncodeString
{
    [Params(
       "",
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
        var encoding = _encoding;

        var bytes = encoding.GetBytes(StringValue);
        return encoding.GetString(bytes);
    }

    [Benchmark]
    public unsafe string Current()
    {
        var count = _byteCount;
        var encoding = _encoding;
        var value = StringValue.AsSpan();

        if (value.IsEmpty)
            return string.Empty;

        var buffer = count <= 256 ? stackalloc byte[count] : new byte[count];

#if NET
        var bytesWritten = encoding.GetBytes(value, buffer);
        return encoding.GetString(buffer.Slice(0, bytesWritten));
#else
        var bytesWritten = GetBytes(encoding, value, buffer);
        fixed (byte* pBuffer = &MemoryMarshal.GetReference(buffer))
            return encoding.GetString(pBuffer, bytesWritten);
#endif
    }


#if !NETSTANDARD2_1_OR_GREATER && !NET
    public static unsafe int GetBytes(Encoding encoding, ReadOnlySpan<char> value, Span<byte> destination)
    {
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        fixed (char* charsPtr = &MemoryMarshal.GetReference(value))
        fixed (byte* bytesPtr = &MemoryMarshal.GetReference(destination))
        {
            return encoding.GetBytes(charsPtr, value.Length, bytesPtr, destination.Length);
        }
    }
#endif
}