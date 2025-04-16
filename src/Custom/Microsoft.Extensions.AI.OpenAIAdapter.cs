// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Audio;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.Responses;

#nullable enable

namespace Microsoft.Extensions.AI;

/// <summary>Represents an <see cref="IChatClient"/> for an OpenAI <see cref="OpenAIClient"/> or <see cref="ChatClient"/>.</summary>
internal sealed partial class OpenAIChatClient : IChatClient
{
    /// <summary>Gets the default OpenAI endpoint.</summary>
    private static Uri DefaultOpenAIEndpoint { get; } = new("https://api.openai.com/v1");

    /// <summary>Metadata about the client.</summary>
    private readonly ChatClientMetadata _metadata;

    /// <summary>The underlying <see cref="ChatClient" />.</summary>
    private readonly ChatClient _chatClient;

    /// <summary>Initializes a new instance of the <see cref="OpenAIChatClient"/> class for the specified <see cref="ChatClient"/>.</summary>
    /// <param name="chatClient">The underlying client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="chatClient"/> is <see langword="null"/>.</exception>
    public OpenAIChatClient(ChatClient chatClient)
    {
        _ = Throw.IfNull(chatClient);

        _chatClient = chatClient;

        // https://github.com/openai/openai-dotnet/issues/215
        // The endpoint and model aren't currently exposed, so use reflection to get at them, temporarily. Once packages
        // implement the abstractions directly rather than providing adapters on top of the public APIs,
        // the package can provide such implementations separate from what's exposed in the public API.
        Uri providerUrl = typeof(ChatClient).GetField("_endpoint", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(chatClient) as Uri ?? DefaultOpenAIEndpoint;
        string? model = typeof(ChatClient).GetField("_model", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(chatClient) as string;

        _metadata = new("openai", providerUrl, model);
    }

    /// <inheritdoc />
    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        _ = Throw.IfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? _metadata :
            serviceType == typeof(ChatClient) ? _chatClient :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    /// <inheritdoc />
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(messages);

        var openAIChatMessages = ToOpenAIChatMessages(messages, AIJsonUtilities.DefaultOptions);
        var openAIOptions = ToOpenAIOptions(options);

        // Make the call to OpenAI.
        var response = await _chatClient.CompleteChatAsync(openAIChatMessages, openAIOptions, cancellationToken).ConfigureAwait(false);

        return FromOpenAIChatCompletion(response.Value, options, openAIOptions);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(messages);

        var openAIChatMessages = ToOpenAIChatMessages(messages, AIJsonUtilities.DefaultOptions);
        var openAIOptions = ToOpenAIOptions(options);

        // Make the call to OpenAI.
        var chatCompletionUpdates = _chatClient.CompleteChatStreamingAsync(openAIChatMessages, openAIOptions, cancellationToken);

        return FromOpenAIStreamingChatCompletionAsync(chatCompletionUpdates, cancellationToken);
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        // Nothing to dispose. Implementation required for the IChatClient interface.
    }

    private static ChatRole ChatRoleDeveloper { get; } = new ChatRole("developer");

    /// <summary>Converts an Extensions chat message enumerable to an OpenAI chat message enumerable.</summary>
    private static IEnumerable<OpenAI.Chat.ChatMessage> ToOpenAIChatMessages(IEnumerable<ChatMessage> inputs, JsonSerializerOptions options)
    {
        // Maps all of the M.E.AI types to the corresponding OpenAI types.
        // Unrecognized or non-processable content is ignored.

        foreach (ChatMessage input in inputs)
        {
            if (input.Role == ChatRole.System ||
                input.Role == ChatRole.User ||
                input.Role == ChatRoleDeveloper)
            {
                var parts = ToOpenAIChatContent(input.Contents);
                yield return
                    input.Role == ChatRole.System ? new SystemChatMessage(parts) { ParticipantName = input.AuthorName } :
                    input.Role == ChatRoleDeveloper ? new DeveloperChatMessage(parts) { ParticipantName = input.AuthorName } :
                    new UserChatMessage(parts) { ParticipantName = input.AuthorName };
            }
            else if (input.Role == ChatRole.Tool)
            {
                foreach (AIContent item in input.Contents)
                {
                    if (item is FunctionResultContent resultContent)
                    {
                        string? result = resultContent.Result as string;
                        if (result is null && resultContent.Result is not null)
                        {
                            try
                            {
                                result = JsonSerializer.Serialize(resultContent.Result, options.GetTypeInfo(typeof(object)));
                            }
                            catch (NotSupportedException)
                            {
                                // If the type can't be serialized, skip it.
                            }
                        }

                        yield return new ToolChatMessage(resultContent.CallId, result ?? string.Empty);
                    }
                }
            }
            else if (input.Role == ChatRole.Assistant)
            {
                AssistantChatMessage message = new(ToOpenAIChatContent(input.Contents))
                {
                    ParticipantName = input.AuthorName
                };

                foreach (var content in input.Contents)
                {
                    if (content is FunctionCallContent callRequest)
                    {
                        message.ToolCalls.Add(
                            ChatToolCall.CreateFunctionToolCall(
                                callRequest.CallId,
                                callRequest.Name,
                                new(JsonSerializer.SerializeToUtf8Bytes(
                                    callRequest.Arguments,
                                    options.GetTypeInfo(typeof(IDictionary<string, object?>))))));
                    }
                }

                if (input.AdditionalProperties?.TryGetValue(nameof(message.Refusal), out string? refusal) is true)
                {
                    message.Refusal = refusal;
                }

                yield return message;
            }
        }
    }

    /// <summary>Converts a list of <see cref="AIContent"/> to a list of <see cref="ChatMessageContentPart"/>.</summary>
    private static List<ChatMessageContentPart> ToOpenAIChatContent(IList<AIContent> contents)
    {
        List<ChatMessageContentPart> parts = [];
        foreach (var content in contents)
        {
            switch (content)
            {
                case TextContent textContent:
                    parts.Add(ChatMessageContentPart.CreateTextPart(textContent.Text));
                    break;

                case UriContent uriContent when uriContent.HasTopLevelMediaType("image"):
                    parts.Add(ChatMessageContentPart.CreateImagePart(uriContent.Uri, GetImageDetail(content)));
                    break;

                case DataContent dataContent when dataContent.HasTopLevelMediaType("image"):
                    parts.Add(ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes(dataContent.Data), dataContent.MediaType, GetImageDetail(content)));
                    break;

                case DataContent dataContent when dataContent.HasTopLevelMediaType("audio"):
                    var audioData = BinaryData.FromBytes(dataContent.Data);
                    if (dataContent.MediaType.Equals("audio/mpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        parts.Add(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Mp3));
                    }
                    else if (dataContent.MediaType.Equals("audio/wav", StringComparison.OrdinalIgnoreCase))
                    {
                        parts.Add(ChatMessageContentPart.CreateInputAudioPart(audioData, ChatInputAudioFormat.Wav));
                    }

                    break;
            }
        }

        if (parts.Count == 0)
        {
            parts.Add(ChatMessageContentPart.CreateTextPart(string.Empty));
        }

        return parts;
    }

    private static ChatImageDetailLevel? GetImageDetail(AIContent content)
    {
        if (content.AdditionalProperties?.TryGetValue("detail", out object? value) is true)
        {
            return value switch
            {
                string detailString => new ChatImageDetailLevel(detailString),
                ChatImageDetailLevel detail => detail,
                _ => null
            };
        }

        return null;
    }

    private static async IAsyncEnumerable<ChatResponseUpdate> FromOpenAIStreamingChatCompletionAsync(
        IAsyncEnumerable<StreamingChatCompletionUpdate> updates,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Dictionary<int, FunctionCallInfo>? functionCallInfos = null;
        ChatRole? streamedRole = null;
        ChatFinishReason? finishReason = null;
        StringBuilder? refusal = null;
        string? responseId = null;
        DateTimeOffset? createdAt = null;
        string? modelId = null;
        string? fingerprint = null;

        // Process each update as it arrives
        await foreach (StreamingChatCompletionUpdate update in updates.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            // The role and finish reason may arrive during any update, but once they've arrived, the same value should be the same for all subsequent updates.
            streamedRole ??= update.Role is ChatMessageRole role ? FromOpenAIChatRole(role) : null;
            finishReason ??= update.FinishReason is OpenAI.Chat.ChatFinishReason reason ? FromOpenAIFinishReason(reason) : null;
            responseId ??= update.CompletionId;
            createdAt ??= update.CreatedAt;
            modelId ??= update.Model;
            fingerprint ??= update.SystemFingerprint;

            // Create the response content object.
            ChatResponseUpdate responseUpdate = new()
            {
                ResponseId = update.CompletionId,
                MessageId = update.CompletionId, // There is no per-message ID, but there's only one message per response, so use the response ID
                CreatedAt = update.CreatedAt,
                FinishReason = finishReason,
                ModelId = modelId,
                RawRepresentation = update,
                Role = streamedRole,
            };

            // Populate it with any additional metadata from the OpenAI object.
            if (update.ContentTokenLogProbabilities is { Count: > 0 } contentTokenLogProbs)
            {
                (responseUpdate.AdditionalProperties ??= [])[nameof(update.ContentTokenLogProbabilities)] = contentTokenLogProbs;
            }

            if (update.RefusalTokenLogProbabilities is { Count: > 0 } refusalTokenLogProbs)
            {
                (responseUpdate.AdditionalProperties ??= [])[nameof(update.RefusalTokenLogProbabilities)] = refusalTokenLogProbs;
            }

            if (fingerprint is not null)
            {
                (responseUpdate.AdditionalProperties ??= [])[nameof(update.SystemFingerprint)] = fingerprint;
            }

            // Transfer over content update items.
            if (update.ContentUpdate is { Count: > 0 })
            {
                foreach (ChatMessageContentPart contentPart in update.ContentUpdate)
                {
                    if (ToAIContent(contentPart) is AIContent aiContent)
                    {
                        responseUpdate.Contents.Add(aiContent);
                    }
                }
            }

            // Transfer over refusal updates.
            if (update.RefusalUpdate is not null)
            {
                _ = (refusal ??= new()).Append(update.RefusalUpdate);
            }

            // Transfer over tool call updates.
            if (update.ToolCallUpdates is { Count: > 0 } toolCallUpdates)
            {
                foreach (StreamingChatToolCallUpdate toolCallUpdate in toolCallUpdates)
                {
                    functionCallInfos ??= [];
                    if (!functionCallInfos.TryGetValue(toolCallUpdate.Index, out FunctionCallInfo? existing))
                    {
                        functionCallInfos[toolCallUpdate.Index] = existing = new();
                    }

                    existing.CallId ??= toolCallUpdate.ToolCallId;
                    existing.Name ??= toolCallUpdate.FunctionName;
                    if (toolCallUpdate.FunctionArgumentsUpdate is { } argUpdate && !argUpdate.ToMemory().IsEmpty)
                    {
                        _ = (existing.Arguments ??= new()).Append(argUpdate.ToString());
                    }
                }
            }

            // Transfer over usage updates.
            if (update.Usage is ChatTokenUsage tokenUsage)
            {
                var usageDetails = FromOpenAIUsage(tokenUsage);
                responseUpdate.Contents.Add(new UsageContent(usageDetails));
            }

            // Now yield the item.
            yield return responseUpdate;
        }

        // Now that we've received all updates, combine any for function calls into a single item to yield.
        if (functionCallInfos is not null)
        {
            ChatResponseUpdate responseUpdate = new()
            {
                ResponseId = responseId,
                MessageId = responseId, // There is no per-message ID, but there's only one message per response, so use the response ID
                CreatedAt = createdAt,
                FinishReason = finishReason,
                ModelId = modelId,
                Role = streamedRole,
            };

            foreach (var entry in functionCallInfos)
            {
                FunctionCallInfo fci = entry.Value;
                if (!string.IsNullOrWhiteSpace(fci.Name))
                {
                    var callContent = ParseCallContentFromJsonString(
                        fci.Arguments?.ToString() ?? string.Empty,
                        fci.CallId!,
                        fci.Name!);
                    responseUpdate.Contents.Add(callContent);
                }
            }

            // Refusals are about the model not following the schema for tool calls. As such, if we have any refusal,
            // add it to this function calling item.
            if (refusal is not null)
            {
                (responseUpdate.AdditionalProperties ??= [])[nameof(ChatMessageContentPart.Refusal)] = refusal.ToString();
            }

            // Propagate additional relevant metadata.
            if (fingerprint is not null)
            {
                (responseUpdate.AdditionalProperties ??= [])[nameof(ChatCompletion.SystemFingerprint)] = fingerprint;
            }

            yield return responseUpdate;
        }
    }

    private static ChatResponse FromOpenAIChatCompletion(ChatCompletion openAICompletion, ChatOptions? options, ChatCompletionOptions chatCompletionOptions)
    {
        _ = Throw.IfNull(openAICompletion);

        // Create the return message.
        ChatMessage returnMessage = new()
        {
            MessageId = openAICompletion.Id, // There's no per-message ID, so we use the same value as the response ID
            RawRepresentation = openAICompletion,
            Role = FromOpenAIChatRole(openAICompletion.Role),
        };

        // Populate its content from those in the OpenAI response content.
        foreach (ChatMessageContentPart contentPart in openAICompletion.Content)
        {
            if (ToAIContent(contentPart) is AIContent aiContent)
            {
                returnMessage.Contents.Add(aiContent);
            }
        }

        // Output audio is handled separately from message content parts.
        if (openAICompletion.OutputAudio is ChatOutputAudio audio)
        {
            string mimeType = chatCompletionOptions?.AudioOptions?.OutputAudioFormat.ToString()?.ToLowerInvariant() switch
            {
                "opus" => "audio/opus",
                "aac" => "audio/aac",
                "flac" => "audio/flac",
                "wav" => "audio/wav",
                "pcm" => "audio/pcm",
                "mp3" or _ => "audio/mpeg",
            };

            var dc = new DataContent(audio.AudioBytes.ToMemory(), mimeType)
            {
                AdditionalProperties = new() { [nameof(audio.ExpiresAt)] = audio.ExpiresAt },
            };

            if (audio.Id is string id)
            {
                dc.AdditionalProperties[nameof(audio.Id)] = id;
            }

            if (audio.Transcript is string transcript)
            {
                dc.AdditionalProperties[nameof(audio.Transcript)] = transcript;
            }

            returnMessage.Contents.Add(dc);
        }

        // Also manufacture function calling content items from any tool calls in the response.
        if (options?.Tools is { Count: > 0 })
        {
            foreach (ChatToolCall toolCall in openAICompletion.ToolCalls)
            {
                if (!string.IsNullOrWhiteSpace(toolCall.FunctionName))
                {
                    var callContent = ParseCallContentFromBinaryData(toolCall.FunctionArguments, toolCall.Id, toolCall.FunctionName);
                    callContent.RawRepresentation = toolCall;

                    returnMessage.Contents.Add(callContent);
                }
            }
        }

        // Wrap the content in a ChatResponse to return.
        var response = new ChatResponse(returnMessage)
        {
            CreatedAt = openAICompletion.CreatedAt,
            FinishReason = FromOpenAIFinishReason(openAICompletion.FinishReason),
            ModelId = openAICompletion.Model,
            RawRepresentation = openAICompletion,
            ResponseId = openAICompletion.Id,
        };

        if (openAICompletion.Usage is ChatTokenUsage tokenUsage)
        {
            response.Usage = FromOpenAIUsage(tokenUsage);
        }

        if (openAICompletion.ContentTokenLogProbabilities is { Count: > 0 } contentTokenLogProbs)
        {
            (response.AdditionalProperties ??= [])[nameof(openAICompletion.ContentTokenLogProbabilities)] = contentTokenLogProbs;
        }

        if (openAICompletion.Refusal is string refusal)
        {
            (response.AdditionalProperties ??= [])[nameof(openAICompletion.Refusal)] = refusal;
        }

        if (openAICompletion.RefusalTokenLogProbabilities is { Count: > 0 } refusalTokenLogProbs)
        {
            (response.AdditionalProperties ??= [])[nameof(openAICompletion.RefusalTokenLogProbabilities)] = refusalTokenLogProbs;
        }

        if (openAICompletion.SystemFingerprint is string systemFingerprint)
        {
            (response.AdditionalProperties ??= [])[nameof(openAICompletion.SystemFingerprint)] = systemFingerprint;
        }

        return response;
    }

    /// <summary>Converts an extensions options instance to an OpenAI options instance.</summary>
    private static ChatCompletionOptions ToOpenAIOptions(ChatOptions? options)
    {
        ChatCompletionOptions result = new();

        if (options is not null)
        {
            result.FrequencyPenalty = options.FrequencyPenalty;
            result.MaxOutputTokenCount = options.MaxOutputTokens;
            result.TopP = options.TopP;
            result.PresencePenalty = options.PresencePenalty;
            result.Temperature = options.Temperature;
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            result.Seed = options.Seed;
#pragma warning restore OPENAI001

            if (options.StopSequences is { Count: > 0 } stopSequences)
            {
                foreach (string stopSequence in stopSequences)
                {
                    result.StopSequences.Add(stopSequence);
                }
            }

            if (options.AdditionalProperties is { Count: > 0 } additionalProperties)
            {
                if (additionalProperties.TryGetValue(nameof(result.AllowParallelToolCalls), out bool allowParallelToolCalls))
                {
                    result.AllowParallelToolCalls = allowParallelToolCalls;
                }

                if (additionalProperties.TryGetValue(nameof(result.AudioOptions), out ChatAudioOptions? audioOptions))
                {
                    result.AudioOptions = audioOptions;
                }

                if (additionalProperties.TryGetValue(nameof(result.EndUserId), out string? endUserId))
                {
                    result.EndUserId = endUserId;
                }

                if (additionalProperties.TryGetValue(nameof(result.IncludeLogProbabilities), out bool includeLogProbabilities))
                {
                    result.IncludeLogProbabilities = includeLogProbabilities;
                }

                if (additionalProperties.TryGetValue(nameof(result.LogitBiases), out IDictionary<int, int>? logitBiases))
                {
                    foreach (KeyValuePair<int, int> kvp in logitBiases!)
                    {
                        result.LogitBiases[kvp.Key] = kvp.Value;
                    }
                }

                if (additionalProperties.TryGetValue(nameof(result.Metadata), out IDictionary<string, string>? metadata))
                {
                    foreach (KeyValuePair<string, string> kvp in metadata)
                    {
                        result.Metadata[kvp.Key] = kvp.Value;
                    }
                }

                if (additionalProperties.TryGetValue(nameof(result.OutputPrediction), out ChatOutputPrediction? outputPrediction))
                {
                    result.OutputPrediction = outputPrediction;
                }

                if (additionalProperties.TryGetValue(nameof(result.ReasoningEffortLevel), out ChatReasoningEffortLevel reasoningEffortLevel))
                {
                    result.ReasoningEffortLevel = reasoningEffortLevel;
                }

                if (additionalProperties.TryGetValue(nameof(result.ResponseModalities), out ChatResponseModalities responseModalities))
                {
                    result.ResponseModalities = responseModalities;
                }

                if (additionalProperties.TryGetValue(nameof(result.StoredOutputEnabled), out bool storeOutputEnabled))
                {
                    result.StoredOutputEnabled = storeOutputEnabled;
                }

                if (additionalProperties.TryGetValue(nameof(result.TopLogProbabilityCount), out int topLogProbabilityCountInt))
                {
                    result.TopLogProbabilityCount = topLogProbabilityCountInt;
                }
            }

            if (options.Tools is { Count: > 0 } tools)
            {
                foreach (AITool tool in tools)
                {
                    if (tool is AIFunction af)
                    {
                        result.Tools.Add(ToOpenAIChatTool(af));
                    }
                }

                if (result.Tools.Count > 0)
                {
                    switch (options.ToolMode)
                    {
                        case NoneChatToolMode:
                            result.ToolChoice = ChatToolChoice.CreateNoneChoice();
                            break;

                        case AutoChatToolMode:
                        case null:
                            result.ToolChoice = ChatToolChoice.CreateAutoChoice();
                            break;

                        case RequiredChatToolMode required:
                            result.ToolChoice = required.RequiredFunctionName is null ?
                                ChatToolChoice.CreateRequiredChoice() :
                                ChatToolChoice.CreateFunctionChoice(required.RequiredFunctionName);
                            break;
                    }
                }
            }

            if (options.ResponseFormat is ChatResponseFormatText)
            {
                result.ResponseFormat = OpenAI.Chat.ChatResponseFormat.CreateTextFormat();
            }
            else if (options.ResponseFormat is ChatResponseFormatJson jsonFormat)
            {
                result.ResponseFormat = jsonFormat.Schema is { } jsonSchema ?
                    OpenAI.Chat.ChatResponseFormat.CreateJsonSchemaFormat(
                        jsonFormat.SchemaName ?? "json_schema",
                        BinaryData.FromBytes(
                            JsonSerializer.SerializeToUtf8Bytes(jsonSchema, ChatClientJsonContext.Default.JsonElement)),
                        jsonFormat.SchemaDescription,
                        jsonSchemaIsStrict: true) :
                    OpenAI.Chat.ChatResponseFormat.CreateJsonObjectFormat();
            }
        }

        return result;
    }

    /// <summary>Converts an Extensions function to an OpenAI chat tool.</summary>
    private static ChatTool ToOpenAIChatTool(AIFunction aiFunction)
    {
        // Default strict to true, but allow to be overridden by an additional Strict property.
        bool strict =
            !aiFunction.AdditionalProperties.TryGetValue("Strict", out object? strictObj) ||
            strictObj is not bool strictValue ||
            strictValue;

        // Map to an intermediate model so that redundant properties are skipped.
        var tool = JsonSerializer.Deserialize(aiFunction.JsonSchema, ChatClientJsonContext.Default.ChatToolJson)!;
        var functionParameters = BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(tool, ChatClientJsonContext.Default.ChatToolJson));
        return ChatTool.CreateFunctionTool(aiFunction.Name, aiFunction.Description, functionParameters, strict);
    }

