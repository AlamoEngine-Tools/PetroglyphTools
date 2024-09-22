using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using Testably.Abstractions.Testing;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class EmpireAtWarMegBuilderBenchmark
{
    private static readonly Random Random = new(1138);

    [Params(200)]
    public int N;

    private EmpireAtWarMegBuilder _builder = null!;
    private string[] _toAdd = null!;

    [GlobalSetup]
    public void Setup()
    {
        _toAdd = new string[N];

        for (int i = 0; i < N; i++)
        {
            _toAdd[i] = RandomString(Random.Next(10, 40));
        }

        var fs = new MockFileSystem();

        fs.Initialize().WithFile("C:/test/test.txt").Which(f => f.HasStringContent("123"));

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();
        _builder = new EmpireAtWarMegBuilder("C:/test", sc.BuildServiceProvider());
    }


    [Benchmark]
    public void ReadBytesAndGetString()
    {
        for (int i = 0; i < N; i++)
        {
            _builder.AddFile("C:/test/test.txt", _toAdd[i]);
        }
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}