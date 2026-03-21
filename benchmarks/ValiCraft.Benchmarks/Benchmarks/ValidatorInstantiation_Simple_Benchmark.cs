using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class ValidatorInstantiation_Simple_Benchmark
{
    [Benchmark(Baseline = true)]
    public object? ValiCraft()
    {
        return new ValiCraftSimpleModelValidator();
    }

    [Benchmark]
    public object? ValiCraftWithMetaData()
    {
        return new ValiCraftSimpleModelValidator_WithMetaData();
    }

    [Benchmark]
    public object? FluentValidation()
    {
        return new FluentSimpleModelValidator();
    }
}