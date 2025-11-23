using System.Diagnostics.CodeAnalysis;
using MonadCraft;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class ValiCraftGeneratorTests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    // Define request types which will be used on our validators
    [StringSyntax("CSharp")] private const string InputRequests = """
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
                                                                 public required List<Discount> Discounts { get; set; }
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
                                                                 public required List<Discount> Discounts { get; set; }
                                                             }
                                                             
                                                             public class Discount
                                                             {
                                                                 public required string Code { get; set; }
                                                                 public decimal Amount { get; set; }
                                                             }
                                                             """;

    // Define validation rules which are annotated with [GenerateRuleExtension("...")].
    // When annotated, it will generate extension methods which the validators can use to generate validation code.
    [StringSyntax("CSharp")] private const string InputValidationRulesToGenerate =
        """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;

        namespace Test.Rules;

        // IsNotEmpty will be the name of the extension method.
        [GenerateRuleExtension("IsNotEmpty")]
        // This is the default message that will be displayed
        // when the validation rule is not valid.
        [DefaultMessage("'{TargetName}' must not be empty.")]
        public class NotEmptyRule : IValidationRule<string?>
        {
            public static bool IsValid(string? value) => !string.IsNullOrEmpty(value);
        }

        [GenerateRuleExtension("IsGreaterThan")]
        // {TargetName} and {TargetValue} are in-built placeholders.
        // {ValueToCompare} is a user-defined placeholder.
        [DefaultMessage("'{TargetName}' must be greater than {ValueToCompare}, but received {TargetValue}.")]
        // Create a user-defined placeholder, which matches the 'valueToCompare' parameter value.
        [RulePlaceholder("{ValueToCompare}", "valueToCompare")]
        public class GreaterThanRule<TTargetValue> : IValidationRule<TTargetValue, TTargetValue>
            where TTargetValue : IComparable
        {
            public static bool IsValid(TTargetValue value, TTargetValue valueToCompare) 
                => value.CompareTo(valueToCompare) > 0;
        }

        [GenerateRuleExtension("IsLessThan")]
        [DefaultMessage("'{TargetName}' must be less than {ValueToCompare}, but received {TargetValue}.")]
        [RulePlaceholder("{ValueToCompare}", "valueToCompare")]
        public class LessThanRule<TTargetValue> : IValidationRule<TTargetValue, TTargetValue>
            where TTargetValue : IComparable
        {
            public static bool IsValid(TTargetValue value, TTargetValue valueToCompare) 
                => value.CompareTo(valueToCompare) < 0;
        }
        
        [GenerateRuleExtension("IsPredicate")]
        [DefaultMessage("{TargetName} doesn't satisfy the condition")]
        [DefaultErrorCode("CustomErrorCode")]
        public class Predicate<TTargetType> : IValidationRule<TTargetType?, Func<TTargetType?, bool>>
        {
            public static bool IsValid(TTargetType? property, Func<TTargetType?, bool> predicate)
            {
                return predicate(property);
            }
        }
        """;

    // Define validation rules already generated.
    // This is here so we can simulate scenarios where the validation rules exist in a different project.
    [StringSyntax("CSharp")] private const string InputValidationRulesAlreadyGenerated =
        """
        using System;
        using ValiCraft;
        using ValiCraft.Attributes;
        using ValiCraft.BuilderTypes;

        namespace Test.Rules;

        // Removing the [GenerateRuleExtension] attribute will not generate the extension method.
        [DefaultMessage("'{TargetName}' must not be null.")]
        public class NotNullRule<TTargetValue> : IValidationRule<TTargetValue?>
        {
            public static bool IsValid(TTargetValue? value) => value is not null;
        }

        [DefaultMessage("'{TargetName}' must have a length of {Length}.")]
        [RulePlaceholder("{Length}", "length")]
        public class LengthRule : IValidationRule<string?, int>
        {
            public static bool IsValid(string? value, int length) => value?.Length == length;
        }

        // We copy any attribute besides [GenerateRuleExtension] to the extension method.
        [DefaultMessage("'{TargetName}' must not be null.")]
        public static class NotNullRuleExtensions
        {
            // We need to define the [MapToValidationRule] attribute so we know how to link the extension method
            // to the validation rule.
            //  - First parameter is the type of the validation rule.
            //  - Second parameter is mapping the generic parameters to the IsValid parameter index.
            [MapToValidationRule(typeof(NotNullRule<>), "<{0}>")]
            public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
                this IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                => throw new NotImplementedException("Extension methods never get called.");
        }

        [DefaultMessage("'{TargetName}' must have a length of {Length}.")]
        [RulePlaceholder("{Length}", "length")]
        public static class LengthRuleExtensions
        {
            [MapToValidationRule(typeof(LengthRule), "")]
            public static IValidationRuleBuilderType<TRequest, TTargetType> HasLength<TRequest, TTargetType>(
                this IBuilderType<TRequest, TTargetType> builder, int length) where TRequest : class
                => throw new NotImplementedException("Never gets called");
        }
        """;

    // Define the validator against our request types.
    // This will generate a Validate method in a partial class.
    [StringSyntax("CSharp")] private const string InputValidatorsToGenerate = """
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
                                                                             private readonly IValidator<Customer> _customerValidator;
                                                                             private readonly IValidator<LineItem> _lineItemValidator;
                                                                             
                                                                             public OrderValidator(IValidator<Customer> customerValidator, IValidator<LineItem> lineItemValidator)
                                                                             {
                                                                                 _customerValidator = customerValidator;
                                                                                 _lineItemValidator = lineItemValidator;
                                                                             }
                                                                             
                                                                             private decimal OrderTotalLimit 
                                                                                 => 999M;

                                                                             private string ShippingReferenceEmptyMessage
                                                                                 => "'{TargetName}' assigned is invalid.";

                                                                             private bool OrderNumberIsNotNull(Order order)
                                                                             {
                                                                                 return order.OrderNumber is not null;
                                                                             }

                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> orderBuilder)
                                                                             {
                                                                                 orderBuilder.Ensure(o => o.Customer.MiddleName)
                                                                                     .IsNotNull();

                                                                                 orderBuilder.Ensure(o => o.Customer)
                                                                                     .IsNotNull().If(o => { return o.OrderTotal != 0; });
                                                                                 
                                                                                 orderBuilder.Ensure(o => o.Customer)
                                                                                     .IsNotNull().If(o => o.OrderTotal != 0);

                                                                                 orderBuilder.If(order => { return order.OrderTotal != 0; }, b =>
                                                                                 {
                                                                                     b.Ensure(o => o.Customer).IsNotNull();            
                                                                                 });

                                                                                 orderBuilder.If(order => order.OrderTotal != 0, b =>
                                                                                 {
                                                                                     b.Ensure(o => o.Customer).IsNotNull();            
                                                                                 });
                                                                                 
                                                                                 orderBuilder.EnsureEach(o => o.Discounts, OnFailureMode.Halt, discountBuilder =>
                                                                                 {
                                                                                     discountBuilder.Ensure(d => d.Code)
                                                                                         .IsNotEmpty();

                                                                                     discountBuilder.Ensure(d => d.Amount)
                                                                                         .IsGreaterThan(5);
                                                                                 });

                                                                                 orderBuilder.Ensure(o => o)
                                                                                     .Must(OrderNumberIsNotNull);

                                                                                 orderBuilder.Ensure(o => o)
                                                                                     .Must(o => OrderNumberIsNotNull(o));

                                                                                 orderBuilder.Ensure(o => o)
                                                                                     .Must((o) => { return o.OrderNumber is not null; });

                                                                                 orderBuilder.Ensure(o => o)
                                                                                     .Must(o => o.OrderNumber is not null);

                                                                                 orderBuilder.Ensure(o => o)
                                                                                     .IsPredicate(o => o.OrderNumber is not null);

                                                                                 orderBuilder.EnsureEach(o => o.LineItems)
                                                                                     .ValidateWith(_lineItemValidator);

                                                                                 orderBuilder.WithOnFailure(OnFailureMode.Halt, b =>
                                                                                 {
                                                                                     b.Ensure(o => o.Customer)
                                                                                         .ValidateWith(_customerValidator);

                                                                                     b.EnsureEach(o => o.LineItems, lineItemBuilder =>
                                                                                     {
                                                                                         lineItemBuilder.WithOnFailure(OnFailureMode.Continue, l =>
                                                                                         {
                                                                                             l.Ensure(li => li.SKU)
                                                                                                 .IsNotNull()
                                                                                                 .IsNotEmpty()
                                                                                                 ;

                                                                                             l.Ensure(li => li.Quantity)
                                                                                                 .IsGreaterThan(0);
                                                                                         });    

                                                                                         lineItemBuilder.EnsureEach(li => li.Discounts, discountBuilder =>
                                                                                         {
                                                                                             discountBuilder.Ensure(li => li.Amount)
                                                                                                 .IsGreaterThan(10);
                                                                                         });
                                                                                     });
                                                                                 });

                                                                                 orderBuilder.Ensure((o) => o.OrderNumber)
                                                                                     .IsNotNull()
                                                                                     .IsNotEmpty();

                                                                                 orderBuilder.Ensure(o => o.OrderNumber)
                                                                                     .HasLength(10);

                                                                                 orderBuilder.Ensure(o => o.ShippingReference, OnFailureMode.Halt)
                                                                                     .IsNotNull().WithMessage("'{TargetName}' needs to be assigned before proceeding").WithTargetName("ReferenceNo")
                                                                                     .IsNotEmpty().WithMessage(ShippingReferenceEmptyMessage);

                                                                                 orderBuilder.Ensure(o => o.OrderTotal)
                                                                                     .IsGreaterThan(0M)
                                                                                     .IsLessThan(OrderTotalLimit).WithErrorCode("TotalReached");
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedNotEmptyRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{TargetName}' must not be empty.")]
            public static class NotEmptyRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.NotEmptyRule), "")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TTargetType> IsNotEmpty<TRequest, TTargetType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TTargetType> builder) where TRequest : class
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedGreaterThanRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{TargetName}' must be greater than {ValueToCompare}, but received {TargetValue}.")]
            [global::ValiCraft.Attributes.RulePlaceholder("{ValueToCompare}", "valueToCompare")]
            public static class GreaterThanRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.GreaterThanRule<>), "<{0}>")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TTargetType> IsGreaterThan<TRequest, TTargetType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TTargetType> builder, TTargetType valueToCompare) where TRequest : class where TTargetType : global::System.IComparable
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedLessThanRuleExtensions = """
        // <auto-generated />
        #nullable enable

        namespace Test.Rules
        {
            [global::ValiCraft.Attributes.DefaultMessage("'{TargetName}' must be less than {ValueToCompare}, but received {TargetValue}.")]
            [global::ValiCraft.Attributes.RulePlaceholder("{ValueToCompare}", "valueToCompare")]
            public static class LessThanRuleExtensions
            {
                [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.LessThanRule<>), "<{0}>")]
                public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TTargetType> IsLessThan<TRequest, TTargetType>(
                    this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TTargetType> builder, TTargetType valueToCompare) where TRequest : class where TTargetType : global::System.IComparable
                    => throw new global::System.NotImplementedException("Never gets called");
            }
        }
        """;

    [StringSyntax("CSharp")] private const string ExpectedPredicateExtensions = """
                                                                           // <auto-generated />
                                                                           #nullable enable

                                                                           namespace Test.Rules
                                                                           {
                                                                               [global::ValiCraft.Attributes.DefaultMessage("{TargetName} doesn't satisfy the condition")]
                                                                               [global::ValiCraft.Attributes.DefaultErrorCode("CustomErrorCode")]
                                                                               public static class PredicateExtensions
                                                                               {
                                                                                   [global::ValiCraft.Attributes.MapToValidationRule(typeof(global::Test.Rules.Predicate<>), "<{0}>")]
                                                                                   public static global::ValiCraft.BuilderTypes.IValidationRuleBuilderType<TRequest, TTargetType> IsPredicate<TRequest, TTargetType>(
                                                                                       this global::ValiCraft.BuilderTypes.IBuilderType<TRequest, TTargetType> builder, System.Func<TTargetType?, bool> predicate) where TRequest : class
                                                                                       => throw new global::System.NotImplementedException("Never gets called");
                                                                               }
                                                                           }
                                                                           """;
/*

*/
    [StringSyntax("CSharp")] private const string ExpectedValidators = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Rules;
                                                                       using Test.Requests;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.BuilderTypes;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           public partial class OrderValidator : global::ValiCraft.IValidator<global::Test.Requests.Order>
                                                                           {
                                                                               public global::MonadCraft.Result<global::System.Collections.Generic.IReadOnlyList<global::ValiCraft.IValidationError>, global::Test.Requests.Order> Validate(global::Test.Requests.Order request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::System.Collections.Generic.IReadOnlyList<global::ValiCraft.IValidationError>, global::Test.Requests.Order>.Failure(errors)
                                                                                       : global::MonadCraft.Result<global::System.Collections.Generic.IReadOnlyList<global::ValiCraft.IValidationError>, global::Test.Requests.Order>.Success(request);
                                                                               }

                                                                               public global::System.Collections.Generic.IReadOnlyList<global::ValiCraft.IValidationError> ValidateToList(global::Test.Requests.Order request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public global::System.Collections.Generic.IReadOnlyList<global::ValiCraft.IValidationError> ValidateToList(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private global::System.Collections.Generic.List<global::ValiCraft.IValidationError>? RunValidationLogic(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ValiCraft.IValidationError>? errors = null;
                                                                       
                                                                                   if (!global::Test.Rules.NotNullRule<string?>.IsValid(request.Customer.MiddleName))
                                                                                   {
                                                                                       errors ??= new(25);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string?>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotNullRule<string?>),
                                                                                           Message = $"'Middle Name' must not be null.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Middle Name",
                                                                                           TargetPath = $"{inheritedTargetPath}Customer.MiddleName",
                                                                                           AttemptedValue = request.Customer.MiddleName,
                                                                                       });
                                                                                   }
                                                                       
                                                                                   bool __ifRule_24(global::Test.Requests.Order o)
                                                                                   { return o.OrderTotal != 0; }
                                                                                   if (__ifRule_24(request) && !global::Test.Rules.NotNullRule<global::Test.Requests.Customer>.IsValid(request.Customer))
                                                                                   {
                                                                                       errors ??= new(24);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Customer>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotNullRule<global::Test.Requests.Customer>),
                                                                                           Message = $"'Customer' must not be null.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Customer",
                                                                                           TargetPath = $"{inheritedTargetPath}Customer",
                                                                                           AttemptedValue = request.Customer,
                                                                                       });
                                                                                   }

                                                                                   if (request.OrderTotal != 0 && !global::Test.Rules.NotNullRule<global::Test.Requests.Customer>.IsValid(request.Customer))
                                                                                   {
                                                                                       errors ??= new(23);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Customer>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotNullRule<global::Test.Requests.Customer>),
                                                                                           Message = $"'Customer' must not be null.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Customer",
                                                                                           TargetPath = $"{inheritedTargetPath}Customer",
                                                                                           AttemptedValue = request.Customer,
                                                                                       });
                                                                                   }

                                                                                   bool __ifRuleChain_22(global::Test.Requests.Order order)
                                                                                   { return order.OrderTotal != 0; }
                                                                                   if (__ifRuleChain_22(request))
                                                                                   {
                                                                                       if (!global::Test.Rules.NotNullRule<global::Test.Requests.Customer>.IsValid(request.Customer))
                                                                                       {
                                                                                           errors ??= new(22);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Customer>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.NotNullRule<global::Test.Requests.Customer>),
                                                                                               Message = $"'Customer' must not be null.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "Customer",
                                                                                               TargetPath = $"{inheritedTargetPath}Customer",
                                                                                               AttemptedValue = request.Customer,
                                                                                           });
                                                                                       }
                                                                                   }

                                                                                   if (request.OrderTotal != 0)
                                                                                   {
                                                                                       if (!global::Test.Rules.NotNullRule<global::Test.Requests.Customer>.IsValid(request.Customer))
                                                                                       {
                                                                                           errors ??= new(21);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Customer>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.NotNullRule<global::Test.Requests.Customer>),
                                                                                               Message = $"'Customer' must not be null.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "Customer",
                                                                                               TargetPath = $"{inheritedTargetPath}Customer",
                                                                                               AttemptedValue = request.Customer,
                                                                                           });
                                                                                       }
                                                                                   }

                                                                                   var index20 = 0;
                                                                                   foreach (var element in request.Discounts)
                                                                                   {
                                                                                       if (!global::Test.Rules.NotEmptyRule.IsValid(element.Code))
                                                                                       {
                                                                                           errors ??= new(20);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.NotEmptyRule),
                                                                                               Message = $"'Code' must not be empty.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "Code",
                                                                                               TargetPath = $"{inheritedTargetPath}Discounts[{index20}].Code",
                                                                                               AttemptedValue = element.Code,
                                                                                           });
                                                                                           goto HaltValidation_20;
                                                                                       }
                                                                                       if (!global::Test.Rules.GreaterThanRule<decimal>.IsValid(element.Amount, 5))
                                                                                       {
                                                                                           errors ??= new(19);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<decimal>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.GreaterThanRule<decimal>),
                                                                                               Message = $"'Amount' must be greater than 5, but received {element.Amount}.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "Amount",
                                                                                               TargetPath = $"{inheritedTargetPath}Discounts[{index20}].Amount",
                                                                                               AttemptedValue = element.Amount,
                                                                                           });
                                                                                           goto HaltValidation_20;
                                                                                       }
                                                                                       index20++;
                                                                                   }

                                                                                   HaltValidation_20:
                                                                       
                                                                                   if (!OrderNumberIsNotNull(request))
                                                                                   {
                                                                                       errors ??= new(18);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Order>
                                                                                       {
                                                                                           Code = "Must",
                                                                                           Message = $"'Order' doesn't satisfy the condition",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order",
                                                                                           TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "Order" : null)}",
                                                                                           AttemptedValue = request,
                                                                                       });
                                                                                   }

                                                                                   if (!OrderNumberIsNotNull(request))
                                                                                   {
                                                                                       errors ??= new(17);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Order>
                                                                                       {
                                                                                           Code = "Must",
                                                                                           Message = $"'Order' doesn't satisfy the condition",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order",
                                                                                           TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "Order" : null)}",
                                                                                           AttemptedValue = request,
                                                                                       });
                                                                                   }

                                                                                   bool __must_16(global::Test.Requests.Order o)
                                                                                   { return o.OrderNumber is not null; }
                                                                                   if (!__must_16(request))
                                                                                   {
                                                                                       errors ??= new(16);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Order>
                                                                                       {
                                                                                           Code = "Must",
                                                                                           Message = $"'Order' doesn't satisfy the condition",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order",
                                                                                           TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "Order" : null)}",
                                                                                           AttemptedValue = request,
                                                                                       });
                                                                                   }

                                                                                   if (!(request.OrderNumber is not null))
                                                                                   {
                                                                                       errors ??= new(15);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Order>
                                                                                       {
                                                                                           Code = "Must",
                                                                                           Message = $"'Order' doesn't satisfy the condition",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order",
                                                                                           TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "Order" : null)}",
                                                                                           AttemptedValue = request,
                                                                                       });
                                                                                   }
    
                                                                                   if (!global::Test.Rules.Predicate<global::Test.Requests.Order>.IsValid(request, o => o.OrderNumber is not null))
                                                                                   {
                                                                                       errors ??= new(14);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<global::Test.Requests.Order>
                                                                                       {
                                                                                           Code = "CustomErrorCode",
                                                                                           Message = $"Order doesn't satisfy the condition",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order",
                                                                                           TargetPath = $"{inheritedTargetPath}{(inheritedTargetPath is not null ? "Order" : null)}",
                                                                                           AttemptedValue = request,
                                                                                       });
                                                                                   }

                                                                                   var index13 = 0;
                                                                                   foreach (var element in request.LineItems)
                                                                                   {
                                                                                       var errors13 = _lineItemValidator.ValidateToList(element, $"{inheritedTargetPath}LineItems[{index13}].");
                                                                                       if (errors13.Count != 0)
                                                                                       {
                                                                                           if (errors is null)
                                                                                           {
                                                                                               errors = new(errors13);
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               errors.AddRange(errors13);
                                                                                           }
                                                                                       }
                                                                                       index13++;
                                                                                   }
                                                                       
                                                                                   var errors12 = _customerValidator.ValidateToList(request.Customer, $"{inheritedTargetPath}Customer.");
                                                                                   if (errors12.Count != 0)
                                                                                   {
                                                                                       if (errors is null)
                                                                                       {
                                                                                           errors = new(errors12);
                                                                                           goto HaltValidation_12;
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                           errors.AddRange(errors12);
                                                                                           goto HaltValidation_12;
                                                                                       }
                                                                                   }
                                                                                   var index11 = 0;
                                                                                   foreach (var element in request.LineItems)
                                                                                   {
                                                                                       if (!global::Test.Rules.NotNullRule<string>.IsValid(element.SKU))
                                                                                       {
                                                                                           errors ??= new(11);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.NotNullRule<string>),
                                                                                               Message = $"'SKU' must not be null.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "SKU",
                                                                                               TargetPath = $"{inheritedTargetPath}LineItems[{index11}].SKU",
                                                                                               AttemptedValue = element.SKU,
                                                                                           });
                                                                                       }
                                                                                       if (!global::Test.Rules.NotEmptyRule.IsValid(element.SKU))
                                                                                       {
                                                                                           errors ??= new(10);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.NotEmptyRule),
                                                                                               Message = $"'SKU' must not be empty.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "SKU",
                                                                                               TargetPath = $"{inheritedTargetPath}LineItems[{index11}].SKU",
                                                                                               AttemptedValue = element.SKU,
                                                                                           });
                                                                                       }
                                                                                       if (!global::Test.Rules.GreaterThanRule<int>.IsValid(element.Quantity, 0))
                                                                                       {
                                                                                           errors ??= new(9);
                                                                                           errors.Add(new global::ValiCraft.ValidationError<int>
                                                                                           {
                                                                                               Code = nameof(global::Test.Rules.GreaterThanRule<int>),
                                                                                               Message = $"'Quantity' must be greater than 0, but received {element.Quantity}.",
                                                                                               Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                               TargetName = "Quantity",
                                                                                               TargetPath = $"{inheritedTargetPath}LineItems[{index11}].Quantity",
                                                                                               AttemptedValue = element.Quantity,
                                                                                           });
                                                                                       }
                                                                                       var index8 = 0;
                                                                                       foreach (var subElement in element.Discounts)
                                                                                       {
                                                                                           if (!global::Test.Rules.GreaterThanRule<decimal>.IsValid(subElement.Amount, 10))
                                                                                           {
                                                                                               errors ??= new(8);
                                                                                               errors.Add(new global::ValiCraft.ValidationError<decimal>
                                                                                               {
                                                                                                   Code = nameof(global::Test.Rules.GreaterThanRule<decimal>),
                                                                                                   Message = $"'Amount' must be greater than 10, but received {subElement.Amount}.",
                                                                                                   Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                                   TargetName = "Amount",
                                                                                                   TargetPath = $"{inheritedTargetPath}LineItems[{index11}].Discounts[{index8}].Amount",
                                                                                                   AttemptedValue = subElement.Amount,
                                                                                               });
                                                                                               goto HaltValidation_12;
                                                                                           }
                                                                                           index8++;
                                                                                       }
                                                                                       index11++;
                                                                                   }

                                                                                   HaltValidation_12:

                                                                                   if (!global::Test.Rules.NotNullRule<string>.IsValid(request.OrderNumber))
                                                                                   {
                                                                                       errors ??= new(7);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotNullRule<string>),
                                                                                           Message = $"'Order Number' must not be null.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order Number",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderNumber",
                                                                                           AttemptedValue = request.OrderNumber,
                                                                                       });
                                                                                   }
                                                                                   if (!global::Test.Rules.NotEmptyRule.IsValid(request.OrderNumber))
                                                                                   {
                                                                                       errors ??= new(6);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotEmptyRule),
                                                                                           Message = $"'Order Number' must not be empty.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order Number",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderNumber",
                                                                                           AttemptedValue = request.OrderNumber,
                                                                                       });
                                                                                   }
                                                                       
                                                                                   if (!global::Test.Rules.LengthRule.IsValid(request.OrderNumber, 10))
                                                                                   {
                                                                                       errors ??= new(5);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.LengthRule),
                                                                                           Message = $"'Order Number' must have a length of 10.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order Number",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderNumber",
                                                                                           AttemptedValue = request.OrderNumber,
                                                                                       });
                                                                                   }
                                                                       
                                                                                   if (!global::Test.Rules.NotNullRule<string?>.IsValid(request.ShippingReference))
                                                                                   {
                                                                                       errors ??= new(4);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string?>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotNullRule<string?>),
                                                                                           Message = $"'ReferenceNo' needs to be assigned before proceeding",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "ReferenceNo",
                                                                                           TargetPath = $"{inheritedTargetPath}ShippingReference",
                                                                                           AttemptedValue = request.ShippingReference,
                                                                                       });
                                                                                   }
                                                                                   else if (!global::Test.Rules.NotEmptyRule.IsValid(request.ShippingReference))
                                                                                   {
                                                                                       errors ??= new(3);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<string?>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.NotEmptyRule),
                                                                                           Message = ShippingReferenceEmptyMessage.Replace("{TargetName}", "Shipping Reference").Replace("{TargetValue}", request.ShippingReference),
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Shipping Reference",
                                                                                           TargetPath = $"{inheritedTargetPath}ShippingReference",
                                                                                           AttemptedValue = request.ShippingReference,
                                                                                       });
                                                                                   }
                                                                       
                                                                                   if (!global::Test.Rules.GreaterThanRule<decimal>.IsValid(request.OrderTotal, 0M))
                                                                                   {
                                                                                       errors ??= new(2);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<decimal>
                                                                                       {
                                                                                           Code = nameof(global::Test.Rules.GreaterThanRule<decimal>),
                                                                                           Message = $"'Order Total' must be greater than 0, but received {request.OrderTotal}.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order Total",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderTotal",
                                                                                           AttemptedValue = request.OrderTotal,
                                                                                       });
                                                                                   }
                                                                                   if (!global::Test.Rules.LessThanRule<decimal>.IsValid(request.OrderTotal, OrderTotalLimit))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ValiCraft.ValidationError<decimal>
                                                                                       {
                                                                                           Code = "TotalReached",
                                                                                           Message = $"'Order Total' must be less than {OrderTotalLimit}, but received {request.OrderTotal}.",
                                                                                           Severity = global::MonadCraft.Errors.ErrorSeverity.Error,
                                                                                           TargetName = "Order Total",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderTotal",
                                                                                           AttemptedValue = request.OrderTotal,
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateValidationRuleExtensionsAndValidator()
    {
        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.ValidationRuleResultTrackingName, TrackingSteps.ValidatorResultTrackingName], 
            inputs: [InputRequests, InputValidationRulesToGenerate, InputValidationRulesAlreadyGenerated, InputValidatorsToGenerate], 
            outputs: [ExpectedNotEmptyRuleExtensions, ExpectedGreaterThanRuleExtensions, ExpectedLessThanRuleExtensions, ExpectedPredicateExtensions, ExpectedValidators],
            diagnostics: []);
    }
}