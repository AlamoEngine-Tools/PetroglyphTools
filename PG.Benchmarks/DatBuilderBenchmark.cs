using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using Testably.Abstractions.Testing;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net48)]
[MemoryDiagnoser]
public class DatBuilderBenchmark
{
    private static readonly Random Random = new(1138);

    [Params(8_000)]
    public int N;

    private string[] _keys;
    private string[] _values;

    private EmpireAtWarMasterTextBuilder _builder;

    [GlobalSetup]
    public void Setup()
    {
        _keys = new string[N];
        _values = new string[N];

        for (int i = 0; i < N; i++)
        {
            _keys[i] = RandomString(Random.Next(10, 30));
            _values[i] = RandomString(Random.Next(15, 200));
        }

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();

        _builder = new EmpireAtWarMasterTextBuilder(false, sc.BuildServiceProvider());
    }

    [Benchmark]
    public void AddEntries()
    {
        for (int i = 0; i < N; i++)
        {
            _builder.AddEntry(_keys[i], _values[i]);
        }
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}