# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Comprehensive XML documentation for public APIs
- NuGet package badges in README
- SourceLink support for debugging into source code
- CHANGELOG.md and CONTRIBUTING.md documentation

### Changed
- Sealed all attribute classes for better design
- Improved NuGet package metadata

## [0.11.0] - 2025-01-XX

### Added
- `WhenNotNull()` - Skip validation for null values on optional properties
- `Either()` - OR-based validation where at least one group must pass
- DateTime validation rules: `IsAfter()`, `IsBefore()`, `IsOnOrAfter()`, `IsOnOrBefore()`
- Collection rules: `IsEmpty()`, `HasMinCount()`, `HasMaxCount()`, `HasCount()`, `HasCountBetween()`
- `IsInValues()` and `IsNotInValues()` - Convenient params-based alternatives to `IsIn()`/`IsNotIn()`
- Support for pre-compiled Regex with `MatchesRegex()` for better performance
- Enhanced weak semantic type resolution for custom validation rules

### Changed
- Improved collection validation performance
- Enhanced error messages with better placeholders

## [0.10.0] - 2024-XX-XX

### Added
- Initial release of ValiCraft
- Source generator-based validation framework
- Fluent API for defining validation rules
- 50+ built-in validation rules
- Support for custom validation rules
- Collection validation with `EnsureEach()`
- Nested object validation with `ValidateWith()`
- Conditional validation with `If()`
- Customizable error messages, codes, and target names
- Result type integration with MonadCraft

[Unreleased]: https://github.com/hquinn/ValiCraft/compare/v0.11.0...HEAD
[0.11.0]: https://github.com/hquinn/ValiCraft/compare/v0.10.0...v0.11.0
[0.10.0]: https://github.com/hquinn/ValiCraft/releases/tag/v0.10.0
