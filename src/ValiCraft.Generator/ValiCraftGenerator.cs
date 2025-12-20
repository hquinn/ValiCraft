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
        var syncValidationRulesValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateRuleExtensionAttribute,
                ValidationRuleExtensionSyntaxProvider.Predicate,
                ValidationRuleExtensionSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidationRuleResultTrackingName);

        // Sync validators
        var syncValidatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateValidatorAttribute,
                ValidatorSyntaxProvider.Predicate,
                ValidatorSyntaxProvider.TransformSync)
            .WithTrackingName(TrackingSteps.ValidatorResultTrackingName);

        // Async validators
        var asyncValidatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.AsyncGenerateValidatorAttribute,
                ValidatorSyntaxProvider.Predicate,
                ValidatorSyntaxProvider.TransformAsync)
            .WithTrackingName(TrackingSteps.ValidatorResultTrackingName);

        var allSyncValidationRulesProvider = syncValidationRulesValuesProvider.Collect();
        var allSyncValidatorProvider = syncValidatorsValuesProvider.Collect();
        var allAsyncValidatorProvider = asyncValidatorsValuesProvider.Collect();
        
        var syncCombinedProvider = allSyncValidationRulesProvider.Combine(allSyncValidatorProvider);
        var asyncCombinedProvider = allSyncValidationRulesProvider.Combine(allAsyncValidatorProvider);

        // Emit sync validation rule extensions
        context.RegisterSourceOutput(
            allSyncValidationRulesProvider,
            static (spc, source) => ValidationRuleExtensionSourceProvider.EmitSourceCode(source, spc));
        
        // Emit sync validators
        context.RegisterSourceOutput(
            syncCombinedProvider,
            static (spc, source) =>
            {
                var (allValidationRules, allValidators) = source;
                ValidatorSourceProvider.EmitSourceCode(false, allValidationRules, allValidators, spc);
            });
        
        // Emit async validators
        context.RegisterSourceOutput(
            asyncCombinedProvider,
            static (spc, source) =>
            {
                var (allValidationRules, allValidators) = source;
                ValidatorSourceProvider.EmitSourceCode(true, allValidationRules, allValidators, spc);
            });
    }
}