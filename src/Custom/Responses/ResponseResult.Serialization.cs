using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using OpenAI;

namespace OpenAI.Responses
{
    public partial class ResponseResult : IJsonModel<ResponseResult>
    {
        internal static ResponseResult DeserializeResponseResult(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            IDictionary<string, string> metadata = default;
            float? temperature = default;
            int? topLogProbabilityCount = default;
            float? topP = default;
            string endUserId = default;
            string safetyIdentifier = default;
            ResponseServiceTier? serviceTier = default;
            string previousResponseId = default;
            string model = default;
            ResponseReasoningOptions reasoningOptions = default;
            bool? backgroundModeEnabled = default;
            int? maxOutputTokenCount = default;
            int? maxToolCallCount = default;
            ResponseTextOptions textOptions = default;
            IList<ResponseTool> tools = default;
            ResponseToolChoice toolChoice = default;
            ResponseTruncationMode? truncationMode = default;
            string id = default;
            string @object = default;
            ResponseStatus? status = default;
            DateTimeOffset createdAt = default;
            ResponseError error = default;
            ResponseIncompleteStatusDetails incompleteStatusDetails = default;
            IList<ResponseItem> outputItems = default;
            IList<ResponseItem> instructions = default;
            ResponseTokenUsage usage = default;
            bool parallelToolCallsEnabled = default;
            ResponseConversationOptions conversationOptions = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("metadata"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        metadata = new ChangeTrackingDictionary<string, string>();
                        continue;
                    }
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (var prop0 in prop.Value.EnumerateObject())
                    {
                        if (prop0.Value.ValueKind == JsonValueKind.Null)
                        {
                            dictionary.Add(prop0.Name, null);
                        }
                        else
                        {
                            dictionary.Add(prop0.Name, prop0.Value.GetString());
                        }
                    }
                    metadata = dictionary;
                    continue;
                }
                if (prop.NameEquals("temperature"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        temperature = null;
                        continue;
                    }
                    temperature = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("top_logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        topLogProbabilityCount = null;
                        continue;
                    }
                    topLogProbabilityCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("top_p"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        topP = null;
                        continue;
                    }
                    topP = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("user"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        endUserId = null;
                        continue;
                    }
                    endUserId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("safety_identifier"u8))
                {
                    safetyIdentifier = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("service_tier"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    serviceTier = new ResponseServiceTier(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("previous_response_id"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        previousResponseId = null;
                        continue;
                    }
                    previousResponseId = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("model"u8))
                {
                    model = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("reasoning"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        reasoningOptions = null;
                        continue;
                    }
                    reasoningOptions = ResponseReasoningOptions.DeserializeResponseReasoningOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("background"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        backgroundModeEnabled = null;
                        continue;
                    }
                    backgroundModeEnabled = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("max_output_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxOutputTokenCount = null;
                        continue;
                    }
                    maxOutputTokenCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("max_tool_calls"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxToolCallCount = null;
                        continue;
                    }
                    maxToolCallCount = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("text"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    textOptions = ResponseTextOptions.DeserializeResponseTextOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("tools"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ResponseTool> array = new List<ResponseTool>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ResponseTool.DeserializeResponseTool(item, item.GetUtf8Bytes(), options));
                    }
                    tools = array;
                    continue;
                }
                if (prop.NameEquals("tool_choice"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    toolChoice = ResponseToolChoice.DeserializeResponseToolChoice(prop.Value, options);
                    continue;
                }
                if (prop.NameEquals("truncation"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        truncationMode = null;
                        continue;
                    }
                    truncationMode = new ResponseTruncationMode(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("id"u8))
                {
                    id = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("object"u8))
                {
                    @object = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("status"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    status = prop.Value.GetString().ToResponseStatus();
                    continue;
                }
                if (prop.NameEquals("created_at"u8))
                {
                    createdAt = DateTimeOffset.FromUnixTimeSeconds(prop.Value.GetInt64());
                    continue;
                }
                if (prop.NameEquals("error"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        error = null;
                        continue;
                    }
                    error = ResponseError.DeserializeResponseError(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("incomplete_details"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        incompleteStatusDetails = null;
                        continue;
                    }
                    incompleteStatusDetails = ResponseIncompleteStatusDetails.DeserializeResponseIncompleteStatusDetails(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("output"u8))
                {
                    List<ResponseItem> array = new List<ResponseItem>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), options));
                    }
                    outputItems = array;
                    continue;
                }
                if (prop.NameEquals("instructions"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        instructions = new ChangeTrackingList<ResponseItem>();
                        continue;
                    }

                    List<ResponseItem> array = new List<ResponseItem>();
                    if (prop.Value.ValueKind == JsonValueKind.String)
                    {
                        array.Add(ResponseItem.CreateDeveloperMessageItem(prop.Value.GetString()));
                    }
                    else if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in prop.Value.EnumerateArray())
                        {
                            array.Add(ResponseItem.DeserializeResponseItem(item, item.GetUtf8Bytes(), options));
                        }
                    }
                    instructions = array;
                    continue;
                }
                if (prop.NameEquals("usage"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    usage = ResponseTokenUsage.DeserializeResponseTokenUsage(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("parallel_tool_calls"u8))
                {
                    parallelToolCallsEnabled = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("conversation"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        conversationOptions = null;
                        continue;
                    }
                    conversationOptions = ResponseConversationOptions.DeserializeResponseConversationOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new ResponseResult(
                metadata,
                temperature,
                topLogProbabilityCount,
                topP,
                endUserId,
                safetyIdentifier,
                serviceTier,
                previousResponseId,
                model,
                reasoningOptions,
                backgroundModeEnabled,
                maxOutputTokenCount,
                maxToolCallCount,
                textOptions,
                tools ?? new ChangeTrackingList<ResponseTool>(),
                toolChoice,
                truncationMode,
                id,
                @object,
                status,
                createdAt,
                error,
                incompleteStatusDetails,
                outputItems,
                instructions,
                usage,
                parallelToolCallsEnabled,
                conversationOptions,
                patch);
        }
    }
}
