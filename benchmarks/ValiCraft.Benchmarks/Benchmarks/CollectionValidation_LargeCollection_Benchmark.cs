using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class CollectionValidation_LargeCollection_Benchmark
{
    private ValiCraftCollectionModelValidator _valiCraftValidator = null!;
    private ValiCraftCollectionModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentCollectionModelValidator _fluentValidator = null!;
    private CollectionModel _largeCollectionModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftCollectionModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftCollectionModelValidator_WithMetaData();
        _fluentValidator = new FluentCollectionModelValidator();

        _largeCollectionModel = new CollectionModel
        {
            Name = "Large Collection",
            Tags = Enumerable.Range(1, 10).Select(i => $"tag{i}").ToList(),
            Scores = Enumerable.Range(1, 100).ToList()
        };
    }

    [Benchmark(Baseline = true)]
    public ValidationErrors? ValiCraft()
    {
        return _valiCraftValidator.Validate(_largeCollectionModel);
    }

    [Benchmark]
    public ValidationErrors? ValiCraftWithMetaData()
    {
        return _valiCraftValidatorWithMetaData.Validate(_largeCollectionModel);
    }

    [Benchmark]
    public object? FluentValidation()
    {
        return _fluentValidator.Validate(_largeCollectionModel);
    }
}
