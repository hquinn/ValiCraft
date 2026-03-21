using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class SimpleValidation_ValidModel_Benchmark
{
    private ValiCraftSimpleModelValidator _valiCraftValidator = null!;
    private ValiCraftSimpleModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentSimpleModelValidator _fluentValidator = null!;
    private SimpleModel _validModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftSimpleModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftSimpleModelValidator_WithMetaData();
        _fluentValidator = new FluentSimpleModelValidator();

        _validModel = new SimpleModel
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };
    }

    [Benchmark(Baseline = true)]
    public ValidationErrors? ValiCraft()
    {
        return _valiCraftValidator.Validate(_validModel);
    }

    [Benchmark]
    public ValidationErrors? ValiCraftWithMetaData()
    {
        return _valiCraftValidatorWithMetaData.Validate(_validModel);
    }

    [Benchmark]
    public object? FluentValidation()
    {
        return _fluentValidator.Validate(_validModel);
    }
}