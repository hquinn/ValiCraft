using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.GeneratorTests;

public class PolymorphicRuleChain_MultipleTypesTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
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
                                                             
                                                             public class CryptoPayment : Payment
                                                             {
                                                                 public required string WalletAddress { get; set; }
                                                             }
                                                             
                                                             public class PayPalPayment : Payment
                                                             {
                                                                 public required string Email { get; set; }
                                                             }
                                                             
                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public Payment? Payment { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputValidatorsToGenerate = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator(
                                                                             IValidator<CreditCardPayment> creditCardValidator,
                                                                             IValidator<CryptoPayment> cryptoValidator) : Validator<Order>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 builder.Polymorphic(x => x.Payment)
                                                                                    .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
                                                                                    .WhenType<CryptoPayment>().ValidateWith(cryptoValidator)
                                                                                    .WhenType<PayPalPayment>().Allow()
                                                                                    .Otherwise().Allow();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedValidators = """
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
                                                                           /// Generated validator for <see cref="global::Test.Requests.Order"/>.
                                                                           /// </summary>
                                                                           public partial class OrderValidator : global::ValiCraft.IValidator<global::Test.Requests.Order>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order> Validate(global::Test.Requests.Order request)
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
                                                                               public global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;
                                                                       
                                                                                   if (request.Payment is global::Test.Requests.CreditCardPayment typedCreditCardPayment)
                                                                                   {
                                                                                       var errors1 = creditCardValidator.ValidateToList(typedCreditCardPayment, $"{inheritedTargetPath}Payment.");
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
                                                                                   else if (request.Payment is global::Test.Requests.CryptoPayment typedCryptoPayment)
                                                                                   {
                                                                                       var errors1 = cryptoValidator.ValidateToList(typedCryptoPayment, $"{inheritedTargetPath}Payment.");
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
                                                                                   else if (request.Payment is global::Test.Requests.PayPalPayment typedPayPalPayment)
                                                                                   {
                                                                                       // Allow - no validation needed
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       // Allow - no validation needed
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputValidatorsToGenerate], 
            outputs: [ExpectedValidators],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
