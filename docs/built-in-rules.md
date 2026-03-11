# Built-in Rules

ValiCraft includes 50+ built-in validation rules:

## String Rules

| Rule | Description |
|------|-------------|
| `IsNotNull()` | Value must not be null |
| `IsNotNullOrEmpty()` | String must not be null or empty |
| `IsNotNullOrWhiteSpace()` | String must not be null, empty, or whitespace |
| `HasMinLength(n)` | Minimum string length |
| `HasMaxLength(n)` | Maximum string length |
| `HasLength(n)` | Exact string length |
| `HasLengthBetween(min, max)` | String length within range |
| `IsEmailAddress()` | Valid email format |
| `IsUrl()` | Valid HTTP/HTTPS URL |
| `IsAlphaNumeric()` | Only letters and digits |
| `StartsWith(prefix)` | Starts with substring |
| `EndsWith(suffix)` | Ends with substring |
| `Contains(substring)` | Contains substring |
| `Matches(pattern)` | Matches regex pattern (string) |
| `MatchesRegex(regex)` | Matches compiled regex |

## Numeric/Comparison Rules

| Rule | Description |
|------|-------------|
| `IsGreaterThan(value)` | Greater than |
| `IsGreaterOrEqualThan(value)` | Greater than or equal |
| `IsLessThan(value)` | Less than |
| `IsLessOrEqualThan(value)` | Less than or equal |
| `IsBetween(min, max)` | Within range (inclusive) |
| `IsBetweenExclusive(min, max)` | Within range (exclusive) |
| `IsPositive()` | Greater than zero |
| `IsPositiveOrZero()` | Greater than or equal to zero |
| `IsNegative()` | Less than zero |
| `IsNegativeOrZero()` | Less than or equal to zero |
| `IsEqual(value)` | Equals value |
| `IsNotEqual(value)` | Not equals value |
| `IsNotDefault()` | Not default value for type |

## Collection Rules

| Rule | Description |
|------|-------------|
| `HasMinCount(n)` | Minimum item count |
| `HasMaxCount(n)` | Maximum item count |
| `HasCount(n)` | Exact item count |
| `HasCountBetween(min, max)` | Item count within range |
| `IsEmpty()` | Collection must be empty |
| `HasItems()` | Collection must have items |
| `IsUnique()` | All items unique |
| `IsUniqueWithComparer(comparer)` | Unique with custom comparer |
| `CollectionContains(item)` | Contains specific item |
| `CollectionNotContains(item)` | Does not contain item |

## Date/Time Rules

| Rule | Description |
|------|-------------|
| `IsInFuture()` | After current UTC time |
| `IsInFutureOrPresent()` | At or after current UTC time |
| `IsInPast()` | Before current UTC time |
| `IsInPastOrPresent()` | At or before current UTC time |
| `IsAfter(date)` | After specified date |
| `IsBefore(date)` | Before specified date |
| `IsAtOrAfter(date)` | At or after specified date |
| `IsAtOrBefore(date)` | At or before specified date |
| `IsDateBetween(start, end)` | Within date range |
| `HasMinAge(years)` | Minimum age from birth date |
| `HasMaxAge(years)` | Maximum age from birth date |

## Inclusion/Exclusion Rules

| Rule | Description |
|------|-------------|
| `IsIn(collection)` | Value in allowed set |
| `IsNotIn(collection)` | Value not in forbidden set |
| `IsInValues(v1, v2, ...)` | Value in allowed values |
| `IsNotInValues(v1, v2, ...)` | Value not in forbidden values |

## Enum Rules

| Rule | Description |
|------|-------------|
| `IsValidEnumName<TEnum>()` | String is a valid enum member name |
| `IsValidEnumValue<TEnum, TValue>()` | Value is a valid enum underlying value |
| `IsValidEnum<TEnum>()` | Enum value is a defined member of its type |

## Custom Predicate

| Rule | Description |
|------|-------------|
| `Is(predicate)` | Custom validation predicate |
| `Is(async predicate)` | Async custom predicate |
