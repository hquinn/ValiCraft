using Microsoft.CodeAnalysis;
using ValiCraft.Generator.SourceProviders;
using ValiCraft.Generator.SyntaxProviders;

namespace ValiCraft.Generator;

[Generator(LanguageNames.CSharp)]
public class ValidatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var validationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedNames.Attributes.GenerateRuleExtensionAttribute,
                ValidationRuleExtensionSyntaxProvider.Predicate,
                ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleInfoResultTrackingName);

        var validatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                FullyQualifiedNames.Attributes.GenerateValidatorAttribute,
                ValidatorInfoProvider.Predicate,
                ValidatorInfoProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidatorInfoResultTrackingName);

        var allValidationRulesProvider = validationRulesValuesProvider.Collect();
        var allValidatorProvider = validatorsValuesProvider.Collect();

        var combinedProvider = allValidationRulesProvider.Combine(allValidatorProvider);

        context.RegisterSourceOutput(
            allValidationRulesProvider,
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));

        context.RegisterImplementationSourceOutput(
            combinedProvider,
            static (spc, source) =>
            {
                var (allValidationRules, allValidators) = source;
                ValidatorSourceProvider.EmitSourceCode(allValidationRules, allValidators, spc);
            });
    }
}