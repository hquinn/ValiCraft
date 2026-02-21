using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class ComplexValidation_ValidModel_Benchmark
{
    private ValiCraftComplexModelValidator _valiCraftValidator = null!;
    private ValiCraftComplexModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentComplexModelValidator _fluentValidator = null!;
    private ComplexModel _validModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftComplexModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftComplexModelValidator_WithMetaData();
        _fluentValidator = new FluentComplexModelValidator();

        _validModel = new ComplexModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Age = 30,
            Salary = 75000m,
            PhoneNumber = "1234567890",
            Address = "123 Main Street",
            City = "New York",
            PostalCode = "10001",
            Country = "USA"
        };
    }

    [Benchmark(Baseline = true)]
    public void ValiCraft()
    {
        var result = _valiCraftValidator.Validate(_validModel);
    }
    
    [Benchmark]
    public void ValiCraftWithMetaData()
    {
        var result = _valiCraftValidatorWithMetaData.Validate(_validModel);
    }

    [Benchmark]
    public void FluentValidation()
    {
        var result = _fluentValidator.Validate(_validModel);
    }
}