    private static UsageDetails FromOpenAIUsage(ChatTokenUsage tokenUsage)
    {
        var destination = new UsageDetails
        {
            InputTokenCount = tokenUsage.InputTokenCount,
            OutputTokenCount = tokenUsage.OutputTokenCount,
            TotalTokenCount = tokenUsage.TotalTokenCount,
            AdditionalCounts = [],
        };

        var counts = destination.AdditionalCounts;

        if (tokenUsage.InputTokenDetails is ChatInputTokenUsageDetails inputDetails)
        {
            const string InputDetails = nameof(ChatTokenUsage.InputTokenDetails);
            counts.Add($"{InputDetails}.{nameof(ChatInputTokenUsageDetails.AudioTokenCount)}", inputDetails.AudioTokenCount);
            counts.Add($"{InputDetails}.{nameof(ChatInputTokenUsageDetails.CachedTokenCount)}", inputDetails.CachedTokenCount);
        }

        if (tokenUsage.OutputTokenDetails is ChatOutputTokenUsageDetails outputDetails)
        {
            const string OutputDetails = nameof(ChatTokenUsage.OutputTokenDetails);
            counts.Add($"{OutputDetails}.{nameof(ChatOutputTokenUsageDetails.ReasoningTokenCount)}", outputDetails.ReasoningTokenCount);
            counts.Add($"{OutputDetails}.{nameof(ChatOutputTokenUsageDetails.AudioTokenCount)}", outputDetails.AudioTokenCount);
            counts.Add($"{OutputDetails}.{nameof(ChatOutputTokenUsageDetails.AcceptedPredictionTokenCount)}", outputDetails.AcceptedPredictionTokenCount);
            counts.Add($"{OutputDetails}.{nameof(ChatOutputTokenUsageDetails.RejectedPredictionTokenCount)}", outputDetails.RejectedPredictionTokenCount);
        }

        return destination;
    }

