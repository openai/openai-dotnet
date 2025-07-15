using OpenAI.Chat;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when creating a new <see cref="ThreadRun"/>.
/// </summary>
[CodeGenType("CreateRunRequest")]
[CodeGenVisibility(nameof(RunCreationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(RunCreationOptions), typeof(string))]
[CodeGenSerialization(nameof(ToolConstraint), "tool_choice", SerializationValueHook = nameof(SerializeToolConstraint))]
public partial class RunCreationOptions
{
    // CUSTOM: assistant_id/stream visibility hidden so that they can be promoted to required method parameters
    [CodeGenMember("AssistantId")]
    internal string AssistantId { get; set; }

    [CodeGenMember("Stream")]
    internal bool? Stream { get; set; }

    /// <inheritdoc cref="AssistantResponseFormat"/>
    [CodeGenMember("ResponseFormat")]
    public AssistantResponseFormat ResponseFormat { get; set; }

    /// <summary>
    /// A run-specific model name that will override the assistant's defined model. If not provided, the assistant's
    /// selection will be used.
    /// </summary>
    [CodeGenMember("Model")]
    public string ModelOverride { get; set; }

    /// <summary>
    /// A run specific replacement for the assistant's default instructions that will override the assistant-level
    /// instructions. If not specified, the assistant's instructions will be used.
    /// </summary>
    [CodeGenMember("Instructions")]
    public string InstructionsOverride { get; set; }

    /// <summary>
    /// Run-specific additional instructions that will be appended to the assistant-level instructions solely for this
    /// run. Unlike <see cref="InstructionsOverride"/>, the assistant's instructions are preserved and these additional
    /// instructions are concatenated.
    /// </summary>
    [CodeGenMember("AdditionalInstructions")]
    public string AdditionalInstructions { get; set; }

    /// <summary> Adds additional messages to the thread before creating the run. </summary>
    public IList<ThreadInitializationMessage> AdditionalMessages { get; private set; }

    [CodeGenMember("AdditionalMessages")]
    internal IList<MessageCreationOptions> InternalMessages
    {
        get => AdditionalMessages?.Select(initializationMessage => initializationMessage as MessageCreationOptions).ToList();
        private set
        {
            // Note: this path is exclusively used in a test or deserialization case; here, we'll convert the
            //          underlying wire-friendly representation into the initialization message abstraction.

            AdditionalMessages ??= new ChangeTrackingList<ThreadInitializationMessage>();
            AdditionalMessages.Clear();
            foreach (MessageCreationOptions baseMessageOptions in value)
            {
                AdditionalMessages.Add(new ThreadInitializationMessage(baseMessageOptions));
            }
        }
    }

    /// <summary>
    /// Whether to enable parallel function calling during tool use. 
    /// </summary>
    /// <remarks>
    /// Assumed <c>true</c> if not otherwise specified.
    /// </remarks>
    [CodeGenMember("ParallelToolCalls")]
    public bool? AllowParallelToolCalls { get; set; }

    /// <summary>
    /// A run-specific collection of tool definitions that will override the assistant-level defaults. If not provided,
    /// the assistant's defined tools will be used. Available tools include:
    /// <para>
    /// <list type="bullet">
    /// <item>
    ///     <c>code_interpreter</c> - <see cref="CodeInterpreterToolDefinition"/> 
    ///     - works with data, math, and computer code
    /// </item>
    /// <item>
    ///     <c>file_search</c> - <see cref="FileSearchToolDefinition"/> 
    ///     - dynamically enriches an Run's context with content from vector stores
    /// </item>
    /// <item>
    ///     <c>function</c> - <see cref="FunctionToolDefinition"/>
    ///     - enables caller-provided custom functions for actions and enrichment
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    [CodeGenMember("Tools")]
    public IList<ToolDefinition> ToolsOverride { get; }

    /// <summary> Set of 16 key-value pairs that can be attached to an object. This can be useful for storing additional information about the object in a structured format. Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long. </summary>
    public IDictionary<string, string> Metadata { get; }

    /// <summary> What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
    ///
    /// We generally recommend altering this or temperature but not both.
    /// </summary>
    [CodeGenMember("TopP")]
    public float? NucleusSamplingFactor { get; set; }

    /// <summary> The maximum number of prompt tokens that may be used over the course of the run. The run will make a best effort to use only the number of prompt tokens specified, across multiple turns of the run. If the run exceeds the number of prompt tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info. </summary>
    [CodeGenMember("MaxPromptTokens")]
    public int? MaxInputTokenCount { get; set; }

    /// <summary> The maximum number of completion tokens that may be used over the course of the run. The run will make a best effort to use only the number of completion tokens specified, across multiple turns of the run. If the run exceeds the number of completion tokens specified, the run will end with status `incomplete`. See `incomplete_details` for more info. </summary>
    [CodeGenMember("MaxCompletionTokens")]
    public int? MaxOutputTokenCount { get; set; }

    /// <summary> Gets or sets the truncation strategy. </summary>
    public RunTruncationStrategy TruncationStrategy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CodeGenMember("ToolChoice")]
    public ToolConstraint ToolConstraint { get; set; }

    // CUSTOM: Made internal.
    [CodeGenMember("ReasoningEffort")]
    internal ChatReasoningEffortLevel? ReasoningEffortLevel { get; set; }

    private void SerializeToolConstraint(Utf8JsonWriter writer, ModelReaderWriterOptions options)
        => writer.WriteObjectValue(ToolConstraint, options);

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
