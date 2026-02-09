using Microsoft.CodeAnalysis;
using ValiCraft.Generator.SourceProviders;
using ValiCraft.Generator.SyntaxProviders;

namespace ValiCraft.Generator;

[Generator(LanguageNames.CSharp)]
public class ValiCraftGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // All validators (sync and async) - async is detected from base class
        var validatorsValuesProvider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                KnownNames.Attributes.GenerateValidatorAttribute,
                ValidatorSyntaxProvider.Predicate,
                ValidatorSyntaxProvider.Transform)
            .WithTrackingName(TrackingSteps.ValidatorResultTrackingName);

        // Emit validators
        context.RegisterSourceOutput(
            validatorsValuesProvider.Collect(),
            static (spc, source) => { ValidatorSourceProvider.EmitSourceCode(source, spc); });
    }
}