    /// <summary>Converts an OpenAI role to an Extensions role.</summary>
    private static ChatRole FromOpenAIChatRole(ChatMessageRole role) =>
        role switch
        {
            ChatMessageRole.System => ChatRole.System,
            ChatMessageRole.User => ChatRole.User,
            ChatMessageRole.Assistant => ChatRole.Assistant,
            ChatMessageRole.Tool => ChatRole.Tool,
            ChatMessageRole.Developer => ChatRoleDeveloper,
            _ => new ChatRole(role.ToString()),
        };

    /// <summary>Creates an <see cref="AIContent"/> from a <see cref="ChatMessageContentPart"/>.</summary>
    /// <param name="contentPart">The content part to convert into a content.</param>
    /// <returns>The constructed <see cref="AIContent"/>, or <see langword="null"/> if the content part could not be converted.</returns>
    private static AIContent? ToAIContent(ChatMessageContentPart contentPart)
    {
        AIContent? aiContent = null;

        if (contentPart.Kind == ChatMessageContentPartKind.Text)
        {
            aiContent = new TextContent(contentPart.Text);
        }
        else if (contentPart.Kind == ChatMessageContentPartKind.Image)
        {
            aiContent =
                contentPart.ImageUri is not null ? new UriContent(contentPart.ImageUri, "image/*") :
                contentPart.ImageBytes is not null ? new DataContent(contentPart.ImageBytes.ToMemory(), contentPart.ImageBytesMediaType) :
                null;

            if (aiContent is not null && contentPart.ImageDetailLevel?.ToString() is string detail)
            {
                (aiContent.AdditionalProperties ??= [])[nameof(contentPart.ImageDetailLevel)] = detail;
            }
        }

        if (aiContent is not null)
        {
            if (contentPart.Refusal is string refusal)
            {
                (aiContent.AdditionalProperties ??= [])[nameof(contentPart.Refusal)] = refusal;
            }

            aiContent.RawRepresentation = contentPart;
        }

        return aiContent;
    }

