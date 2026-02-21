using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ValiCraft.Benchmarks.Benchmarks;

namespace ValiCraft.Benchmarks;

class Program
{
    static void Main(string[] args)
    {
        // 1. Create a config that tells BenchmarkDotNet to join all summaries into one table
        var config = DefaultConfig.Instance.WithOption(ConfigOptions.JoinSummary, true);

        if (args.Length > 0 && args[0] == "all")
        {
            // 2. Put all your benchmark types into an array
            var benchmarks = new[]
            {
                typeof(SimpleValidation_ValidModel_Benchmark),
                typeof(SimpleValidation_InvalidModel_Benchmark),
                typeof(ComplexValidation_ValidModel_Benchmark),
                typeof(ComplexValidation_InvalidModel_Benchmark),
                typeof(CollectionValidation_SmallCollection_Benchmark),
                typeof(CollectionValidation_LargeCollection_Benchmark),
                typeof(ValidatorInstantiation_Simple_Benchmark),
                typeof(ValidatorInstantiation_Complex_Benchmark)
            };
            
            // 3. Run them together with the joined summary config
            BenchmarkRunner.Run(benchmarks, config);
        }
        else
        {
            // 4. Apply the same config to the Switcher so targeted runs are also joined
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }
}