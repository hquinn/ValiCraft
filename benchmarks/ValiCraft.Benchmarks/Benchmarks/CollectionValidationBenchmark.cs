using BenchmarkDotNet.Attributes;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Benchmarks.Validators;

namespace ValiCraft.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CollectionValidationBenchmark
{
    private ValiCraftCollectionModelValidator _valiCraftValidator = null!;
    private FluentCollectionModelValidator _fluentValidator = null!;
    private CollectionModel _smallCollectionModel = null!;
    private CollectionModel _largeCollectionModel = null!;

    [GlobalSetup]
    public void Setup()
    {
        _valiCraftValidator = new ValiCraftCollectionModelValidator();
        _fluentValidator = new FluentCollectionModelValidator();

        _smallCollectionModel = new CollectionModel
        {
            Name = "Small Collection",
            Tags = new List<string> { "tag1", "tag2", "tag3" },
            Scores = new List<int> { 1, 2, 3, 4, 5 }
        };

        _largeCollectionModel = new CollectionModel
        {
            Name = "Large Collection",
            Tags = Enumerable.Range(1, 10).Select(i => $"tag{i}").ToList(),
            Scores = Enumerable.Range(1, 100).ToList()
        };
    }

    [Benchmark(Baseline = true)]
    public void ValiCraft_SmallCollection()
    {
        var result = _valiCraftValidator.Validate(_smallCollectionModel);
    }
    
    [Benchmark]
    public void FluentValidation_SmallCollection()
    {
        var result = _fluentValidator.Validate(_smallCollectionModel);
    }
    
    [Benchmark]
    public void ValiCraft_LargeCollection()
    {
        var result = _valiCraftValidator.Validate(_largeCollectionModel);
    }
    
    [Benchmark]
    public void FluentValidation_LargeCollection()
    {
        var result = _fluentValidator.Validate(_largeCollectionModel);
    }
}