    /// <summary>Converts an OpenAI finish reason to an Extensions finish reason.</summary>
    private static ChatFinishReason? FromOpenAIFinishReason(OpenAI.Chat.ChatFinishReason? finishReason) =>
        finishReason?.ToString() is not string s ? null :
        finishReason switch
        {
            OpenAI.Chat.ChatFinishReason.Stop => ChatFinishReason.Stop,
            OpenAI.Chat.ChatFinishReason.Length => ChatFinishReason.Length,
            OpenAI.Chat.ChatFinishReason.ContentFilter => ChatFinishReason.ContentFilter,
            OpenAI.Chat.ChatFinishReason.ToolCalls or OpenAI.Chat.ChatFinishReason.FunctionCall => ChatFinishReason.ToolCalls,
            _ => new ChatFinishReason(s),
        };

    private static FunctionCallContent ParseCallContentFromJsonString(string json, string callId, string name) =>
        FunctionCallContent.CreateFromParsedArguments(json, callId, name,
            argumentParser: (Func<string, IDictionary<string, object?>?>)(json => JsonSerializer.Deserialize(json, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<IDictionary<string, object>>)ChatClientJsonContext.Default.IDictionaryStringObject)!));

    private static FunctionCallContent ParseCallContentFromBinaryData(BinaryData ut8Json, string callId, string name) =>
        FunctionCallContent.CreateFromParsedArguments(ut8Json, callId, name,
            argumentParser: (Func<BinaryData, IDictionary<string, object?>?>)(json => JsonSerializer.Deserialize(json, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<IDictionary<string, object>>)ChatClientJsonContext.Default.IDictionaryStringObject)!));

    /// <summary>Used to create the JSON payload for an OpenAI chat tool description.</summary>
    private sealed class ChatToolJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("required")]
        public HashSet<string> Required { get; set; } = [];

        [JsonPropertyName("properties")]
        public Dictionary<string, JsonElement> Properties { get; set; } = [];

        [JsonPropertyName("additionalProperties")]
        public bool AdditionalProperties { get; set; }
    }

    /// <summary>POCO representing function calling info. Used to concatenation information for a single function call from across multiple streaming updates.</summary>
    private sealed class FunctionCallInfo
    {
        public string? CallId;
        public string? Name;
        public StringBuilder? Arguments;
    }

    /// <summary>Source-generated JSON type information.</summary>
    [JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
        UseStringEnumConverter = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true)]
    [JsonSerializable(typeof(ChatToolJson))]
    [JsonSerializable(typeof(IDictionary<string, object?>))]
    [JsonSerializable(typeof(string[]))]
    private sealed partial class ChatClientJsonContext : JsonSerializerContext;
}

/// <summary>Provides extension methods for working with <see cref="OpenAIClient"/>s.</summary>
public static class OpenAIClientExtensions
{
    /// <summary>Gets an <see cref="IChatClient"/> for use with this <see cref="OpenAIClient"/>.</summary>
    /// <param name="openAIClient">The client.</param>
    /// <param name="modelId">The model.</param>
    /// <returns>An <see cref="IChatClient"/> that can be used to converse via the <see cref="OpenAIClient"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method will be removed in an upcoming release.")]
    public static IChatClient AsChatClient(this OpenAIClient openAIClient, string modelId) =>
        new OpenAIChatClient(Throw.IfNull(openAIClient).GetChatClient(modelId));

    /// <summary>Gets an <see cref="IChatClient"/> for use with this <see cref="ChatClient"/>.</summary>
    /// <param name="chatClient">The client.</param>
    /// <returns>An <see cref="IChatClient"/> that can be used to converse via the <see cref="ChatClient"/>.</returns>
    public static IChatClient AsIChatClient(this ChatClient chatClient) =>
        new OpenAIChatClient(chatClient);

    /// <summary>Gets an <see cref="IChatClient"/> for use with this <see cref="OpenAIResponseClient"/>.</summary>
    /// <param name="responseClient">The client.</param>
    /// <returns>An <see cref="IChatClient"/> that can be used to converse via the <see cref="OpenAIResponseClient"/>.</returns>
    public static IChatClient AsIChatClient(this OpenAIResponseClient responseClient) =>
        new OpenAIResponseChatClient(responseClient);

    /// <summary>Gets an <see cref="ISpeechToTextClient"/> for use with this <see cref="AudioClient"/>.</summary>
    /// <param name="audioClient">The client.</param>
    /// <returns>An <see cref="ISpeechToTextClient"/> that can be used to transcribe audio via the <see cref="AudioClient"/>.</returns>
    [Experimental("MEAI001")]
    public static ISpeechToTextClient AsISpeechToTextClient(this AudioClient audioClient) =>
        new OpenAISpeechToTextClient(audioClient);

    /// <summary>Gets an <see cref="IEmbeddingGenerator{String, Single}"/> for use with this <see cref="OpenAIClient"/>.</summary>
    /// <param name="openAIClient">The client.</param>
    /// <param name="modelId">The model to use.</param>
    /// <param name="dimensions">The number of dimensions to generate in each embedding.</param>
    /// <returns>An <see cref="IEmbeddingGenerator{String, Embedding}"/> that can be used to generate embeddings via the <see cref="EmbeddingClient"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method will be removed in an upcoming release.")]
    public static IEmbeddingGenerator<string, Embedding<float>> AsEmbeddingGenerator(this OpenAIClient openAIClient, string modelId, int? dimensions = null) =>
        new OpenAIEmbeddingGenerator(Throw.IfNull(openAIClient).GetEmbeddingClient(modelId), dimensions);

    /// <summary>Gets an <see cref="IEmbeddingGenerator{String, Single}"/> for use with this <see cref="EmbeddingClient"/>.</summary>
    /// <param name="embeddingClient">The client.</param>
    /// <param name="defaultModelDimensions">The number of dimensions to generate in each embedding.</param>
    /// <returns>An <see cref="IEmbeddingGenerator{String, Embedding}"/> that can be used to generate embeddings via the <see cref="EmbeddingClient"/>.</returns>
    public static IEmbeddingGenerator<string, Embedding<float>> AsIEmbeddingGenerator(this EmbeddingClient embeddingClient, int? defaultModelDimensions = null) =>
        new OpenAIEmbeddingGenerator(embeddingClient, defaultModelDimensions);
}

