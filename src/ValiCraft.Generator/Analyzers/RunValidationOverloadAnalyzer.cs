using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace ValiCraft.Generator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RunValidationOverloadAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] ValidatorInterfaceNames =
    [
        KnownNames.Interfaces.IValidator,
        KnownNames.Interfaces.IAsyncValidator,
        KnownNames.Interfaces.IStaticValidator,
        KnownNames.Interfaces.IStaticAsyncValidator
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(DefinedDiagnostics.DisallowedRunValidationCall);

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

        if (!IsRunValidationMethod(method))
        {
            return;
        }

        if (!IsOnValidatorType(method))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DefinedDiagnostics.DisallowedRunValidationCall,
            invocation.Syntax.GetLocation()));
    }

    private static bool IsRunValidationMethod(IMethodSymbol method)
    {
        return method.Name is KnownNames.Methods.RunValidation or KnownNames.Methods.RunValidationAsync;
    }

    private static bool IsOnValidatorType(IMethodSymbol method)
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
