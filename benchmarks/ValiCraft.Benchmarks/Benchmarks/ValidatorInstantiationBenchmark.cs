using BenchmarkDotNet.Attributes;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ValidatorInstantiationBenchmark
{
    [Benchmark(Baseline = true)]
    public void ValiCraft_SimpleValidator_Instantiation()
    {
        var validator = new ValiCraftSimpleModelValidator();
    }

    [Benchmark]
    public void FluentValidation_SimpleValidator_Instantiation()
    {
        var validator = new FluentSimpleModelValidator();
    }

    [Benchmark]
    public void ValiCraft_ComplexValidator_Instantiation()
    {
        var validator = new ValiCraftComplexModelValidator();
    }

    [Benchmark]
    public void FluentValidation_ComplexValidator_Instantiation()
    {
        var validator = new FluentComplexModelValidator();
    }
}