/// <summary>An <see cref="IEmbeddingGenerator{String, Embedding}"/> for an OpenAI <see cref="EmbeddingClient"/>.</summary>
internal sealed class OpenAIEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    /// <summary>Default OpenAI endpoint.</summary>
    private const string DefaultOpenAIEndpoint = "https://api.openai.com/v1";

    /// <summary>Metadata about the embedding generator.</summary>
    private readonly EmbeddingGeneratorMetadata _metadata;

    /// <summary>The underlying <see cref="OpenAI.Chat.ChatClient" />.</summary>
    private readonly EmbeddingClient _embeddingClient;

    /// <summary>The number of dimensions produced by the generator.</summary>
    private readonly int? _dimensions;

    /// <summary>Initializes a new instance of the <see cref="OpenAIEmbeddingGenerator"/> class.</summary>
    /// <param name="embeddingClient">The underlying client.</param>
    /// <param name="defaultModelDimensions">The number of dimensions to generate in each embedding.</param>
    /// <exception cref="ArgumentNullException"><paramref name="embeddingClient"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultModelDimensions"/> is not positive.</exception>
    public OpenAIEmbeddingGenerator(EmbeddingClient embeddingClient, int? defaultModelDimensions = null)
    {
        _ = Throw.IfNull(embeddingClient);
        if (defaultModelDimensions < 1)
        {
            Throw.ArgumentOutOfRangeException(nameof(defaultModelDimensions), "Value must be greater than 0.");
        }

        _embeddingClient = embeddingClient;
        _dimensions = defaultModelDimensions;

        // https://github.com/openai/openai-dotnet/issues/215
        // The endpoint and model aren't currently exposed, so use reflection to get at them, temporarily. Once packages
        // implement the abstractions directly rather than providing adapters on top of the public APIs,
        // the package can provide such implementations separate from what's exposed in the public API.
        string providerUrl = (typeof(EmbeddingClient).GetField("_endpoint", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(embeddingClient) as Uri)?.ToString() ??
            DefaultOpenAIEndpoint;

        FieldInfo? modelField = typeof(EmbeddingClient).GetField("_model", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        string? modelId = modelField?.GetValue(embeddingClient) as string;

        _metadata = CreateMetadata("openai", providerUrl, modelId, defaultModelDimensions);
    }

    /// <inheritdoc />
    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        OpenAI.Embeddings.EmbeddingGenerationOptions? openAIOptions = ToOpenAIOptions(options);

        var embeddings = (await _embeddingClient.GenerateEmbeddingsAsync(values, openAIOptions, cancellationToken).ConfigureAwait(false)).Value;

        return new(embeddings.Select(e =>
                new Embedding<float>(e.ToFloats())
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    ModelId = embeddings.Model,
                }))
        {
            Usage = new()
            {
                InputTokenCount = embeddings.Usage.InputTokenCount,
                TotalTokenCount = embeddings.Usage.TotalTokenCount
            },
        };
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        // Nothing to dispose. Implementation required for the IEmbeddingGenerator interface.
    }

    /// <inheritdoc />
    object? IEmbeddingGenerator.GetService(Type serviceType, object? serviceKey)
    {
        _ = Throw.IfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(EmbeddingGeneratorMetadata) ? _metadata :
            serviceType == typeof(EmbeddingClient) ? _embeddingClient :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    /// <summary>Creates the <see cref="EmbeddingGeneratorMetadata"/> for this instance.</summary>
    private static EmbeddingGeneratorMetadata CreateMetadata(string providerName, string providerUrl, string? defaultModelId, int? defaultModelDimensions) =>
        new(providerName, Uri.TryCreate(providerUrl, UriKind.Absolute, out Uri? providerUri) ? providerUri : null, defaultModelId, defaultModelDimensions);

    /// <summary>Converts an extensions options instance to an OpenAI options instance.</summary>
    private OpenAI.Embeddings.EmbeddingGenerationOptions? ToOpenAIOptions(EmbeddingGenerationOptions? options)
    {
        OpenAI.Embeddings.EmbeddingGenerationOptions openAIOptions = new()
        {
            Dimensions = options?.Dimensions ?? _dimensions,
        };

        if (options?.AdditionalProperties is { Count: > 0 } additionalProperties)
        {
            if (additionalProperties.TryGetValue(nameof(openAIOptions.EndUserId), out string? endUserId))
            {
                openAIOptions.EndUserId = endUserId;
            }
        }

        return openAIOptions;
    }
}

/// <summary>Represents an <see cref="IChatClient"/> for an <see cref="OpenAIResponseClient"/>.</summary>
internal sealed partial class OpenAIResponseChatClient : IChatClient
{
    /// <summary>Gets the default OpenAI endpoint.</summary>
    private static Uri DefaultOpenAIEndpoint { get; } = new("https://api.openai.com/v1");

    /// <summary>A <see cref="ChatRole"/> for "developer".</summary>
    private static readonly ChatRole _chatRoleDeveloper = new("developer");

    /// <summary>Metadata about the client.</summary>
    private readonly ChatClientMetadata _metadata;

    /// <summary>The underlying <see cref="OpenAIResponseClient" />.</summary>
    private readonly OpenAIResponseClient _responseClient;

