using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class GetBytesBenchmark
{
    public int Size => sizeof(uint) * 2;

    public uint ValueLength => 123123;

    public uint KeyLength => 10;

    [Benchmark(Baseline = true)]
    public byte[] Old()
    {
        var bytes = new byte[Size];
        Buffer.BlockCopy(BitConverter.GetBytes(ValueLength), 0, bytes, 0, sizeof(uint));
        Buffer.BlockCopy(BitConverter.GetBytes(KeyLength), 0, bytes, sizeof(uint) * 1, sizeof(uint));
        return bytes;
    }

    [Benchmark]
    public byte[] New()
    {
        // Using stackalloc is tempting, but we need to allocate an array anyways and Span<T>.ToArray() is very expensive - especially on .NET Framework.
        var data = new byte[Size];
        var dataSpan = data.AsSpan();
        BinaryPrimitives.WriteUInt32LittleEndian(dataSpan, ValueLength);

        var keyArea = dataSpan.Slice(sizeof(uint) * 1);
        BinaryPrimitives.WriteUInt32LittleEndian(keyArea, KeyLength);
        return data;
    }
}