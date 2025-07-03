using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.SyntaxProviders;

public static class ValidatorInfoProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<ValidatorInfo> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();

        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
            return new ProviderResult<ValidatorInfo>(diagnostics);

        var succeeded = TryCheckPartialKeyword(classDeclarationSyntax!, diagnostics);
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax!, classSymbol!, diagnostics, out var requestTypeName);

        if (!succeeded) return new ProviderResult<ValidatorInfo>(diagnostics);

        cancellationToken.ThrowIfCancellationRequested();

        var invocations = DiscoverRuleInvocations(context, classDeclarationSyntax!, diagnostics);
        var classInfo = new ClassInfo(classDeclarationSyntax!, classSymbol!, null);

        var validatorInfo = new ValidatorInfo(
            classInfo,
            requestTypeName!,
            invocations.ToEquatableImmutableArray());

        return new ProviderResult<ValidatorInfo>(validatorInfo, diagnostics);
    }

    private static List<RuleInvocation> DiscoverRuleInvocations(
        GeneratorAttributeSyntaxContext context,
        ClassDeclarationSyntax classDeclarationSyntax,
        List<Diagnostic> diagnostics)
    {
        var ruleInvocations = new List<RuleInvocation>();

        var defineRulesMethod = classDeclarationSyntax.Members
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == "DefineRules");

        if (defineRulesMethod?.Body is null) return ruleInvocations;

        // 1. Instead of looping over all invocations, we loop over each statement in the method.
        // This allows us to treat each fluent chain as a single unit.
        foreach (var statement in defineRulesMethod.Body.Statements.OfType<ExpressionStatementSyntax>())
        {
            // A valid chain must end with a method call.
            if (statement.Expression is not InvocationExpressionSyntax outermostInvocation) continue;

            // 2. "Walk the chain" backwards from the outermost call (e.g., .IsGenericRule2)
            // to the innermost call (e.g., .Ensure) to collect all parts of the chain.
            var invocationChain = new List<InvocationExpressionSyntax>();
            var currentExpression = (ExpressionSyntax)outermostInvocation;

            while (currentExpression is InvocationExpressionSyntax currentInvocation)
            {
                invocationChain.Add(currentInvocation);
                if (currentInvocation.Expression is MemberAccessExpressionSyntax memberAccess)
                    currentExpression = memberAccess.Expression;
                else
                    break; // We've reached the start of the chain (the 'builder' identifier).
            }

            // The collected chain is backwards, so we reverse it to get the correct execution order.
            invocationChain.Reverse();

            // 3. A valid chain must start with an 'Ensure(...)' call.
            if (invocationChain.Count < 2) continue; // Must have at least Ensure() and one rule.

            var ensureInvocation = invocationChain[0];
            var ensureMemberAccess = ensureInvocation.Expression as MemberAccessExpressionSyntax;
            if (ensureMemberAccess?.Name.Identifier.ValueText != "Ensure") continue;

            // 4. Extract the property information from the 'Ensure' call's lambda.
            // This property applies to ALL subsequent rule calls in this chain.
            if (ensureInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression is not SimpleLambdaExpressionSyntax
                    lambda ||
                lambda.Body is not MemberAccessExpressionSyntax propertyAccess)
                continue; // Ensure() call is malformed.

            ArgumentInfo property;
            if (context.SemanticModel.GetSymbolInfo(propertyAccess).Symbol is IPropertySymbol
                propertySymbol)
                property = new ArgumentInfo(
                    "",
                    propertyAccess.Name.Identifier.ValueText,
                    propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    true);
            else
                // If we can't resolve the property, we can't safely process this chain.
                // You could add a diagnostic here.
                continue;

            RuleInvocationBuilder? currentRuleBuilder = null;

            // 5. Now, process each *actual* rule call in the chain, skipping the initial Ensure() call.
            foreach (var invocation in invocationChain.Skip(1))
            {
                var memberName = ((MemberAccessExpressionSyntax)invocation.Expression).Name.Identifier.ValueText;
                var argumentExpression = invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;

                switch (memberName)
                {
                    case "WithMessage":
                        if (argumentExpression is not null)
                            currentRuleBuilder?.WithMessage(CreateMessageInfo(argumentExpression));

                        continue;
                    case "WithErrorCode":
                        if (argumentExpression is not null)
                            currentRuleBuilder?.WithErrorCode(CreateMessageInfo(argumentExpression));

                        continue;
                    case "WithPropertyName":
                        if (argumentExpression is not null)
                            currentRuleBuilder?.WithPropertyName(CreateMessageInfo(argumentExpression));

                        continue;
                }

                // If it's not a modifier, it must be a new rule.

                // 1. If we were building a previous rule, finalize it and add it to the list.
                if (currentRuleBuilder is not null) ruleInvocations.Add(currentRuleBuilder.Build());

                // 2. Start building the new rule.
                var invocationSymbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
                var methodSymbol = invocationSymbolInfo.Symbol as IMethodSymbol;
                MapToValidationRuleData? mapToValidationRuleData = null;
                MessageInfo? defaultMessage = null;
                EquatableArray<ArgumentInfo> ruleInvocationArguments;
                EquatableArray<RulePlaceholderInfo> rulePlaceholders;
                if (methodSymbol is not null)
                {
                    var attributeDisplayFormat = SymbolDisplayFormats.FormatAttributeWithoutParameters;
                    var mapToValidationRuleAttribute = methodSymbol
                        .GetAttributes()
                        .FirstOrDefault(attributeData
                            => attributeData.AttributeClass?.ToDisplayString(attributeDisplayFormat) ==
                               FullyQualifiedNames.Attributes.MapToValidationRuleAttribute);

                    if (mapToValidationRuleAttribute is not null)
                        mapToValidationRuleData = new MapToValidationRuleData(mapToValidationRuleAttribute);

                    var containingType = methodSymbol.ContainingType;
                    defaultMessage = MessageInfo.CreateFromAttribute(
                        containingType,
                        FullyQualifiedNames.Attributes.DefaultMessageAttribute);
                    ruleInvocationArguments = invocation.GetArguments(context.SemanticModel, methodSymbol);
                    rulePlaceholders = RulePlaceholderInfo.CreateFromRulePlaceholderAttributes(containingType);
                }
                else
                {
                    ruleInvocationArguments = invocation.GetArguments(context.SemanticModel, null);
                    rulePlaceholders = EquatableArray<RulePlaceholderInfo>.Empty;
                    
                    //if (methodSymbol is null)
                    // {
                    //     // --- THIS IS THE DEBUGGING LOGIC ---
                    //     // Let's find out WHY the symbol is null.
                    //     // If CandidateSymbols has items, the compiler found methods but rejected them.
                    //     if (invocationSymbolInfo.CandidateSymbols.IsEmpty)
                    //     {
                    //         // Case 1: The compiler found no methods with that name at all.
                    //         diagnostics.Add(Diagnostic.Create(
                    //             Diagnostics.MethodNotFound,
                    //             invocation.GetLocation(),
                    //             memberName));
                    //     }
                    //     else
                    //     {
                    //         // Case 2: The compiler found candidates but rejected them all.
                    //         // We'll format the list of candidates for the error message.
                    //         var candidateSignatures = string.Join("\n  - ", 
                    //             invocationSymbolInfo.CandidateSymbols.Select(s => s.ToDisplayString()));
                    //
                    //         diagnostics.Add(Diagnostic.Create(
                    //             Diagnostics.AmbiguousMethod,
                    //             invocation.GetLocation(),
                    //             memberName,
                    //             candidateSignatures,
                    //             invocationSymbolInfo.CandidateReason.ToString()));
                    //     }
                    //     // --- END OF DEBUGGING LOGIC ---
                    // }
                }

                currentRuleBuilder = new RuleInvocationBuilder(
                    property,
                    memberName,
                    ruleInvocationArguments,
                    mapToValidationRuleData,
                    defaultMessage,
                    rulePlaceholders);
            }

            // 3. After the loop, add the very last rule that was being built.
            if (currentRuleBuilder is not null) ruleInvocations.Add(currentRuleBuilder.Build());
        }

        return ruleInvocations;
    }

    private static class Diagnostics
    {
        // For when the compiler can't find any method with that name.
        public static readonly DiagnosticDescriptor MethodNotFound = new(
            "VC010",
            "Validation method not found",
            "The validation method '{0}' could not be found. Ensure the extension method is defined, accessible, and its source or assembly is referenced.",
            "ValiCraft.Validation",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        // For when the compiler finds multiple potential methods and can't choose.
        public static readonly DiagnosticDescriptor AmbiguousMethod = new(
            "VC011",
            "Ambiguous validation method",
            "The call to '{0}' is ambiguous and compiler could not choose between the following candidates for the reason '{2}': {1}",
            "ValiCraft.Validation",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
    
    private static MessageInfo CreateMessageInfo(ExpressionSyntax expression)
    {
        if (expression is LiteralExpressionSyntax literal &&
            literal.IsKind(SyntaxKind.StringLiteralExpression))
            // It's a string literal, so we capture its raw value.
            return new MessageInfo(literal.Token.ValueText, true);

        // It's any other C# expression. We capture its source text.
        return new MessageInfo(expression.ToString(), false);
    }

    private static bool TryGetRequestTypeName(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<Diagnostic> diagnostics,
        out string? requestTypeName)
    {
        requestTypeName = null;
        if (!classSymbol.Inherits(FullyQualifiedNames.Classes.Validator, 1))
        {
            diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor("VC004", "Invalid Base Class",
                    "Classes attributed with [GenerateValidator] must inherit from Validator<TRequest>", "ValiCraft",
                    DiagnosticSeverity.Error, true),
                classDeclarationSyntax.GetLocation()));
            return false;
        }

        requestTypeName = $"global::{classSymbol.BaseType!.TypeArguments[0].ToDisplayString()}";
        return true;
    }

    private static bool TryCheckPartialKeyword(ClassDeclarationSyntax classDeclarationSyntax,
        List<Diagnostic> diagnostics)
    {
        if (!classDeclarationSyntax.IsPartial())
        {
            diagnostics.Add(Diagnostic.Create(
                new DiagnosticDescriptor("VC003", "Missing partial keyword",
                    "Classes attributed with [GenerateValidator] must have the partial keyword", "ValiCraft",
                    DiagnosticSeverity.Error, true),
                classDeclarationSyntax.GetLocation()));
            return false;
        }

        return true;
    }
}