using BenchmarkDotNet.Attributes;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class SimpleValidationBenchmark
{
    private ValiCraftSimpleModelValidator _valiCraftValidator = null!;
    private FluentSimpleModelValidator _fluentValidator = null!;
    private SimpleModel _validModel = null!;
    private SimpleModel _invalidModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftSimpleModelValidator();
        _fluentValidator = new FluentSimpleModelValidator();

        _validModel = new SimpleModel
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };

        _invalidModel = new SimpleModel
        {
            Name = "J",
            Age = 200,
            Email = "invalid"
        };
    }

    [Benchmark(Baseline = true)]
    public void ValiCraft_ValidModel()
    {
        var result = _valiCraftValidator.Validate(_validModel);
    }
    
    [Benchmark]
    public void FluentValidation_ValidModel()
    {
        var result = _fluentValidator.Validate(_validModel);
    }
    
    [Benchmark]
    public void ValiCraft_InvalidModel()
    {
        var result = _valiCraftValidator.Validate(_invalidModel);
    }
    
    [Benchmark]
    public void FluentValidation_InvalidModel()
    {
        var result = _fluentValidator.Validate(_invalidModel);
    }
}
