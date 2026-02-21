using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class SimpleValidation_InvalidModel_Benchmark
{
    private ValiCraftSimpleModelValidator _valiCraftValidator = null!;
    private ValiCraftSimpleModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentSimpleModelValidator _fluentValidator = null!;
    private SimpleModel _invalidModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftSimpleModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftSimpleModelValidator_WithMetaData();
        _fluentValidator = new FluentSimpleModelValidator();

        _invalidModel = new SimpleModel
        {
            Name = "J",
            Age = 200,
            Email = "invalid"
        };
    }

    [Benchmark(Baseline = true)]
    public void ValiCraft()
    {
        var result = _valiCraftValidator.Validate(_invalidModel);
    }

    [Benchmark]
    public void ValiCraftWithMetaData()
    {
        var result = _valiCraftValidatorWithMetaData.Validate(_invalidModel);
    }

    [Benchmark]
    public void FluentValidation()
    {
        var result = _fluentValidator.Validate(_invalidModel);
    }
}