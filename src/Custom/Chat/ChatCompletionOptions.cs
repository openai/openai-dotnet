using System.Collections.Generic;

namespace OpenAI.Chat;

/// <summary>
/// Request-level options for chat completion.
/// </summary>
[CodeGenModel("CreateChatCompletionRequest")]
[CodeGenSuppress("ChatCompletionOptions", typeof(IEnumerable<ChatMessage>), typeof(InternalCreateChatCompletionRequestModel))]
[CodeGenSerialization(nameof(StopSequences), SerializationValueHook = nameof(SerializeStopSequencesValue), DeserializationValueHook = nameof(DeserializeStopSequencesValue))]
[CodeGenSerialization(nameof(LogitBiases), SerializationValueHook = nameof(SerializeLogitBiasesValue), DeserializationValueHook = nameof(DeserializeLogitBiasesValue))]
public partial class ChatCompletionOptions
{
    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// A list of messages comprising the conversation so far. [Example Python code](https://cookbook.openai.com/examples/how_to_format_inputs_to_chatgpt_models).
    /// Please note <see cref="ChatMessage"/> is the base class. According to the scenario, a derived class of the base class might need to be assigned here, or this property needs to be casted to one of the possible derived classes.
    /// The available derived classes include <see cref="AssistantChatMessage"/>, <see cref="FunctionChatMessage"/>, <see cref="SystemChatMessage"/>, <see cref="ToolChatMessage"/> and <see cref="UserChatMessage"/>.
    /// </summary>
    [CodeGenMember("Messages")]
    internal IList<ChatMessage> Messages { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// ID of the model to use. See the <see href="https://platform.openai.com/docs/models/model-endpoint-compatibility">model endpoint compatibility</see> table for details on which models work with the Chat API.
    /// </summary>
    [CodeGenMember("Model")]
    internal InternalCreateChatCompletionRequestModel Model { get; set; }

    // CUSTOM: Made internal. We only ever request a single choice.
    /// <summary> How many chat completion choices to generate for each input message. Note that you will be charged based on the number of generated tokens across all of the choices. Keep `n` as `1` to minimize costs. </summary>
    [CodeGenMember("N")]
    internal int? N { get; set; }

    // CUSTOM: Made internal. We set this manually based on the client method that is called.
    /// <summary> If set, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only [server-sent events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#Event_stream_format) as they become available, with the stream terminated by a `data: [DONE]` message. [Example Python code](https://cookbook.openai.com/examples/how_to_stream_completions). </summary>
    [CodeGenMember("Stream")]
    internal bool? Stream { get; set; }

    /// <summary> Gets or sets the stream options. </summary>
    [CodeGenMember("StreamOptions")]
    internal InternalChatCompletionStreamOptions StreamOptions { get; set; }
        = new() { IncludeUsage = true };

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ChatCompletionOptions"/> for deserialization. </summary>
    public ChatCompletionOptions()
    {
        LogitBiases = new ChangeTrackingDictionary<int, int>();
        StopSequences = new ChangeTrackingList<string>();
        Tools = new ChangeTrackingList<ChatTool>();
        Functions = new ChangeTrackingList<ChatFunction>();
    }

    // CUSTOM: Renamed.
    /// <summary> Whether to return log probabilities of the output tokens or not. If true, returns the log probabilities of each output token returned in the message content. </summary>
    [CodeGenMember("Logprobs")]
    public bool? IncludeLogProbabilities { get; set; }

    // CUSTOM: Renamed.
    /// <summary> An integer between 0 and 20 specifying the number of most likely tokens to return at each token position, each with an associated log probability. <see cref="IncludeLogProbabilities"/> must be set to <see langword="true"/> if this property is used. </summary>
    [CodeGenMember("TopLogprobs")]
    public int? TopLogProbabilityCount { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type to treat a single string as a collection of strings with one item.
    /// <summary> Up to 4 sequences where the API will stop generating further tokens. </summary>
    [CodeGenMember("Stop")]
    public IList<string> StopSequences { get; }

    // CUSTOM:
    // - Renamed.
    // - Changed type to treat tokens as integers instead of strings.
    /// <summary>
    /// Modifies the likelihood of specified tokens appearing in the completion. It maps tokens (specified by their token ID in the tokenizer) to an associated bias value from -100 to 100. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values like -100 or 100 should result in a ban or exclusive selection of the relevant token.
    /// </summary>
    [CodeGenMember("LogitBias")]
    public IDictionary<int, int> LogitBiases { get; }

    // CUSTOM: Changed type to avoid BinaryData.
    /// <summary>
    /// Specifies which tool is called by the model, if any. <see cref="ChatToolChoice.None"/> means the model will not call any tool and instead generates a message. <see cref="ChatToolChoice.Auto"/> means the model can pick between generating a message or calling one or more tools.
    /// <see cref="ChatToolChoice.Required"/> means the model must call one or more tools. The model can also be forced to call a specific tool by constructing a new instance of <see cref="ChatToolChoice"/> while passing the desired <see cref="ChatTool"/> as a constructor parameter.
    /// <remarks>
    /// <see cref="ChatToolChoice.None"/> is the default behavior when no tools are present, while <see cref="ChatToolChoice.Auto"/> is the default if tools are present.
    /// </remarks>
    /// </summary>
    [CodeGenMember("ToolChoice")]
    public ChatToolChoice ToolChoice { get; set; }

    // CUSTOM:
    // - Renamed.
    // - Changed type to avoid BinaryData.
    /// <summary>
    /// Deprecated in favor of <see cref="ToolChoice"/>.
    /// </summary>
    [CodeGenMember("FunctionCall")]
    public ChatFunctionChoice FunctionChoice { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    /// Whether to enable parallel function calling during tool use. 
    /// </summary>
    /// <remarks>
    /// Assumed <c>true</c> if not otherwise specified.
    /// </remarks>
    [CodeGenMember("ParallelToolCalls")]
    public bool? ParallelToolCallsEnabled { get; set; }

    // CUSTOM: Renamed.
    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse.
    ///     <see href="https://platform.openai.com/docs/guides/safety-best-practices/end-user-ids">Learn more</see>.
    /// </summary>
    [CodeGenMember("User")]
    public string EndUserId { get; set; }
}