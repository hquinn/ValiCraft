using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Tests.Helpers;

public static class AssertionExtensions
{
    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) AssertDiagnostics(
        this (ImmutableArray<Diagnostic> Diagnostics, string[] Output) result,
        string[] expected)
    {
        var actualDiagnosticMessages = result.Diagnostics.Select(d => d.GetMessage()).ToArray();
        actualDiagnosticMessages.Should().BeEquivalentTo(expected);

        return result;
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) AssertOutputs(
        this (ImmutableArray<Diagnostic> Diagnostics, string[] Output) result,
        string[] expected)
    {
        result.Output.Select(NormalizeCode).Should().BeEquivalentTo(expected.Select(NormalizeCode));

        return result;
    }

    private static string NormalizeCode(string code)
    {
        return code.Replace("\r\n", "\n");
    }
}