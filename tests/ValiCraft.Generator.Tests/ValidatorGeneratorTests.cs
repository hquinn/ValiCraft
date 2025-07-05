using System.Diagnostics.CodeAnalysis;
using AwesomeAssertions.Execution;
using LitePrimitives;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class ValidatorGeneratorTests
{
    // Define request types which will be used on our validators
    [StringSyntax("CSharp")] private const string Requests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public required Customer Customer { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public string? ShippingReference { get; set; }
                                                                 public required List<LineItem> LineItems { get; set; }
                                                             }

                                                             public class Customer
                                                             {
                                                                 public required Guid CustomerId { get; set; }
                                                                 public required string FirstName { get; set; }
                                                                 public string? MiddleName { get; set; }
                                                                 public required string LastName { get; set; }
                                                             }

                                                             public class LineItem
                                                             {
                                                                 public required string SKU { get; set; }
                                                                 public int Quantity { get; set; }
                                                                 public decimal PricePerUnit { get; set; }
                                                             }
                                                             """;

    // Define validation rules which are annotated with [GenerateRuleExtension("...")].
    // When annotated, it will generate extension methods which the validators can use to generate validation code.
    [StringSyntax("CSharp")] private const string ValidationRulesToGenerate =
        """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        // IsNotEmpty will be the name of the extension method.
        [GenerateRuleExtension("IsNotEmpty")]
        // This is the default message that will be displayed
        // when the validation rule is not valid.
        [DefaultMessage("'{PropertyName}' must not be empty.")]
        public class NotEmptyRule : IValidationRule<string?>
        {
            public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
        }

        [GenerateRuleExtension("IsGreaterThan")]
        // {PropertyName} and {PropertyValue} are in-built placeholders.
        // {ValueToCompare} is a user-defined placeholder.
        [DefaultMessage("'{PropertyName}' must be greater than {ValueToCompare}, but received {PropertyValue}.")]
        // Create a user-defined placeholder, which matches the 'valueToCompare' parameter value.
        [RulePlaceholder("{ValueToCompare}", "valueToCompare")]
        public class GreaterThanRule<TPropertyValue> : IValidationRule<TPropertyValue, TPropertyValue>
            where TPropertyValue : IComparable
        {
            public static bool IsValid(TPropertyValue value, TPropertyValue valueToCompare) 
                => value.CompareTo(valueToCompare) > 0;
        }

        [GenerateRuleExtension("IsLessThan")]
        [DefaultMessage("'{PropertyName}' must be less than {ValueToCompare}, but received {PropertyValue}.")]
        [RulePlaceholder("{ValueToCompare}", "valueToCompare")]
        public class LessThanRule<TPropertyValue> : IValidationRule<TPropertyValue, TPropertyValue>
            where TPropertyValue : IComparable
        {
            public static bool IsValid(TPropertyValue value, TPropertyValue valueToCompare) 
                => value.CompareTo(valueToCompare) < 0;
        }
        """;

    // Define validation rules already generated.
    // This is here so we can simulate scenarios where the validation rules exist in a different project.
    [StringSyntax("CSharp")] private const string ValidationRulesAlreadyGenerated =
        """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;
        using ValiCraft.BuilderTypes;

        namespace Test.Rules;

        // Removing the [GenerateRuleExtension] attribute will not generate the extension method.
        [DefaultMessage("'{PropertyName}' must not be null.")]
        public class NotNullRule<TPropertyValue> : IValidationRule<TPropertyValue?>
        {
            public static bool IsValid(TPropertyValue? value) => value is not null;
        }

        [DefaultMessage("'{PropertyName}' must have a length of {Length}.")]
        [RulePlaceholder("{Length}", "length")]
        public class LengthRule : IValidationRule<string?, int>
        {
            public static bool IsValid(string? value, int length) => value?.Length == length;
        }

        // We copy any attribute besides [GenerateRuleExtension] to the extension method.
        [DefaultMessage("'{PropertyName}' must not be null.")]
        public static class NotNullRuleExtensions
        {
            // We need to define the [MapToValidationRule] attribute so we know how to link the extension method
            // to the validation rule.
            //  - First parameter is the type of the validation rule.
            //  - Second parameter is mapping the generic parameters to the IsValid parameter index.
            [MapToValidationRule(typeof(NotNullRule<>), "<{0}>")]
            public static IValidationRuleBuilderType<TRequest, TPropertyType> IsNotNull<TRequest, TPropertyType>(
                this IBuilderType<TRequest, TPropertyType> builder) where TRequest : class
                => throw new NotImplementedException("Extension methods never get called.");
        }

        [DefaultMessage("'{PropertyName}' must have a length of {Length}.")]
        [RulePlaceholder("{Length}", "length")]
        public static class LengthRuleExtensions
        {
            [MapToValidationRule(typeof(LengthRule), "")]
            public static IValidationRuleBuilderType<TRequest, TPropertyType> HasLength<TRequest, TPropertyType>(
                this IBuilderType<TRequest, TPropertyType> builder, int length) where TRequest : class
                => throw new NotImplementedException("Never gets called");
        }
        """;

    // Define the validator against our request types.
    // This will generate a Validate method in a partial class.
    [StringSyntax("CSharp")] private const string ValidatorsToGenerate = """
                                                                         using Test.Rules;
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;

                                                                         namespace Test.Validators;

                                                                         // This attribute is required to get the validator generating
                                                                         // The validation logic.
                                                                         // Must also have the partial keyword as well.
                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : Validator<Order>
                                                                         {
                                                                             private decimal OrderTotalLimit 
                                                                                 => 999M;

                                                                             private string ShippingReferenceEmptyMessage
                                                                                 => "'{PropertyName}' assigned is invalid.";

                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 builder.Ensure(x => x.OrderNumber)
                                                                                     .IsNotNull()
                                                                                     .IsNotEmpty();
                                                                                 
                                                                                 builder.Ensure(x => x.OrderNumber)
                                                                                     .HasLength(10);
                                                                                     
                                                                                 builder.Ensure(x => x.ShippingReference)
                                                                                     .IsNotNull().WithMessage("'{PropertyName}' needs to be assigned before proceeding").WithPropertyName("Reference")
                                                                                     .IsNotEmpty().WithMessage(ShippingReferenceEmptyMessage);
                                                                                     
                                                                                 builder.Ensure(x => x.OrderTotal)
                                                                                     .IsGreaterThan(0)
                                                                                     .IsLessThan(OrderTotalLimit).WithErrorCode("TotalReached");
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedNotEmptyRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{PropertyName}' must not be empty.")]
            public static class NotEmptyRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.NotEmptyRule), "")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsNotEmpty<TRequest, TPropertyType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder) where TRequest : class
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedGreaterThanRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{PropertyName}' must be greater than {ValueToCompare}, but received {PropertyValue}.")]
            [global::ValiCraft.Attributes.RulePlaceholder("{ValueToCompare}", "valueToCompare")]
            public static class GreaterThanRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.GreaterThanRule<>), "<{0}>")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsGreaterThan<TRequest, TPropertyType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder, TPropertyType valueToCompare) where TRequest : class where TPropertyType : global::System.IComparable
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedLessThanRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{PropertyName}' must be less than {ValueToCompare}, but received {PropertyValue}.")]
            [global::ValiCraft.Attributes.RulePlaceholder("{ValueToCompare}", "valueToCompare")]
            public static class LessThanRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.LessThanRule<>), "<{0}>")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TPropertyType> IsLessThan<TRequest, TPropertyType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TPropertyType> builder, TPropertyType valueToCompare) where TRequest : class where TPropertyType : global::System.IComparable
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedValidators = """
                                                                       // <auto-generated />
                                                                       #nullable enable

                                                                       namespace Test.Validators
                                                                       {
                                                                           public partial class OrderValidator : global::ValiCraft.IValidator<global::Test.Requests.Order>
                                                                           {
                                                                               public global::LitePrimitives.Validation<global::Test.Requests.Order> Validate(global::Test.Requests.Order request)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::LitePrimitives.Error>? errors = null;
                                                                                   
                                                                                   if (!global::Test.Rules.NotNullRule<string>.IsValid(request.OrderNumber))
                                                                                   {
                                                                                       errors ??= new(7);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.NotNullRule<string>), $"'OrderNumber' must not be null."));
                                                                                   }
                                                                                   if (!global::Test.Rules.NotEmptyRule.IsValid(request.OrderNumber))
                                                                                   {
                                                                                       errors ??= new(6);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.NotEmptyRule), $"'OrderNumber' must not be empty."));
                                                                                   }
                                                                                   if (!global::Test.Rules.LengthRule.IsValid(request.OrderNumber, 10))
                                                                                   {
                                                                                       errors ??= new(5);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.LengthRule), $"'OrderNumber' must have a length of 10."));
                                                                                   }
                                                                                   if (!global::Test.Rules.NotNullRule<string>.IsValid(request.ShippingReference))
                                                                                   {
                                                                                       errors ??= new(4);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.NotNullRule<string>), $"'Reference' needs to be assigned before proceeding"));
                                                                                   }
                                                                                   if (!global::Test.Rules.NotEmptyRule.IsValid(request.ShippingReference))
                                                                                   {
                                                                                       errors ??= new(3);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.NotEmptyRule), ShippingReferenceEmptyMessage.Replace("{PropertyName}", "ShippingReference").Replace("{PropertyValue}", request.ShippingReference)));
                                                                                   }
                                                                                   if (!global::Test.Rules.GreaterThanRule<decimal>.IsValid(request.OrderTotal, 0))
                                                                                   {
                                                                                       errors ??= new(2);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation(nameof(global::Test.Rules.GreaterThanRule<decimal>), $"'OrderTotal' must be greater than 0, but received {request.OrderTotal}."));
                                                                                   }
                                                                                   if (!global::Test.Rules.LessThanRule<decimal>.IsValid(request.OrderTotal, OrderTotalLimit))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(global::LitePrimitives.Error.Validation("TotalReached", $"'OrderTotal' must be less than {OrderTotalLimit}, but received {request.OrderTotal}."));
                                                                                   }

                                                                                   return errors is not null
                                                                                       ? global::LitePrimitives.Validation<global::Test.Requests.Order>.Failure(errors)
                                                                                       : global::LitePrimitives.Validation<global::Test.Requests.Order>.Success(request);
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateValidationRuleExtensionsAndValidator()
    {
        using var assertionScope = new AssertionScope();

        var options = IncrementalGeneratorTestOptions.CreateDefault(typeof(Validator<>), typeof(Validation<>));

        new IncrementalGeneratorAdapter(options)
            .GetGeneratedTrees<ValidatorGenerator>(
                [Requests, ValidationRulesToGenerate, ValidationRulesAlreadyGenerated, ValidatorsToGenerate],
                [
                    TrackingSteps.ValidationRuleInfoResultTrackingName,
                    TrackingSteps.ValidatorInfoResultTrackingName
                ])
            .AssertHasNoDiagnostics()
            .AssertOutputs(
                ExpectedNotEmptyRuleExtensions,
                ExpectedGreaterThanRuleExtensions,
                ExpectedLessThanRuleExtensions,
                ExpectedValidators);
    }
}