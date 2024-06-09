using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
[SimpleJob(RuntimeMoniker.Net481)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public class EmpireAtWarValidatorBenchmark
{
    private static readonly Random Random = new(1138);

    private EmpireAtWarMegDataEntryValidator _validator;

    private MegFileDataEntryBuilderInfo[] _toValidate;

    //[Params(2000, 200_000)]
    [Params(2000)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        _toValidate = new MegFileDataEntryBuilderInfo[N];

        for (int i = 0; i < N; i++)
        {
            _toValidate[i] =
                new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(RandomString(Random.Next(5, 200))));
        }

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new FileSystem());

        _validator = new EmpireAtWarMegDataEntryValidator(sc.BuildServiceProvider());
    }

    [Benchmark(Baseline = true)]
    public void ReadBytesAndGetString()
    {
        for (var i = 0; i < N; i++)
        {
            _validator.Validate(_toValidate[i]);
        }
    }

    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}