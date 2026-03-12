using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.AsyncGeneratorTests;

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
                                                             
                                                             public class BankTransferPayment : Payment
                                                             {
                                                                 public required string AccountNumber { get; set; }
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
                                                                             IAsyncValidator<CreditCardPayment> creditCardValidator,
                                                                             IAsyncValidator<CryptoPayment> cryptoValidator,
                                                                             IAsyncValidator<BankTransferPayment> bankTransferValidator) : AsyncValidator<Order>
                                                                         {
                                                                             protected override void DefineRules(IAsyncValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 builder.Polymorphic(x => x.Payment)
                                                                                    .WhenType<CreditCardPayment>().ValidateWith(creditCardValidator)
                                                                                    .WhenType<CryptoPayment>().ValidateWith(cryptoValidator)
                                                                                    .WhenType<BankTransferPayment>().ValidateWith(bankTransferValidator)
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
                                                                           /// Generated async validator for <see cref="global::Test.Requests.Order"/>.
                                                                           /// </summary>
                                                                           public partial class OrderValidator : global::ValiCraft.IAsyncValidator<global::Test.Requests.Order>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public async global::System.Threading.Tasks.Task<global::ErrorCraft.ValidationErrors?> ValidateAsync(global::Test.Requests.Order request, global::System.Threading.CancellationToken cancellationToken = default)
                                                                               {
                                                                                   var errors = await RunValidationLogicAsync(request, null, cancellationToken);

                                                                                   if (errors is null) return null;

                                                                                   return new global::ErrorCraft.ValidationErrors
                                                                                   {
                                                                                       Code = "OrderErrors",
                                                                                       Message = "One or more validation errors occurred.",
                                                                                       Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                       Errors = errors
                                                                                   };
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public async global::System.Threading.Tasks.Task<global::ErrorCraft.ValidationErrors?> ValidateAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken = default)
                                                                               {
                                                                                   var errors = await RunValidationLogicAsync(request, inheritedTargetPath, cancellationToken);

                                                                                   if (errors is null) return null;

                                                                                   return new global::ErrorCraft.ValidationErrors
                                                                                   {
                                                                                       Code = "OrderErrors",
                                                                                       Message = "One or more validation errors occurred.",
                                                                                       Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                       Errors = errors
                                                                                   };
                                                                               }

                                                                               private async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>?> RunValidationLogicAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;
                                                                       
                                                                                   if (request.Payment is global::Test.Requests.CreditCardPayment typedCreditCardPayment)
                                                                                   {
                                                                                       var errors1 = await creditCardValidator.ValidateAsync(typedCreditCardPayment, $"{inheritedTargetPath}Payment.", cancellationToken);
                                                                                       if (errors1 is not null)
                                                                                       {
                                                                                           if (errors is null)
                                                                                           {
                                                                                               errors = new(errors1.Errors);
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               errors.AddRange(errors1.Errors);
                                                                                           }
                                                                                       }
                                                                                   }
                                                                                   else if (request.Payment is global::Test.Requests.CryptoPayment typedCryptoPayment)
                                                                                   {
                                                                                       var errors1 = await cryptoValidator.ValidateAsync(typedCryptoPayment, $"{inheritedTargetPath}Payment.", cancellationToken);
                                                                                       if (errors1 is not null)
                                                                                       {
                                                                                           if (errors is null)
                                                                                           {
                                                                                               errors = new(errors1.Errors);
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               errors.AddRange(errors1.Errors);
                                                                                           }
                                                                                       }
                                                                                   }
                                                                                   else if (request.Payment is global::Test.Requests.BankTransferPayment typedBankTransferPayment)
                                                                                   {
                                                                                       var errors1 = await bankTransferValidator.ValidateAsync(typedBankTransferPayment, $"{inheritedTargetPath}Payment.", cancellationToken);
                                                                                       if (errors1 is not null)
                                                                                       {
                                                                                           if (errors is null)
                                                                                           {
                                                                                               errors = new(errors1.Errors);
                                                                                           }
                                                                                           else
                                                                                           {
                                                                                               errors.AddRange(errors1.Errors);
                                                                                           }
                                                                                       }
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