    /// <summary>Initializes a new instance of the <see cref="OpenAIResponseChatClient"/> class for the specified <see cref="OpenAIResponseClient"/>.</summary>
    /// <param name="responseClient">The underlying client.</param>
    /// <exception cref="ArgumentNullException"><paramref name="responseClient"/> is <see langword="null"/>.</exception>
    public OpenAIResponseChatClient(OpenAIResponseClient responseClient)
    {
        _ = Throw.IfNull(responseClient);

        _responseClient = responseClient;

        // https://github.com/openai/openai-dotnet/issues/215
        // The endpoint and model aren't currently exposed, so use reflection to get at them, temporarily. Once packages
        // implement the abstractions directly rather than providing adapters on top of the public APIs,
        // the package can provide such implementations separate from what's exposed in the public API.
        Uri providerUrl = typeof(OpenAIResponseClient).GetField("_endpoint", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(responseClient) as Uri ?? DefaultOpenAIEndpoint;
        string? model = typeof(OpenAIResponseClient).GetField("_model", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(responseClient) as string;

        _metadata = new("openai", providerUrl, model);
    }

    /// <inheritdoc />
    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        _ = Throw.IfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? _metadata :
            serviceType == typeof(OpenAIResponseClient) ? _responseClient :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    /// <inheritdoc />
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(messages);

        // Convert the inputs into what OpenAIResponseClient expects.
        var openAIResponseItems = ToOpenAIResponseItems(messages);
        var openAIOptions = ToOpenAIResponseCreationOptions(options);

        // Make the call to the OpenAIResponseClient.
        var openAIResponse = (await _responseClient.CreateResponseAsync(openAIResponseItems, openAIOptions, cancellationToken).ConfigureAwait(false)).Value;

        // Convert and return the results.
        ChatResponse response = new()
        {
            ResponseId = openAIResponse.Id,
            ChatThreadId = openAIResponse.Id,
            CreatedAt = openAIResponse.CreatedAt,
            FinishReason = ToFinishReason(openAIResponse.IncompleteStatusDetails?.Reason),
            Messages = [new(ChatRole.Assistant, new List<AIContent>())],
            ModelId = openAIResponse.Model,
            Usage = ToUsageDetails(openAIResponse),
        };

        if (!string.IsNullOrEmpty(openAIResponse.EndUserId))
        {
            (response.AdditionalProperties ??= [])[nameof(openAIResponse.EndUserId)] = openAIResponse.EndUserId;
        }

        if (openAIResponse.Error is not null)
        {
            (response.AdditionalProperties ??= [])[nameof(openAIResponse.Error)] = openAIResponse.Error;
        }

        if (openAIResponse.OutputItems is not null)
        {
            ChatMessage message = response.Messages[0];
            Debug.Assert(message.Contents is List<AIContent>, "Expected a List<AIContent> for message contents.");

            foreach (ResponseItem outputItem in openAIResponse.OutputItems)
            {
                switch (outputItem)
                {
                    case MessageResponseItem messageItem:
                        message.MessageId = messageItem.Id;
                        message.RawRepresentation = messageItem;
                        message.Role = ToChatRole(messageItem.Role);
                        (message.AdditionalProperties ??= []).Add(nameof(messageItem.Id), messageItem.Id);
                        ((List<AIContent>)message.Contents).AddRange(ToAIContents(messageItem.Content));
                        break;

                    case FunctionCallResponseItem functionCall:
                        response.FinishReason ??= ChatFinishReason.ToolCalls;
                        message.Contents.Add(
                            FunctionCallContent.CreateFromParsedArguments(
                                functionCall.FunctionArguments.ToMemory(),
                                functionCall.CallId,
                                functionCall.FunctionName,
                                static json => JsonSerializer.Deserialize(json.Span, ResponseClientJsonContext.Default.IDictionaryStringObject)!));
                        break;
                }
            }

            if (openAIResponse.Error is { } error)
            {
                message.Contents.Add(new ErrorContent(error.Message) { ErrorCode = error.Code });
            }
        }

        return response;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(messages);

        // Convert the inputs into what OpenAIResponseClient expects.
        var openAIResponseItems = ToOpenAIResponseItems(messages);
        var openAIOptions = ToOpenAIResponseCreationOptions(options);

        // Make the call to the OpenAIResponseClient and process the streaming results.
        DateTimeOffset? createdAt = null;
        string? responseId = null;
        string? modelId = null;
        string? lastMessageId = null;
        ChatRole? lastRole = null;
        Dictionary<int, MessageResponseItem> outputIndexToMessages = [];
        Dictionary<int, FunctionCallInfo>? functionCallInfos = null;
        await foreach (var streamingUpdate in _responseClient.CreateResponseStreamingAsync(openAIResponseItems, openAIOptions, cancellationToken).ConfigureAwait(false))
        {
            switch (streamingUpdate)
            {
                case StreamingResponseCreatedUpdate createdUpdate:
                    createdAt = createdUpdate.Response.CreatedAt;
                    responseId = createdUpdate.Response.Id;
                    modelId = createdUpdate.Response.Model;
                    break;

                case StreamingResponseCompletedUpdate completedUpdate:
                    yield return new()
                    {
                        Contents = ToUsageDetails(completedUpdate.Response) is { } usage ? [new UsageContent(usage)] : [],
                        CreatedAt = createdAt,
                        ResponseId = responseId,
                        ChatThreadId = responseId,
                        FinishReason =
                            ToFinishReason(completedUpdate.Response?.IncompleteStatusDetails?.Reason) ??
                            (functionCallInfos is not null ? ChatFinishReason.ToolCalls : ChatFinishReason.Stop),
                        MessageId = lastMessageId,
                        ModelId = modelId,
                        Role = lastRole,
                    };
                    break;

                case StreamingResponseOutputItemAddedUpdate outputItemAddedUpdate:
                    switch (outputItemAddedUpdate.Item)
                    {
                        case MessageResponseItem mri:
                            outputIndexToMessages[outputItemAddedUpdate.OutputIndex] = mri;
                            break;

                        case FunctionCallResponseItem fcri:
                            (functionCallInfos ??= [])[outputItemAddedUpdate.OutputIndex] = new(fcri);
                            break;
                    }

                    break;

                case StreamingResponseOutputItemDoneUpdate outputItemDoneUpdate:
                    _ = outputIndexToMessages.Remove(outputItemDoneUpdate.OutputIndex);
                    break;

                case StreamingResponseOutputTextDeltaUpdate outputTextDeltaUpdate:
                    _ = outputIndexToMessages.TryGetValue(outputTextDeltaUpdate.OutputIndex, out MessageResponseItem? messageItem);
                    lastMessageId = messageItem?.Id;
                    lastRole = ToChatRole(messageItem?.Role);
                    yield return new ChatResponseUpdate(lastRole, outputTextDeltaUpdate.Delta)
                    {
                        CreatedAt = createdAt,
                        MessageId = lastMessageId,
                        ModelId = modelId,
                        ResponseId = responseId,
                        ChatThreadId = responseId,
                    };
                    break;

                case StreamingResponseFunctionCallArgumentsDeltaUpdate functionCallArgumentsDeltaUpdate:
                    {
                        if (functionCallInfos?.TryGetValue(functionCallArgumentsDeltaUpdate.OutputIndex, out FunctionCallInfo? callInfo) is true)
                        {
                            _ = (callInfo.Arguments ??= new()).Append(functionCallArgumentsDeltaUpdate.Delta);
                        }

                        break;
                    }

                case StreamingResponseFunctionCallArgumentsDoneUpdate functionCallOutputDoneUpdate:
                    {
                        if (functionCallInfos?.TryGetValue(functionCallOutputDoneUpdate.OutputIndex, out FunctionCallInfo? callInfo) is true)
                        {
                            _ = functionCallInfos.Remove(functionCallOutputDoneUpdate.OutputIndex);

                            var fci = FunctionCallContent.CreateFromParsedArguments(
                                callInfo.Arguments?.ToString() ?? string.Empty,
                                callInfo.ResponseItem.CallId,
                                callInfo.ResponseItem.FunctionName,
                                static json => JsonSerializer.Deserialize(json, ResponseClientJsonContext.Default.IDictionaryStringObject)!);

                            lastMessageId = callInfo.ResponseItem.Id;
                            lastRole = ChatRole.Assistant;
                            yield return new ChatResponseUpdate(lastRole, [fci])
                            {
                                CreatedAt = createdAt,
                                MessageId = lastMessageId,
                                ModelId = modelId,
                                ResponseId = responseId,
                                ChatThreadId = responseId,
                            };
                        }

                        break;
                    }

                case StreamingResponseErrorUpdate errorUpdate:
                    yield return new ChatResponseUpdate
                    {
                        CreatedAt = createdAt,
                        MessageId = lastMessageId,
                        ModelId = modelId,
                        ResponseId = responseId,
                        ChatThreadId = responseId,
                        Contents =
                        [
                            new ErrorContent(errorUpdate.Message)
                            {
                                ErrorCode = errorUpdate.Code,
                                Details = errorUpdate.Param,
                            }
                        ],
                    };
                    break;
            }
        }
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        // Nothing to dispose. Implementation required for the IChatClient interface.
    }

    /// <summary>Creates a <see cref="ChatRole"/> from a <see cref="MessageRole"/>.</summary>
    private static ChatRole ToChatRole(MessageRole? role) =>
        role switch
        {
            MessageRole.System => ChatRole.System,
            MessageRole.Developer => _chatRoleDeveloper,
            MessageRole.User => ChatRole.User,
            _ => ChatRole.Assistant,
        };

    /// <summary>Creates a <see cref="ChatFinishReason"/> from a <see cref="ResponseIncompleteStatusReason"/>.</summary>
    private static ChatFinishReason? ToFinishReason(ResponseIncompleteStatusReason? statusReason) =>
        statusReason == ResponseIncompleteStatusReason.ContentFilter ? ChatFinishReason.ContentFilter :
        statusReason == ResponseIncompleteStatusReason.MaxOutputTokens ? ChatFinishReason.Length :
        null;

    /// <summary>Converts a <see cref="ChatOptions"/> to a <see cref="ResponseCreationOptions"/>.</summary>
    private static ResponseCreationOptions ToOpenAIResponseCreationOptions(ChatOptions? options)
    {
        ResponseCreationOptions result = new();

        if (options is not null)
        {
            // Handle strongly-typed properties.
            result.MaxOutputTokenCount = options.MaxOutputTokens;
            result.PreviousResponseId = options.ChatThreadId;
            result.TopP = options.TopP;
            result.Temperature = options.Temperature;

            // Handle loosely-typed properties from AdditionalProperties.
            if (options.AdditionalProperties is { Count: > 0 } additionalProperties)
            {
                if (additionalProperties.TryGetValue(nameof(result.ParallelToolCallsEnabled), out bool allowParallelToolCalls))
                {
                    result.ParallelToolCallsEnabled = allowParallelToolCalls;
                }

                if (additionalProperties.TryGetValue(nameof(result.EndUserId), out string? endUserId))
                {
                    result.EndUserId = endUserId;
                }

                if (additionalProperties.TryGetValue(nameof(result.Instructions), out string? instructions))
                {
                    result.Instructions = instructions;
                }

                if (additionalProperties.TryGetValue(nameof(result.Metadata), out IDictionary<string, string>? metadata))
                {
                    foreach (KeyValuePair<string, string> kvp in metadata)
                    {
                        result.Metadata[kvp.Key] = kvp.Value;
                    }
                }

                if (additionalProperties.TryGetValue(nameof(result.ReasoningOptions), out ResponseReasoningOptions? reasoningOptions))
                {
                    result.ReasoningOptions = reasoningOptions;
                }

                if (additionalProperties.TryGetValue(nameof(result.StoredOutputEnabled), out bool storeOutputEnabled))
                {
                    result.StoredOutputEnabled = storeOutputEnabled;
                }

                if (additionalProperties.TryGetValue(nameof(result.TruncationMode), out ResponseTruncationMode truncationMode))
                {
                    result.TruncationMode = truncationMode;
                }
            }

            // Populate tools if there are any.
            if (options.Tools is { Count: > 0 } tools)
            {
                foreach (AITool tool in tools)
                {
                    switch (tool)
                    {
                        case AIFunction af:
                            var oaitool = JsonSerializer.Deserialize(af.JsonSchema, ResponseClientJsonContext.Default.ResponseToolJson)!;
                            var functionParameters = BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(oaitool, ResponseClientJsonContext.Default.ResponseToolJson));
                            result.Tools.Add(ResponseTool.CreateFunctionTool(af.Name, af.Description, functionParameters));
                            break;

                        case HostedWebSearchTool:
                            WebSearchToolLocation? location = null;
                            if (tool.AdditionalProperties.TryGetValue(nameof(WebSearchToolLocation), out object? objLocation))
                            {
                                location = objLocation as WebSearchToolLocation;
                            }

                            WebSearchToolContextSize? size = null;
                            if (tool.AdditionalProperties.TryGetValue(nameof(WebSearchToolContextSize), out object? objSize) &&
                                objSize is WebSearchToolContextSize)
                            {
                                size = (WebSearchToolContextSize)objSize;
                            }

                            result.Tools.Add(ResponseTool.CreateWebSearchTool(location, size));
                            break;
                    }
                }

                switch (options.ToolMode)
                {
                    case NoneChatToolMode:
                        result.ToolChoice = ResponseToolChoice.CreateNoneChoice();
                        break;

                    case AutoChatToolMode:
                    case null:
                        result.ToolChoice = ResponseToolChoice.CreateAutoChoice();
                        break;

                    case RequiredChatToolMode required:
                        result.ToolChoice = required.RequiredFunctionName is not null ?
                            ResponseToolChoice.CreateFunctionChoice(required.RequiredFunctionName) :
                            ResponseToolChoice.CreateRequiredChoice();
                        break;
                }
            }

            // Handle response format.
            if (options.ResponseFormat is ChatResponseFormatText)
            {
                result.TextOptions = new()
                {
                    TextFormat = ResponseTextFormat.CreateTextFormat()
                };
            }
            else if (options.ResponseFormat is ChatResponseFormatJson jsonFormat)
            {
                result.TextOptions = new()
                {
                    TextFormat = jsonFormat.Schema is { } jsonSchema ?
                        ResponseTextFormat.CreateJsonSchemaFormat(
                            jsonFormat.SchemaName ?? "json_schema",
                            BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(jsonSchema, ResponseClientJsonContext.Default.JsonElement)),
                            jsonFormat.SchemaDescription,
                            jsonSchemaIsStrict: true) :
                        ResponseTextFormat.CreateJsonObjectFormat(),
                };
            }
        }

