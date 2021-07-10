# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2021-07-10

### Added

- Ability to refresh read-only models
- Reply to inbox messages
- Retrieve and interact with authenticated user's multireddits
- Request retries for transient HTTP response status codes
- Documentation for errors and request retries

### Changed

- `AuthenticationContext` now uses `SupportedAuthenticationContextAttribute` to determine whether a command is supported, rather than command identifiers

### Fixed

- Ensure `JsonDocument` is disposed during dynamic thing conversion

This release also includes a number of new tests, both for new and existing functionality.

## [1.0.0] - 2021-07-03

### Added

- Support for multiple authentication modes (read-only, script, web-app + more)
- Interactions for primary reddit objects:
    - Authenticated user account
    - Other user accounts
    - Front page
    - Subreddits
    - Submissions
    - Comments

[1.1.0]: https://github.com/JedS6391/Reddit.NET/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/JedS6391/Reddit.NET/tree/1.0.0