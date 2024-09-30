# Release History

## 2.0.0 (2024-09-30)

> [!NOTE]
> First stable version of the official OpenAI library for .NET.

### Features Added

- Support for OpenAI's latest flagship models, including GPT-4o, GPT-4o mini, o1-preview, and o1-mini
- Support for the entire OpenAI REST API, including:
  - Structured outputs
  - Reasoning tokens
  - Experimental support for Assistants beta v2
- Support for sync and async APIs
- Convenient APIs to facilitate working with streaming chat completions and assistants
- Tons of other quality-of-life features for ease of use and productivity

### Breaking Changes

> [!NOTE]
> The following breaking changes only apply when upgrading from the previous 2.0.0-beta.* versions.

- Implemented `ChatMessageContent` to encapsulate the representation of content parts in `ChatMessage`, `ChatCompletion`, and `StreamingChatCompletionUpdate`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Changed the representation of function arguments to `BinaryData` in `ChatToolCall`, `StreamingChatToolCallUpdate`, `ChatFunctionCall`, and `StreamingChatFunctionCallUpdate`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Renamed `OpenAIClientOptions`'s `ApplicationId` to `UserAgentApplicationId`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Renamed `StreamingChatToolCallUpdate`'s `Id` to `ToolCallId`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Renamed `StreamingChatCompletionUpdate`'s `Id` to `CompletionId`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Replaced `Auto` and `None` in the deprecated `ChatFunctionChoice` with `CreateAutoChoice()` and `CreateNoneChoice()`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Replaced the deprecated `ChatFunctionChoice(ChatFunction)` constructor with `CreateNamedChoice(string functionName)`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Renamed `FileClient` to `OpenAIFileClient` and the corresponding `GetFileClient()` method in `OpenAIClient` to `GetOpenAIFileClient()`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))
- Renamed `ModelClient` to `OpenAIModelClient` and the corresponding `GetModelClient()` method in `OpenAIClient` to `GetOpenAIModelClient()`. ([31c2ba6](https://github.com/openai/openai-dotnet/commit/31c2ba63c625b1b4fc2640ddf378a97e89b89167))

## 2.0.0-beta.13 (2024-09-27)

### Breaking Changes

- Refactored `ModerationResult` by merging `ModerationCategories` and `ModerationCategoryScores` into individual `ModerationCategory` properties, each with `Flagged` and `Score` properties. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed type `OpenAIFileInfo` to `OpenAIFile` and `OpenAIFileInfoCollection` to `OpenAIFileCollection`. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed type `OpenAIModelInfo` to `OpenAIModel` and `OpenAIModelInfoCollection` to `OpenAIModelCollection`. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed type `Embedding` to `OpenAIEmbedding` and `EmbeddingCollection` to `OpenAIEmbeddingCollection`. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed property `ImageUrl` to `ImageUri` and method `FromImageUrl` to `FromImageUri` in the `MessageContent` type. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed property `ParallelToolCallsEnabled` to `AllowParallelToolCalls` in the `RunCreationOptions`, `ThreadRun`, and `ChatCompletionOptions` types. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed properties `PromptTokens` to `InputTokenCount`, `CompletionTokens` to `OutputTokenCount`, and `TotalTokens` to `TotalTokenCount` in the `RunTokenUsage` and `RunStepTokenUsage` types. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed properties `InputTokens` to `InputTokenCount` and `TotalTokens` to `TotalTokenCount` in the `EmbeddingTokenUsage` type. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Renamed properties `MaxPromptTokens` to `MaxInputTokenCount` and `MaxCompletionTokens` to `MaxOutputTokenCount` in the `ThreadRun`, `RunCreationOptions`, and `RunIncompleteReason` types. ([19ceae4](https://github.com/openai/openai-dotnet/commit/19ceae44172fdc17af1f47aa30edf4a3bddcb9d6))
- Removed the `virtual` keyword from the `Pipeline` property across all clients. ([75eded5](https://github.com/openai/openai-dotnet/commit/75eded51db8c8bcec41cd894f3575374e40a4103))
- Renamed the `Granularities` property of `AudioTranscriptionOptions` to `TimestampGranularities`. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `AudioTranscriptionFormat` from an enum to an "extensible enum". ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `AudioTranslationFormat` from an enum to an "extensible enum". ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `GenerateImageFormat` from an enum to an "extensible enum". ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `GeneratedImageQuality` from an enum to an "extensible enum". ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `GeneratedImageStyle` from an enum to an "extensible enum". ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Removed method overloads in `AssistantClient` and `VectorStoreClient` that take complex parameters in favor of methods that take simple string IDs. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Updated the `TokenIds` property type in the `TranscribedSegment` type from `IReadOnlyList<int>` to `ReadOnlyMemory<int>`. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Updated the `inputs` parameter type in the `GenerateEmbeddings` and `GenerateEmbeddingsAsync` methods of `EmbeddingClient` from `IEnumerable<IEnumerable<int>>` to `IEnumerable<ReadOnlyMemory<int>>`. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `ChatMessageContentPartKind` from an extensible enum to an enum. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `ChatToolCallKind` from an extensible enum to an enum. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `ChatToolKind` from an extensible enum to an enum. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `OpenAIFilePurpose` from an extensible enum to an enum. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Changed `OpenAIFileStatus` from an extensible enum to an enum. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Renamed `OpenAIFilePurpose` to `FilePurpose`. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Renamed `OpenAIFileStatus` to `FileStatus`. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))
- Removed constructors that take string API key and options. ([a330c2e](https://github.com/openai/openai-dotnet/commit/a330c2e703e48179991905e991b0f4186a017198))

## 2.0.0-beta.12 (2024-09-20)

### Features Added

- The library now includes support for the new [OpenAI o1](https://openai.com/o1/) model family. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `ChatCompletionOptions` will automatically apply its `MaxOutputTokenCount` value (renamed from `MaxTokens`) to the new `max_completion_tokens` request body property
  - `Usage` includes a new `OutputTokenDetails` property with a `ReasoningTokenCount` value that will reflect `o1` model use of this new subcategory of output tokens.
  - Note that `OutputTokenCount` (`completion_tokens`) is the *sum* of displayed tokens generated by the model *and* (when applicable) these new reasoning tokens
- Assistants file search now includes support for `RankingOptions`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - Use of the `include[]` query string parameter and retrieval of run step detail result content is currently only available via protocol methods
- Added support for the Uploads API in `FileClient`. This `Experimental` feature allows uploading large files in multiple parts. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - The feature is supported by the `CreateUpload`, `AddUploadPart`, `CompleteUpload`, and `CancelUpload` protocol methods.

### Breaking Changes

- Renamed `ChatMessageContentPart`'s `CreateTextMessageContentPart` factory method to `CreateTextPart`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ChatMessageContentPart`'s `CreateImageMessageContentPart` factory method to `CreateImagePart`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ChatMessageContentPart`'s `CreateRefusalMessageContentPart` factory method to `CreateRefusalPart`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ImageChatMessageContentPartDetail` to `ChatImageDetailLevel`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed `ChatMessageContentPart`'s `ToString` overload. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed the `MaxTokens` property in `ChatCompletionOptions` to `MaxOutputTokenCount`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed properties in `ChatTokenUsage`:
  - `InputTokens` is renamed to `InputTokenCount`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `OutputTokens` is renamed to `OutputTokenCount`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `TotalTokens` is renamed to `TotalTokenCount`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed the common `ListOrder` enum from the top-level `OpenAI` namespace in favor of individual enums in their corresponding sub-namespace. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed the `PageSize` property to `PageSizeLimit`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Updated deletion methods to return a result object instead of a `bool`. Affected methods:
  - `DeleteAssitant`, `DeleteMessage`, and `DeleteThread` in `AssistantClient`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `DeleteVectorStore` and `RemoveFileFromStore` in `VectorStoreClient`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `DeleteModel` in `ModelClient`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
  - `DeleteFile` in `FileClient`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed setters from collection properties. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ChatTokenLogProbabilityInfo` to `ChatTokenLogProbabilityDetails`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ChatTokenTopLogProbabilityInfo` to `ChatTokenTopLogProbabilityDetails`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed the `Utf8ByteValues` properties of `ChatTokenLogProbabilityDetails` and `ChatTokenTopLogProbabilityDetails` to `Utf8Bytes` and changed their type from `IReadOnlyList<int>` to `ReadOnlyMemory<byte>?`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed the `Start` and `End` properties of `TranscribedSegment` and `TranscribedWord` to `StartTime` and `EndTime`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Changed the type of `TranscribedSegment`'s `AverageLogProbability` and `NoSpeechProbability` properties from `double` to `float`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Changed the type of `TranscribedSegment`'s `SeekOffset` property from `long` to `int`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Changed the type of `TranscribedSegment`'s `TokenIds` property from `IReadOnlyList<long>` to `IReadOnlyList<int>`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Updated the `Embedding.Vector` property to the `Embedding.ToFloats()` method. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed the optional parameter from the constructors of `VectorStoreCreationHelper`, `AssistantChatMessage`, and `ChatFunction`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed the optional `purpose` parameter from `FileClient.GetFilesAsync` and `FileClient.GetFiles` methods, and added overloads where `purpose` is required. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Renamed `ModerationClient`'s `ClassifyTextInput` methods to `ClassifyText`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Removed duplicated `Created` property from `GeneratedImageCollection`. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))

### Bugs Fixed

- Addressed an issue that caused multi-page queries of fine-tuning jobs, checkpoints, and events to fail. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- `ChatCompletionOptions` can now be serialized via `ModelReaderWriter.Write()` prior to calling `CompleteChat` using the options. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))

### Other Changes

- Added support for `CancellationToken` to `ModelClient` methods. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))
- Applied the `Obsolete` attribute where appropriate to align with the existing deprecations in the REST API. ([2ab1a94](https://github.com/openai/openai-dotnet/commit/2ab1a94269125e6bed45d134a402ad8addd8fea4))

## 2.0.0-beta.11 (2024-09-03)

### Features Added

- Added the `OpenAIChatModelFactory` in the `OpenAI.Chat` namespace (a static class that can be used to instantiate OpenAI models for mocking in non-live test scenarios). ([79014ab](https://github.com/openai/openai-dotnet/commit/79014abc01a00e13d5a334d3f6529ed590b8ee98))

### Breaking Changes

- Updated fine-tuning pagination methods `GetJobs`, `GetEvents`, and `GetJobCheckpoints` to return `IEnumerable<ClientResult>` instead of `ClientResult`. ([5773292](https://github.com/openai/openai-dotnet/commit/57732927575c6c48f30bded0afb9f5b16d4f30da))
- Updated the batching pagination method `GetBatches` to return `IEnumerable<ClientResult>` instead of `ClientResult`. ([5773292](https://github.com/openai/openai-dotnet/commit/57732927575c6c48f30bded0afb9f5b16d4f30da))
- Changed `GeneratedSpeechVoice` from an enum to an "extensible enum". ([79014ab](https://github.com/openai/openai-dotnet/commit/79014abc01a00e13d5a334d3f6529ed590b8ee98))
- Changed `GeneratedSpeechFormat` from an enum to an "extensible enum". ([cc9169a](https://github.com/openai/openai-dotnet/commit/cc9169ad2ff92bb7312eed3b7e64e45da5da1d18))
- Renamed `SpeechGenerationOptions`'s `Speed` property to `SpeedRatio`. ([cc9169a](https://github.com/openai/openai-dotnet/commit/cc9169ad2ff92bb7312eed3b7e64e45da5da1d18))

### Bugs Fixed

- Corrected an internal deserialization issue that caused recent updates to Assistants `file_search` to fail when streaming a run. Strongly typed support for `ranking_options` is not included but will arrive soon. ([cc9169a](https://github.com/openai/openai-dotnet/commit/cc9169ad2ff92bb7312eed3b7e64e45da5da1d18))
- Mitigated a .NET runtime issue that prevented `ChatResponseFormat` from serializing correct on targets including Unity. ([cc9169a](https://github.com/openai/openai-dotnet/commit/cc9169ad2ff92bb7312eed3b7e64e45da5da1d18))

### Other Changes

- Reverted the removal of the version path parameter "v1" from the default endpoint URL. ([583e9f6](https://github.com/openai/openai-dotnet/commit/583e9f6f519feeee0e2907e80bf7d5bf8302d93f))
- Added the `Experimental` attribute to the following APIs:
  - All public APIs in the `OpenAI.Assistants` namespace. ([79014ab](https://github.com/openai/openai-dotnet/commit/79014abc01a00e13d5a334d3f6529ed590b8ee98))
  - All public APIs in the `OpenAI.VectorStores` namespace. ([79014ab](https://github.com/openai/openai-dotnet/commit/79014abc01a00e13d5a334d3f6529ed590b8ee98))
  - All public APIs in the `OpenAI.Batch` namespace. ([0f5e024](https://github.com/openai/openai-dotnet/commit/0f5e0249cffd42755fc9a820e65fb025fd4f986c))
  - All public APIs in the `OpenAI.FineTuning` namespace. ([0f5e024](https://github.com/openai/openai-dotnet/commit/0f5e0249cffd42755fc9a820e65fb025fd4f986c))
  - The `ChatCompletionOptions.Seed` property. ([0f5e024](https://github.com/openai/openai-dotnet/commit/0f5e0249cffd42755fc9a820e65fb025fd4f986c))

## 2.0.0-beta.10 (2024-08-26)

### Breaking Changes

- Renamed `AudioClient`'s `GenerateSpeechFromText` methods to simply `GenerateSpeech`. ([d84bf54](https://github.com/openai/openai-dotnet/commit/d84bf54df14ddac4c49f6efd61467b600d34ecd7))
- Changed the type of `OpenAIFileInfo`'s `SizeInBytes` property from `long?` to `int?`. ([d84bf54](https://github.com/openai/openai-dotnet/commit/d84bf54df14ddac4c49f6efd61467b600d34ecd7)) 

### Bugs Fixed

- Fixed a newly introduced bug ([#185](https://github.com/openai/openai-dotnet/pull/185)) where providing `OpenAIClientOptions` to a top-level `OpenAIClient` did not carry over to scenario clients (e.g. `ChatClient`) created via that top-level client ([d84bf54](https://github.com/openai/openai-dotnet/commit/d84bf54df14ddac4c49f6efd61467b600d34ecd7))

### Other Changes

- Removed the version path parameter "v1" from the default endpoint URL. ([d84bf54](https://github.com/openai/openai-dotnet/commit/d84bf54df14ddac4c49f6efd61467b600d34ecd7))

## 2.0.0-beta.9 (2024-08-23)

### Features Added

- Added support for the new [structured outputs](https://platform.openai.com/docs/guides/structured-outputs/introduction) response format feature, which enables chat completions, assistants, and tools on each of those clients to provide a specific JSON Schema that generated content should adhere to. ([3467b53](https://github.com/openai/openai-dotnet/commit/3467b535c918e72237a4c0dc36d4bda5548edb7a))
  - To enable top-level structured outputs for response content, use `ChatResponseFormat.CreateJsonSchemaFormat()` and `AssistantResponseFormat.CreateJsonSchemaFormat()` as the `ResponseFormat` in method options like `ChatCompletionOptions`
  - To enable structured outputs for function tools, set `StrictParameterSchemaEnabled` to `true` on the tool definition
  - For more information, please see [the new section in readme.md](readme.md#how-to-use-structured-outputs)
- Chat completions: the request message types of `AssistantChatMessage`, `SystemChatMessage`, and `ToolChatMessage` now support array-based content part collections in addition to simple string input. ([3467b53](https://github.com/openai/openai-dotnet/commit/3467b535c918e72237a4c0dc36d4bda5548edb7a))
- Added the following model factories (static classes that can be used to instantiate OpenAI models for mocking in non-live test scenarios):
  - `OpenAIAudioModelFactory` in the `OpenAI.Audio` namespace ([3284295](https://github.com/openai/openai-dotnet/commit/3284295e7fd9922a3395d921513473bcb483655e))
  - `OpenAIEmbeddingsModelFactory` in the `OpenAI.Embeddings` namespace ([3284295](https://github.com/openai/openai-dotnet/commit/3284295e7fd9922a3395d921513473bcb483655e))
  - `OpenAIFilesModelFactory` in the `OpenAI.Files` namespace ([b1ce397](https://github.com/openai/openai-dotnet/commit/b1ce397ff4f9a55db797167be9e86e138ed5d403))
  - `OpenAIImagesModelFactory` in the `OpenAI.Images` namespace ([3284295](https://github.com/openai/openai-dotnet/commit/3284295e7fd9922a3395d921513473bcb483655e))
  - `OpenAIModelsModelFactory` in the `OpenAI.Models` namespace ([b1ce397](https://github.com/openai/openai-dotnet/commit/b1ce397ff4f9a55db797167be9e86e138ed5d403))
  - `OpenAIModerationsModelFactory` in the `OpenAI.Moderations` namespace ([b1ce397](https://github.com/openai/openai-dotnet/commit/b1ce397ff4f9a55db797167be9e86e138ed5d403))

### Breaking Changes

- Removed client constructors that do not explicitly take an API key parameter or an endpoint via an `OpenAIClientOptions` parameter, making it clearer how to appropriately instantiate a client. ([13a9c68](https://github.com/openai/openai-dotnet/commit/13a9c68647c8d54475f1529a63b13ad711bd4ba6))
- Removed the endpoint parameter from all client constructors, making it clearer that an alternative endpoint must be specified via the `OpenAIClientOptions` parameter. ([13a9c68](https://github.com/openai/openai-dotnet/commit/13a9c68647c8d54475f1529a63b13ad711bd4ba6))
- Removed `OpenAIClient`'s `Endpoint` `protected` property. ([13a9c68](https://github.com/openai/openai-dotnet/commit/13a9c68647c8d54475f1529a63b13ad711bd4ba6))
- Made `OpenAIClient`'s constructor that takes a `ClientPipeline` parameter `protected internal` instead of just `protected`. ([13a9c68](https://github.com/openai/openai-dotnet/commit/13a9c68647c8d54475f1529a63b13ad711bd4ba6))
- Renamed the `User` property in applicable Options classes to `EndUserId`, making its purpose clearer. ([13a9c68](https://github.com/openai/openai-dotnet/commit/13a9c68647c8d54475f1529a63b13ad711bd4ba6))

### Bugs Fixed

- The `Assistants` namespace `VectorStoreCreationHelper` type now properly includes a `ChunkingStrategy` property. ([3467b53](https://github.com/openai/openai-dotnet/commit/3467b535c918e72237a4c0dc36d4bda5548edb7a))

### Other Changes

- `ChatCompletion.ToString()` will no longer throw an exception when no content is present, as is the case for tool calls. Additionally, if a tool call is present with no content, `ToString()` will return the serialized form of the first available tool call. ([3467b53](https://github.com/openai/openai-dotnet/commit/3467b535c918e72237a4c0dc36d4bda5548edb7a))

## 2.0.0-beta.8 (2024-07-31)

### Breaking Changes

- Changed name of return types from methods returning streaming collections from `ResultCollection` to `CollectionResult`. ([7bdecfd](https://github.com/openai/openai-dotnet/commit/7bdecfd8d294be933c7779c7e5b6435ba8a8eab0))
- Changed return types from methods returning paginated collections from `PageableCollection` to `PageCollection`. ([7bdecfd](https://github.com/openai/openai-dotnet/commit/7bdecfd8d294be933c7779c7e5b6435ba8a8eab0))
- Users must now call `GetAllValues` on the collection of pages to enumerate collection items directly. Corresponding protocol methods return `IEnumerable<ClientResult>` where each collection item represents a single service response holding a page of values. ([7bdecfd](https://github.com/openai/openai-dotnet/commit/7bdecfd8d294be933c7779c7e5b6435ba8a8eab0))
- Updated `VectorStoreFileCounts` and `VectorStoreFileAssociationError` types from `readonly struct` to `class`. ([58f93c8](https://github.com/openai/openai-dotnet/commit/58f93c8d5ea080adfee8b37ae3cc034ebb06c79f))

### Bugs Fixed

- ([#49](https://github.com/openai/openai-dotnet/issues/49)) Fixed a bug with extensible enums implementing case-insensitive equality but case-sensitive hash codes. ([0c12500](https://github.com/openai/openai-dotnet/commit/0c125002ffd791594597ef837f4d10582bdff004))
- ([#57](https://github.com/openai/openai-dotnet/issues/57)) Fixed a bug with requests URIs with query string parameter potentially containing a malformed double question mark (`??`) on .NET Framework (net481). ([0c12500](https://github.com/openai/openai-dotnet/commit/0c125002ffd791594597ef837f4d10582bdff004))
- Added optional `CancellationToken` parameters to methods for `AssistantClient` and `VectorStore` client, consistent with past changes in [19a65a0](https://github.com/openai/openai-dotnet/commit/19a65a0a943fa3bef1ec8504708aaa526a1ee03a). ([d77539c](https://github.com/openai/openai-dotnet/commit/d77539ca04467c166f848953eb866012a265555c))
- Fixed Assistants `FileSearchToolDefinition`'s `MaxResults` parameter to appropriately serialize and deserialize the value ([d77539c](https://github.com/openai/openai-dotnet/commit/d77539ca04467c166f848953eb866012a265555c))
- Added missing `[EditorBrowsable(EditorBrowsableState.Never)]` attributes to `AssistantClient` protocol methods, which should improve discoverability of the strongly typed methods. ([d77539c](https://github.com/openai/openai-dotnet/commit/d77539ca04467c166f848953eb866012a265555c))

### Other Changes

- Removed the usage of `init` and updated properties to use `set`. ([58f93c8](https://github.com/openai/openai-dotnet/commit/58f93c8d5ea080adfee8b37ae3cc034ebb06c79f))

## 2.0.0-beta.7 (2024-06-24)

### Bugs Fixed

- ([#84](https://github.com/openai/openai-dotnet/issues/84)) Fixed a `NullReferenceException` thrown when adding the custom headers policy while `OpenAIClientOptions` is null ([0b97311](https://github.com/openai/openai-dotnet/commit/0b97311f58dfb28bd883d990f68d548da040a807))

## 2.0.0-beta.6 (2024-06-21)

### Features Added

- `OrganizationId` and `ProjectId` are now present on `OpenAIClientOptions`. When instantiating a client, providing an instance of `OpenAIClientOptions` with these properties set will cause the client to add the appropriate request headers for org/project, eliminating the need to manually configure the headers. ([9ee7dff](https://github.com/openai/openai-dotnet/commit/9ee7dff064a9412c069a793ff62096b8db4aa43d))

### Bugs Fixed

- ([#72](https://github.com/openai/openai-dotnet/issues/72)) Fixed `filename` request encoding in operations using `multipart/form-data`, including `files` and `audio` ([2ba8e69](https://github.com/openai/openai-dotnet/commit/2ba8e69512e187ea0b761edda8bce2cd5c79c58a))
- ([#79](https://github.com/openai/openai-dotnet/issues/79)) Fixed hard-coded `user` role for caller-created Assistants API messages on threads ([d665b61](https://github.com/openai/openai-dotnet/commit/d665b61fc7ef1ada00a8ef5c00d1a47d276be032))
- Fixed non-streaming Assistants API run step details not reporting code interpreter logs when present ([d665b61](https://github.com/openai/openai-dotnet/commit/d665b61fc7ef1ada00a8ef5c00d1a47d276be032))

### Breaking Changes

**Assistants (beta)**:

- `AssistantClient.CreateMessage()` and the explicit constructor for `ThreadInitializationMessage` now require a `MessageRole` parameter. This properly enables the ability to create an Assistant message representing conversation history on a new thread. ([d665b61](https://github.com/openai/openai-dotnet/commit/d665b61fc7ef1ada00a8ef5c00d1a47d276be032))

## 2.0.0-beta.5 (2024-06-14)

### Features Added

- API updates, current to [openai/openai-openapi@dd73070b](https://github.com/openai/openai-openapi/commit/dd73070b1d507645d24c249a63ebebd3ec38c0cb) ([1af6569](https://github.com/openai/openai-dotnet/commit/1af6569e2ceae9d840b8826e42d7e3b2569b43f6))
  - This includes `MaxResults` for Assistants `FileSearchToolDefinition`, `ParallelToolCallsEnabled` for function tools in Assistants and Chat, and `FileChunkingStrategy` for Assistants VectorStores
- Optional `CancellationToken` parameters are now directly present on most methods, eliminating the need to use protocol methods ([19a65a0](https://github.com/openai/openai-dotnet/commit/19a65a0a943fa3bef1ec8504708aaa526a1ee03a))

### Bugs Fixed

- ([#30](https://github.com/openai/openai-dotnet/issues/30)) Mitigated a .NET runtime issue that prevented chat message content and several other types from serializing correct on targets including mono and wasm ([896b9e0](https://github.com/openai/openai-dotnet/commit/896b9e0bc60f0ace90bd0d1af1254cf2680f8df6))
- Assistants: Fixed an issue that threw an exception when receiving code interpreter run step logs when streaming a run ([207d597](https://github.com/openai/openai-dotnet/commit/207d59762e7eeb666b7ab2728a0bbee7c0cdd918))
- Fixed a concurrency condition that could cause `multipart/form-data` requests to no longer generate random content part boundaries (no direct scenario impact) ([7cacdee](https://github.com/openai/openai-dotnet/commit/7cacdee2366df3cfa7e6c43bb050da54d23f8db9))

### Breaking Changes

**Assistants (beta)**:

- `InputQuote` is removed from Assistants `TextAnnotation` and `TextAnnotationUpdate`, per [openai/openai-openapi@dd73070b](https://github.com/openai/openai-openapi/commit/dd73070b1d507645d24c249a63ebebd3ec38c0cb) ([1af6569](https://github.com/openai/openai-dotnet/commit/1af6569e2ceae9d840b8826e42d7e3b2569b43f6))

### Other Changes

- Added an environment-variable-based test project to the repository with baseline scenario coverage ([db6328a](https://github.com/openai/openai-dotnet/commit/db6328accdd7927f19915cdc5412eb841f2447c1))
- Build/analyzer warnings cleaned up throughout the project ([45fc4d7](https://github.com/openai/openai-dotnet/commit/45fc4d72c12314aea83264ebe2e1dc18870e5c06), [b1fa082](https://github.com/openai/openai-dotnet/commit/b1fa0828a875906ef33ffe43ff1cd1a85fbd1a60), [22ab606](https://github.com/openai/openai-dotnet/commit/22ab606b867bbe0ea7f6918843dbc5e11dfe78eb))
- Proactively aligned library's implementation of server-sent event (SSE) handling with the source of the incoming `System.Net.ServerSentEvents` namespace ([674e0f7](https://github.com/openai/openai-dotnet/commit/674e0f773b26a22eb039e879539c4c7a44fdffdd))

## 2.0.0-beta.4 (2024-06-10)

### Features Added

- Added new, built-in helpers to simplify the use of text-only message content ([1c40de6](https://github.com/openai/openai-dotnet/commit/1c40de673a67ddf834b673aaabb94b2c42076e03))

### Bugs Fixed

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