# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2022-10-28

- Support for submission flairs
  - Add `FlairId` and `FlairText` to submission details
  - Add ability to get the flairs available for submissions in a subreddit
  - Add ability to set submission flairs

## [1.2.0] - 2021-07-19

### Added

- Support the `installed_client` grant type when using the `ReadOnlyInstalledApp` authentication mode
- Ability to stream various listings, including new subreddit submissions/comments, new user submissions/comments, new user inbox messages

### Changed

- Asynchronous methods now accept a `CancellationToken` to allow cancellation of the operation

### Fixed

- Basic argument validation

## [1.1.1] - 2021-07-11

### Fixed

- `AuthenticateWithUsernamePasswordCommand` will no longer send a `duration` parameter, as it not supported for the `password` grant type

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

[1.3.0]: https://github.com/JedS6391/Reddit.NET/compare/1.2.0...1.3.0
[1.2.0]: https://github.com/JedS6391/Reddit.NET/compare/1.1.1...1.2.0
[1.1.1]: https://github.com/JedS6391/Reddit.NET/compare/1.1.0...1.1.1
[1.1.0]: https://github.com/JedS6391/Reddit.NET/compare/1.0.0...1.1.0
[1.0.0]: https://github.com/JedS6391/Reddit.NET/tree/1.0.0
