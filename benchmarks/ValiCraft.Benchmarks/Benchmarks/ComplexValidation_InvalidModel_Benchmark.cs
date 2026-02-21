using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class ComplexValidation_InvalidModel_Benchmark
{
    private ValiCraftComplexModelValidator _valiCraftValidator = null!;
    private ValiCraftComplexModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentComplexModelValidator _fluentValidator = null!;
    private ComplexModel _invalidModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftComplexModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftComplexModelValidator_WithMetaData();
        _fluentValidator = new FluentComplexModelValidator();

        _invalidModel = new ComplexModel
        {
            FirstName = "J",
            LastName = "",
            Email = "invalid",
            Age = 17,
            Salary = -1000m,
            PhoneNumber = "123",
            Address = "123",
            City = "N",
            PostalCode = "10",
            Country = "U"
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
