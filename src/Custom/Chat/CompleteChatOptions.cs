using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat
{
    public partial class CompleteChatOptions : JsonModel<CompleteChatOptions>
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        public CompleteChatOptions(IEnumerable<ChatMessage> messages, string model) :
            this(default, default, default, default, default, messages?.ToList(), model, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default, default)
        {
            Argument.AssertNotNull(messages, nameof(messages));
        }

#pragma warning disable  SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal CompleteChatOptions(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ChatServiceTier? serviceTier, IList<ChatMessage> messages, string model, IList<ChatRequestModality> modalities, ChatReasoningEffortLevel? reasoningEffort, int? maxCompletionTokens, float? frequencyPenalty, float? presencePenalty, ChatWebSearchOptions webSearchOptions, int? topLogprobs, ResponseFormat responseFormat, ChatAudioOptions audio, bool? store, bool? stream, IList<string> stop, IDictionary<int, int> logitBias, bool? logprobs, int? maxTokens, int? n, ChatOutputPrediction prediction, long? seed, ChatCompletionStreamOptions streamOptions, IList<ChatTool> tools, BinaryData toolChoice, bool? parallelToolCalls, BinaryData functionCall, IList<ChatFunction> functions, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            User = user;
            ServiceTier = serviceTier;
            Messages = messages ?? new ChangeTrackingList<ChatMessage>();
            Model = model;
            Modalities = modalities ?? new ChangeTrackingList<ChatRequestModality>();
            ReasoningEffort = reasoningEffort;
            MaxCompletionTokens = maxCompletionTokens;
            FrequencyPenalty = frequencyPenalty;
            PresencePenalty = presencePenalty;
            WebSearchOptions = webSearchOptions;
            TopLogprobs = topLogprobs;
            ResponseFormat = responseFormat;
            Audio = audio;
            Store = store;
            Stream = stream;
            Stop = stop ?? new ChangeTrackingList<string>();
            LogitBias = logitBias ?? new ChangeTrackingDictionary<int, int>();
            Logprobs = logprobs;
            MaxTokens = maxTokens;
            N = n;
            Prediction = prediction;
            Seed = seed;
            StreamOptions = streamOptions;
            Tools = tools ?? new ChangeTrackingList<ChatTool>();
            ToolChoice = toolChoice;
            ParallelToolCalls = parallelToolCalls;
            FunctionCall = functionCall;
            Functions = functions ?? new ChangeTrackingList<ChatFunction>();
            _patch = patch;
        }