        return result;
    }

    /// <summary>Convert a sequence of <see cref="ChatMessage"/>s to <see cref="ResponseItem"/>s.</summary>
    private static IEnumerable<ResponseItem> ToOpenAIResponseItems(
        IEnumerable<ChatMessage> inputs)
    {
        foreach (ChatMessage input in inputs)
        {
            if (input.Role == ChatRole.System ||
                input.Role == _chatRoleDeveloper)
            {
                string text = input.Text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    yield return input.Role == ChatRole.System ?
                        ResponseItem.CreateSystemMessageItem(text) :
                        ResponseItem.CreateDeveloperMessageItem(text);
                }

                continue;
            }

            if (input.Role == ChatRole.User)
            {
                yield return ResponseItem.CreateUserMessageItem(ToOpenAIResponsesContent(input.Contents));
                continue;
            }

            if (input.Role == ChatRole.Tool)
            {
                foreach (AIContent item in input.Contents)
                {
                    switch (item)
                    {
                        case FunctionResultContent resultContent:
                            string? result = resultContent.Result as string;
                            if (result is null && resultContent.Result is not null)
                            {
                                try
                                {
                                    result = JsonSerializer.Serialize(resultContent.Result, AIJsonUtilities.DefaultOptions.GetTypeInfo(typeof(object)));
                                }
                                catch (NotSupportedException)
                                {
                                    // If the type can't be serialized, skip it.
                                }
                            }

                            yield return ResponseItem.CreateFunctionCallOutputItem(resultContent.CallId, result ?? string.Empty);
                            break;
                    }
                }

                continue;
            }

            if (input.Role == ChatRole.Assistant)
            {
                foreach (AIContent item in input.Contents)
                {
                    switch (item)
                    {
                        case TextContent textContent:
                            yield return ResponseItem.CreateAssistantMessageItem(textContent.Text);
                            break;

                        case FunctionCallContent callContent:
                            yield return ResponseItem.CreateFunctionCallItem(
                                callContent.CallId,
                                callContent.Name,
                                BinaryData.FromBytes(JsonSerializer.SerializeToUtf8Bytes(
                                    callContent.Arguments,
                                    AIJsonUtilities.DefaultOptions.GetTypeInfo(typeof(IDictionary<string, object?>)))));
                            break;
                    }
                }

                continue;
            }
        }
    }

    /// <summary>Extract usage details from an <see cref="OpenAIResponse"/>.</summary>
    private static UsageDetails? ToUsageDetails(OpenAIResponse? openAIResponse)
    {
        UsageDetails? ud = null;
        if (openAIResponse?.Usage is { } usage)
        {
            ud = new()
            {
                InputTokenCount = usage.InputTokenCount,
                OutputTokenCount = usage.OutputTokenCount,
                TotalTokenCount = usage.TotalTokenCount,
            };

            if (usage.OutputTokenDetails is { } outputDetails)
            {
                ud.AdditionalCounts ??= [];

                const string OutputDetails = nameof(usage.OutputTokenDetails);
                ud.AdditionalCounts.Add($"{OutputDetails}.{nameof(outputDetails.ReasoningTokenCount)}", outputDetails.ReasoningTokenCount);
            }
        }

        return ud;
    }

    /// <summary>Convert a sequence of <see cref="ResponseContentPart"/>s to a list of <see cref="AIContent"/>.</summary>
    private static List<AIContent> ToAIContents(IEnumerable<ResponseContentPart> contents)
    {
        List<AIContent> results = [];

        foreach (ResponseContentPart part in contents)
        {
            if (part.Kind == ResponseContentPartKind.OutputText)
            {
                results.Add(new TextContent(part.Text));
            }
        }

        return results;
    }

    /// <summary>Convert a list of <see cref="AIContent"/>s to a list of <see cref="ResponseContentPart"/>.</summary>
    private static List<ResponseContentPart> ToOpenAIResponsesContent(IList<AIContent> contents)
    {
        List<ResponseContentPart> parts = [];
        foreach (var content in contents)
        {
            switch (content)
            {
                case TextContent textContent:
                    parts.Add(ResponseContentPart.CreateInputTextPart(textContent.Text));
                    break;

                case UriContent uriContent when uriContent.HasTopLevelMediaType("image"):
                    parts.Add(ResponseContentPart.CreateInputImagePart(uriContent.Uri));
                    break;

                case DataContent dataContent when dataContent.HasTopLevelMediaType("image"):
                    parts.Add(ResponseContentPart.CreateInputImagePart(BinaryData.FromBytes(dataContent.Data), dataContent.MediaType));
                    break;
            }
        }

        if (parts.Count == 0)
        {
            parts.Add(ResponseContentPart.CreateInputTextPart(string.Empty));
        }

        return parts;
    }

    /// <summary>Used to create the JSON payload for an OpenAI chat tool description.</summary>
    private sealed class ResponseToolJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        [JsonPropertyName("required")]
        public HashSet<string> Required { get; set; } = [];

        [JsonPropertyName("properties")]
        public Dictionary<string, JsonElement> Properties { get; set; } = [];

        [JsonPropertyName("additionalProperties")]
        public bool AdditionalProperties { get; set; }
    }

    /// <summary>POCO representing function calling info.</summary>
    /// <remarks>Used to concatenation information for a single function call from across multiple streaming updates.</remarks>
    private sealed class FunctionCallInfo(FunctionCallResponseItem item)
    {
        public readonly FunctionCallResponseItem ResponseItem = item;
        public StringBuilder? Arguments;
    }

    /// <summary>Source-generated JSON type information.</summary>
    [JsonSourceGenerationOptions(JsonSerializerDefaults.Web,
        UseStringEnumConverter = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true)]
    [JsonSerializable(typeof(ResponseToolJson))]
    [JsonSerializable(typeof(JsonElement))]
    [JsonSerializable(typeof(IDictionary<string, object?>))]
    [JsonSerializable(typeof(string[]))]
    private sealed partial class ResponseClientJsonContext : JsonSerializerContext;
}

/// <summary>Represents an <see cref="ISpeechToTextClient"/> for an OpenAI <see cref="OpenAIClient"/> or <see cref="OpenAI.Audio.AudioClient"/>.</summary>
[Experimental("MEAI001")]
internal sealed class OpenAISpeechToTextClient : ISpeechToTextClient
{
    /// <summary>Default OpenAI endpoint.</summary>
    private static readonly Uri _defaultOpenAIEndpoint = new("https://api.openai.com/v1");

    /// <summary>Metadata about the client.</summary>
    private readonly SpeechToTextClientMetadata _metadata;

    /// <summary>The underlying <see cref="AudioClient" />.</summary>
    private readonly AudioClient _audioClient;

