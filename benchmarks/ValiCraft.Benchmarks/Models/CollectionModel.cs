using System.Collections.Generic;

namespace ValiCraft.Benchmarks.Models;

public class CollectionModel
{
    public List<string> Tags { get; set; } = new();
    public List<int> Scores { get; set; } = new();
    public string Name { get; set; } = string.Empty;
}
