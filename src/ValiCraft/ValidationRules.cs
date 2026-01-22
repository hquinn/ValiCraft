using System.Text.RegularExpressions;
using ValiCraft.Attributes;

namespace ValiCraft;

public static class ValidationRules
{
    /// <summary>
    /// Validates that a DateTime value is in the future (after the specified reference time).
    /// This allows for testable date validation by providing a reference point.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be after {ReferenceDate}")]
    [RulePlaceholder("{ReferenceDate}", "referenceDate")]
    public static bool After(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue > referenceDate;
    }

    /// <summary>
    /// Validates that a string contains only alphanumeric characters (letters and digits).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain only letters and numbers")]
    public static bool AlphaNumeric(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        foreach (var c in targetValue)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates that a DateTime value is at or after the specified reference time.
    /// This allows for testable date validation by providing a reference point.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be at or after {ReferenceDate}")]
    [RulePlaceholder("{ReferenceDate}", "referenceDate")]
    public static bool AtOrAfter(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue >= referenceDate;
    }

    /// <summary>
    /// Validates that a DateTime value is at or before the specified reference time.
    /// This allows for testable date validation by providing a reference point.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be at or before {ReferenceDate}")]
    [RulePlaceholder("{ReferenceDate}", "referenceDate")]
    public static bool AtOrBefore(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue <= referenceDate;
    }

    /// <summary>
    /// Validates that a DateTime value is in the past (before the specified reference time).
    /// This allows for testable date validation by providing a reference point.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ReferenceDate}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be before {ReferenceDate}")]
    [RulePlaceholder("{ReferenceDate}", "referenceDate")]
    public static bool Before(DateTime targetValue, DateTime referenceDate)
    {
        return targetValue < referenceDate;
    }

    /// <summary>
    /// Validates that a value is between a minimum and maximum value (inclusive).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Min}</c>, <c>{Max}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be between {Min} and {Max}. Value received is {TargetValue}")]
    [RulePlaceholder("{Min}", "min")]
    [RulePlaceholder("{Max}", "max")]
    public static bool Between<TTargetType>(TTargetType targetValue, TTargetType min, TTargetType max)
        where TTargetType : IComparable
    {
        return targetValue.CompareTo(min) >= 0 && targetValue.CompareTo(max) <= 0;
    }

    /// <summary>
    /// Validates that a value is between a minimum and maximum value (exclusive).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Min}</c>, <c>{Max}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be between {Min} and {Max} (exclusive). Value received is {TargetValue}")]
    [RulePlaceholder("{Min}", "min")]
    [RulePlaceholder("{Max}", "max")]
    public static bool BetweenExclusive<TTargetType>(TTargetType targetValue, TTargetType min, TTargetType max)
        where TTargetType : IComparable
    {
        return targetValue.CompareTo(min) > 0 && targetValue.CompareTo(max) < 0;
    }

    /// <summary>
    /// Validates that a collection contains a specified item.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection. Must implement IEquatable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain the specified item")]
    [RulePlaceholder("{Item}", "parameter")]
    public static bool CollectionContains<TTargetType>(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
        where TTargetType : IEquatable<TTargetType>
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(parameter);
    }

    /// <summary>
    /// Validates that a collection contains a specified item using a custom equality comparer.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>, <c>{Comparer}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain the specified item")]
    [RulePlaceholder("{Item}", "item")]
    [RulePlaceholder("{Comparer}", "comparer")]
    public static bool CollectionContains<TTargetType>(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(item, comparer);
    }

    /// <summary>
    /// Validates that a collection does not contain a specified item.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection. Must implement IEquatable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not contain the specified item")]
    [RulePlaceholder("{Item}", "parameter")]
    public static bool CollectionNotContains<TTargetType>(IEnumerable<TTargetType>? targetValue, TTargetType parameter)
        where TTargetType : IEquatable<TTargetType>
    {
        if (targetValue == null)
        {
            return true;
        }

        return !targetValue.Contains(parameter);
    }

    /// <summary>
    /// Validates that a collection does not contain a specified item using a custom equality comparer.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Item}</c>, <c>{Comparer}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not contain the specified item")]
    [RulePlaceholder("{Item}", "item")]
    [RulePlaceholder("{Comparer}", "comparer")]
    public static bool CollectionNotContains<TTargetType>(IEnumerable<TTargetType>? targetValue, TTargetType item, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return true;
        }

        return !targetValue.Contains(item, comparer);
    }

