using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Rules.Builders;

public class ExtensionMethodRuleBuilder(
    EquatableArray<string> genericArguments,
    EquatableArray<ArgumentInfo> arguments,
    MapToValidationRuleData? mapData,
    MessageInfo? defaultMessage,
    MessageInfo? defaultErrorCode,
    EquatableArray<RulePlaceholder> rulePlaceholders,
    LocationInfo location) : RuleBuilder
{
    public static ExtensionMethodRuleBuilder? Create(
        IMethodSymbol methodSymbol,
        InvocationExpressionSyntax invocation,
        List<DiagnosticInfo> diagnostics,
        SemanticModel semanticModel)
    {
        var mapToValidationRuleAttribute = MapToValidationRuleData.CreateFromMethodAndAttribute(
            methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute);
        
        if (mapToValidationRuleAttribute is null)
        {
            diagnostics.Add(DefinedDiagnostics.MissingMapToValidationRuleAttribute(invocation.GetLocation()));
            return null;
        }
        
        return new ExtensionMethodRuleBuilder(
            invocation.GetGenericArguments(),
            invocation.GetArguments(methodSymbol, semanticModel).ToEquatableImmutableArray(),
            MapToValidationRuleData.CreateFromMethodAndAttribute(
                methodSymbol, KnownNames.Attributes.MapToValidationRuleAttribute),
            MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultMessageAttribute),
            MessageInfo.CreateFromAttribute(methodSymbol, KnownNames.Attributes.DefaultErrorCodeAttribute),
            RulePlaceholder.CreateFromRulePlaceholderAttributes(methodSymbol),
            LocationInfo.CreateFrom(invocation)!);
    }

    public override Rule Build()
    {
        return new ExtensionMethodRule(
            genericArguments,
            mapData,
            arguments,
            defaultMessage,
            defaultErrorCode,
            GetRuleOverrideData(),
            IfCondition,
            rulePlaceholders,
            location);
    }
}