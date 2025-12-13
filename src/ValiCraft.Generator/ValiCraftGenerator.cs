using Microsoft.CodeAnalysis;
using ValiCraft.Generator.SourceProviders;
using ValiCraft.Generator.SyntaxProviders;

namespace ValiCraft.Generator;

[Generator(LanguageNames.CSharp)]
public class ValiCraftGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Sync validation rules
        var validationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateRuleExtensionAttribute,
                ValidationRuleExtensionSyntaxProvider.Predicate,
                ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleResultTrackingName);

        // Sync validators
        var validatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateValidatorAttribute,
                ValidatorSyntaxProvider.Predicate,
                ValidatorSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidatorResultTrackingName);

        // Async validation rules
        var asyncValidationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateAsyncRuleExtensionAttribute,
                AsyncValidationRuleExtensionSyntaxProvider.Predicate,
                AsyncValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.AsyncValidationRuleResultTrackingName);

        // Async validators
        var asyncValidatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateAsyncValidatorAttribute,
                AsyncValidatorSyntaxProvider.Predicate,
                AsyncValidatorSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.AsyncValidatorResultTrackingName);

        var allValidationRulesProvider = validationRulesValuesProvider.Collect();
        var allValidatorProvider = validatorsValuesProvider.Collect();
        var allAsyncValidationRulesProvider = asyncValidationRulesValuesProvider.Collect();
        var allAsyncValidatorProvider = asyncValidatorsValuesProvider.Collect();

        var combinedProvider = allValidationRulesProvider.Combine(allValidatorProvider);
        var asyncCombinedProvider = allAsyncValidationRulesProvider
            .Combine(allValidationRulesProvider)
            .Combine(allAsyncValidatorProvider);

        // Emit sync validation rule extensions
        context.RegisterSourceOutput(
            allValidationRulesProvider,
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));

        // Emit sync validators
        context.RegisterSourceOutput(
            combinedProvider,
            static (spc, source) =>
            {
                var (allValidationRules, allValidators) = source;
                ValidatorSourceProvider.EmitSourceCode(allValidationRules, allValidators, spc);
            });

        // Emit async validation rule extensions
        context.RegisterSourceOutput(
            allAsyncValidationRulesProvider,
            static (spc, source) => AsyncValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));

        // Emit async validators
        context.RegisterSourceOutput(
            asyncCombinedProvider,
            static (spc, source) =>
            {
                var ((allAsyncValidationRules, allValidationRules), allAsyncValidators) = source;
                AsyncValidatorSourceProvider.EmitSourceCode(allAsyncValidationRules, allValidationRules, allAsyncValidators, spc);
            });
    }
}