using AwesomeAssertions.Execution;
using ErrorCraft;
using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Tests.Helpers;

public abstract class IncrementalGeneratorTestBase<TGenerator> where TGenerator : IIncrementalGenerator, new()
{
    /// <summary>
    /// Hint name suffixes for DI-generated files that are excluded from validator output assertions.
    /// </summary>
    private static readonly string[] DiGeneratedHintNameSuffixes =
    [
        "ValiCraftModuleRegistrar.g.cs",
        "ValiCraftModuleAttribute.g.cs",
        "ValiCraftServiceCollectionExtensions.g.cs",
        "ValiCraftExtensions.g.cs"
    ];

    protected void AssertGenerator(
        string[] inputs,
        string[]? outputs,
        string[] diagnostics,
        bool assertInitialCompilation = false,
        bool assertTrackingSteps = true)
    {
        const string errorCodePrefix = "VALC";
        Type[] additionalMetadataReferences = [typeof(Validator<>), typeof(IValidationError)];
        string[] trackingSteps =
            [TrackingSteps.ValidatorResultTrackingName, TrackingSteps.AsyncValidatorResultTrackingName];
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
                assertTrackingSteps,
                DiGeneratedHintNameSuffixes)
            .AssertDiagnostics(diagnostics);

        // Only assert outputs if explicitly provided
        if (outputs is not null)
        {
            result.AssertOutputs(outputs);
        }
    }
}