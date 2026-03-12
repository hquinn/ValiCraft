using System.Collections.Immutable;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using ErrorCraft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ValiCraft.Generator.Tests.Helpers;

public abstract class AnalyzerTestBase<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected void AssertAnalyzer(
        string[] inputs,
        string[] diagnostics)
    {
        using var assertionScope = new AssertionScope();
        assertionScope.FormattingOptions.MaxLines = 30000;
        assertionScope.FormattingOptions.MaxDepth = 1000;

        var options = IncrementalGeneratorTestOptions.CreateDefault([typeof(Validator<>), typeof(IValidationError)]);

        var syntaxTrees = inputs.Select(x => CSharpSyntaxTree.ParseText(x));

        var compilation = CSharpCompilation.Create(
            "TestCompilation",
            syntaxTrees,
            options.MetadataReferences,
            options.CompilationOptions);

        var analyzer = new TAnalyzer();
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));

        var analyzerDiagnostics = compilationWithAnalyzers
            .GetAnalyzerDiagnosticsAsync().Result
            .Where(d => d.Id.StartsWith("VALC"))
            .ToArray();

        var actualMessages = analyzerDiagnostics.Select(d => d.GetMessage()).ToArray();
        actualMessages.Should().BeEquivalentTo(diagnostics);
    }
}
