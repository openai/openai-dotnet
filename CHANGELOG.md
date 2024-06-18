# Release History

## 2.0.0-beta.6 (Unreleased)

## Bugs Fixed

- Added optional `CancellationToken` parameters to methods for `AssistantClient` and `VectorStore` client, consistent with past changes in [19a65a0](https://github.com/openai/openai-dotnet/commit/19a65a0a943fa3bef1ec8504708aaa526a1ee03a) (commit_link_)
- Fixed Assistants `FileSearchToolDefinition`'s `MaxResults` parameter to appropriate serialize and deserialize the value (commit_link)
- Added missing `[EditorBrowsable(EditorBrowsableState.Never)]` attributes to `AssistantClient` protocol methods, which should improve discoverability of the strongly typed methods (commit_link)

## 2.0.0-beta.5 (2024-06-14)

## Features Added

- API updates, current to [openai/openai-openapi@dd73070b](https://github.com/openai/openai-openapi/commit/dd73070b1d507645d24c249a63ebebd3ec38c0cb) ([1af6569](https://github.com/openai/openai-dotnet/commit/1af6569e2ceae9d840b8826e42d7e3b2569b43f6))
  - This includes `MaxResults` for Assistants `FileSearchToolDefinition`, `ParallelToolCallsEnabled` for function tools in Assistants and Chat, and `FileChunkingStrategy` for Assistants VectorStores
- Optional `CancellationToken` parameters are now directly present on most methods, eliminating the need to use protocol methods ([19a65a0](https://github.com/openai/openai-dotnet/commit/19a65a0a943fa3bef1ec8504708aaa526a1ee03a))

## Bugs Fixed

- ([#30](https://github.com/openai/openai-dotnet/issues/30)) Mitigated a .NET runtime issue that prevented chat message content and several other types from serializing correct on targets including mono and wasm ([896b9e0](https://github.com/openai/openai-dotnet/commit/896b9e0bc60f0ace90bd0d1af1254cf2680f8df6))
- Assistants: Fixed an issue that threw an exception when receiving code interpreter run step logs when streaming a run ([207d597](https://github.com/openai/openai-dotnet/commit/207d59762e7eeb666b7ab2728a0bbee7c0cdd918))
- Fixed a concurrency condition that could cause `multipart/form-data` requests to no longer generate random content part boundaries (no direct scenario impact) ([7cacdee](https://github.com/openai/openai-dotnet/commit/7cacdee2366df3cfa7e6c43bb050da54d23f8db9))

## Breaking Changes

**Assistants**:
- `InputQuote` is removed from Assistants `TextAnnotation` and `TextAnnotationUpdate`, per [openai/openai-openapi@dd73070b](https://github.com/openai/openai-openapi/commit/dd73070b1d507645d24c249a63ebebd3ec38c0cb) ([1af6569](https://github.com/openai/openai-dotnet/commit/1af6569e2ceae9d840b8826e42d7e3b2569b43f6))

## Other Changes

- Added an environment-variable-based test project to the repository with baseline scenario coverage ([db6328a](https://github.com/openai/openai-dotnet/commit/db6328accdd7927f19915cdc5412eb841f2447c1))
- Build/analyzer warnings cleaned up throughout the project ([45fc4d7](https://github.com/openai/openai-dotnet/commit/45fc4d72c12314aea83264ebe2e1dc18870e5c06), [b1fa082](https://github.com/openai/openai-dotnet/commit/b1fa0828a875906ef33ffe43ff1cd1a85fbd1a60), [22ab606](https://github.com/openai/openai-dotnet/commit/22ab606b867bbe0ea7f6918843dbc5e11dfe78eb))
- Proactively aligned library's implementation of server-sent event (SSE) handling with the source of the incoming `System.Net.ServerSentEvents` namespace ([674e0f7](https://github.com/openai/openai-dotnet/commit/674e0f773b26a22eb039e879539c4c7a44fdffdd))

## 2.0.0-beta.4 (2024-06-10)

## Features Added

- Added new, built-in helpers to simplify the use of text-only message content ([1c40de6](https://github.com/openai/openai-dotnet/commit/1c40de673a67ddf834b673aaabb94b2c42076e03))

## Bugs Fixed

- Optimized embedding deserialization and addressed correctness on big endian systems ([e28b5a7](https://github.com/openai/openai-dotnet/commit/e28b5a7787df4b1baa772406b09a0f650a45c77f))
- Optimized b64_json message parsing via regex ([efd76b5](https://github.com/openai/openai-dotnet/commit/efd76b50f094c585350240aea051ba342c6f6622))

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