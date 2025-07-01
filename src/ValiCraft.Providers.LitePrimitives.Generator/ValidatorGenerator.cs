using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Shared;
using ValiCraft.Rules.Generator.Shared;

namespace ValiCraft.Providers.LitePrimitives.Generator;

[Generator(LanguageNames.CSharp)]
public class ValidatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var validationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedNames.Attributes.GenerateRuleExtensionAttribute,
                predicate: ValidationRuleExtensionSyntaxProvider.Predicate,
                transform: ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleInfoResultTrackingName);

        var validatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedNames.Attributes.GenerateValidatorAttribute,
                predicate: ValidatorInfoProvider.Predicate,
                transform: ValidatorInfoProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidatorInfoResultTrackingName);

        var allValidationRulesProvider = validationRulesValuesProvider.Collect();
        var allValidatorProvider = validatorsValuesProvider.Collect();

        var combinedProvider = allValidationRulesProvider.Combine(allValidatorProvider);

        context.RegisterSourceOutput(
            allValidationRulesProvider,
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));

        context.RegisterImplementationSourceOutput(
            combinedProvider,
            static (spc, source) => ValidatorSourceProvider.EmitSourceCode(source.Left, source.Right, spc));
    }
}