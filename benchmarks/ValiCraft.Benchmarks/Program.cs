using BenchmarkDotNet.Running;
using ValiCraft.Benchmarks.Benchmarks;

namespace ValiCraft.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "all")
        {
            BenchmarkRunner.Run<SimpleValidationBenchmark>();
            BenchmarkRunner.Run<ComplexValidationBenchmark>();
            BenchmarkRunner.Run<CollectionValidationBenchmark>();
            BenchmarkRunner.Run<ValidatorInstantiationBenchmark>();
        }
        else
        {
            var summary = BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
