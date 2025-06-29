using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Shared;
using ValiCraft.Rules.Generator.Shared;

namespace ValiCraft.Rules.Generator;

[Generator(LanguageNames.CSharp)]
public class ValidationRuleExtensionGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var validationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedNames.Attributes.GenerateRuleExtensionAttribute,
                predicate: ValidationRuleExtensionSyntaxProvider.Predicate,
                transform: ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleInfoResultTrackingName);

        context.RegisterSourceOutput(
            validationRulesValuesProvider.Collect(),
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));
    }
}