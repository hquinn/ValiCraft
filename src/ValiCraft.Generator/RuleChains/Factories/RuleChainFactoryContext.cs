using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public record RuleChainFactoryContext(
    bool IsAsyncValidator,
    ValidationTarget Object,
    ValidationTarget? Target,
    InvocationExpressionSyntax Invocation,
    List<InvocationExpressionSyntax> InvocationChain,
    int Depth,
    IndentModel Indent,
    List<DiagnosticInfo> Diagnostics,
    GeneratorAttributeSyntaxContext GeneratorContext);
