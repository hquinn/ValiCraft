using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Tests.Helpers;

/// <summary>
///     A test adapter for running C# Incremental Source Generators and asserting their behavior.
///     This class is designed to be instantiated with a configuration and exposes a fluent pipeline
///     for executing the generator test steps.
/// </summary>
/// <example>
///     Simple usage:
///     <code>
/// var adapter = new IncrementalGeneratorAdapter();
/// var (diagnostics, output) = adapter.GetGeneratedTrees&lt;MyGenerator&gt;(sources, stages);
/// </code>
///     Pipeline usage:
///     <code>
/// var adapter = new IncrementalGeneratorAdapter();
/// var result = adapter.CreatePipeline&lt;MyGenerator&gt;(sources, stages)
///     .ParseSources()
///     .CreateCompilation()
///     .RunGenerator()
///     .AssertCacheability()
///     .GetResult();
/// </code>
/// </example>
public class IncrementalGeneratorAdapter
{
    internal readonly IncrementalGeneratorTestOptions Options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IncrementalGeneratorAdapter{TGenerator}" /> class.
    /// </summary>
    /// <param name="options">The configuration options for this test run. If null, default options will be used.</param>
    public IncrementalGeneratorAdapter(IncrementalGeneratorTestOptions? options = null)
    {
        Options = options ?? IncrementalGeneratorTestOptions.CreateDefault([]);
    }

    /// <summary>
    ///     A convenience method that creates and runs a full generator test pipeline.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IIncrementalGenerator" /> to run.</typeparam>
    /// <param name="sources">The C# source code strings to compile.</param>
    /// <param name="trackingSteps">The names of the generator's tracking stages to verify for cacheability.</param>
    /// <param name="errorCodePrefix">The prefix used for retrieving custom diagnostics.</param>
    /// <param name="assertInitialCompilation">Flag used to assert the incremental generators diagnostics from compilation</param>
    /// <param name="assertTrackingSteps">Flag used to assert if the tracking steps should not be empty</param>
    /// <returns>A tuple containing the generator diagnostics and the string content of the generated source files.</returns>
    public (ImmutableArray<Diagnostic> Diagnostics, string[] Output) GetGeneratedTrees<T>(
        string[] sources,
        string[] trackingSteps,
        string? errorCodePrefix,
        bool assertInitialCompilation = false,
        bool assertTrackingSteps = true)
        where T : IIncrementalGenerator, new()
    {
        return CreatePipeline<T>(sources, trackingSteps)
            .RunGenerator(assertInitialCompilation)
            .AssertCacheability(assertTrackingSteps)
            .GetResult(errorCodePrefix);
    }

    /// <summary>
    ///     Creates a new test pipeline for executing a generator.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IIncrementalGenerator" /> to run.</typeparam>
    /// <param name="sources">The C# source code strings to compile.</param>
    /// <param name="trackingSteps">The names of the generator's tracking steps to verify.</param>
    /// <returns>A new <see cref="GeneratorPipeline{T}" /> instance.</returns>
    public GeneratorPipeline<T> CreatePipeline<T>(string[] sources, string[] trackingSteps)
        where T : IIncrementalGenerator, new()
    {
        return new GeneratorPipeline<T>(this, sources, trackingSteps);
    }

    /// <summary>
    ///     Asserts that two generator runs produced identical tracked step outputs.
    ///     This method is for internal use by the pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AssertRunsEqual(
        GeneratorDriverRunResult runResult1,
        GeneratorDriverRunResult runResult2,
        string[] trackingSteps)
    {
        // Extract the tracked steps from each run that we are interested in
        var trackedSteps1 = GetTrackedSteps(runResult1, trackingSteps);
        var trackedSteps2 = GetTrackedSteps(runResult2, trackingSteps);

        // Both runs should have the same number of tracked steps with the same names
        trackedSteps1.Should()
            .NotBeEmpty()
            .And.HaveSameCount(trackedSteps2)
            .And.ContainKeys(trackedSteps2.Keys);
        
        // For each tracked step, assert that the outputs are equal
        foreach (var (trackingName, runSteps1) in trackedSteps1)
        {
            var runSteps2 = trackedSteps2[trackingName];
            AssertEqual(runSteps1, runSteps2, trackingName);
        }

        return;

        // Local function to extract and filter the tracked steps from a run result
        static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(
            GeneratorDriverRunResult runResult,
            string[] trackingNames)
        {
            return runResult
                .Results[0] // We're only running a single generator
                .TrackedSteps
                .Where(step => trackingNames.Contains(step.Key)) // filter to known steps
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    /// <summary>
    ///     Asserts that the outputs of a specific step are equal between two runs and that the second run was cached.
    ///     This method is for internal use by the pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AssertEqual(
        ImmutableArray<IncrementalGeneratorRunStep> runSteps1,
        ImmutableArray<IncrementalGeneratorRunStep> runSteps2,
        string stepName)
    {
        runSteps1.Should().HaveSameCount(runSteps2);

        for (var i = 0; i < runSteps1.Length; i++)
        {
            var runStep1 = runSteps1[i];
            var runStep2 = runSteps2[i];

            // The output values should be equal between runs
            var outputs1 = runStep1.Outputs.Select(x => x.Value);
            var outputs2 = runStep2.Outputs.Select(x => x.Value);

            outputs1.Should()
                .Equal(outputs2, $"because {stepName} should produce cacheable outputs");

            // In the second run, results should be either Cached or Unchanged
            runStep2.Outputs.Should()
                .OnlyContain(
                    x => x.Reason == IncrementalStepRunReason.Cached || x.Reason == IncrementalStepRunReason.Unchanged,
                    $"{stepName} expected to have reason {IncrementalStepRunReason.Cached} or {IncrementalStepRunReason.Unchanged}");

            // Finally, verify the object graph of the output doesn't contain banned types
            AssertObjectGraph(runStep1, stepName);
        }
    }

    /// <summary>
    ///     Traverses the object graph of a generator step's output to ensure it doesn't
    ///     contain any "banned" types (like Compilation, ISymbol, or SyntaxNode).
    ///     This method is for internal use by the pipeline.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
    {
        // Including the stepName in error messages to make it easy to isolate issues
        var because = $"{stepName} shouldn't contain banned symbols";
        var visited = new HashSet<object>();
        var bannedTypes = Options.BannedTypesForAnalysis;

        if (bannedTypes.Count == 0)
        {
            return; // No types to check against
        }

        // Check all of the outputs
        foreach (var (obj, _) in runStep.Outputs)
        {
            Visit(obj);
        }

        void Visit(object? node)
        {
            // If we've already seen this object, or it's null, stop.
            if (node is null || !visited.Add(node))
            {
                return;
            }

            foreach (var bannedType in bannedTypes)
                // Assert that the node is not one of the banned types.
            {
                node.Should().NotBeAssignableTo(bannedType, because);
            }

            // We don't need to inspect primitives, enums, or strings further.
            var type = node.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return;
            }

            // If the object is a collection, recursively visit each element.
            if (node is IEnumerable collection)
            {
                foreach (var element in collection)
                {
                    Visit(element);
                }

                return;
            }

            // Recursively check each field in the object.
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var fieldValue = field.GetValue(node);
                Visit(fieldValue);
            }
        }
    }
}