#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        public IDictionary<string, string> Metadata { get; }

        public float? Temperature { get; set; }

        public float? TopP { get; set; }

        public string User { get; set; }

        public ChatServiceTier? ServiceTier { get; set; }

        public IList<ChatMessage> Messages { get; }

        public string Model { get; set; }

        public IList<ChatRequestModality> Modalities { get; set; }

        public ChatReasoningEffortLevel? ReasoningEffort { get; set; }

        public int? MaxCompletionTokens { get; set; }

        public float? FrequencyPenalty { get; set; }

        public float? PresencePenalty { get; set; }

        public ChatWebSearchOptions WebSearchOptions { get; set; }

        public int? TopLogprobs { get; set; }

        public ResponseFormat ResponseFormat { get; set; }

        public ChatAudioOptions Audio { get; set; }

        public bool? Store { get; set; }

        public bool? Stream { get; set; }

        public IList<string> Stop { get; set; }

        public IDictionary<int, int> LogitBias { get; set; }

        public bool? Logprobs { get; set; }

        public int? MaxTokens { get; set; }

        public int? N { get; set; }

        public ChatOutputPrediction Prediction { get; set; }

        public long? Seed { get; set; }

        public ChatCompletionStreamOptions StreamOptions { get; set; }

        public IList<ChatTool> Tools { get; }

        public BinaryData ToolChoice { get; set; }

        public bool? ParallelToolCalls { get; set; }

        public BinaryData FunctionCall { get; set; }

        public IList<ChatFunction> Functions { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        public static CompleteChatOptions Create(IEnumerable<ChatMessage> messages, ChatClient client, ChatCompletionOptions options = null, bool isStreaming = false)
        {
            Argument.AssertNotNull(messages, nameof(messages));
            options ??= new();
            client.CreateChatCompletionOptions(messages, ref options, isStreaming);

            var request = new CompleteChatOptions(messages, options.Model);

            // Populate request properties from options
            request.Audio = options.AudioOptions;
            request.Temperature = options.Temperature;
            request.TopP = options.TopP;
            request.User = options.EndUserId;
            request.ServiceTier = options.ServiceTier;
            request.ReasoningEffort = options.ReasoningEffortLevel;
            request.MaxCompletionTokens = options.MaxOutputTokenCount;
            request.FrequencyPenalty = options.FrequencyPenalty;
            request.PresencePenalty = options.PresencePenalty;
            request.WebSearchOptions = options.WebSearchOptions;
            request.TopLogprobs = options.TopLogProbabilityCount;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            request.ResponseFormat = options.ResponseFormat switch
            {
                InternalDotNetChatResponseFormatText => new ResponseFormatText(),
                InternalDotNetChatResponseFormatJsonObject => new ResponseFormatJsonObject(),
                InternalDotNetChatResponseFormatJsonSchema js => new ResponseFormatJsonSchema(new ResponseFormatJsonSchemaJsonSchema(js.JsonSchema.Name)),
                _ => null,
            };
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            request.Store = options.StoredOutputEnabled;
            request.Logprobs = options.IncludeLogProbabilities;
            request.Prediction = options.OutputPrediction;
            request.Seed = options.Seed;
            request.Stream = options.Stream;
            request.ParallelToolCalls = options.AllowParallelToolCalls;

            // Handle collections and complex types
            if (options.StopSequences != null && options.StopSequences.Count > 0)
            {
                request.Stop = options.StopSequences.ToList();
            }

            if (options.LogitBiases != null && options.LogitBiases.Count > 0)
            {
                foreach (var kvp in options.LogitBiases)
                {
                    request.LogitBias[kvp.Key] = kvp.Value;
                }
            }

            if (options.Tools != null && options.Tools.Count > 0)
            {
                foreach (var tool in options.Tools)
                {
                    request.Tools.Add(tool);
                }
            }

            if (options.ToolChoice != null)
            {
                request.ToolChoice = ModelReaderWriter.Write(options.ToolChoice, ModelSerializationExtensions.WireOptions);
            }

            if (options.Functions != null && options.Functions.Count > 0)
            {
                foreach (var function in options.Functions)
                {
                    request.Functions.Add(function);
                }
            }

            if (options.FunctionChoice != null)
            {
                request.FunctionCall = ModelReaderWriter.Write(options.FunctionChoice, ModelSerializationExtensions.WireOptions);
            }

            if (options.Metadata != null && options.Metadata.Count > 0)
            {
                foreach (var kvp in options.Metadata)
                {
                    request.Metadata[kvp.Key] = kvp.Value;
                }
            }

            if (options.ResponseModalities.HasFlag(ChatResponseModalities.Audio))
            {
                request.Modalities.Add(ChatRequestModality.Audio);
            }
            if (options.ResponseModalities.HasFlag(ChatResponseModalities.Text))
            {
                request.Modalities.Add(ChatRequestModality.Text);
            }
            if (options.StreamOptions != null)
            {
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
                request.StreamOptions = new(options.StreamOptions.IncludeUsage, options.StreamOptions.Patch);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            }

            return request;
        }

        private static BinaryData SerializeStopSequences(IList<string> stopSequences)
        {
            using var stream = new System.IO.MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartArray();
            foreach (var item in stopSequences)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
            writer.Flush();
            return new BinaryData(stream.ToArray());
        }

        public BinaryContent Body { get; set; }

        protected override CompleteChatOptions CreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options) => JsonModelCreateCore(ref reader, options);

        [Experimental("OPENAI001")]
        protected virtual CompleteChatOptions JsonModelCreateCore(ref Utf8JsonReader reader, ModelReaderWriterOptions options)
        {
            string format = options.Format == "W" ? ((IPersistableModel<CompleteChatOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(CompleteChatOptions)} does not support reading '{format}' format.");
            }
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return DeserializeCreateChatCompletionRequest(document.RootElement, null, options);
        }

        internal static CompleteChatOptions DeserializeCreateChatCompletionRequest(JsonElement element, BinaryData data, ModelReaderWriterOptions options)
        {
            if (element.ValueKind == JsonValueKind.Null)
            {
                return null;
            }
            IDictionary<string, string> metadata = default;
            float? temperature = default;
            float? topP = default;
            string user = default;
            ChatServiceTier? serviceTier = default;
            IList<ChatMessage> messages = default;
            string model = default;
            IList<ChatRequestModality> modalities = default;
            ChatReasoningEffortLevel? reasoningEffort = default;
            int? maxCompletionTokens = default;
            float? frequencyPenalty = default;
            float? presencePenalty = default;
            ChatWebSearchOptions webSearchOptions = default;
            int? topLogprobs = default;
            ResponseFormat responseFormat = default;
            ChatAudioOptions audio = default;
            bool? store = default;
            bool? stream = default;
            IList<string> stop = default;
            IDictionary<int, int> logitBias = default;
            bool? logprobs = default;
            int? maxTokens = default;
            int? n = default;
            ChatOutputPrediction prediction = default;
            long? seed = default;
            ChatCompletionStreamOptions streamOptions = default;
            IList<ChatTool> tools = default;
            BinaryData toolChoice = default;
            bool? parallelToolCalls = default;
            BinaryData functionCall = default;
            IList<ChatFunction> functions = default;
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            JsonPatch patch = new JsonPatch(data is null ? ReadOnlyMemory<byte>.Empty : data.ToMemory());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            foreach (var prop in element.EnumerateObject())
            {
                if (prop.NameEquals("metadata"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
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
                    user = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("service_tier"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    serviceTier = new ChatServiceTier(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("messages"u8))
                {
                    List<ChatMessage> array = new List<ChatMessage>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatMessage.DeserializeChatMessage(item, item.GetUtf8Bytes(), options));
                    }
                    messages = array;
                    continue;
                }
                if (prop.NameEquals("model"u8))
                {
                    model = prop.Value.GetString();
                    continue;
                }
                if (prop.NameEquals("modalities"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatRequestModality> array = new List<ChatRequestModality>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(item.GetString().ToCreateChatCompletionRequestModality());
                    }
                    modalities = array;
                    continue;
                }
                if (prop.NameEquals("reasoning_effort"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        reasoningEffort = null;
                        continue;
                    }
                    reasoningEffort = new ChatReasoningEffortLevel(prop.Value.GetString());
                    continue;
                }
                if (prop.NameEquals("max_completion_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxCompletionTokens = null;
                        continue;
                    }
                    maxCompletionTokens = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("frequency_penalty"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        frequencyPenalty = null;
                        continue;
                    }
                    frequencyPenalty = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("presence_penalty"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        presencePenalty = null;
                        continue;
                    }
                    presencePenalty = prop.Value.GetSingle();
                    continue;
                }
                if (prop.NameEquals("web_search_options"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    webSearchOptions = ChatWebSearchOptions.DeserializeChatWebSearchOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("top_logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        topLogprobs = null;
                        continue;
                    }
                    topLogprobs = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("response_format"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    responseFormat = ResponseFormat.DeserializeResponseFormat(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("audio"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        audio = null;
                        continue;
                    }
                    audio = ChatAudioOptions.DeserializeChatAudioOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("store"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        store = null;
                        continue;
                    }
                    store = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("stream"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        stream = null;
                        continue;
                    }
                    stream = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("stop"u8))
                {
                    DeserializeStopSequencesValue(prop, ref stop);
                    continue;
                }
                if (prop.NameEquals("logit_bias"u8))
                {
                    DeserializeLogitBiasesValue(prop, ref logitBias);
                    continue;
                }
                if (prop.NameEquals("logprobs"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        logprobs = null;
                        continue;
                    }
                    logprobs = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("max_tokens"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        maxTokens = null;
                        continue;
                    }
                    maxTokens = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("n"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        n = null;
                        continue;
                    }
                    n = prop.Value.GetInt32();
                    continue;
                }
                if (prop.NameEquals("prediction"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        prediction = null;
                        continue;
                    }
                    prediction = ChatOutputPrediction.DeserializeChatOutputPrediction(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("seed"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        seed = null;
                        continue;
                    }
                    seed = prop.Value.GetInt64();
                    continue;
                }
                if (prop.NameEquals("stream_options"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        streamOptions = null;
                        continue;
                    }
                    streamOptions = ChatCompletionStreamOptions.DeserializeChatCompletionStreamOptions(prop.Value, prop.Value.GetUtf8Bytes(), options);
                    continue;
                }
                if (prop.NameEquals("tools"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatTool> array = new List<ChatTool>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatTool.DeserializeChatTool(item, item.GetUtf8Bytes(), options));
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
                    toolChoice = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("parallel_tool_calls"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    parallelToolCalls = prop.Value.GetBoolean();
                    continue;
                }
                if (prop.NameEquals("function_call"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    functionCall = BinaryData.FromString(prop.Value.GetRawText());
                    continue;
                }
                if (prop.NameEquals("functions"u8))
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null)
                    {
                        continue;
                    }
                    List<ChatFunction> array = new List<ChatFunction>();
                    foreach (var item in prop.Value.EnumerateArray())
                    {
                        array.Add(ChatFunction.DeserializeChatFunction(item, item.GetUtf8Bytes(), options));
                    }
                    functions = array;
                    continue;
                }
                patch.Set([.. "$."u8, .. Encoding.UTF8.GetBytes(prop.Name)], prop.Value.GetUtf8Bytes());
            }
            return new CompleteChatOptions(
                metadata ?? new ChangeTrackingDictionary<string, string>(),
                temperature,
                topP,
                user,
                serviceTier,
                messages,
                model,
                modalities ?? new ChangeTrackingList<ChatRequestModality>(),
                reasoningEffort,
                maxCompletionTokens,
                frequencyPenalty,
                presencePenalty,
                webSearchOptions,
                topLogprobs,
                responseFormat,
                audio,
                store,
                stream,
                stop,
                logitBias ?? new ChangeTrackingDictionary<int, int>(),
                logprobs,
                maxTokens,
                n,
                prediction,
                seed,
                streamOptions,
                tools ?? new ChangeTrackingList<ChatTool>(),
                toolChoice,
                parallelToolCalls,
                functionCall,
                functions ?? new ChangeTrackingList<ChatFunction>(),
                patch);
        }

        protected override void WriteCore(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            string format = options.Format == "W" ? ((IPersistableModel<CompleteChatOptions>)this).GetFormatFromOptions(options) : options.Format;
            if (format != "J")
            {
                throw new FormatException($"The model {nameof(ChatCompletionOptions)} does not support writing '{format}' format.");
            }
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            if (Optional.IsCollectionDefined(Metadata) && !Patch.Contains("$.metadata"u8))
            {
                writer.WritePropertyName("metadata"u8);
                writer.WriteStartObject();
#if NET8_0_OR_GREATER
                global::System.Span<byte> buffer = stackalloc byte[256];
#endif
                foreach (var item in Metadata)
                {
#if NET8_0_OR_GREATER
                    int bytesWritten = global::System.Text.Encoding.UTF8.GetBytes(item.Key.AsSpan(), buffer);
                    bool patchContains = (bytesWritten == 256) ? Patch.Contains("$.metadata"u8, global::System.Text.Encoding.UTF8.GetBytes(item.Key)) : Patch.Contains("$.metadata"u8, buffer.Slice(0, bytesWritten));
#else
                    bool patchContains = Patch.Contains("$.metadata"u8, Encoding.UTF8.GetBytes(item.Key));
#endif
                    if (!patchContains)
                    {
                        writer.WritePropertyName(item.Key);
                        if (item.Value == null)
                        {
                            writer.WriteNullValue();
                            continue;
                        }
                        writer.WriteStringValue(item.Value);
                    }
                }

                Patch.WriteTo(writer, "$.metadata"u8);
                writer.WriteEndObject();
            }
            if (Optional.IsDefined(Temperature) && !Patch.Contains("$.temperature"u8))
            {
                writer.WritePropertyName("temperature"u8);
                writer.WriteNumberValue(Temperature.Value);
            }
            if (Optional.IsDefined(TopP) && !Patch.Contains("$.top_p"u8))
            {
                writer.WritePropertyName("top_p"u8);
                writer.WriteNumberValue(TopP.Value);
            }
            if (Optional.IsDefined(User) && !Patch.Contains("$.user"u8))
            {
                writer.WritePropertyName("user"u8);
                writer.WriteStringValue(User);
            }
            if (Optional.IsDefined(ServiceTier) && !Patch.Contains("$.service_tier"u8))
            {
                writer.WritePropertyName("service_tier"u8);
                writer.WriteStringValue(ServiceTier.Value.ToString());
            }
            if (Patch.Contains("$.messages"u8))
            {
                if (!Patch.IsRemoved("$.messages"u8))
                {
                    writer.WritePropertyName("messages"u8);
                    writer.WriteRawValue(Patch.GetJson("$.messages"u8));
                }
            }
            else
            {
                // Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup
                if (Optional.IsCollectionDefined(Messages))
                {
                    writer.WritePropertyName("messages"u8);
                    SerializeMessagesValue(writer, options);
                }
            }
            // Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup
            if (Optional.IsDefined(Model) && !Patch.Contains("$.model"u8))
            {
                writer.WritePropertyName("model"u8);
                writer.WriteStringValue(Model);
            }
            if (Patch.Contains("$.modalities"u8))
            {
                if (!Patch.IsRemoved("$.modalities"u8))
                {
                    writer.WritePropertyName("modalities"u8);
                    writer.WriteRawValue(Patch.GetJson("$.modalities"u8));
                }
            }
            else if (Optional.IsCollectionDefined(Modalities))
            {
                writer.WritePropertyName("modalities"u8);
                writer.WriteStartArray();
                for (int i = 0; i < Modalities.Count; i++)
                {
                    if (Patch.IsRemoved(Encoding.UTF8.GetBytes($"$.modalities[{i}]")))
                    {
                        continue;
                    }
                    writer.WriteStringValue(Modalities[i].ToSerialString());
                }
                Patch.WriteTo(writer, "$.modalities"u8);
                writer.WriteEndArray();
            }
            if (Optional.IsDefined(ReasoningEffort) && !Patch.Contains("$.reasoning_effort"u8))
            {
                writer.WritePropertyName("reasoning_effort"u8);
                writer.WriteStringValue(ReasoningEffort.Value.ToString());
            }
            if (Optional.IsDefined(MaxCompletionTokens) && !Patch.Contains("$.max_completion_tokens"u8))
            {
                writer.WritePropertyName("max_completion_tokens"u8);
                writer.WriteNumberValue(MaxCompletionTokens.Value);
            }
            if (Optional.IsDefined(FrequencyPenalty) && !Patch.Contains("$.frequency_penalty"u8))
            {
                writer.WritePropertyName("frequency_penalty"u8);
                writer.WriteNumberValue(FrequencyPenalty.Value);
            }
            if (Optional.IsDefined(PresencePenalty) && !Patch.Contains("$.presence_penalty"u8))
            {
                writer.WritePropertyName("presence_penalty"u8);
                writer.WriteNumberValue(PresencePenalty.Value);
            }
            if (Optional.IsDefined(WebSearchOptions) && !Patch.Contains("$.web_search_options"u8))
            {
                writer.WritePropertyName("web_search_options"u8);
                writer.WriteObjectValue(WebSearchOptions, options);
            }
            if (Optional.IsDefined(TopLogprobs) && !Patch.Contains("$.top_logprobs"u8))
            {
                writer.WritePropertyName("top_logprobs"u8);
                writer.WriteNumberValue(TopLogprobs.Value);
            }
            if (Optional.IsDefined(ResponseFormat) && !Patch.Contains("$.response_format"u8))
            {
                writer.WritePropertyName("response_format"u8);
                writer.WriteObjectValue(ResponseFormat, options);
            }
            if (Optional.IsDefined(Audio) && !Patch.Contains("$.audio"u8))
            {
                writer.WritePropertyName("audio"u8);
                writer.WriteObjectValue(Audio, options);
            }
            if (Optional.IsDefined(Store) && !Patch.Contains("$.store"u8))
            {
                writer.WritePropertyName("store"u8);
                writer.WriteBooleanValue(Store.Value);
            }
            if (Optional.IsDefined(Stream) && !Patch.Contains("$.stream"u8))
            {
                writer.WritePropertyName("stream"u8);
                writer.WriteBooleanValue(Stream.Value);
            }
            if (Patch.Contains("$.stop"u8))
            {
                if (!Patch.IsRemoved("$.stop"u8))
                {
                    writer.WritePropertyName("stop"u8);
                    writer.WriteRawValue(Patch.GetJson("$.stop"u8));
                }
            }
            else if (Optional.IsCollectionDefined(Stop))
            {
                writer.WritePropertyName("stop"u8);
                SerializeStopSequencesValue(writer, options);
            }
            if (Optional.IsCollectionDefined(LogitBias) && !Patch.Contains("$.logit_bias"u8))
            {
                writer.WritePropertyName("logit_bias"u8);
                SerializeLogitBiasesValue(writer, options);
            }
            if (Optional.IsDefined(Logprobs) && !Patch.Contains("$.logprobs"u8))
            {
                writer.WritePropertyName("logprobs"u8);
                writer.WriteBooleanValue(Logprobs.Value);
            }
            if (Optional.IsDefined(MaxCompletionTokens) && !Patch.Contains("$.max_tokens"u8))
            {
                writer.WritePropertyName("max_tokens"u8);
                writer.WriteNumberValue(MaxCompletionTokens.Value);
            }
            if (Optional.IsDefined(N) && !Patch.Contains("$.n"u8))
            {
                writer.WritePropertyName("n"u8);
                writer.WriteNumberValue(N.Value);
            }
            if (Optional.IsDefined(Prediction) && !Patch.Contains("$.prediction"u8))
            {
                writer.WritePropertyName("prediction"u8);
                writer.WriteObjectValue(Prediction, options);
            }
            if (Optional.IsDefined(Seed) && !Patch.Contains("$.seed"u8))
            {
                writer.WritePropertyName("seed"u8);
                writer.WriteNumberValue(Seed.Value);
            }
            if (Optional.IsDefined(StreamOptions) && !Patch.Contains("$.stream_options"u8))
            {
                writer.WritePropertyName("stream_options"u8);
                writer.WriteObjectValue(StreamOptions, options);
            }
            if (Patch.Contains("$.tools"u8))
            {
                if (!Patch.IsRemoved("$.tools"u8))
                {
                    writer.WritePropertyName("tools"u8);
                    writer.WriteRawValue(Patch.GetJson("$.tools"u8));
                }
            }
            else if (Optional.IsCollectionDefined(Tools))
            {
                writer.WritePropertyName("tools"u8);
                writer.WriteStartArray();
                for (int i = 0; i < Tools.Count; i++)
                {
                    if (Tools[i].Patch.IsRemoved("$"u8))
                    {
                        continue;
                    }
                    writer.WriteObjectValue(Tools[i], options);
                }
                Patch.WriteTo(writer, "$.tools"u8);
                writer.WriteEndArray();
            }
            if (Optional.IsDefined(ToolChoice) && !Patch.Contains("$.tool_choice"u8))
            {
                writer.WritePropertyName("tool_choice"u8);
                writer.WriteObjectValue(ToolChoice, options);
            }
            if (Optional.IsDefined(ParallelToolCalls) && !Patch.Contains("$.parallel_tool_calls"u8))
            {
                writer.WritePropertyName("parallel_tool_calls"u8);
                writer.WriteBooleanValue(ParallelToolCalls.Value);
            }
            if (Optional.IsDefined(FunctionCall) && !Patch.Contains("$.function_call"u8))
            {
                writer.WritePropertyName("function_call"u8);
                writer.WriteObjectValue(FunctionCall, options);
            }
            if (Patch.Contains("$.functions"u8))
            {
                if (!Patch.IsRemoved("$.functions"u8))
                {
                    writer.WritePropertyName("functions"u8);
                    writer.WriteRawValue(Patch.GetJson("$.functions"u8));
                }
            }
            else if (Optional.IsCollectionDefined(Functions))
            {
                writer.WritePropertyName("functions"u8);
                writer.WriteStartArray();
                for (int i = 0; i < Functions.Count; i++)
                {
                    if (Functions[i].Patch.IsRemoved("$"u8))
                    {
                        continue;
                    }
                    writer.WriteObjectValue(Functions[i], options);
                }
                Patch.WriteTo(writer, "$.functions"u8);
                writer.WriteEndArray();
            }

            Patch.WriteTo(writer);
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
            writer.WriteEndObject();
        }

        // CUSTOM: Added custom serialization to treat a single string as a collection of strings with one item.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeserializeStopSequencesValue(JsonProperty property, ref IList<string> stop)
        {
            if (property.Value.ValueKind == JsonValueKind.Null)
            {
                stop = null;
            }
            else if (property.Value.ValueKind == JsonValueKind.String)
            {
                List<string> array = [property.Value.GetString()];
                stop = array;
            }
            else
            {
                List<string> array = [];
                foreach (var item in property.Value.EnumerateArray())
                {
                    array.Add(item.GetString());
                }
                stop = array;
            }
        }

        // CUSTOM: Added custom serialization to treat a single string as a collection of strings with one item.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SerializeStopSequencesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in Stop)
            {
                writer.WriteStringValue(item);
            }
            writer.WriteEndArray();
        }

        // CUSTOM: Added custom serialization to circumvent serialization failure of required 'messages', which is moved
        //         to a method parameter and should not block object serialization validity.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SerializeMessagesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in Messages)
            {
                writer.WriteObjectValue<ChatMessage>(item, options);
            }
            writer.WriteEndArray();
        }

        // CUSTOM: Added custom serialization to represent tokens as integers instead of strings.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SerializeLogitBiasesValue(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        {
            writer.WriteStartObject();
            foreach (var item in LogitBias)
            {
                writer.WritePropertyName(item.Key.ToString(CultureInfo.InvariantCulture));
                writer.WriteNumberValue(item.Value);
            }
            writer.WriteEndObject();
        }

        // CUSTOM: Added custom serialization to represent tokens as integers instead of strings.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DeserializeLogitBiasesValue(JsonProperty property, ref IDictionary<int, int> logitBias)
        {
            if (property.Value.ValueKind == JsonValueKind.Null)
            {
                logitBias = null;
            }
            else
            {
                Dictionary<int, int> dictionary = new Dictionary<int, int>();
                foreach (var property0 in property.Value.EnumerateObject())
                {
                    dictionary.Add(int.Parse(property0.Name, CultureInfo.InvariantCulture), property0.Value.GetInt32());
                }
                logitBias = dictionary;
            }
        }

        public static implicit operator BinaryContent(CompleteChatOptions createCompletionRequest)
        {
            if (createCompletionRequest == null)
            {
                return null;
            }
            return BinaryContent.Create(createCompletionRequest, ModelSerializationExtensions.WireOptions);
        }
    }
}
