using BenchmarkDotNet.Running;

namespace PG.Benchmarks;

class Benchmark
{
    private static void Main()
    {
        //BenchmarkRunner.Run<GetStringCrc32>();
        //BenchmarkRunner.Run<GetBytesBenchmark>();
        //BenchmarkRunner.Run<GetStringFromBinary>();
        BenchmarkRunner.Run<EncodeString>();
    }
}