    /// <summary>Initializes a new instance of the <see cref="OpenAISpeechToTextClient"/> class for the specified <see cref="AudioClient"/>.</summary>
    /// <param name="audioClient">The underlying client.</param>
    public OpenAISpeechToTextClient(AudioClient audioClient)
    {
        _ = Throw.IfNull(audioClient);

        _audioClient = audioClient;

        // https://github.com/openai/openai-dotnet/issues/215
        // The endpoint and model aren't currently exposed, so use reflection to get at them, temporarily. Once packages
        // implement the abstractions directly rather than providing adapters on top of the public APIs,
        // the package can provide such implementations separate from what's exposed in the public API.
        Uri providerUrl = typeof(AudioClient).GetField("_endpoint", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(audioClient) as Uri ?? _defaultOpenAIEndpoint;
        string? model = typeof(AudioClient).GetField("_model", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(audioClient) as string;

        _metadata = new("openai", providerUrl, model);
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        _ = Throw.IfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(SpeechToTextClientMetadata) ? _metadata :
            serviceType == typeof(AudioClient) ? _audioClient :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<SpeechToTextResponseUpdate> GetStreamingTextAsync(
        Stream audioSpeechStream, SpeechToTextOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(audioSpeechStream);

        var speechResponse = await GetTextAsync(audioSpeechStream, options, cancellationToken).ConfigureAwait(false);

        foreach (var update in speechResponse.ToSpeechToTextResponseUpdates())
        {
            yield return update;
        }
    }

    /// <inheritdoc />
    public async Task<SpeechToTextResponse> GetTextAsync(
        Stream audioSpeechStream, SpeechToTextOptions? options = null, CancellationToken cancellationToken = default)
    {
        _ = Throw.IfNull(audioSpeechStream);

        SpeechToTextResponse response = new();

        // <summary>A translation is triggered when the target text language is specified and the source language is not provided or different.</summary>
        static bool IsTranslationRequest(SpeechToTextOptions? options)
             => options is not null && options.TextLanguage is not null
                && (options.SpeechLanguage is null || options.SpeechLanguage != options.TextLanguage);

        if (IsTranslationRequest(options))
        {
            _ = Throw.IfNull(options);

            var openAIOptions = ToOpenAITranslationOptions(options);
            AudioTranslation translationResult;

#if NET
            await using (audioSpeechStream.ConfigureAwait(false))
#else
            using (audioSpeechStream)
#endif
            {
                translationResult = (await _audioClient.TranslateAudioAsync(
                    audioSpeechStream,
                    "file.wav", // this information internally is required but is only being used to create a header name in the multipart request.
                    openAIOptions, cancellationToken).ConfigureAwait(false)).Value;
            }

            UpdateResponseFromOpenAIAudioTranslation(response, translationResult);
        }
        else
        {
            var openAIOptions = ToOpenAITranscriptionOptions(options);

            // Transcription request
            AudioTranscription transcriptionResult;

#if NET
            await using (audioSpeechStream.ConfigureAwait(false))
#else
            using (audioSpeechStream)
#endif
            {
                transcriptionResult = (await _audioClient.TranscribeAudioAsync(
                    audioSpeechStream,
                    "file.wav", // this information internally is required but is only being used to create a header name in the multipart request.
                    openAIOptions, cancellationToken).ConfigureAwait(false)).Value;
            }

            UpdateResponseFromOpenAIAudioTranscription(response, transcriptionResult);
        }

        return response;
    }

    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        // Nothing to dispose. Implementation required for the IAudioTranscriptionClient interface.
    }

    /// <summary>Updates a <see cref="SpeechToTextResponse"/> from an OpenAI <see cref="AudioTranscription"/>.</summary>
    /// <param name="response">The response to update.</param>
    /// <param name="audioTranscription">The OpenAI audio transcription.</param>
    private static void UpdateResponseFromOpenAIAudioTranscription(SpeechToTextResponse response, AudioTranscription audioTranscription)
    {
        _ = Throw.IfNull(audioTranscription);

        var segmentCount = audioTranscription.Segments.Count;
        var wordCount = audioTranscription.Words.Count;

        TimeSpan? endTime = null;
        TimeSpan? startTime = null;
        if (segmentCount > 0)
        {
            endTime = audioTranscription.Segments[segmentCount - 1].EndTime;
            startTime = audioTranscription.Segments[0].StartTime;
        }
        else if (wordCount > 0)
        {
            endTime = audioTranscription.Words[wordCount - 1].EndTime;
            startTime = audioTranscription.Words[0].StartTime;
        }

        // Update the response
        response.RawRepresentation = audioTranscription;
        response.Contents = [new TextContent(audioTranscription.Text)];
        response.StartTime = startTime;
        response.EndTime = endTime;
        response.AdditionalProperties = new AdditionalPropertiesDictionary
        {
            [nameof(audioTranscription.Language)] = audioTranscription.Language,
            [nameof(audioTranscription.Duration)] = audioTranscription.Duration
        };
    }

    /// <summary>Converts an extensions options instance to an OpenAI options instance.</summary>
    private static AudioTranscriptionOptions ToOpenAITranscriptionOptions(SpeechToTextOptions? options)
    {
        AudioTranscriptionOptions result = new();

        if (options is not null)
        {
            if (options.SpeechLanguage is not null)
            {
                result.Language = options.SpeechLanguage;
            }

            if (options.AdditionalProperties is { Count: > 0 } additionalProperties)
            {
                if (additionalProperties.TryGetValue(nameof(result.Temperature), out float? temperature))
                {
                    result.Temperature = temperature;
                }

                if (additionalProperties.TryGetValue(nameof(result.TimestampGranularities), out object? timestampGranularities))
                {
                    result.TimestampGranularities = timestampGranularities is AudioTimestampGranularities granularities ? granularities : default;
                }

                if (additionalProperties.TryGetValue(nameof(result.ResponseFormat), out AudioTranscriptionFormat? responseFormat))
                {
                    result.ResponseFormat = responseFormat;
                }

                if (additionalProperties.TryGetValue(nameof(result.Prompt), out string? prompt))
                {
                    result.Prompt = prompt;
                }
            }
        }

        return result;
    }

    /// <summary>Updates a <see cref="SpeechToTextResponse"/> from an OpenAI <see cref="AudioTranslation"/>.</summary>
    /// <param name="response">The response to update.</param>
    /// <param name="audioTranslation">The OpenAI audio translation.</param>
    private static void UpdateResponseFromOpenAIAudioTranslation(SpeechToTextResponse response, AudioTranslation audioTranslation)
    {
        _ = Throw.IfNull(audioTranslation);

        var segmentCount = audioTranslation.Segments.Count;

        TimeSpan? endTime = null;
        TimeSpan? startTime = null;
        if (segmentCount > 0)
        {
            endTime = audioTranslation.Segments[segmentCount - 1].EndTime;
            startTime = audioTranslation.Segments[0].StartTime;
        }

        // Update the response
        response.RawRepresentation = audioTranslation;
        response.Contents = [new TextContent(audioTranslation.Text)];
        response.StartTime = startTime;
        response.EndTime = endTime;
        response.AdditionalProperties = new AdditionalPropertiesDictionary
        {
            [nameof(audioTranslation.Language)] = audioTranslation.Language,
            [nameof(audioTranslation.Duration)] = audioTranslation.Duration
        };
    }

    /// <summary>Converts an extensions options instance to an OpenAI options instance.</summary>
    private static AudioTranslationOptions ToOpenAITranslationOptions(SpeechToTextOptions? options)
    {
        AudioTranslationOptions result = new();

        if (options is not null)
        {
            if (options.AdditionalProperties is { Count: > 0 } additionalProperties)
            {
                if (additionalProperties.TryGetValue(nameof(result.Temperature), out float? temperature))
                {
                    result.Temperature = temperature;
                }

                if (additionalProperties.TryGetValue(nameof(result.ResponseFormat), out AudioTranslationFormat? responseFormat))
                {
                    result.ResponseFormat = responseFormat;
                }

                if (additionalProperties.TryGetValue(nameof(result.Prompt), out string? prompt))
                {
                    result.Prompt = prompt;
                }
            }
        }

        return result;
    }
}

internal static class Throw
{
    /// <summary>
    /// Throws an <see cref="System.ArgumentNullException"/> if the specified argument is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">Argument type to be checked for <see langword="null"/>.</typeparam>
    /// <param name="argument">Object to be checked for <see langword="null"/>.</param>
    /// <param name="paramName">The name of the parameter being checked.</param>
    /// <returns>The original value of <paramref name="argument"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T IfNull<T>(T argument, string paramName = "")
    {
        if (argument is null)
        {
            ArgumentNullException(paramName);
        }

        return argument;
    }

    /// <summary>
    /// Throws an <see cref="System.ArgumentOutOfRangeException"/>.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <param name="message">A message that describes the error.</param>
#if !NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.NoInlining)]
#endif
    public static void ArgumentOutOfRangeException(string paramName, string? message)
        => throw new ArgumentOutOfRangeException(paramName, message);

    /// <summary>
    /// Throws an <see cref="System.ArgumentNullException"/>.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
#if !NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.NoInlining)]
#endif
    public static void ArgumentNullException(string paramName)
        => throw new ArgumentNullException(paramName);
}
