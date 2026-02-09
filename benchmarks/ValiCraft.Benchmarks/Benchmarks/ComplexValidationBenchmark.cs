using BenchmarkDotNet.Attributes;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ComplexValidationBenchmark
{
    private ValiCraftComplexModelValidator _valiCraftValidator = null!;
    private FluentComplexModelValidator _fluentValidator = null!;
    private ComplexModel _validModel = null!;
    private ComplexModel _invalidModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftComplexModelValidator();
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
