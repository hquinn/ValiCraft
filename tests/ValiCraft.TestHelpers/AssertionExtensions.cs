using System.Collections.Immutable;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;

namespace ValiCraft.TestHelpers;

public static class AssertionExtensions
{
    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) AssertHasNoDiagnostics(
        this (ImmutableArray<Diagnostic> Diagnostics, string[] Output) result)
    {
        result.Diagnostics.Should().BeEmpty();

        return result;
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, string[] Output) AssertOutputs(
        this (ImmutableArray<Diagnostic> Diagnostics, string[] Output) result,
        params string[] expected)
    {
        
        
        result.Output.Select(NormalizeCode).Should().BeEquivalentTo(expected.Select(NormalizeCode));
        
        return result;
    }

    private static string NormalizeCode(string code)
    {
        return code.Replace("\r\n", "\n");
    }
}