# Release History

## 2.0.0-beta.4 (2024-06-10)

## Features Added

- Added new, built-in helpers to simplify the use of text-only message content ([1c40de67](https://github.com/openai/openai-dotnet/commit/1c40de673a67ddf834b673aaabb94b2c42076e03))

## Bugs Fixed

- Optimized embedding deserialization and addressed correctness on big endian systems ([e28b5a77](https://github.com/openai/openai-dotnet/commit/e28b5a7787df4b1baa772406b09a0f650a45c77f))
- Optimized b64_json message parsing via regex ([efd76b50](https://github.com/openai/openai-dotnet/commit/efd76b50f094c585350240aea051ba342c6f6622))

## 2.0.0-beta.3 (2024-06-07)

### Bugs Fixed

- Removed a vestigial package reference ([5874f53](https://github.com/openai/openai-dotnet/commit/5874f533722ab46a3e077dacb6c3474e0ecca96e))

## 2.0.0-beta.2 (2024-06-06)

### Bugs Fixed

- Addressed an assembly properties issue ([bf21eb5](https://github.com/openai/openai-dotnet/commit/bf21eb5ad367aaac418dbbf320f98187fee5089a))
- Added migration guide to package ([f150666](https://github.com/openai/openai-dotnet/commit/f150666cd2ed552720207098b3b604a8e1ca73df))

## 2.0.0-beta.1 (2024-06-06)

### Features Added

This is the official OpenAI client library for C# / .NET. It provides convenient access to the OpenAI REST API from .NET applications and supports all the latest features. It is generated from our [OpenAPI specification](https://github.com/openai/openai-openapi) in collaboration with Microsoft.

### Breaking Changes

If you are a user migrating from version 1.11.0 or earlier, we will soon share a migration guide to help you get started.
- ***Addendum:** the [migration guide](https://github.com/openai/openai-dotnet/blob/main/MigrationGuide.md) is now available.*