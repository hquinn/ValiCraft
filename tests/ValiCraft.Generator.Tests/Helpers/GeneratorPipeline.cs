using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ValiCraft.Generator.Tests.Helpers;

/// <summary>
///     A fluent pipeline for compiling sources and running an incremental generator.
/// </summary>
/// <typeparam name="T">The type of the <see cref="IIncrementalGenerator" /> to run.</typeparam>
public class GeneratorPipeline<T> where T : IIncrementalGenerator, new()
{
    private readonly IncrementalGeneratorAdapter _adapter;
    private readonly IncrementalGeneratorTestOptions _options;
    private readonly string[] _sources;
    private readonly string[] _trackingSteps;
    private CSharpCompilation? _compilation;
    private GeneratorDriver? _driver;
    private CSharpCompilation? _finalCompilation;
    private GeneratorDriverRunResult? _firstRunResult;

    // Intermediate state populated by pipeline steps
    private IEnumerable<SyntaxTree>? _syntaxTrees;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GeneratorPipeline{T}" /> class.
    /// </summary>
    internal GeneratorPipeline(IncrementalGeneratorAdapter adapter, string[] sources, string[] trackingSteps)
    {
        _adapter = adapter;
        _sources = sources;
        _trackingSteps = trackingSteps;
        _options = adapter.Options;
    }

    /// <summary>
    ///     Step 1: Parses the source code strings into a collection of syntax trees.
    /// </summary>
    private GeneratorPipeline<T> ParseSources()
    {
        _syntaxTrees = _sources.Select(static x => CSharpSyntaxTree.ParseText(x));
        return this;
    }

    /// <summary>
    ///     Step 2: Creates a C# compilation from the parsed syntax trees and configured options.
    ///     Requires <see cref="ParseSources" /> to have been called.
    /// </summary>
    private GeneratorPipeline<T> CreateInitialCompilation(bool assertInitialCompilation = false)
    {
        if (_syntaxTrees is null)
        {
            throw new InvalidOperationException(
                "Cannot create compilation before parsing sources. Call ParseSources() first.");
        }

        _compilation = CSharpCompilation.Create(
            _options.CompilationName,
            _syntaxTrees,
            _options.MetadataReferences,
            _options.CompilationOptions);

        if (assertInitialCompilation)
        {
            var diagnostics = _compilation
                .GetDiagnostics()
                .Where(x => x.Severity == DiagnosticSeverity.Error)
                .ToList();

            diagnostics.Should().BeEmpty();
        }

        return this;
    }

    /// <summary>
    ///     Step 3: Creates the generator driver and performs the initial generator run.
    ///     Requires <see cref="CreateInitialCompilation" /> to have been called.
    /// </summary>
    public GeneratorPipeline<T> RunGenerator(bool assertInitialCompilation = false)
    {
        ParseSources();
        CreateInitialCompilation(assertInitialCompilation);

        var generator = new T().AsSourceGenerator();
        var opts = new GeneratorDriverOptions(
            IncrementalGeneratorOutputKind.None,
            true);
        _driver = CSharpGeneratorDriver.Create([generator], driverOptions: opts);

        if (_compilation is not null)
        {
            // Perform the initial run
            _driver = _driver.RunGenerators(_compilation);
            _firstRunResult = _driver.GetRunResult();

            // Create a new compilation with the generated code for validation.
            // We can expect errors in the first run, but the final compilation should be clean.
            var allSyntaxTrees = _compilation.SyntaxTrees.Concat(_firstRunResult.GeneratedTrees);
            _finalCompilation = CSharpCompilation.Create(
                "FinalCompilation",
                allSyntaxTrees,
                _compilation.References,
                _compilation.Options);
        }

        return this;
    }

    /// <summary>
    ///     Step 4 (Optional): Performs a second generator run to assert that all outputs were cached.
    ///     This step is skipped if <see cref="IncrementalGeneratorTestOptions.AssertCacheability" /> is false.
    ///     Requires <see cref="RunGenerator" /> to have been called.
    /// </summary>
    public GeneratorPipeline<T> AssertCacheability(bool assertTrackingSteps = true)
    {
        if (!_options.AssertCacheability)
        {
            return this;
        }

        if (_driver is null || _compilation is null || _firstRunResult is null)
        {
            throw new InvalidOperationException(
                "Cannot assert cacheability before running the generator. Call RunGenerator() first.");
        }

        var clone = _compilation.Clone();
        var secondRunResult = _driver
            .RunGenerators(clone)
            .GetRunResult();

        // Compare all the tracked outputs; this will throw on failure
        if (assertTrackingSteps)
        {
            _adapter.AssertRunsEqual(_firstRunResult, secondRunResult, _trackingSteps);
        }

        // Verify the second run only generated cached source outputs
        secondRunResult.Results[0]
            .TrackedOutputSteps
            .SelectMany(x => x.Value) // step executions
            .SelectMany(x => x.Outputs) // execution results
            .Should()
            .OnlyContain(x => x.Reason == IncrementalStepRunReason.Cached);

        return this;
    }

    /// <summary>
    ///     Final Step: Extracts the diagnostics and generated source strings from the result.
    ///     Requires <see cref="RunGenerator" /> to have been called.
    /// </summary>
    /// <returns>A tuple containing the final diagnostics and generated output.</returns>
    public (ImmutableArray<Diagnostic> Diagnostics, string[] Output) GetResult(string? errorCodePrefix)
    {
        if (_firstRunResult is null || _finalCompilation is null)
        {
            throw new InvalidOperationException(
                "Cannot get the result before running the generator. Call RunGenerator() first.");
        }
        
        // Get diagnostics from the first compilation (only our custom diagnostics) and the final compilation 
        var allDiagnostics = _firstRunResult.Diagnostics
            .Where(x => errorCodePrefix is not null && x.Id.StartsWith(errorCodePrefix))
            .Concat(_finalCompilation.GetDiagnostics()
                    .Where(x => x.Severity >= DiagnosticSeverity.Error))
            .ToImmutableArray();

        var output = _firstRunResult.GeneratedTrees.Select(x => x.ToString()).ToArray();

        return (allDiagnostics, output);
    }
}