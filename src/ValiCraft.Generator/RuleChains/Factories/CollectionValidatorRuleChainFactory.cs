using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains.Factories;

public class CollectionValidatorRuleChainFactory : IRuleChainFactory
{
    public RuleChain? Create(
        bool isAsyncValidator,
        ValidationTarget @object,
        ValidationTarget? target,
        InvocationExpressionSyntax invocation,
        List<InvocationExpressionSyntax> invocationChain,
        int depth,
        IndentModel indent,
        List<DiagnosticInfo> diagnostics,
        GeneratorAttributeSyntaxContext context)
    {
        var validatorInvocation = invocationChain.Skip(1).First();

        var resolution = ValidatorResolutionHelper.Resolve(validatorInvocation, context);
        if (resolution is null)
        {
            return null;
        }

        return new CollectionValidatorRuleChain(
            isAsyncValidator,
            @object,
            target!,
            depth,
            indent,
            invocation.GetOnFailureModeFromSyntax(),
            resolution.ValidatorCallTarget,
            resolution.IsAsyncValidatorCall,
            HoistValidator: resolution.HoistValidator);
    }
}
