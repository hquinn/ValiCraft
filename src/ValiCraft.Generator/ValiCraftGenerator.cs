using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.SourceProviders;
using ValiCraft.Generator.SyntaxProviders;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator;

[Generator(LanguageNames.CSharp)]
public class ValiCraftGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
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

        // Emit sync validators
        context.RegisterSourceOutput(
            syncValidatorsValuesProvider.Collect(),
            static (spc, source) => { ValidatorSourceProvider.EmitSourceCode(source, spc); });

        // Emit async validators
        context.RegisterSourceOutput(
            asyncValidatorsValuesProvider.Collect(),
            static (spc, source) => { ValidatorSourceProvider.EmitSourceCode(source, spc); });
    }
}