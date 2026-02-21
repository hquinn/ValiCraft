using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(runtimeMoniker: RuntimeMoniker.Net10_0, warmupCount: 3, iterationCount: 10)]
public class CollectionValidation_SmallCollection_Benchmark
{
    private ValiCraftCollectionModelValidator _valiCraftValidator = null!;
    private ValiCraftCollectionModelValidator_WithMetaData _valiCraftValidatorWithMetaData = null!;
    private FluentCollectionModelValidator _fluentValidator = null!;
    private CollectionModel _smallCollectionModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftCollectionModelValidator();
        _valiCraftValidatorWithMetaData = new ValiCraftCollectionModelValidator_WithMetaData();
        _fluentValidator = new FluentCollectionModelValidator();

        _smallCollectionModel = new CollectionModel
        {
            Name = "Small Collection",
            Tags = new List<string> { "tag1", "tag2", "tag3" },
            Scores = new List<int> { 1, 2, 3, 4, 5 }
        };
    }

    [Benchmark(Baseline = true)]
    public void ValiCraft()
    {
        var result = _valiCraftValidator.Validate(_smallCollectionModel);
    }

    [Benchmark]
    public void ValiCraftWithMetaData()
    {
        var result = _valiCraftValidatorWithMetaData.Validate(_smallCollectionModel);
    }

    [Benchmark]
    public void FluentValidation()
    {
        var result = _fluentValidator.Validate(_smallCollectionModel);
    }
}