    /// <summary>
    /// Validates that a string contains a specified substring.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Substring}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain {Substring}")]
    [RulePlaceholder("{Substring}", "parameter")]
    public static bool Contains(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.Contains(parameter, StringComparison.Ordinal);
    }

    /// <summary>
    /// Validates that a collection has exactly a specified number of items.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ExpectedCount}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have exactly {ExpectedCount} items")]
    [RulePlaceholder("{ExpectedCount}", "parameter")]
    public static bool Count<TTargetType>(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count == parameter;
        }

        return targetValue.Count() == parameter;
    }

    /// <summary>
    /// Validates that a collection has a number of items within a specified range.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinCount}</c>, <c>{MaxCount}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have between {MinCount} and {MaxCount} items")]
    [RulePlaceholder("{MinCount}", "minCount")]
    [RulePlaceholder("{MaxCount}", "maxCount")]
    public static bool CountBetween<TTargetType>(IEnumerable<TTargetType>? targetValue, int minCount, int maxCount)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        int count;
        if (targetValue is ICollection<TTargetType> collection)
        {
            count = collection.Count;
        }
        else
        {
            count = targetValue.Count();
        }

        return count >= minCount && count <= maxCount;
    }

    /// <summary>
    /// Validates that a DateTime value is between a start date and end date (inclusive).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{StartDate}</c>, <c>{EndDate}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be between {StartDate} and {EndDate}")]
    [RulePlaceholder("{StartDate}", "startDate")]
    [RulePlaceholder("{EndDate}", "endDate")]
    public static bool DateBetween(DateTime targetValue, DateTime startDate, DateTime endDate)
    {
        return targetValue >= startDate && targetValue <= endDate;
    }

    /// <summary>
    /// Validates that a string is a valid email address format.
    /// Performs a simple check for presence of @ with text before and after.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be a valid email address")]
    public static bool EmailAddress(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        var atIndex = targetValue.IndexOf('@');

        // Must contain @ with at least one character before and after
        return atIndex > 0 && atIndex < targetValue.Length - 1;
    }

    /// <summary>
    /// Validates that a collection is empty or null.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be empty")]
    public static bool Empty<TTargetType>(IEnumerable<TTargetType>? targetValue)
    {
        if (targetValue == null)
        {
            return true;
        }

        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count == 0;
        }

        if (targetValue is IReadOnlyCollection<TTargetType> readOnlyCollection)
        {
            return readOnlyCollection.Count == 0;
        }

        return !targetValue.Any();
    }

    /// <summary>
    /// Validates that a string ends with a specified suffix.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Suffix}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must end with {Suffix}")]
    [RulePlaceholder("{Suffix}", "parameter")]
    public static bool EndsWith(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.EndsWith(parameter, StringComparison.Ordinal);
    }

    /// <summary>
    /// Validates that a value equals another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool Equal<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IEquatable<TTargetType>
    {
        return property.Equals(parameter);
    }

    /// <summary>
    /// Validates that a value is equal to another specified value using a custom equality comparer.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>, <c>{Comparer}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be equal to {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "value")]
    [RulePlaceholder("{Comparer}", "comparer")]
    public static bool Equal<TTargetType>(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return comparer.Equals(property, value);
    }

    /// <summary>
    /// Validates that a value is greater than or equal to another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be greater or equal than {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool GreaterOrEqualThan<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IComparable
    {
        return property.CompareTo(parameter) >= 0;
    }

    /// <summary>
    /// Validates that a value is greater than another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be greater than {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool GreaterThan<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IComparable
    {
        return property.CompareTo(parameter) > 0;
    }

    /// <summary>
    /// Validates that a collection is not null and contains at least one item.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} cannot be empty.")]
    public static bool HasItems<TTargetType>(IEnumerable<TTargetType>? property)
    {
        if (property == null)
        {
            return false;
        }

        if (property is ICollection<TTargetType> collection)
        {
            return collection.Count > 0;
        }

        if (property is IReadOnlyCollection<TTargetType> readOnlyCollection)
        {
            return readOnlyCollection.Count > 0;
        }

        return property.Any();
    }

    /// <summary>
    /// Validates that a value is contained within a specified set of allowed values.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{AllowedValues}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be one of the allowed values")]
    [RulePlaceholder("{AllowedValues}", "allowedValues")]
    public static bool In<TTargetType>(TTargetType targetValue, IEnumerable<TTargetType> allowedValues)
        where TTargetType : IEquatable<TTargetType>
    {
        if (allowedValues is ICollection<TTargetType> collection)
        {
            return collection.Contains(targetValue);
        }

        return allowedValues.Contains(targetValue);
    }

    /// <summary>
    /// Validates that a DateTime value is in the future (after the current UTC time).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be in the future")]
    public static bool InFuture(DateTime targetValue)
    {
        return targetValue > DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that a DateTime value is in the future or present (at or after the current UTC time).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be in the future or present")]
    public static bool InFutureOrPresent(DateTime targetValue)
    {
        return targetValue >= DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that a DateTime value is in the past (before the current UTC time).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be in the past")]
    public static bool InPast(DateTime targetValue)
    {
        return targetValue < DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that a DateTime value is in the past or present (at or before the current UTC time).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be in the past or present")]
    public static bool InPastOrPresent(DateTime targetValue)
    {
        return targetValue <= DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that a value is contained within a specified set of allowed values (params version).
    /// This allows a more convenient syntax: IsInValues("A", "B", "C") instead of IsIn(new[] { "A", "B", "C" })
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{AllowedValues}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be one of the allowed values")]
    [RulePlaceholder("{AllowedValues}", "allowedValues")]
    public static bool InValues<TTargetType>(TTargetType targetValue, TTargetType[] allowedValues)
        where TTargetType : IEquatable<TTargetType>
    {
        return Array.IndexOf(allowedValues, targetValue) >= 0;
    }

    /// <summary>
    /// Validates that a string has exactly a specified length.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ExpectedLength}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have exactly {ExpectedLength} characters")]
    [RulePlaceholder("{ExpectedLength}", "parameter")]
    public static bool Length(string? targetValue, int parameter)
    {
        return targetValue?.Length == parameter;
    }

    /// <summary>
    /// Validates that a string has a length within a specified range.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinLength}</c>, <c>{MaxLength}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have a length between {MinLength} and {MaxLength}")]
    [RulePlaceholder("{MinLength}", "minLength")]
    [RulePlaceholder("{MaxLength}", "maxLength")]
    public static bool LengthBetween(string? targetValue, int minLength, int maxLength)
    {
        if (targetValue == null)
        {
            return false;
        }

        var length = targetValue.Length;
        return length >= minLength && length <= maxLength;
    }

    /// <summary>
    /// Validates that a value is less than or equal to another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be less or equal than {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool LessOrEqualThan<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IComparable
    {
        return property.CompareTo(parameter) <= 0;
    }

    /// <summary>
    /// Validates that a value is less than another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be less than {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool LessThan<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IComparable
    {
        return property.CompareTo(parameter) < 0;
    }

    /// <summary>
    /// Validates that a string matches a specified regular expression pattern.
    /// </summary>
    /// <remarks>
    /// <para>Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Pattern}</c>.</para>
    /// <para>
    /// For frequently validated patterns, consider using <see cref="MatchesRegex"/> with a
    /// pre-compiled <see cref="Regex"/> instance for better performance.
    /// </para>
    /// </remarks>
    [DefaultMessage("{TargetName} must match the pattern {Pattern}")]
    [RulePlaceholder("{Pattern}", "pattern")]
    public static bool Matches(string? targetValue, string pattern)
    {
        if (targetValue == null)
        {
            return false;
        }

        return Regex.IsMatch(targetValue, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Validates that a string matches a specified compiled regular expression.
    /// This is more efficient than the string pattern version as the Regex is pre-compiled.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must match the specified pattern")]
    public static bool MatchesRegex(string? targetValue, Regex regex)
    {
        if (targetValue == null)
        {
            return false;
        }

        return regex.IsMatch(targetValue);
    }

    /// <summary>
    /// Validates that a birth date represents a person who is at most a specified maximum age in years.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxAge}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must represent an age of at most {MaxAge} years")]
    [RulePlaceholder("{MaxAge}", "parameter")]
    public static bool MaxAge(DateTime birthDate, int parameter)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }
        return age <= parameter;
    }

    /// <summary>
    /// Validates that a collection has at most a maximum number of items.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxCount}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have at most {MaxCount} items")]
    [RulePlaceholder("{MaxCount}", "parameter")]
    public static bool MaxCount<TTargetType>(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count <= parameter;
        }

        return targetValue.Count() <= parameter;
    }

    /// <summary>
    /// Validates that a string does not exceed a maximum length.
    /// Null strings are considered valid (length 0).
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MaxLength}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have a maximum length of {MaxLength}")]
    [RulePlaceholder("{MaxLength}", "parameter")]
    public static bool MaxLength(string? targetValue, int parameter)
    {
        // Null or empty strings have length 0, which should pass MaxLength validation
        return (targetValue?.Length ?? 0) <= parameter;
    }

    /// <summary>
    /// Validates that a birth date represents a person who is at least a specified minimum age in years.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinAge}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must represent an age of at least {MinAge} years")]
    [RulePlaceholder("{MinAge}", "parameter")]
    public static bool MinAge(DateTime birthDate, int parameter)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }
        return age >= parameter;
    }

    /// <summary>
    /// Validates that a collection has at least a minimum number of items.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinCount}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have at least {MinCount} items")]
    [RulePlaceholder("{MinCount}", "parameter")]
    public static bool MinCount<TTargetType>(IEnumerable<TTargetType>? targetValue, int parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        // Optimize for ICollection<T> to avoid enumeration
        if (targetValue is ICollection<TTargetType> collection)
        {
            return collection.Count >= parameter;
        }

        return targetValue.Count() >= parameter;
    }

    /// <summary>
    /// Validates that a string has at least a minimum length.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{MinLength}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must have a minimum length of {MinLength}")]
    [RulePlaceholder("{MinLength}", "parameter")]
    public static bool MinLength(string? targetValue, int parameter)
    {
        return targetValue?.Length >= parameter;
    }

    /// <summary>
    /// Validates that a numeric value is negative (less than zero).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be negative. Value received is {TargetValue}")]
    public static bool Negative<TTargetType>(TTargetType targetValue)
        where TTargetType : IComparable<TTargetType>
    {
        return targetValue.CompareTo(default!) < 0;
    }

    /// <summary>
    /// Validates that a numeric value is negative or zero (less than or equal to zero).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be negative or zero. Value received is {TargetValue}")]
    public static bool NegativeOrZero<TTargetType>(TTargetType targetValue)
        where TTargetType : IComparable<TTargetType>
    {
        return targetValue.CompareTo(default!) <= 0;
    }

    /// <summary>
    /// Validates that a value is not the default value for its type.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} cannot be the default value.")]
    public static bool NotDefault<TTargetType>(TTargetType? property)
    {
        return !EqualityComparer<TTargetType>.Default.Equals(property, default);
    }

    /// <summary>
    /// Validates that a value does not equal another specified value.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "parameter")]
    public static bool NotEqual<TTargetType>(TTargetType property, TTargetType parameter)
        where TTargetType : IEquatable<TTargetType>
    {
        return !property.Equals(parameter);
    }

    /// <summary>
    /// Validates that a value is not equal to another specified value using a custom equality comparer.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being compared.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ValueToCompare}</c>, <c>{Comparer}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be equal to {ValueToCompare}. Value received is {TargetValue}")]
    [RulePlaceholder("{ValueToCompare}", "value")]
    [RulePlaceholder("{Comparer}", "comparer")]
    public static bool NotEqual<TTargetType>(TTargetType property, TTargetType value, IEqualityComparer<TTargetType> comparer)
    {
        return !comparer.Equals(property, value);
    }

    /// <summary>
    /// Validates that a value is not contained within a specified set of forbidden values.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ForbiddenValues}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be one of the forbidden values")]
    [RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
    public static bool NotIn<TTargetType>(TTargetType targetValue, IEnumerable<TTargetType> forbiddenValues)
        where TTargetType : IEquatable<TTargetType>
    {
        if (forbiddenValues is ICollection<TTargetType> collection)
        {
            return !collection.Contains(targetValue);
        }

        return !forbiddenValues.Contains(targetValue);
    }

    /// <summary>
    /// Validates that a value is not contained within a specified set of forbidden values (params version).
    /// This allows a more convenient syntax: IsNotInValues("A", "B", "C") instead of IsNotIn(new[] { "A", "B", "C" })
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{ForbiddenValues}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be one of the forbidden values")]
    [RulePlaceholder("{ForbiddenValues}", "forbiddenValues")]
    public static bool NotInValues<TTargetType>(TTargetType targetValue, TTargetType[] forbiddenValues)
        where TTargetType : IEquatable<TTargetType>
    {
        return Array.IndexOf(forbiddenValues, targetValue) < 0;
    }

    /// <summary>
    /// Validates that a value is not null.
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} is required.")]
    public static bool NotNull<TTargetType>(TTargetType? targetValue)
    {
        return targetValue is not null;
    }

    /// <summary>
    /// Validates that a string is not null or empty.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be null or empty.")]
    public static bool NotNullOrEmpty(string? targetValue)
    {
        return !string.IsNullOrEmpty(targetValue);
    }

    /// <summary>
    /// Validates that a string is not null, empty, or consists only of white-space characters.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must not be null or contain only whitespace.")]
    public static bool NotNullOrWhiteSpace(string? targetValue)
    {
        return !string.IsNullOrWhiteSpace(targetValue);
    }

    /// <summary>
    /// Validates that a numeric value is positive (greater than zero).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be positive. Value received is {TargetValue}")]
    public static bool Positive<TTargetType>(TTargetType targetValue)
        where TTargetType : IComparable<TTargetType>
    {
        return targetValue.CompareTo(default!) > 0;
    }

    /// <summary>
    /// Validates that a numeric value is positive or zero (greater than or equal to zero).
    /// </summary>
    /// <typeparam name="TTargetType">The type of value being validated. Must implement IComparable.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be positive or zero. Value received is {TargetValue}")]
    public static bool PositiveOrZero<TTargetType>(TTargetType targetValue)
        where TTargetType : IComparable<TTargetType>
    {
        return targetValue.CompareTo(default!) >= 0;
    }

    /// <summary>
    /// Validates that a string starts with a specified prefix.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Prefix}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must start with {Prefix}")]
    [RulePlaceholder("{Prefix}", "parameter")]
    public static bool StartsWith(string? targetValue, string parameter)
    {
        if (targetValue == null)
        {
            return false;
        }

        return targetValue.StartsWith(parameter, StringComparison.Ordinal);
    }

    /// <summary>
    /// Validates that a collection contains only unique items (no duplicates).
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain only unique items")]
    public static bool Unique<TTargetType>(IEnumerable<TTargetType>? targetValue)
    {
        if (targetValue == null)
        {
            return true;
        }

        var seen = new HashSet<TTargetType>();
        foreach (var item in targetValue)
        {
            if (!seen.Add(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates that a collection contains only unique items (no duplicates) using a custom equality comparer.
    /// </summary>
    /// <typeparam name="TTargetType">The type of items in the collection.</typeparam>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>, <c>{Comparer}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must contain only unique items")]
    [RulePlaceholder("{Comparer}", "comparer")]
    public static bool Unique<TTargetType>(IEnumerable<TTargetType>? targetValue, IEqualityComparer<TTargetType> comparer)
    {
        if (targetValue == null)
        {
            return true;
        }

        var seen = new HashSet<TTargetType>(comparer);
        foreach (var item in targetValue)
        {
            if (!seen.Add(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates that a string is a valid URL format.
    /// </summary>
    /// <remarks>
    /// Available message placeholders: <c>{TargetName}</c>, <c>{TargetValue}</c>.
    /// </remarks>
    [DefaultMessage("{TargetName} must be a valid URL")]
    public static bool Url(string? targetValue)
    {
        if (string.IsNullOrWhiteSpace(targetValue))
        {
            return false;
        }

        return Uri.TryCreate(targetValue, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}