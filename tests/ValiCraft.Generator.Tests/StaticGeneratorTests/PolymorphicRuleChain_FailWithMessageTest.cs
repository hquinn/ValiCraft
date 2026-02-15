using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticGeneratorTests;

public class PolymorphicRuleChain_FailWithMessageTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public abstract class Payment
                                                             {
                                                                 public decimal Amount { get; set; }
                                                             }
                                                             
                                                             public class CreditCardPayment : Payment
                                                             {
                                                                 public required string CardNumber { get; set; }
                                                             }
                                                             
                                                             public class CashPayment : Payment
                                                             {
                                                             }
                                                             
                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public Payment? Payment { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputCreditCardValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class CreditCardPaymentValidator : StaticValidator<CreditCardPayment>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<CreditCardPayment> builder)
                                                                             {
                                                                                 builder.Ensure(x => x.CardNumber)
                                                                                    .IsNotNullOrWhiteSpace();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string InputOrderValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : StaticValidator<Order>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 builder.Polymorphic(x => x.Payment)
                                                                                    .WhenType<CreditCardPayment>().Validate<CreditCardPaymentValidator>()
                                                                                    .WhenType<CashPayment>().Fail("Payment doesn't support cash")
                                                                                    .Otherwise().Fail();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedCreditCardValidator = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Requests;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.BuilderTypes;
                                                                       using ErrorCraft;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           /// <summary>
                                                                           /// Generated static validator for <see cref="global::Test.Requests.CreditCardPayment"/>.
                                                                           /// </summary>
                                                                           public partial class CreditCardPaymentValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.CreditCardPayment>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment> Validate(global::Test.Requests.CreditCardPayment request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "CreditCardPaymentErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CreditCardPayment request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CreditCardPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.CreditCardPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                   if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.CardNumber))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                           Message = $"Card Number must not be null or contain only whitespace.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Card Number",
                                                                                           TargetPath = $"{inheritedTargetPath}CardNumber",
                                                                                           AttemptedValue = request.CardNumber,
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [StringSyntax("CSharp")] private const string ExpectedOrderValidator = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Requests;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.BuilderTypes;
                                                                       using ErrorCraft;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           /// <summary>
                                                                           /// Generated static validator for <see cref="global::Test.Requests.Order"/>.
                                                                           /// </summary>
                                                                           public partial class OrderValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.Order>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order> Validate(global::Test.Requests.Order request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "OrderErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;
                                                                       
                                                                                   if (request.Payment is global::Test.Requests.CreditCardPayment typedCreditCardPayment)
                                                                                   {
                                                                                       var errors1 = global::Test.Validators.CreditCardPaymentValidator.ValidateToList(typedCreditCardPayment, $"{inheritedTargetPath}Payment.");
                                                                                       if (errors1.Count != 0)
                                                                                       {
                                                                                           if (errors is null)
                                                                                           {
                                                                                               errors = new(errors1);
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               errors.AddRange(errors1);
                                                                                           }
                                                                                       }
                                                                                   }
                                                                                   else if (request.Payment is global::Test.Requests.CashPayment typedCashPayment)
                                                                                   {
                                                                                       (errors ??= []).Add(new global::ErrorCraft.ValidationError<global::Test.Requests.Payment?>
                                                                                       {
                                                                                           Code = "PaymentUnsupportedType",
                                                                                           Message = $"Payment doesn't support cash",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Payment",
                                                                                           TargetPath = $"{inheritedTargetPath}Payment",
                                                                                           AttemptedValue = request.Payment
                                                                                       });
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       (errors ??= []).Add(new global::ErrorCraft.ValidationError<global::Test.Requests.Payment?>
                                                                                       {
                                                                                           Code = "PaymentUnsupportedType",
                                                                                           Message = $"Payment is not a supported type.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Payment",
                                                                                           TargetPath = $"{inheritedTargetPath}Payment",
                                                                                           AttemptedValue = request.Payment
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateStaticValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputCreditCardValidator, InputOrderValidator], 
            outputs: [ExpectedCreditCardValidator, ExpectedOrderValidator],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
