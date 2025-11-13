using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public partial class CreateResponseOptions
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        public CreateResponseOptions(List<ResponseItem> input)
        {
            Argument.AssertNotNull(input, nameof(input));

            Metadata = new ChangeTrackingDictionary<string, string>();
            Tools = new ChangeTrackingList<ResponseTool>();
            Input = input;
            Include = new ChangeTrackingList<Includable>();
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal CreateResponseOptions(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ResponseServiceTier? serviceTier, string previousResponseId, ModelIdsResponses? model, ResponseReasoningOptions reasoning, bool? background, int? maxOutputTokens, string instructions, ResponseTextOptions text, IList<ResponseTool> tools, ResponseToolChoice toolChoice, ResponseTruncationMode? truncation, IList<ResponseItem> input, IList<Includable> include, bool? parallelToolCalls, bool? store, bool? stream, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            EndUserId = user;
            ServiceTier = serviceTier;
            PreviousResponseId = previousResponseId;
            Model = model;
            ReasoningOptions = reasoning;
            IsBackgroundModeEnabled = background;
            MaxOutputTokenCount = maxOutputTokens;
            Instructions = instructions;
            TextOptions = text;
            Tools = tools ?? new ChangeTrackingList<ResponseTool>();
            ToolChoice = toolChoice;
            TruncationMode = truncation;
            Input = input;
            Include = include ?? new ChangeTrackingList<Includable>();
            IsParallelToolCallsEnabled = parallelToolCalls;
            IsStoredOutputEnabled = store;
            IsStreamingEnabled = stream;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        /// <summary>
        /// Gets a dictionary of custom metadata for the response. This corresponds to the "metadata" property in the JSON representation.
        /// </summary>
        public IDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Gets or sets the sampling temperature to use, between 0 and 2. This corresponds to the "temperature" property in the JSON representation.
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Gets or sets the nucleus sampling parameter, between 0 and 1. This corresponds to the "top_p" property in the JSON representation.
        /// </summary>
        public float? TopP { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier representing the end-user. This corresponds to the "user" property in the JSON representation.
        /// </summary>
        public string EndUserId { get; set; }

        /// <summary>
        /// Gets or sets the service tier to be used for processing the request. This corresponds to the "service_tier" property in the JSON representation.
        /// </summary>
        public ResponseServiceTier? ServiceTier { get; set; }

        /// <summary>
        /// Gets or sets the ID of the response to continue from, enabling streaming responses. This corresponds to the "previous_response_id" property in the JSON representation.
        /// </summary>
        public string PreviousResponseId { get; set; }

        /// <summary>
        /// Gets or sets the model to be used for generating the response. This corresponds to the "model" property in the JSON representation.
        /// </summary>
        public ModelIdsResponses? Model { get; set; }

        /// <summary>
        /// Gets or sets the reasoning options for the response. This corresponds to the "reasoning" property in the JSON representation.
        /// </summary>
        public ResponseReasoningOptions ReasoningOptions { get; set; }

        /// <summary>
        /// Gets or sets whether to run the response in background mode. This corresponds to the "background" property in the JSON representation.
        /// </summary>
        public bool? IsBackgroundModeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of output tokens to generate. This corresponds to the "max_output_tokens" property in the JSON representation.
        /// </summary>
        public int? MaxOutputTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the instructions to guide the response generation. This corresponds to the "instructions" property in the JSON representation.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets or sets the text format options for the response. This corresponds to the "text" property in the JSON representation.
        /// </summary>
        public ResponseTextOptions TextOptions { get; set; }

        /// <summary>
        /// Gets a list of tools available to the response. This corresponds to the "tools" property in the JSON representation.
        /// </summary>
        public IList<ResponseTool> Tools { get; }

        /// <summary>
        /// Gets or sets how tool calls should be selected during response generation. This corresponds to the "tool_choice" property in the JSON representation.
        /// </summary>
        public ResponseToolChoice ToolChoice { get; set; }

        /// <summary>
        /// Gets or sets the truncation mode for the response. This corresponds to the "truncation" property in the JSON representation.
        /// </summary>
        public ResponseTruncationMode? TruncationMode { get; set; }

        /// <summary>
        /// Gets or sets the input items to be processed for the response. This corresponds to the "input" property in the JSON representation.
        /// </summary>
        public IList<ResponseItem> Input { get; internal set; }

        /// <summary>
        /// Gets or sets the list of fields to include in the response. This corresponds to the "include" property in the JSON representation.
        /// </summary>
        public IList<Includable> Include { get; set; }

        /// <summary>
        /// Gets or sets whether multiple tool calls can be made in parallel. This corresponds to the "parallel_tool_calls" property in the JSON representation.
        /// </summary>
        public bool? IsParallelToolCallsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the response should be stored for later retrieval. This corresponds to the "store" property in the JSON representation.
        /// </summary>
        public bool? IsStoredOutputEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the response should be streamed. This corresponds to the "stream" property in the JSON representation.
        /// </summary>
        public bool? IsStreamingEnabled { get; set; }

        internal static CreateResponseOptions Create(IEnumerable<ResponseItem> inputItems, ResponsesClient client, ResponseCreationOptions options = null, bool isStreaming = false)
        {
            Argument.AssertNotNull(inputItems, nameof(inputItems));
            options ??= new();
            var responseCreationOptions = client.CreatePerCallOptions(options, inputItems, isStreaming);

            return new CreateResponseOptions(
                responseCreationOptions.Metadata,
                responseCreationOptions.Temperature,
                responseCreationOptions.TopP,
                responseCreationOptions.EndUserId,
                responseCreationOptions.ServiceTier,
                responseCreationOptions.PreviousResponseId,
                responseCreationOptions.Model,
                responseCreationOptions.ReasoningOptions,
                responseCreationOptions.BackgroundModeEnabled,
                responseCreationOptions.MaxOutputTokenCount,
                responseCreationOptions.Instructions,
                responseCreationOptions.TextOptions,
                responseCreationOptions.Tools,
                responseCreationOptions.ToolChoice,
                responseCreationOptions.TruncationMode,
                inputItems.ToList(),
                [.. responseCreationOptions.Include.Select(x => x.ToIncludable())],
                responseCreationOptions.ParallelToolCallsEnabled,
                responseCreationOptions.StoredOutputEnabled,
                responseCreationOptions.Stream,
                new JsonPatch());
        }

        internal CreateResponseOptions GetClone()
        {
            CreateResponseOptions copiedOptions = (CreateResponseOptions)this.MemberwiseClone();
            copiedOptions.Patch = _patch;

            return copiedOptions;
        }
    }
}
