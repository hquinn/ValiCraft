using Microsoft.CodeAnalysis;
using ValiCraft.Generator.SourceProviders;
using ValiCraft.Generator.SyntaxProviders;

namespace ValiCraft.Generator;

[Generator(LanguageNames.CSharp)]
public class ValiCraftGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var validationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateRuleExtensionAttribute,
                ValidationRuleExtensionSyntaxProvider.Predicate,
                ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleResultTrackingName);

        var validatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateValidatorAttribute,
                ValidatorSyntaxProvider.Predicate,
                ValidatorSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidatorResultTrackingName);

        var allValidationRulesProvider = validationRulesValuesProvider.Collect();
        var allValidatorProvider = validatorsValuesProvider.Collect();

        var combinedProvider = allValidationRulesProvider.Combine(allValidatorProvider);

        context.RegisterSourceOutput(
            allValidationRulesProvider,
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));

        context.RegisterSourceOutput(
            combinedProvider,
            static (spc, source) =>
            {
                var (allValidationRules, allValidators) = source;
                ValidatorSourceProvider.EmitSourceCode(allValidationRules, allValidators, spc);
            });
    }
}