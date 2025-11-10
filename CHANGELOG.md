# Release History

## 2.6.0 (2025-10-31)

### Acknowledgments

Thank you to our developer community members who helped to make the OpenAI client library better with their contributions to this release:

- Maksim Kurnakov _([GitHub](https://github.com/kurnakovv))_

### Features Added

- OpenAI.Chat:
  - Added the `Minimal` property to `ChatReasoningEffortLevel`. _(A community contribution, courtesy of [kurnakovv](https://github.com/kurnakovv))_
  - Added support for System.Client.Model's `JsonPatch`, which enables users to get and set additional JSON properties in response and request payloads.
    - See the following examples for more information:
      - [AdditionalProperties](https://github.com/openai/openai-dotnet/blob/main/examples/Chat/Example10_AdditionalProperties.cs)
      - [AdditionalPropertiesAsync](https://github.com/openai/openai-dotnet/blob/main/examples/Chat/Example10_AdditionalPropertiesAsync.cs)
      - Go to the OpenAI.Responses section in this changelog for more examples that can be extrapolated to Chat.
- OpenAI.Conversations:
  - Introduced the new `ConversationClient` to support the Conversations API with protocol methods for the following operations:
    - `CreateConversation` and `CreateConversationAsync`
    - `GetConversation` and `GetConversationAsync`
    - `UpdateConversation` and `UpdateConversationAsync`
    - `DeleteConversation` and `DeleteConversationAsync`
    - `CreateConversationItems` and `CreateConversationItemsAsync`
    - `GetConversationItems` and `GetConversationItemsAsync`
    - `DeleteConversationItem` and `DeleteConversationItemAsync`
- OpenAI.Embeddings:
  - Added support for System.Client.Model's `JsonPatch`, which enables users to get and set additional JSON properties in response and request payloads.
    - Go to the OpenAI.Chat and OpenAI.Responses section in this changelog for examples that can be extrapolated to Embeddings.
- OpenAI.Responses:
  - Added the `Minimal` property to `ResponseReasoningEffortLevel`. _(A community contribution, courtesy of [kurnakovv](https://github.com/kurnakovv))_
  - Added the `ContainerFileCitationMessageAnnotation` class which is derived from `ResponseMessageAnnotation` and is used to cite files in the container of the Code Interpreter tool.
  - Enabled support for the Image Generation tool, which can be used to generate images using models like `gpt-image-1`.
    - Users can add the new `ImageGenerationTool` to the `Tools` property of their `ResponseCreationOptions` and configure it using properties such as `Background`, `Quality`, `Size`, and more.
  - Added support for System.Client.Model's `JsonPatch`, which enables users to get and set additional JSON properties in response and request payloads.
    - See the following examples for more information:
      - [InputAdditionalProperties](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example07_InputAdditionalProperties.cs)
      - [InputAdditionalPropertiesAsync](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example07_InputAdditionalPropertiesAsync.cs)
      - [OutputAdditionalProperties](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example08_OutputAdditionalProperties.cs)
      - [OutputAdditionalPropertiesAsync](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example08_OutputAdditionalPropertiesAsync.cs)
      - [ModelOverridePerRequest](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example09_ModelOverridePerRequest.cs)
      - [ModelOverridePerRequestAsync](https://github.com/openai/openai-dotnet/blob/main/examples/Responses/Example09_ModelOverridePerRequestAsync.cs)
      - Go to the OpenAI.Chat section for more examples that can be extrapolated to Responses.
- OpenAI.Videos:
  - Introduced the new `VideoClient` to support the Videos API with protocol methods for the following operations:
    - `CreateVideo` and `CreateVideoAsync`
    - `GetVideo` and `GetVideoAsync`
    - `DeleteVideo` and `DeleteVideoAsync`
    - `DownloadVideo` and `DownloadVideoAsync`
    - `GetVideos` and `GetVideosAsync`
    - `CreateVideoRemix` and `GetVideoRemixAsync`

### Bugs Fixed

- OpenAI.Audio:
  - Added the explicit conversion operators from `ClientResult` that were missing in the `AudioTranscription` and `AudioTranslation` classes.
- OpenAI.Chat:
  - Added the `ContentParts` property that was missing in the `ChatCompletionMessageListDatum` class.

### Breaking Changes in Preview APIs

- OpenAI.Containers:
  - Renamed the `GetContainerFileContent` and `GetContainerFileContentAsync` methods of `ContainerClient` to `DownloadContainerFile` and `DownloadContainerFileAsync`.
- OpenAI.Responses:
  - Removed the duplicated `GetInputItems` and `GetInputItemsAsync` methods of the `OpenAIResponseClient` in favor of the existing `GetResponseInputItems` and `GetResponseInputItemsAsync` methods.

## 2.5.0 (2025-09-23)

### Acknowledgments

Thank you to our developer community members who helped to make the OpenAI client library better with their contributions to this release:

- Benjamin Pinter _([GitHub](https://github.com/BenjaminDavidPinter))_

### Features Added

- OpenAI.Responses:
  - Added the `Model` property to `OpenAIResponseClient`. _(A community contribution, courtesy of [BenjaminDavidPinter](https://github.com/BenjaminDavidPinter))_
  - Added the `ServiceDescription` property to `McpTool`.
  - Enabled support for connectors, which are OpenAI-maintained MCP wrappers for popular services like Microsoft Outlook or Dropbox.
    - Added the `ConnectorId` property to `McpTool`.
  - Enabled support for authentication with remote MCP servers.
    - Added the `AuthorizationToken` property to `McpTool`.
  - Enabled support for the Code Interpreter tool, which allows models to write and run Python code in a sandboxed environment to solve complex problems in domains like data analysis, coding, and math.
    - Users can add the new `CodeInterpreterTool` to the `Tools` property of their `ResponseCreationOptions` and configure it.
      - Use the `Container` property to configure the sandboxed environment, including any files that should be made available.

### Bugs Fixed

- OpenAI.Responses:
  - Fixed an issue with the constructor of `McpToolCallApprovalRequestItem` not taking the item ID as a parameter. MCP approval requests are correlated to MCP approval responses using this ID, which implies that this ID should be required.
  - Fixed an issue with some of the MCP-related `StreamingResponseUpdate` classes missing the `ItemId` and `OutputIndex` properties.

## 2.4.0 (2025-09-05)

### Features Added

- OpenAI.Audio:
  - Added the `Endpoint` property to `AudioClient`.
- OpenAI.Batch:
  - Added the `Endpoint` property to `BatchClient`.
- OpenAI.Chat:
  - Added the `Endpoint` property to `ChatClient`.
  - Added the `ServiceTier` property to `ChatCompletionOptions`, `ChatCompletion`, and `StreamingChatCompletionUpdate` to configure the policy that the server will use to process the request in terms of pricing, performance, etc.
  - Added an explicit conversion operator to `ChatCompletion` and `ChatCompletionDeletionResult` to convert from `ClientResult`.
- OpenAI.Containers:
  - Added the `Endpoint` property to `ContainerClient`.
  - Added convenience counterparts to the protocol methods of `ContainerClient` that use the strongly-typed model classes.
  - Added automatic pagination support to the following methods of `ContainerClient`:
    - `GetContainers` and `GetContainersAsync`
    - `GetContainerFiles` and `GetContainerFilesAsync`
  - Added an explicit conversion operator to `ContainerResource`, `ContainerFileResource`, `DeleteContainerResponse`, and `DeleteContainerFileResponse` to convert from `ClientResult`.
- OpenAI.Embeddings:
  - Added the `Endpoint` property to `EmbeddingsClient`.
  - Added an explicit conversion operator to `OpenAIEmbeddingCollection` to convert from `ClientResult`.
- OpenAI.Evals:
  - Added the `Endpoint` property to `EvaluationClient`.
- OpenAI.Files:
  - Added the `Endpoint` property to `OpenAIFileClient`.
  - Added an explicit conversion operator to `OpenAIFile`, `OpenAIFileCollection`, and `FileDeletionResult` to convert from `ClientResult`.
- OpenAI.FineTuning:
  - Added the `Endpoint` property to `FineTuningClient`.
- OpenAI.Graders:
  - Added the `Endpoint` property to `GraderClient`.
  - Added an explicit conversion operator to `RunGraderResponse` and `ValidateGraderResponse` to convert from `ClientResult`.
- OpenAI.Images:
  - Added the `Endpoint` property to `ImageClient`.
  - Added an explicit conversion operator to `GeneratedImageCollection` to convert from `ClientResult`.
  - Added the `Background` property to `ImageEditOptions` to set transparency for the background of the generated image(s).
  - Added the `Quality` property to `ImageEditOptions` to set the quality of the generated image(s).
- OpenAI.Images:
  - Added the `Endpoint` property to `ImageClient`.
  - Added an explicit conversion operator to `GeneratedImageCollection` to convert from `ClientResult`.
  - Added the `Background` property to `ImageEditOptions` to set transparency for the background of the generated image(s).
  - Added the `Quality` property to `ImageEditOptions` to set the quality of the generated image(s).
- OpenAI.Models:
  - Added the `Endpoint` property to `OpenAIModelClient`.
  - Added an explicit conversion operator to `OpenAIModel`, `OpenAIModelCollection`, and `ModelDeletionResult` to convert from `ClientResult`.
- OpenAI.Moderations:
  - Added the `Endpoint` property to `ModerationClient`.
  - Added an explicit conversion operator to `ModerationResultCollection` to convert from `ClientResult`.
- OpenAI.Realtime:
  - Added a constructor that can take a string API key to `RealtimeClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `RealtimeClient`.
  - Added the `Endpoint` property to `RealtimeClient`.
  - Replaced the `RequestOptions` parameter of the following methods of `RealtimeClient` for a new `RealtimeSessionOptions` parameter and a `CancellationToken` parameter:
    - `StartConversationSession` and `StartConversationSessionAsync`
    - `StartTranscriptionSession` and `StartTranscriptionSessionAsync`
    - `StartSession` and `StartSessionAsync`
- OpenAI.Responses:
  - Added the `Endpoint` property to `OpenAIResponseClient`.
  - Added an explicit conversion operator to `OpenAIResponse` to convert from `ClientResult`.
  - Added new classes derived from `ResponseTool` to facilitate certain scenarios:
    - `FunctionTool`
    - `FileSearchTool`
    - `WebSearchTool`
    - `ComputerTool`
  - Added initial support for integrating with remote MCP servers via the Responses API in streaming and non-streaming scenarios.
    - Users can add the new `McpTool` to the `Tools` property of their `ResponseCreationOptions` and configure it.
      - Use the `AllowedTools` property to limit which of the server tools can be called by the model.
      - Use the `ToolCallApprovalPolicy` property to specify which tools require an explicit approval before being called by the model.
    - Support for selecting the `McpTool` via the `ToolChoice` property is coming soon.
    - Support for configuring the `McpTool` with an access token that can be used to authenticate with the remote MCP server is coming soon.
    - Support for connectors is coming soon.
  - Added new classes derived from `ResponseMessageAnnotation` to facilitate certain scenarios:
    - `FileCitationMessageAnnotation`
    - `FilePathMessageAnnotation`
    - `UriCitationMessageAnnotation`
  - Added the `ServiceTier` property to `ResponseCreationOptions`, `OpenAIResponse`, and `StreamingChatCompletionUpdate` to configure the policy that the server will use to process the request in terms of pricing, performance, etc.
- OpenAI.VectorStores:
  - Added the `Endpoint` property to `OpenAIResponseClient`.
  - Added an explicit conversion operator to `VectorStore`, `VectorStoreFile`, `VectorStoreFileBatch`, `VectorStoreDeletionResult`, and `FileFromStoreRemovalResult` to convert from `ClientResult`.

### Bug Fixed

- OpenAI.Realtime:
  - Fixed an issue with the classes derived from `ResponseItem` (such as `ReasoningResponseItem`) missing some constructors or property setters, which made it difficult to use them as inputs.
  - Fixed an issue with the HTTP pipeline of the `RealtimeClient` that was preventing the following methods from working correctly:
    - `CreateEphemeralToken` and `CreateEphemeralTokenAsync`
    - `CreateEphemeralTranscriptionToken` and `CreateEphemeralTranscriptionTokenAsync`

### Breaking Changes in Preview APIs

- OpenAI.Chat:
  - Changed the type of the `options` parameter of the `GetChatCompletionMessages` method of the `ChatClient` from `ChatCompletionCollectionOptions` to `ChatCompletionMessageCollectionOptions`.
- OpenAI.Realtime:
  - Replaced the `RequestOptions` parameter for a new `RealtimeSessionOptions` parameter and a `CancellationToken` parameter.
- OpenAI.Responses:
  - Renamed the `Background` property to `BackgroundModeEnabled` for clarity.
  - Renamed the `ComputerOutput` class to `ComputerCallOutput`.
  - Changed the type of the `Delta` property of `StreamingResponseFunctionCallArgumentsDeltaUpdate` from `string` to `BinaryData`.
  - Changed the type of the `Arguments` property of `StreamingResponseFunctionCallArgumentsDoneUpdate` from `string` to `BinaryData` and renamed it to `FunctionArguments`.
  - Renamed the `WebSearchContextSize` class to `WebSearchToolContextSize`.
  - Renamed the `WebSearchUserLocation` class to `WebSearchToolLocation`.
  - Refactored the factory methods of `ResponseItem`.
  - Refactored the factory methods of `ResponseTool`.
  - Removed the properties of `ResponseMessageAnnotation` except for the `Kind` property and moved them to the new derived types.
- OpenAI.VectorStores:
  - Removed the `OperationResult` pattern along with the `CreateVectorStoreOperation` `AddFileToVectorStoreOperation`, and `CreateBatchFileJobOperation` classes.
  - Renamed the `VectorStoreBatchFileJob` class to `VectorStoreFileBatch`
  - Renamed the `VectorStoreFileAssociation` class to `VectorStoreFile`
  - Renamed the `VectorStoreFileAssociationError` class to `VectorStoreFileError`
  - Renamed the `VectorStoreFileAssociationStatus` class to `VectorStoreFileStatus`
  - Renamed the `VectorStoreFileAssociationErrorCode` class to `VectorStoreFileErrorCode`
  - Renamed the `VectorStoreFileAssociationCollectionOptions` class to `VectorStoreFileCollectionOptions`
  - Renamed the `VectorStoreFileAssociationCollectionOrder` class to `VectorStoreFileCollectionOrder`
  - Renamed the `CancelBatchFileJob` and `CancelBatchFileJobAsync` methods of `VectorStoreClient` to `CancelVectorStoreFileBatch` and `CancelVectorStoreFileBatchAsync`
  - Renamed the `CreateBatchFileJob` and `CreateBatchFileJobAsync` methods of `VectorStoreClient` to `AddFileBatchToVectorStore` and `AddFileBatchToVectorStoreAsync`
  - Renamed the `GetBatchFileJob` and `GetBatchFileJobAsync` methods of `VectorStoreClient` to `GetVectorStoreFileBatch` and `GetVectorStoreFileBatchAsync`
  - Renamed the `GetFileAssociation` and `GetFileAssociationAsync` methods of `VectorStoreClient` to `GetVectorStoreFile` and `GetVectorStoreFileAsync`
  - Renamed the `GetFileAssociations` and `GetFileAssociationsAsync` methods of `VectorStoreClient` to `GetVectorStoreFiles` and `GetVectorStoreFilesAsync`
  - Renamed the `GetFileAssociationsInBatch` and `GetFileAssociationsInBatchAsync` methods of `VectorStoreClient` to `GetVectorStoreFilesInBatch` and `GetVectorStoreFilesInBatchAsync`
  - Renamed the `RemoveFileFromStore` and `RemoveFileFromStoreAsync` methods of `VectorStoreClient` to `RemoveFileFromVectorStore` and `RemoveFileFromVectorStoreAsync`

### Other Changes

- Updated the `System.ClientModel` dependency to version 1.6.1.

## 2.3.0 (2025-08-01)

### Features Added

- OpenAI.Audio:
  - Added the `Model` property to `AudioClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `AudioClient`.
- OpenAI.Batch:
  - Added new methods to `BatchClient`:
    - `GetBatch` and `GetBatchAsync`
  - Added constructors that can take a custom `AuthenticationPolicy` to `BatchClient`.
- OpenAI.Chat:
  - Added new methods to `ChatClient`:
    - `UpdateChatCompletion` and `UpdateChatCompletionAsync`
    - `GetChatCompletions` and `GetChatCompletionsAsync`
    - `GetChatCompletionMessages` and `GetChatCompletionMessagesAsync`
  - Added the `Model` property to `ChatClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `ChatClient`.
- OpenAI.Containers:
  - Introduced the new `ContainersClient` to support the Containers API with protocol methods for the following operations:
    - `CreateContainer` and `CreateContainerAsync`
    - `GetContainers` and `GetContainersAsync`
    - `GetContainer` and `GetContainerAsync`
    - `DeleteContainer` and `DeleteContainerAsync`
    - `CreateContainerFile` and `CreateContainerFileAsync`
    - `GetContainerFiles` and `GetContainerFilesAsync`
    - `GetContainerFile` and `GetContainerFileAsync`
    - `GetContainerFileContent` and `GetContainerFileContentAsync`
    - `DeleteContainerFile` and `DeleteContainerFileAsync`
- OpenAI.Embeddings:
  - Added the `Model` property to `EmbeddingClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `EmbeddingClient`.
- OpenAI.Evals:
  - Added constructors that can take a custom `AuthenticationPolicy` to `EvaluationClient`.
- OpenAI.Files:
  - Added constructors that can take a custom `AuthenticationPolicy` to `OpenAIFileClient`.
- OpenAI.FineTuning:
  - Added constructors that can take a custom `AuthenticationPolicy` to `FineTuningClient`.
- OpenAI.Graders:
  - Introduced the new `GraderClient` to support the Graders API with protocol methods for the following operations:
    - `RunGrader` and `RunGraderAsync`
    - `ValidateGrader` and `ValidateGraderAsync`
- OpenAI.Images:
  - Added the `Model` property to `ImageClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `ImageClient`.
- OpenAI.Models:
  - Added constructors that can take a custom `AuthenticationPolicy` to `OpenAIModelClient`.
- OpenAI.Moderations:
  - Added the `Model` property to `ModerationClient`.
  - Added constructors that can take a custom `AuthenticationPolicy` to `ModerationClient`.
- OpenAI.Realtime:
  - Enabled support for semantic voice activity detection (VAD) via the new `CreateSemanticVoiceActivityTurnDetectionOptions` method of `TurnDetectionOptions`.
- OpenAI.Responses:
  - Added a model factory.
  - Added constructors that can take a custom `AuthenticationPolicy` to `OpenAIResponseClient`.
- OpenAI.VectorStores:
  - Added constructors that can take a custom `AuthenticationPolicy` to `VectorStoreClient`.

### Bug Fixed

- OpenAI.Assistants:
  - Fixed an issue causing the `ImageDetail` property of `MessageContent` to not be serialized correctly.
- OpenAI.Audio:
  - Added a check to all overloads of `TranscribeAudioStreaming` and `TranscribeAudioStreamingAsync` in the `AudioClient` to prevent using the `whisper-1` model, which does not support streaming and simply ignores the `stream` parameter.
- OpenAI.Realtime:
  - Improved the disposal logic in `AsyncWebsocketMessageResultEnumerator` to prevent multiple disposals.
- OpenAI.Responses:
  - Fixed an issue in code generation that caused the `StreamingResponseTextAnnotationAddedUpdate` class to not be generated correctly as part of the set of possible handles when streaming.
  - Fixed an issue in code generation that caused the `Status` property of `ReasoningResponseItem` and the `ReasoningStatus` enum to not be generated correctly and lead to incorrect behavior.

### Other Changes

- The `OpenAI` NuGet package now contains signed binaries.
- Updated the `System.ClientModel` dependency to version 1.5.1, which contains a fix for a concurrency bug which could cause some applications running on the legacy .NET Framework to experience an infinite loop while deserializing service responses.
- Removed the explicit `net6.0` target framework, as this version reached end-of-life in November 2024 and is no longer maintained nor supported by Microsoft. This does not prevent using the OpenAI library on .NET 6, as the runtime will fallback to the `netstandard2.0` target.

## 2.2.0 (2025-07-02)

### Features Added

- OpenAI.Audio:
  - Enabled support for streaming audio transcriptions:
    - Added new methods to `AudioClient`:
      - `TranscribeAudioStreaming` and `TranscribeAudioStreamingAsync`
    - Added new types of `StreamingAudioTranscriptionUpdate` to work with streaming transcriptions:
      - `StreamingAudioTranscriptionTextDeltaUpdate`
      - `StreamingAudioTranscriptionTextDoneUpdate`
  - Added the `TranscriptionTokenLogProbabilities` property to `AudioTranscription` to represent token-level log probability information.
  - Added the `AudioTranscriptionIncludes` enum and `Includes` property to `AudioTranscriptionOptions` to request additional information in the transcription response.
  - Added new voices to `GeneratedSpeechVoice`.
  - Added the `Instructions` property to `SpeechGenerationOptions` to control the voice of the generated audio with additional instructions.
- OpenAI.Batch:
  - Added new `Rehydrate` method overloads that receive a batch ID instead of rehydration token.
- OpenAI.Chat:
  - Added new methods to `ChatClient`:
    - `DeleteChatCompletion` and `DeleteChatCompletionAsync`
    - `GetChatCompletion` and `GetChatCompletionAsync`
  - Added `Aac` format to `ChatOutputAudioFormat`.
  - Added new voices to `ChatOutputAudioVoice`.
- OpenAI.Evals:
  - Introduced the new `EvaluationClient` to support the Evaluations API with protocol methods for the following operations:
    - `CreateEvaluation` and `CreateEvaluationAsync`
    - `GetEvaluation` and `GetEvaluationAsync`
    - `GetEvaluations` and `GetEvaluationsAsync`
    - `UpdateEvaluation` and `UpdateEvaluationAsync`
    - `DeleteEvaluation` and `DeleteEvaluationAsync`
    - `CreateEvaluatinRun` and `CreateEvaluationRunAsync`
    - `GetEvaluationRun` and `GetEvaluationRunAsync`
    - `GetEvaluationRuns` and `GetEvaluationRunsAsync`
    - `CancelEvaluationRun` and `CancelEvaluationRunAsync`
    - `DeleteEvaluationRun` and `DeleteEvaluationRunAsync`
    - `GetEvaluationRunOutputItem` and `GetEvaluationRunOutputItemAsync`
    - `GetEvaluationRunOutputItems` and `GetEvaluationRunOutputItemsAsync`
- OpenAI.FineTuning:
  - Added new methods to `FineTuningClient`:
    - `GetFineTuningCheckpointPermissions` and `GetFineTuningCheckpointPermissionsAsync`
    - `CreateFineTuningCheckpointPermission` and `CreateFineTuningCheckpointPermissionAsync`
    - `DeleteFineTuningCheckpointPermission` and `DeleteFineTuningCheckpointPermission`
    - `PauseFineTuningJob` and `PauseFineTuningJobAsync`
    - `ResumeFineTuningJob` and `ResumeFineTuningJobAsync`
  - Added new experimental types to support fine-tuning workflows.
- OpenAI.Images:
  - Added the `Usage` property to `GeneratedImageCollection` to represent image token usage information.
  - Added the `Background` property to `ImageGenerationOptions` to set transparency for the background of the generated image(s).
  - Added the `ModerationLevel` property to `ImageGenerationOptions` to control the content-moderation level for generated image(s).
  - Added the `OutputCompressionFactor` property to `ImageGenerationOptions` to set the compression level (0-100%) for the generated images.
  - Added the `OutputFileFormat` property to `ImageGenerationOptions` to set the file format in which the generated images are returned.
  - Added support for new values to the `GeneratedImageSize` enum.
- OpenAI.Responses:
  - Added new methods to the `OpenAIResponseClient`:
    - `CancelResponse` and `CancelResponseAsync`
    - `GetResponseStreaming` and `GetResponseStreamingAsync`.
  - Added `Linux` property to `ComputerToolEnvironment`.
  - Added `Background` property to `ResponseCreationOptions` to support background mode.
  - Added `SequenceNumber` property to `StreamingResponseUpdate` to support background mode while streaming.
  - Added `InputTokenDetails` property to `ResponseTokenUsage` to represent token usage information.
  - Added `EncryptedContent` property to `ReasoningResponseItem` to represent the encrypted content of the reasoning item.
- OpenAI.VectorStores:
  - Added new methods to `VectorStoreClient`:
    - `RetrieveVectorStoreFileContent` and `RetrieveVectorStoreFileContentAsync`
    - `SearchVectorStore` and `SearchVectorStoreAsync`
    - `UpdateVectorStoreFileAttributes` and `UpdateVectorStoreFileAttributes`

### Bugs Fixed

- OpenAI.Files:
  - Added a `SizeInBytesLong` property to `OpenAIFile` to correctly represent the size of a file.
- OpenAI.Responses:
  - Fixed an issue where setting the `ReasoningSummaryVerbosity` property of `ResponseReasoningOptions` was sending the wrong property to the service.
  - Fixed an issue with the `CreateInputFilePart` method of `ResponseContentPart` not being able to send files as `BinaryData`.

### Breaking Changes in Preview APIs

- Removed the implicit operator from all models that converts a model to `BinaryContent`.
- Removed the explicit operator from all models that converts a `ClientResult` to a model.
- OpenAI:
  - Renamed the `GetRealtimeConversationClient` method from `OpenAIClient` to `GetRealtimeClient`.
- OpenAI.FineTuning:
  - Renamed the `FineTuningJobOperation` class to `FineTuningJob`.
  - Removed protocol methods for `CreateFineTuningJob`, `GetJob`, and `GetJobs` and added convenience methods for them.
- OpenAI.Realtime:
  - Updated namespace from `OpenAI.Conversations` to `OpenAI.Realtime`. All APIs and types related to real-time conversations have been moved to the new `OpenAI.Realtime` namespace.
- OpenAI.Responses:
  - Removed the `SummaryTextParts` property of `ReasoningResponseItem` in favor a new property called `SummaryParts`.
  - Removed the following public constructors:
    - `FileSearchCallResponseItem(IEnumerable<string> queries, IEnumerable<FileSearchCallResult> results)`
    - `FunctionCallOutputResponseItem(string callId, string functionOutput)`
    - `FunctionCallResponseItem(string callId, string functionName, BinaryData functionArguments)`
  - Made several properties read-only that were previously settable:
    - `CallId` and `Output` in `ComputerCallOutputResponseItem`
    - `Action`, `CallId`, and `Status` in `ComputerCallResponseItem`
    - `Results` and `Status` in `FileSearchCallResponseItem`
    - `CallId` in `FunctionCallOutputResponseItem`
    - `CallId` in `FunctionCallResponseItem`
  - Changed the following property types:
    - `Attributes` in `FileSearchCallResult` is now `IReadOnlyDictionary<string, BinaryData>` instead of `IDictionary<string, BinaryData>`.
    - `Status` properties are now nullable in multiple response item types.
    - `Code` in `ResponseError` is now `ResponseErrorCode` instead of `string`.
  - Renamed the `WebSearchToolContextSize` extensible enum to `WebSearchContextSize`.
  - Renamed the `WebSearchToolLocation` class to `WebSearchUserLocation`.
- OpenAI.VectorStores:
  - Renamed method parameters from `vectorStore` to `options` in `CreateVectorStore` and `ModifyVectorStore` methods in `VectorStoreClient`.

## 2.2.0-beta.4 (2025-03-18)

### Features Added

- OpenAI.Chat:
  - Enabled support for file inputs. When using models with vision capabilities, you can now also provide PDF files as inputs, either as a file ID or as base64-encoded data. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))
    - Added the `CreateFilePart(string fileId)` and `CreateFilePart(BinaryData fileBytes, string fileBytesMediaType, string filename)` factory methods to `ChatMessageContentPart`.
- OpenAI.Responses:
  - Added the `ResponseToolChoice` class to help specify which tool the model should select when generating a response. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))

### Breaking Changes in Preview APIs

- OpenAI.Assistants:
  - Removed the default constructor and the use of the `required` keyword from the `FileSearchRankingOptions` and `FunctionToolDefinition` classes to align with the rest of the library. ([86407c8](https://github.com/openai/openai-dotnet/commit/86407c80b35271713b2d92c87943a0c7e025d28f))
- OpenAI.RealtimeConversation:
  - Removed the default constructor and the use of the `required` keyword from the `ConversationFunctionTool` class to align with the rest of the library. ([86407c8](https://github.com/openai/openai-dotnet/commit/86407c80b35271713b2d92c87943a0c7e025d28f))
- OpenAI.Responses:
  - Removed the `id` parameter from the factory methods of the `ResponseItem` class.
  - Renamed the `AllowParallelToolCalls` property of the `ResponseCreationOptions` and `OpenAIResponse` classes to `ParallelToolCallsEnabled`. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))
  - Changed the type of the `ToolChoice` property of the `ResponseCreationOptions` and `OpenAIResponse`classes from `BinaryData` to `ResponseToolChoice`. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))
  - Changed `MessageRole` from an "extensible enum" to a regular enum. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))
  - Refactored the `StreamingResponse*` classes. ([aaa924e](https://github.com/openai/openai-dotnet/commit/aaa924ecde1b2281257f26824fea038a3b1efe35))
- OpenAI.VectorStores:
  - Removed the default constructor and the use of the `required` keyword from the `VectorStoreExpirationPolicy` class to align with the rest of the library. ([86407c8](https://github.com/openai/openai-dotnet/commit/86407c80b35271713b2d92c87943a0c7e025d28f))

## 2.2.0-beta.3 (2025-03-11)

### Features Added

- OpenAI.Responses:
  - Enabled support for the new Responses API. ([0ca4c06](https://github.com/openai/openai-dotnet/commit/0ca4c062c2fce219ce42cb4d2f1f2ab2056811aa))
    - Added a new `OpenAIResponseClient` in a corresponding scenario namespace.
    - This release aims to bring the new Responses API to .NET as soon as possible. Details, examples, and comprehensive updates for other operations are coming soon!
- OpenAI.Chat:
  - Enabled support for web search. ([0ca4c06](https://github.com/openai/openai-dotnet/commit/0ca4c062c2fce219ce42cb4d2f1f2ab2056811aa))
    - Added a `WebSearchOptions` property to `ChatCompletionOptions` ([`web_search_options` in the REST API](https://platform.openai.com/docs/api-reference/chat/create)).
    - Added an `Annotations` property to `ChatCompletion`.
- OpenAI.Files:
  - Enabled support for user data and evals files. ([0ca4c06](https://github.com/openai/openai-dotnet/commit/0ca4c062c2fce219ce42cb4d2f1f2ab2056811aa))

## 2.2.0-beta.2 (2025-02-18)

### Bugs Fixed

- OpenAI.Chat:
  - Fixed an issue that caused calls to the `CompleteChatStreaming` and `CompleteChatStreamingAsync` methods to fail with audio-enabled models unless provided a `ChatCompletionOptions` instance that had previously been used in a non-streaming `CompleteChat` or `CompleteChatAsync` method call. ([d6615ab](https://github.com/openai/openai-dotnet/commit/d6615abe2d04d8d09fbe150941cd8d3c118117d2))
  - Fixed an issue that caused calls to the `CompleteChatStreaming` and `CompleteChatStreamingAsync` methods to not report token usage when provided a `ChatCompletionOptions` instance that had previously been used in a non-streaming `CompleteChat` or `CompleteChatAsync` method call. ([d6615ab](https://github.com/openai/openai-dotnet/commit/d6615abe2d04d8d09fbe150941cd8d3c118117d2))
  - Fixed a series of issues with standalone serialization and deserialization of `ChatCompletionOptions` that impacted the ability to manipulate chat completion requests via `System.ClientModel.Primitives.ModelReaderWriter` and related utilities. ([d6615ab](https://github.com/openai/openai-dotnet/commit/d6615abe2d04d8d09fbe150941cd8d3c118117d2))

## 2.2.0-beta.1 (2025-02-07)

### Features Added

- OpenAI.Audio:
  - Added explicit support for new values of `GeneratedSpeechVoice`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
- OpenAI.Chat:
  - Enabled support for input and output audio in chat completions using the `gpt-4o-audio-preview` model.
    - Added a `ResponseModalities` property to `ChatCompletionOptions` ([`modalities` in the REST API](https://platform.openai.com/docs/api-reference/chat/create)). ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Set this flags enum property to `ChatResponseModalities.Text | ChatResponseModalities.Audio` in order to request output audio. Note that you need to set the new `AudioOptions` property too.
    - Added an `AudioOptions` property to `ChatCompletionOptions` ([`audio` in the REST API](https://platform.openai.com/docs/api-reference/chat/create)). ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Use this property to configure the output audio voice and format.
    - Added a `CreateInputAudioPart(BinaryData, ChatInputAudioFormat)` static method to `ChatMessageContentPart`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Use this method to send input audio as a `ChatMessageContentPart` in the `Content` property of a `UserChatMessage`.
    - Added an `OutputAudio` property to `ChatCompletion`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Use this property to retrieve the output audio that was generated by the model.
    - Added an `OutputAudioReference` property to `AssistantChatMessage`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Use this property to reference the output audio of a prior `ChatCompletion` to continue an existing chat.
    - For more information, see the example in the [README](README.md).
  - Enabled support for [Predicted Outputs](https://platform.openai.com/docs/guides/predicted-outputs) in chat completions using the `gpt-4o` and `gpt-4o-mini` models. Predicted Outputs can greatly improve response times when large parts of the model response are known ahead of time. This is most common when you are regenerating a file with only minor changes to most of the content.
    - Added an `OutputPrediction` property to `ChatCompletionOptions` ([`prediction` in the REST API](https://platform.openai.com/docs/api-reference/chat/create)). ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - Use this property to configure a Predicted Output. The property is of a new type called `ChatOutputPrediction`, which you can create via the `ChatOutputPrediction.CreateStaticContentPrediction(string)` or `ChatOutputPrediction.CreateStaticContentPrediction(IEnumerable<ChatMessageContentPart>)` static methods.
    - Added an `AcceptedPredictionTokenCount` property to `ChatOutputTokenUsageDetails`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - When using Predicted Outputs, use this property to track the number of tokens in the prediction that appeared in the completion.
    - Added a `RejectedPredictionTokenCount` property to `ChatOutputTokenUsageDetails`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
      - When using Predicted Outputs, use this property to track the number of tokens in the prediction that did not appear in the completion. Note that these tokens are still counted in the total completion tokens for purposes of billing, output, and context window limits.
  - Added a `ReasoningEffortLevel` property to `ChatCompletionOptions` ([`reasoning_effort` in the REST API](https://platform.openai.com/docs/api-reference/chat/create)). ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
    - Use this property to constrain the effort on reasoning for the models with reasoning capabilities (such as `o3-mini` and `o1`). Reducing the reasoning effort can result in faster responses and fewer tokens used on reasoning.
  - Added a `DeveloperChatMessage` class. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
    - Use this type of message to provide instructions that the model should follow, regardless of messages sent by the user. It replaces the existing `SystemChatMessage` class when using `o1` and newer models.
- OpenAI.RealtimeConversation:
  - Added explicit support for new values of `ConversationVoice`. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))

### Breaking Changes in Preview APIs

- OpenAI.Assistants:
  - Removed the setters of the `IDictionary<string, string> Metadata` properties of the "options" classes (e.g., `AssistantCreationOptions`) to be able to guarantee that the collections are always initialized. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
    - The dictionaries remain writeable, and you can add elements to it using collection initializer syntax or the `Add(TKey, TValue)` method.
- OpenAI.RealtimeConversation:
  - Renamed the `InputTokens`, `OutputTokens`, and `TotalTokens` properties in `ConversationTokenUsage` to `InputTokenCount`, `OutputTokenCount`, and `TotalTokenCount`, respectively. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
  - Renamed the `AudioTokens`, `CachedTokens`, and `TextTokens` properties in `ConversationInputTokenUsageDetails` to `AudioTokenCount`, `CachedTokenCount`, and `TextTokenCount`, respectively. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
  - Renamed the `AudioTokens` and `TextTokens` properties in `ConversationOutputTokenUsageDetails` to `AudioTokenCount` and `TextTokenCount`, respectively. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
- OpenAI.VectorStores:
  - Removed the setters of the `IDictionary<string, string> Metadata` properties of the "options" classes (e.g., `VectorStoreCreationOptions`) to be able to guarantee that the collections are always initialized. ([0e0c460](https://github.com/openai/openai-dotnet/commit/0e0c460c88424fc2241956ed5ead6dd5ed7638ec))
    - The dictionaries remain writeable, and you can add elements to it using collection initializer syntax or the `Add(TKey, TValue)` method.

### Other Changes

- Added .NET 8 as a target framework. ([6203354](https://github.com/openai/openai-dotnet/commit/6203354b36395fa79a34e3b4fd7b97b43b9720b9))
- Enabled support for trimming. ([4cd8529](https://github.com/openai/openai-dotnet/commit/4cd85298d110afcbef40f07746d6502a73493e16))
- Enabled support for native AOT compilation. ([4cd8529](https://github.com/openai/openai-dotnet/commit/4cd85298d110afcbef40f07746d6502a73493e16))

## 2.1.0 (2024-12-04)

### Features Added

- OpenAI.Assistants:
  - Added a `Content` property to `RunStepFileSearchResult` ([`step_details.tool_calls.file_search.results.content` in the REST API](https://platform.openai.com/docs/api-reference/run-steps/step-object)). ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
    - When using an Assistant with the File Search tool, you can use this property to retrieve the contents of the File Search results that were used by the model.
  - Added `FileSearchRankingOptions` and `FileSearchResults` properties to `RunStepDetailsUpdate`. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))

### Breaking Changes in Preview APIs

- OpenAI.RealtimeConversation:
  - Renamed the `From*()` factory methods on `ConversationContentPart` to `Create*Part()` for consistency. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Removed an extraneous `toolCallId` parameter from `ConversationItem.CreateSystemMessage()`. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
- OpenAI.Assistants:
  - Renamed `RunStepType` to `RunStepKind`. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Changed `RunStepKind` from an "extensible enum" to a regular enum. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Renamed the `ToolCallId` property of `RunStepToolCall` to `Id`. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Renamed the `ToolKind` property of `RunStepToolCall` to `Kind`. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Replaced the `FileSearchRanker` and `FileSearchScoreThreshold` properties of `RunStepToolCall` with a new `FileSearchRankingOptions` property that contains both values to make it clearer how they are related. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))

### Bugs Fixed

- OpenAI.RealtimeConversation:
  - Fixed serialization issues with `ConversationItem` creation of system and assistant messages. ([bf3f0ed](https://github.com/openai/openai-dotnet/commit/bf3f0eddeda1957a998491e36d7fb551e99be916))
  - Fixed an issue causing a deadlock when calling the `RealtimeConversationSession`'s `SendInputAudio` method overload that takes a `BinaryData` parameter. ([f491c2d](https://github.com/openai/openai-dotnet/commit/f491c2d5a3894953e0bc112431ea3844a64496da))

## 2.1.0-beta.2 (2024-11-04)

### Features Added

- OpenAI.Chat:
  - Added a `StoredOutputEnabled` property to `ChatCompletionOptions` ([`store` in the REST API](https://platform.openai.com/docs/api-reference/chat/create#chat-create-store)). ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
    - Use this property to indicate whether or not to store the output of the chat completion for use in model distillation or evals.
  - Added a `Metadata` property to `ChatCompletionOptions` ([`metadata` in the REST API](https://platform.openai.com/docs/api-reference/chat/create#chat-create-metadata)). ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
    - Use this property to add custom tags and values to the chat completions for filtering in the OpenAI dashboard.
  - Added an `InputTokenDetails` property to `ChatTokenUsage` ([`usage.prompt_token_details` in the REST API](https://platform.openai.com/docs/api-reference/chat/object#chat/object-usage)). ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
    - The property is of a new type called `ChatInputTokenUsageDetails`, which contains properties for `AudioTokenCount` and `CachedTokenCount` for usage with supported models.
  - Added an `AudioTokenCount` property to `ChatOutputTokenUsageDetails` ([`usage.completion_token_details` in the REST API](https://platform.openai.com/docs/api-reference/chat/object#chat/object-usage)). Audio support in chat completions is coming soon. ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
- OpenAI.Moderations:
  - Added `Illicit` and `IllicitViolent` properties `ModerationResult` to represent these two new moderation categories. ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))

### Breaking Changes in Preview APIs

- OpenAI.RealtimeConversation:
  - Made improvements to the experimental Realtime API. Please note this features area is currently under rapid development and not all changes may be reflected here. ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
    - Several types have been renamed for consistency and clarity.
    - `ConversationRateLimitsUpdate` (previously `ConversationRateLimitsUpdatedUpdate`) now includes named `RequestDetails` and `TokenDetails` properties, mapping to the corresponding named items in the underlying `rate_limits` command payload.

### Bugs Fixed

- OpenAI.RealtimeConversation:
  - Fixed serialization and deserialization of `ConversationToolChoice` literal values (such as `"required"`). ([9de3709](https://github.com/openai/openai-dotnet/commit/9de37095eaad6f1e2e87c201fd693ac1d9757142))

### Other Changes

- Updated the `System.ClientModel` dependency to version `1.2.1`. ([b0f9e5c](https://github.com/openai/openai-dotnet/commit/b0f9e5c3b9708a802afa6ce7489636d2084e7d61))
  - This updates the `System.Text.Json` transitive dependency to version `6.0.10`, which includes a security compliance fix for [CVE-2024-43485](https://github.com/advisories/GHSA-8g4q-xg66-9fp4). Please note that the OpenAI library was not impacted by this vulnerability since it does not use the `[JsonExtensionData]` feature.

## 2.1.0-beta.1 (2024-10-01)

> [!NOTE]
> With this updated preview library release, we're excited to bring early support for the newly-announced `/realtime` beta API. You can read more about `/realtime` here: https://openai.com/index/introducing-the-realtime-api/. Given the scope and recency of the feature area, the new `RealtimeConversationClient` is subject to substantial refinement and change over the coming weeks -- this release is purely intended to empower early development against `gpt-4o-realtime-preview` as quickly and efficiently as possible.

### Features Added

- Added a new `RealtimeConversationClient` in a corresponding scenario namespace. ([ff75da4](https://github.com/openai/openai-dotnet/commit/ff75da4167bc83fa85eb69ac142cab88a963ed06))
  - This maps to the new `/realtime` beta endpoint and is thus marked with a new `[Experimental("OPENAI002")]` diagnostic tag.
  - This is a very early version of the convenience surface and thus subject to significant change
  - Documentation and samples will arrive soon; in the interim, see [the scenario test files](/tests/RealtimeConversation) for basic usage
  - You can also find an external sample employing this client, together with Azure OpenAI support, at https://github.com/Azure-Samples/aoai-realtime-audio-sdk/tree/main/dotnet/samples/console

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