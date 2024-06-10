using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Running;

namespace PG.Benchmarks;

[ExcludeFromCodeCoverage]
class Benchmark
{
    private static void Main()
    {
        //BenchmarkRunner.Run<GetStringCrc32>();
        //BenchmarkRunner.Run<GetBytesBenchmark>();
        //BenchmarkRunner.Run<GetStringFromBinary>();
        //BenchmarkRunner.Run<EncodeString>();
        //BenchmarkRunner.Run<ContainsInvalidCharsBenchmark>();
        //BenchmarkRunner.Run<DatBuilderBenchmark>();
        //BenchmarkRunner.Run<EmpireAtWarValidatorBenchmark>();
        BenchmarkRunner.Run<EmpireAtWarMegBuilderBenchmark>();
    }
}
