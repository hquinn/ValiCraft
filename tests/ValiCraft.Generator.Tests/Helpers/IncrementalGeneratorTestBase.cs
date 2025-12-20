using AwesomeAssertions.Execution;
using Microsoft.CodeAnalysis;
using MonadCraft;

namespace ValiCraft.Generator.Tests.Helpers;

public abstract class IncrementalGeneratorTestBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
    protected void AssertGenerator(
        string[] inputs,
        string[]? outputs,
        string[] diagnostics,
        bool assertInitialCompilation = false,
        bool assertTrackingSteps = true)
    {
        const string errorCodePrefix = "VALC";
        Type[] additionalMetadataReferences = [typeof(Validator<>), typeof(Result<,>)];
        string[] trackingSteps =
            [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName];
        using var assertionScope = new AssertionScope();
        assertionScope.FormattingOptions.MaxLines = 30000;
        assertionScope.FormattingOptions.MaxDepth = 1000;

        var options = IncrementalGeneratorTestOptions.CreateDefault(additionalMetadataReferences);

        var result = new IncrementalGeneratorAdapter(options)
            .GetGeneratedTrees<TGenerator>(
                inputs,
                trackingSteps,
                errorCodePrefix,
                assertInitialCompilation,
                assertTrackingSteps)
            .AssertDiagnostics(diagnostics);
        
        // Only assert outputs if explicitly provided
        if (outputs is not null)
        {
            result.AssertOutputs(outputs);
        }
    }
}