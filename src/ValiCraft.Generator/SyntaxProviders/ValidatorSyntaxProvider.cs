using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SyntaxProviders;

public static class ValidatorSyntaxProvider
{
    public static bool Predicate(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax;
    }

    public static ProviderResult<Validator> Transform(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();

        if (!context.TryGetClassNodeAndSymbol(diagnostics, out var classDeclarationSyntax, out var classSymbol))
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        var succeeded = TryCheckPartialKeyword(classDeclarationSyntax!, diagnostics);
        succeeded &= TryGetRequestTypeName(classDeclarationSyntax!, classSymbol!, diagnostics, out var isAsyncValidator, out var isStaticValidator, out var requestTypeName);

        if (!succeeded)
        {
            return new ProviderResult<Validator>(diagnostics);
        }

        // For static validators, check for instance members
        if (isStaticValidator)
        {
            CheckStaticValidatorForInstanceMembers(classDeclarationSyntax!, classSymbol!, diagnostics);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var classInfo = ClassInfo.CreateFromSyntaxAndSymbols(classDeclarationSyntax!, classSymbol!);
        var ruleChains = RuleChainsSyntaxProvider.DiscoverRuleChains(
            isAsyncValidator,
            diagnostics,
            classDeclarationSyntax!,
            classSymbol!,
            context);

        // Extract IncludeDefaultMetadata from attribute, defaulting to false
        var includeDefaultMetadata = false;
        var attributeData = context.Attributes.FirstOrDefault();
        if (attributeData != null)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key == "IncludeDefaultMetadata" && namedArg.Value.Value is bool value)
                {
                    includeDefaultMetadata = value;
                    break;
                }
            }
        }

        var validator = new Validator(
            isAsyncValidator,
            isStaticValidator,
            classInfo,
            requestTypeName!,
            ruleChains,
            classDeclarationSyntax!.GetUsingDirectives(),
            includeDefaultMetadata);

        return new ProviderResult<Validator>(validator, diagnostics);
    }

    private static bool TryGetRequestTypeName(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics,
        out bool isAsyncValidator,
        out bool isStaticValidator,
        out SymbolNameInfo? requestTypeName)
    {
        requestTypeName = null;
        isAsyncValidator = false;
        isStaticValidator = false;
        
        // Detect sync vs async and instance vs static from base class
        var inheritsValidator = classSymbol.Inherits(KnownNames.Classes.Validator, 1);
        var inheritsAsyncValidator = classSymbol.Inherits(KnownNames.Classes.AsyncValidator, 1);
        var inheritsStaticValidator = classSymbol.Inherits(KnownNames.Classes.StaticValidator, 1);
        var inheritsStaticAsyncValidator = classSymbol.Inherits(KnownNames.Classes.StaticAsyncValidator, 1);
        
        if (!inheritsValidator && !inheritsAsyncValidator && !inheritsStaticValidator && !inheritsStaticAsyncValidator)
        {
            diagnostics.Add(DefinedDiagnostics.MissingValidatorBaseClass(false, classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }
        
        isAsyncValidator = inheritsAsyncValidator || inheritsStaticAsyncValidator;
        isStaticValidator = inheritsStaticValidator || inheritsStaticAsyncValidator;

        var typeArgument = classSymbol.BaseType!.TypeArguments[0];
        
        requestTypeName = new SymbolNameInfo(typeArgument);
        return true;
    }

    private static bool TryCheckPartialKeyword(
        ClassDeclarationSyntax classDeclarationSyntax,
        List<DiagnosticInfo> diagnostics)
    {
        if (!classDeclarationSyntax.IsPartial())
        {
            diagnostics.Add(DefinedDiagnostics.MissingPartialKeyword(classDeclarationSyntax.Identifier.GetLocation()));
            return false;
        }

        return true;
    }

    private static void CheckStaticValidatorForInstanceMembers(
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        List<DiagnosticInfo> diagnostics)
    {
        // Check for parameterized constructors (primary constructors or regular constructors)
        // Primary constructor parameters appear as constructor parameters on the class
        foreach (var constructor in classSymbol.Constructors)
        {
            // Skip implicitly declared (default) constructors and static constructors
            if (constructor.IsImplicitlyDeclared || constructor.IsStatic)
            {
                continue;
            }

            // If the constructor has parameters, report an error
            if (constructor.Parameters.Length > 0)
            {
                var location = constructor.Locations.Length > 0 
                    ? constructor.Locations[0] 
                    : classDeclarationSyntax.Identifier.GetLocation();
                diagnostics.Add(DefinedDiagnostics.StaticValidatorHasInstanceConstructor(location));
            }
        }

        // Check for instance fields (excluding backing fields for properties)
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IFieldSymbol { IsStatic: false, IsImplicitlyDeclared: false, AssociatedSymbol: null } field) // Not a backing field
            {
                var location = field.Locations.Length > 0 
                    ? field.Locations[0] 
                    : classDeclarationSyntax.Identifier.GetLocation();
                diagnostics.Add(DefinedDiagnostics.StaticValidatorHasInstanceField(field.Name, location));
            }

            // Check for instance properties (excluding auto-generated ones)
            if (member is IPropertySymbol { IsStatic: false, IsImplicitlyDeclared: false } property)
            {
                var location = property.Locations.Length > 0 
                    ? property.Locations[0] 
                    : classDeclarationSyntax.Identifier.GetLocation();
                diagnostics.Add(DefinedDiagnostics.StaticValidatorHasInstanceProperty(property.Name, location));
            }
            
            // Check for instance methods (excluding auto-generated ones and DefineRules)
            if (member is IMethodSymbol { IsStatic: false, IsImplicitlyDeclared: false, MethodKind: MethodKind.Ordinary } method &&
                method.Name != "DefineRules") // DefineRules is required by the base class
            {
                var location = method.Locations.Length > 0 
                    ? method.Locations[0] 
                    : classDeclarationSyntax.Identifier.GetLocation();
                diagnostics.Add(DefinedDiagnostics.StaticValidatorHasInstanceMethod(method.Name, location));
            }
        }
    }
}