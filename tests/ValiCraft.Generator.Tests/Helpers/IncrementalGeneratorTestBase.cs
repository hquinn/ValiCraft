using AwesomeAssertions.Execution;
using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Tests.Helpers;

public abstract class IncrementalGeneratorTestBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
    protected void AssertGenerator(
        string? errorCodePrefix,
        Type[] additionalMetadataReferences,
        string[] trackingSteps,
        string[] inputs,
        string[] outputs,
        string[] diagnostics,
        bool assertInitialCompilation = false,
        bool assertTrackingSteps = true)
    {
        using var assertionScope = new AssertionScope();
        assertionScope.FormattingOptions.MaxLines = 30000;
        assertionScope.FormattingOptions.MaxDepth = 1000;

        var options = IncrementalGeneratorTestOptions.CreateDefault(additionalMetadataReferences);

        new IncrementalGeneratorAdapter(options)
            .GetGeneratedTrees<TGenerator>(
                inputs,
                trackingSteps,
                errorCodePrefix,
                assertInitialCompilation,
                assertTrackingSteps)
            .AssertDiagnostics(diagnostics)
            .AssertOutputs(outputs);
    }
}