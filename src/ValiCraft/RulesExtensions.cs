using System.Collections;
using System.Text.RegularExpressions;
using ValiCraft.Attributes;
using ValiCraft.BuilderTypes;

namespace ValiCraft;

/// <summary>
/// Provides extension methods for building validation rules related to various constraints.
/// </summary>
public static class RulesExtensions
{
   /// <summary>
   /// Validates that a DateTime value is in the future (after the specified reference time).
   /// This allows for testable date validation by providing a reference point.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be after {ReferenceDate}")]
   [RulePlaceholder("{ReferenceDate}", "referenceDate")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.After))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsAfter<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      DateTime referenceDate)
      where TRequest : class
   {
      return builder.Is(Rules.After, referenceDate);
   }

   /// <summary>
   /// Validates that a string contains only alphanumeric characters (letters and digits).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain only letters and numbers")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.AlphaNumeric))]
   public static IValidationRuleBuilderType<TRequest, string> IsAlphaNumeric<TRequest>(
      this IBuilderType<TRequest, string> builder)
      where TRequest : class
   {
      return builder.Is(Rules.AlphaNumeric);
   }

   /// <summary>
   /// Validates that a DateTime value is at or after the specified reference time.
   /// This allows for testable date validation by providing a reference point.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be at or after {ReferenceDate}")]
   [RulePlaceholder("{ReferenceDate}", "referenceDate")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.AtOrAfter))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsAtOrAfter<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      DateTime referenceDate)
      where TRequest : class
   {
      return builder.Is(Rules.AtOrAfter, referenceDate);
   }

   /// <summary>
   /// Validates that a DateTime value is at or before the specified reference time.
   /// This allows for testable date validation by providing a reference point.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be at or before {ReferenceDate}")]
   [RulePlaceholder("{ReferenceDate}", "referenceDate")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.AtOrBefore))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsAtOrBefore<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      DateTime referenceDate)
      where TRequest : class
   {
      return builder.Is(Rules.AtOrBefore, referenceDate);
   }

   /// <summary>
   /// Validates that a DateTime value is in the past (before the specified reference time).
   /// This allows for testable date validation by providing a reference point.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be before {ReferenceDate}")]
   [RulePlaceholder("{ReferenceDate}", "referenceDate")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Before))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsBefore<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      DateTime referenceDate)
      where TRequest : class
   {
      return builder.Is(Rules.Before, referenceDate);
   }

   /// <summary>
   /// Validates that a value is between a minimum and maximum value (inclusive).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Min}</c>, <c>{Max}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be between {Min} and {Max}. Value received is {TargetValue}")]
   [RulePlaceholder("{Min}", "min")]
   [RulePlaceholder("{Max}", "max")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Between))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsBetween<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType min,
      TTargetType max) 
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.Between, min, max);
   }

   /// <summary>
   /// Validates that a value is between a minimum and maximum value (exclusive).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Min}</c>, <c>{Max}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be between {Min} and {Max} (exclusive). Value received is {TargetValue}")]
   [RulePlaceholder("{Min}", "min")]
   [RulePlaceholder("{Max}", "max")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.BetweenExclusive))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsBetweenExclusive<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType min,
      TTargetType max)
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.BetweenExclusive, min, max);
   }

   /// <summary>
   /// Validates that a collection contains a specified item.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection. Must implement IEquatable.</typeparam>
   /// <typeparam name="TItemType">The type of items in the collection. Must implement IEquatable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain the specified item")]
   [RulePlaceholder("{Item}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.CollectionContains), [2])]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> CollectionContains<TRequest, TCollectionType, TItemType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      TItemType parameter)
      where TRequest : class
      where TCollectionType : IEnumerable<TItemType>?
      where TItemType : IEquatable<TItemType>
   {
      return null!;
   }

   /// <summary>
   /// Validates that a collection contains a specified item using a custom equality comparer.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection.</typeparam>
   /// <typeparam name="TItemType">The type of items in the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>, <c>{Comparer}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain the specified item")]
   [RulePlaceholder("{Item}", "parameter")]
   [RulePlaceholder("{Comparer}", "comparer")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.CollectionContains), [2])]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> CollectionContains<TRequest, TCollectionType, TItemType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      TItemType parameter,
      IEqualityComparer<TItemType> comparer)
      where TRequest : class
   {
      return null!;
   }

   /// <summary>
   /// Validates that a collection does not contain a specified item.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection. Must implement IEquatable.</typeparam>
   /// <typeparam name="TItemType">The type of items in the collection. Must implement IEquatable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not contain the specified item")]
   [RulePlaceholder("{Item}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.CollectionNotContains), [2])]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> CollectionNotContains<TRequest, TCollectionType, TItemType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      TItemType parameter)
      where TRequest : class
      where TCollectionType : IEnumerable<TItemType>?
      where TItemType : IEquatable<TItemType>
   {
      return null!;
   }

   /// <summary>
   /// Validates that a collection does not contain a specified item using a custom equality comparer.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection.</typeparam>
   /// <typeparam name="TItemType">The type of items in the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>, <c>{Comparer}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not contain the specified item")]
   [RulePlaceholder("{Item}", "parameter")]
   [RulePlaceholder("{Comparer}", "comparer")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.CollectionNotContains), [2])]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> CollectionNotContains<TRequest, TCollectionType, TItemType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      TItemType parameter,
      IEqualityComparer<TItemType> comparer)
      where TRequest : class
   {
      return null!;
   }

   /// <summary>
   /// Validates that a string contains a specified substring.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Substring}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain {Substring}")]
   [RulePlaceholder("{Substring}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Contains))]
   public static IValidationRuleBuilderType<TRequest, string> Contains<TRequest>(
      this IBuilderType<TRequest, string> builder,
      string parameter)
      where TRequest : class
   {
      return builder.Is(Rules.Contains, parameter);
   }

   /// <summary>
   /// Validates that a collection has exactly a specified number of items.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ExpectedCount}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have exactly {ExpectedCount} items")]
   [RulePlaceholder("{ExpectedCount}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Count))]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> HasCount<TRequest, TCollectionType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      int parameter)
      where TRequest : class
      where TCollectionType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a collection has a number of items within a specified range.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of collection</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinCount}</c>, <c>{MaxCount}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have between {MinCount} and {MaxCount} items")]
   [RulePlaceholder("{MinCount}", "minCount")]
   [RulePlaceholder("{MaxCount}", "maxCount")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.CountBetween))]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> HasCountBetween<TRequest, TCollectionType>(
      this IBuilderType<TRequest, TCollectionType> builder,
      int minCount,
      int maxCount)
      where TRequest : class
      where TCollectionType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a DateTime value is between a start date and end date (inclusive).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{StartDate}</c>, <c>{EndDate}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be between {StartDate} and {EndDate}")]
   [RulePlaceholder("{StartDate}", "startDate")]
   [RulePlaceholder("{EndDate}", "endDate")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.DateBetween))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsDateBetween<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      DateTime startDate,
      DateTime endDate)
      where TRequest : class
   {
      return builder.Is(Rules.DateBetween, startDate, endDate);
   }

   /// <summary>
   /// Validates that a string is a valid email address format.
   /// Performs a simple check for presence of @ with text before and after.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be a valid email address")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.EmailAddress))]
   public static IValidationRuleBuilderType<TRequest, string> IsEmailAddress<TRequest>(
      this IBuilderType<TRequest, string> builder)
      where TRequest : class
   {
      return builder.Is(Rules.EmailAddress);
   }

   /// <summary>
   /// Validates that a collection is empty or null.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be empty")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Empty))]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> IsEmpty<TRequest, TCollectionType>(
      this IBuilderType<TRequest, TCollectionType> builder)
      where TRequest : class
      where TCollectionType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a string ends with a specified suffix.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Suffix}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must end with {Suffix}")]
   [RulePlaceholder("{Suffix}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.EndsWith))]
   public static IValidationRuleBuilderType<TRequest, string> EndsWith<TRequest>(
      this IBuilderType<TRequest, string> builder,
      string parameter)
      where TRequest : class
   {
      return builder.Is(Rules.EndsWith, parameter);
   }

   /// <summary>
   /// Validates that a value equals another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Equal))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsEqualTo<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.Equal, parameter);
   }

   /// <summary>
   /// Validates that a value is equal to another specified value using a custom equality comparer.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>, <c>{Comparer}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "value")]
   [RulePlaceholder("{Comparer}", "comparer")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Equal))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsEqualTo<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType value,
      IEqualityComparer<TTargetType> comparer)
      where TRequest : class
   {
      return builder.Is(Rules.Equal, value, comparer);
   }

   /// <summary>
   /// Validates that a value is greater than or equal to another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be greater or equal than {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.GreaterOrEqualThan))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsGreaterOrEqualThan<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.GreaterOrEqualThan, parameter);
   }

   /// <summary>
   /// Validates that a value is greater than another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be greater than {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.GreaterThan))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsGreaterThan<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.GreaterThan, parameter);
   }

   /// <summary>
   /// Validates that a collection is not null and contains at least one item.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TCollectionType">The type of the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} cannot be empty.")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.HasItems))]
   public static IValidationRuleBuilderType<TRequest, TCollectionType> HasItems<TRequest, TCollectionType>(
      this IBuilderType<TRequest, TCollectionType> builder)
      where TRequest : class
      where TCollectionType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a value is contained within a specified set of allowed values.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{AllowedValues}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be one of the allowed values")]
   [RulePlaceholder("{AllowedValues}", "allowedValues")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.In))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsIn<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      IEnumerable<TTargetType> allowedValues)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.In, allowedValues);
   }

   /// <summary>
   /// Validates that a DateTime value is in the future (after the current UTC time).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be in the future")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.InFuture))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsInFuture<TRequest>(
      this IBuilderType<TRequest, DateTime> builder)
      where TRequest : class
   {
      return builder.Is(Rules.InFuture);
   }

   /// <summary>
   /// Validates that a DateTime value is in the future or present (at or after the current UTC time).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be in the future or present")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.InFutureOrPresent))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsInFutureOrPresent<TRequest>(
      this IBuilderType<TRequest, DateTime> builder)
      where TRequest : class
   {
      return builder.Is(Rules.InFutureOrPresent);
   }

   /// <summary>
   /// Validates that a DateTime value is in the past (before the current UTC time).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be in the past")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.InPast))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsInPast<TRequest>(
      this IBuilderType<TRequest, DateTime> builder)
      where TRequest : class
   {
      return builder.Is(Rules.InPast);
   }

   /// <summary>
   /// Validates that a DateTime value is in the past or present (at or before the current UTC time).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be in the past or present")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.InPastOrPresent))]
   public static IValidationRuleBuilderType<TRequest, DateTime> IsInPastOrPresent<TRequest>(
      this IBuilderType<TRequest, DateTime> builder)
      where TRequest : class
   {
      return builder.Is(Rules.InPastOrPresent);
   }

   /// <summary>
   /// Validates that a value is contained within a specified set of allowed values (params version).
   /// This allows a more convenient syntax: IsInValues("A", "B", "C") instead of IsIn(new[] { "A", "B", "C" })
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{AllowedValues}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be one of the allowed values")]
   [RulePlaceholder("{AllowedValues}", "allowedValues")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.InValues))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsInValues<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      params TTargetType[] allowedValues)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.InValues, allowedValues);
   }

   /// <summary>
   /// Validates that a string has exactly a specified length.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ExpectedLength}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have exactly {ExpectedLength} characters")]
   [RulePlaceholder("{ExpectedLength}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Length))]
   public static IValidationRuleBuilderType<TRequest, string> HasLength<TRequest>(
      this IBuilderType<TRequest, string> builder,
      int parameter)
      where TRequest : class
   {
      return builder.Is(Rules.Length, parameter);
   }

   /// <summary>
   /// Validates that a string has a length within a specified range.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinLength}</c>, <c>{MaxLength}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have a length between {MinLength} and {MaxLength}")]
   [RulePlaceholder("{MinLength}", "minLength")]
   [RulePlaceholder("{MaxLength}", "maxLength")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.LengthBetween))]
   public static IValidationRuleBuilderType<TRequest, string> HasLengthBetween<TRequest>(
      this IBuilderType<TRequest, string> builder,
      int minLength,
      int maxLength)
      where TRequest : class
   {
      return builder.Is(Rules.LengthBetween, minLength, maxLength);
   }

   /// <summary>
   /// Validates that a value is less than or equal to another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be less or equal than {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.LessOrEqualThan))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsLessOrEqualThan<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.LessOrEqualThan, parameter);
   }

   /// <summary>
   /// Validates that a value is less than another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be less than {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.LessThan))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsLessThan<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IComparable
   {
      return builder.Is(Rules.LessThan, parameter);
   }

   /// <summary>
   /// Validates that a string matches a specified regular expression pattern.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// <para>Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Pattern}</c>.</para>
   /// <para>
   /// For frequently validated patterns, consider using <see cref="MatchesRegex"/> with a
   /// pre-compiled <see cref="Regex"/> instance for better performance.
   /// </para>
   /// </remarks>
   [DefaultMessage("{TargetName} must match the pattern {Pattern}")]
   [RulePlaceholder("{Pattern}", "pattern")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Matches))]
   public static IValidationRuleBuilderType<TRequest, string> Matches<TRequest>(
      this IBuilderType<TRequest, string> builder,
      string pattern)
      where TRequest : class
   {
      return builder.Is(Rules.Matches, pattern);
   }

   /// <summary>
   /// Validates that a string matches a specified compiled regular expression.
   /// This is more efficient than the string pattern version as the Regex is pre-compiled.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must match the specified pattern")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MatchesRegex))]
   public static IValidationRuleBuilderType<TRequest, string> MatchesRegex<TRequest>(
      this IBuilderType<TRequest, string> builder,
      Regex regex)
      where TRequest : class
   {
      return builder.Is(Rules.MatchesRegex, regex);
   }

   /// <summary>
   /// Validates that a birth date represents a person who is at most a specified maximum age in years.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxAge}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must represent an age of at most {MaxAge} years")]
   [RulePlaceholder("{MaxAge}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MaxAge))]
   public static IValidationRuleBuilderType<TRequest, DateTime> HasMaxAge<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      int parameter)
      where TRequest : class
   {
      return builder.Is(Rules.MaxAge, parameter);
   }

   /// <summary>
   /// Validates that a collection has at most a maximum number of items.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of items in the collection</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxCount}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have at most {MaxCount} items")]
   [RulePlaceholder("{MaxCount}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MaxCount))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> HasMaxCount<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      int parameter)
      where TRequest : class
      where TTargetType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a string does not exceed a maximum length.
   /// Null strings are considered valid (length 0).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxLength}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have a maximum length of {MaxLength}")]
   [RulePlaceholder("{MaxLength}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MaxLength))]
   public static IValidationRuleBuilderType<TRequest, string> HasMaxLength<TRequest>(
      this IBuilderType<TRequest, string> builder,
      int parameter)
      where TRequest : class
   {
      return builder.Is(Rules.MaxLength, parameter);
   }

   /// <summary>
   /// Validates that a birth date represents a person who is at least a specified minimum age in years.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinAge}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must represent an age of at least {MinAge} years")]
   [RulePlaceholder("{MinAge}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MinAge))]
   public static IValidationRuleBuilderType<TRequest, DateTime> HasMinAge<TRequest>(
      this IBuilderType<TRequest, DateTime> builder,
      int parameter)
      where TRequest : class
   {
      return builder.Is(Rules.MinAge, parameter);
   }

   /// <summary>
   /// Validates that a collection has at least a minimum number of items.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinCount}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have at least {MinCount} items")]
   [RulePlaceholder("{MinCount}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MinCount))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> HasMinCount<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      int parameter)
      where TRequest : class
      where TTargetType : IEnumerable
   {
      return null!;
   }

   /// <summary>
   /// Validates that a string has at least a minimum length.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinLength}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must have a minimum length of {MinLength}")]
   [RulePlaceholder("{MinLength}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.MinLength))]
   public static IValidationRuleBuilderType<TRequest, string> HasMinLength<TRequest>(
      this IBuilderType<TRequest, string> builder,
      int parameter)
      where TRequest : class
   {
      return builder.Is(Rules.MinLength, parameter);
   }

   /// <summary>
   /// Validates that a numeric value is negative (less than zero).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be negative. Value received is {TargetValue}")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Negative))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNegative<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
      where TTargetType : IComparable<TTargetType>
   {
      return builder.Is(Rules.Negative);
   }

   /// <summary>
   /// Validates that a numeric value is negative or zero (less than or equal to zero).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be negative or zero. Value received is {TargetValue}")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NegativeOrZero))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNegativeOrZero<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
      where TTargetType : IComparable<TTargetType>
   {
      return builder.Is(Rules.NegativeOrZero);
   }

   /// <summary>
   /// Validates that a value is not the default value for its type.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} cannot be the default value.")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotDefault))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotDefault<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
   {
      return builder.Is(Rules.NotDefault);
   }

   /// <summary>
   /// Validates that a value does not equal another specified value.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotEqual))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotEqualTo<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType parameter)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.NotEqual, parameter);
   }

   /// <summary>
   /// Validates that a value is not equal to another specified value using a custom equality comparer.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being compared.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>, <c>{Comparer}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
   [RulePlaceholder("{ValueToCompare}", "value")]
   [RulePlaceholder("{Comparer}", "comparer")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotEqual))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotEqualTo<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      TTargetType value,
      IEqualityComparer<TTargetType> comparer)
      where TRequest : class
   {
      return builder.Is(Rules.NotEqual, value, comparer);
   }

   /// <summary>
   /// Validates that a value is not contained within a specified set of forbidden values.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ForbiddenValues}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be one of the forbidden values")]
   [RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotIn))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotIn<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      IEnumerable<TTargetType> forbiddenValues)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.NotIn, forbiddenValues);
   }

   /// <summary>
   /// Validates that a value is not contained within a specified set of forbidden values (params version).
   /// This allows a more convenient syntax: IsNotInValues("A", "B", "C") instead of IsNotIn(new[] { "A", "B", "C" })
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ForbiddenValues}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be one of the forbidden values")]
   [RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotInValues))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotInValues<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder,
      params TTargetType[] forbiddenValues)
      where TRequest : class
      where TTargetType : IEquatable<TTargetType>
   {
      return builder.Is(Rules.NotInValues, forbiddenValues);
   }

   /// <summary>
   /// Validates that a value is not null.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} is required.")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotNull))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsNotNull<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
   {
      return builder.Is(Rules.NotNull);
   }

   /// <summary>
   /// Validates that a string is not null or empty.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be null or empty.")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotNullOrEmpty))]
   public static IValidationRuleBuilderType<TRequest, string> IsNotNullOrEmpty<TRequest>(
      this IBuilderType<TRequest, string> builder)
      where TRequest : class
   {
      return builder.Is(Rules.NotNullOrEmpty);
   }

   /// <summary>
   /// Validates that a string is not null, empty, or consists only of white-space characters.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must not be null or contain only whitespace.")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.NotNullOrWhiteSpace))]
   public static IValidationRuleBuilderType<TRequest, string> IsNotNullOrWhiteSpace<TRequest>(
      this IBuilderType<TRequest, string> builder)
      where TRequest : class
   {
      return builder.Is(Rules.NotNullOrWhiteSpace);
   }

   /// <summary>
   /// Validates that a numeric value is positive (greater than zero).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be positive. Value received is {TargetValue}")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Positive))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsPositive<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
      where TTargetType : IComparable<TTargetType>
   {
      return builder.Is(Rules.Positive);
   }

   /// <summary>
   /// Validates that a numeric value is positive or zero (greater than or equal to zero).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be positive or zero. Value received is {TargetValue}")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.PositiveOrZero))]
   public static IValidationRuleBuilderType<TRequest, TTargetType> IsPositiveOrZero<TRequest, TTargetType>(
      this IBuilderType<TRequest, TTargetType> builder)
      where TRequest : class
      where TTargetType : IComparable<TTargetType>
   {
      return builder.Is(Rules.PositiveOrZero);
   }

   /// <summary>
   /// Validates that a string starts with a specified prefix.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Prefix}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must start with {Prefix}")]
   [RulePlaceholder("{Prefix}", "parameter")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.StartsWith))]
   public static IValidationRuleBuilderType<TRequest, string> StartsWith<TRequest>(
      this IBuilderType<TRequest, string> builder,
      string parameter)
      where TRequest : class
   {
      return builder.Is(Rules.StartsWith, parameter);
   }

   /// <summary>
   /// Validates that a collection contains only unique items (no duplicates).
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain only unique items")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Unique))]
   public static IValidationRuleBuilderType<TRequest, IEnumerable<TTargetType>?> IsUnique<TRequest, TTargetType>(
      this IBuilderType<TRequest, IEnumerable<TTargetType>?> builder)
      where TRequest : class
   {
      return builder.Is(Rules.Unique);
   }

   /// <summary>
   /// Validates that a collection contains only unique items (no duplicates) using a custom equality comparer.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Comparer}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must contain only unique items")]
   [RulePlaceholder("{Comparer}", "comparer")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Unique))]
   public static IValidationRuleBuilderType<TRequest, IEnumerable<TTargetType>?> IsUnique<TRequest, TTargetType>(
      this IBuilderType<TRequest, IEnumerable<TTargetType>?> builder,
      IEqualityComparer<TTargetType> comparer)
      where TRequest : class
   {
      return builder.Is(Rules.Unique, comparer);
   }

   /// <summary>
   /// Validates that a string is a valid URL format.
   /// </summary>
   /// <typeparam name="TRequest">The request type in a validator.</typeparam>
   /// <remarks>
   /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
   /// </remarks>
   [DefaultMessage("{TargetName} must be a valid URL")]
   [MapToValidationRule(typeof(Rules), nameof(Rules.Url))]
   public static IValidationRuleBuilderType<TRequest, string> IsUrl<TRequest>(
      this IBuilderType<TRequest, string> builder)
      where TRequest : class
   {
      return builder.Is(Rules.Url);
   }
}
