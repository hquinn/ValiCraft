using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class ValidatorInstantiation_Simple_Benchmark
{
    [Benchmark(Baseline = true)]
    public void ValiCraft()
    {
        var validator = new ValiCraftSimpleModelValidator();
    }
    
    [Benchmark]
    public void ValiCraftWithMetaData()
    {
        var validator = new ValiCraftSimpleModelValidator_WithMetaData();
    }

    [Benchmark]
    public void FluentValidation()
    {
        var validator = new FluentSimpleModelValidator();
    }
}