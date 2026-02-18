# Key File Locations

Quick reference for all paths involved in spec ingestion.

## Upstream (Source)

| What | Path |
|------|------|
| Base spec (all areas) | `https://github.com/microsoft/openai-openapi-pr/tree/main/packages/openai-typespec/src` |
| Common shared types | `https://github.com/microsoft/openai-openapi-pr/tree/main/packages/openai-typespec/src/common` |
| Area spec (e.g., audio) | `https://github.com/microsoft/openai-openapi-pr/tree/main/packages/openai-typespec/src/{area}` |

## Local Repository — Specification

| What | Path |
|------|------|
| Base spec entry point | `specification/base/typespec/main.tsp` |
| Area base spec | `specification/base/typespec/{area}/` |
| Common shared types | `specification/base/typespec/common/` |
| Common models | `specification/base/typespec/common/models.tsp` |
| Common custom types | `specification/base/typespec/common/custom.tsp` |
| SDK entrypoint | `specification/base/entrypoints/sdk.dotnet/main.tsp` |
| Client customizations | `specification/client/{area}.client.tsp` |
| Client model overrides | `specification/client/models/{area}.models.tsp` |
| Main TSP entry (all imports) | `specification/main.tsp` |
| TSP config | `specification/tspconfig.yaml` |

## Local Repository — C# Source

| What | Path |
|------|------|
| Custom C# code (per area) | `src/Custom/{area}/` |
| Internal generator stubs | `src/Custom/{area}/Internal/GeneratorStubs.cs` |
| Generated C# code | `src/Generated/` |
| Generated models (per area) | `src/Generated/Models/{area}/` |
| Generated client | `src/Generated/{area}Client.cs` |
| Generated REST client | `src/Generated/{area}Client.RestClient.cs` |

## Local Repository — Scripts

| What | Path |
|------|------|
| Code generation script | `scripts/Invoke-CodeGen.ps1` |
| API export script | `scripts/Export-Api.ps1` |
| API compatibility test | `scripts/Test-ApiCompatibility.ps1` |
| AOT compatibility test | `scripts/Test-AotCompatibility.ps1` |

## Local Repository — API Surface

| What | Path |
|------|------|
| .NET 8.0 API surface | `api/OpenAI.net8.0.cs` |
| .NET 10.0 API surface | `api/OpenAI.net10.0.cs` |
| .NET Standard 2.0 API surface | `api/OpenAI.netstandard2.0.cs` |

## Local Repository — Codegen Plugin

| What | Path |
|------|------|
| Codegen plugin source | `codegen/generator/src/` |
| Numeric types visitor | `codegen/generator/src/Visitors/NumericTypesVisitor.cs` |

## Available Areas

These areas map between the base spec directories, client TSP files, and C# custom code:

| Area Folder | Client TSP | C# Custom Folder |
|-------------|-----------|-------------------|
| `audio` | `audio.client.tsp` | `Audio` |
| `assistants` | `assistants.client.tsp` | `Assistants` |
| `batch` | `batch.client.tsp` | `Batch` |
| `chat` | `chat.client.tsp` | `Chat` |
| `containers` | `containers.client.tsp` | `Containers` |
| `conversations` | `conversations.client.tsp` | `Conversations` |
| `embeddings` | `embeddings.client.tsp` | `Embeddings` |
| `files` | `files.client.tsp` | `Files` |
| `fine-tuning` | `fine-tuning.client.tsp` | `FineTuning` |
| `graders` | `graders.client.tsp` | `Graders` |
| `images` | `images.client.tsp` | `Images` |
| `models` | `models.client.tsp` | `Models` |
| `moderations` | `moderations.client.tsp` | `Moderations` |
| `realtime` | — | `Realtime` |
| `responses` | `responses.client.tsp` | `Responses` |
| `vector-stores` | `vector-stores.client.tsp` | `VectorStores` |
| `videos` | `videos.client.tsp` | `Videos` |
