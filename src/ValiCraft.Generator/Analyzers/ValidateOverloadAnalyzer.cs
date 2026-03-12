using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ValiCraft.Generator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ValidateOverloadAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] ValidatorInterfaceNames =
    [
        KnownNames.Interfaces.IValidator,
        KnownNames.Interfaces.IAsyncValidator,
        KnownNames.Interfaces.IStaticValidator,
        KnownNames.Interfaces.IStaticAsyncValidator
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DefinedDiagnostics.DisallowedValidateOverload);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterOperationAction(AnalyzeInvocation, OperationKind.Invocation);
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var method = invocation.TargetMethod;

        if (!IsValidateMethod(method))
        {
            return;
        }

        if (!HasInheritedTargetPathParameter(method))
        {
            return;
        }

        if (!IsOnValidatorInterface(method))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DefinedDiagnostics.DisallowedValidateOverload,
            invocation.Syntax.GetLocation()));
    }

    private static bool IsValidateMethod(IMethodSymbol method)
    {
        return method.Name is KnownNames.Methods.Validate or KnownNames.Methods.ValidateAsync;
    }

    private static bool HasInheritedTargetPathParameter(IMethodSymbol method)
    {
        foreach (var parameter in method.Parameters)
        {
            if (parameter.Name == "inheritedTargetPath" &&
                parameter.Type.SpecialType == SpecialType.System_String)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsOnValidatorInterface(IMethodSymbol method)
    {
        var containingType = method.ContainingType;

        if (IsValidatorInterface(containingType))
        {
            return true;
        }

        foreach (var iface in containingType.AllInterfaces)
        {
            if (IsValidatorInterface(iface))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsValidatorInterface(INamedTypeSymbol type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var fullName = $"{type.ContainingNamespace}.{type.Name}";

        foreach (var name in ValidatorInterfaceNames)
        {
            if (fullName == name)
            {
                return true;
            }
        }

        return false;
    }
}
