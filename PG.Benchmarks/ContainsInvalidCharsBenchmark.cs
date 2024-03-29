using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
public class ContainsInvalidCharsBenchmark
{
    private static readonly Random Random = new();

    private static readonly char[] InvalidFileNameChars = {
        '\"', '<', '>', '|', ':', '*', '?', '\\', '/', '\0'
        //(char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        //(char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        //(char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        //(char)31,
    };

    [Params(20, 257)]
    public int N;
    
    private string _s1 = null!;


    [GlobalSetup]
    public void Setup()
    {
        _s1 = RandomString(N);
    }


    [Benchmark(Baseline = true)]
    public bool ContainsInvalidChars()
    {
        var span = _s1.AsSpan();
        foreach (var t in span)
            if (IsInvalidFileCharacter(t))
                return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInvalidFileCharacter(char c)
    {
        // Check if character is in bounds [32-126] in general
        if ((uint)(c - '\x0020') > '\x007F' - '\x0020')  // (c >= '\x0020' && c <= '\x007F')
            return true;

        // Additional check for invalid Windows file name characters
        if (InvalidFileNameChars.Contains(c))
            return true;

        return false;
    }


    [Benchmark]
    public bool New()
    {
        var span = _s1.AsSpan();
        ref var start = ref MemoryMarshal.GetReference(span);
        ref var end = ref Unsafe.Add(ref start, _s1.Length);

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            if (IsInvalidFileCharacter_New(start))
                return true;
            start = ref Unsafe.Add(ref start, 1);
        }

        return false;
    }

    // Seems to be the winner
    [Benchmark]
    public bool New_WithoutMarshal()
    {
        var span = _s1.AsSpan();
        foreach (var t in span)
            if (IsInvalidFileCharacter_New(t))
                return true;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInvalidFileCharacter_New(char c)
    {
        // Check if character is in bounds [32-126] in general
        if ((uint)(c - '\x0020') > '\x007F' - '\x0020')  // (c >= '\x0020' && c <= '\x007F')
            return true;

        // Additional check for invalid Windows file name characters
        foreach (var charToCheck in InvalidFileNameChars.AsSpan())
        {
            if (charToCheck == c)
                return true;
        }

        return false;
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Make sure strings only enter the slow path
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}