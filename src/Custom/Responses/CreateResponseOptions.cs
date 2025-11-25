using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses
{
    [CodeGenType("DotNetCreateResponse")]
    public partial class CreateResponseOptions
    {
        public CreateResponseOptions(IEnumerable<ResponseItem> inputItems, string model)
        {
            Argument.AssertNotNull(inputItems, nameof(inputItems));
            Argument.AssertNotNullOrEmpty(model, nameof(model));

            Metadata = new ChangeTrackingDictionary<string, string>();
            Tools = new ChangeTrackingList<ResponseTool>();
            InputItems = inputItems.ToList();
            IncludedProperties = new ChangeTrackingList<IncludedResponseProperty>();
            Model = model;
        }

        /// <summary>
        /// Gets or sets whether to run the response in background mode. This corresponds to the "background" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Background")]
        public bool? BackgroundModeEnabled { get; set; }

        /// <summary>
        /// Gets or sets how tool calls should be selected during response generation. This corresponds to the "tool_choice" property in the JSON representation.
        /// </summary>
        [CodeGenMember("ToolChoice")]
        public ResponseToolChoice ToolChoice { get; set; }

        /// <summary>
        /// Gets or sets the input items to be processed for the response. This corresponds to the "input" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Input")]
        public IList<ResponseItem> InputItems { get; internal set; }

        /// <summary>
        /// Gets or sets the list of fields to include in the response. This corresponds to the "include" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Include")]
        public IList<IncludedResponseProperty> IncludedProperties { get; }

        /// <summary>
        /// Gets or sets whether multiple tool calls can be made in parallel. This corresponds to the "parallel_tool_calls" property in the JSON representation.
        /// </summary>
        [CodeGenMember("ParallelToolCalls")]
        public bool? ParallelToolCallsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the response should be stored for later retrieval. This corresponds to the "store" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Store")]
        public bool? StoredOutputEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the response should be streamed. This corresponds to the "stream" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Stream")]
        public bool? StreamingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of output tokens to generate. This corresponds to the "max_output_tokens" property in the JSON representation.
        /// </summary>
        [CodeGenMember("MaxOutputTokens")]
        public int? MaxOutputTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the model to be used for generating the response. This corresponds to the "model" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the reasoning options for the response. This corresponds to the "reasoning" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Reasoning")]
        public ResponseReasoningOptions ReasoningOptions { get; set; }

        /// <summary>
        /// Gets or sets the text format options for the response. This corresponds to the "text" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Text")]
        public ResponseTextOptions TextOptions { get; set; }

         /// <summary>
        /// Gets or sets the truncation mode for the response. This corresponds to the "truncation" property in the JSON representation.
        /// </summary>
        [CodeGenMember("Truncation")]
        public ResponseTruncationMode? TruncationMode { get; set; }

        /// <summary>
        /// Gets or sets the end user identifier. This corresponds to the "user" property in the JSON representation.
        /// </summary>
        [CodeGenMember("User")]
        public string EndUserId { get; set; }

        internal static CreateResponseOptions Create(IEnumerable<ResponseItem> inputItems, string model, ResponsesClient client, ResponseCreationOptions options = null, bool isStreaming = false)
        {
            Argument.AssertNotNull(inputItems, nameof(inputItems));
            options ??= new();
            var responseCreationOptions = client.CreatePerCallOptions(options, inputItems, model, isStreaming);
#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
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
                [.. inputItems],
                [.. responseCreationOptions.IncludedProperties],
                responseCreationOptions.ParallelToolCallsEnabled,
                responseCreationOptions.StoredOutputEnabled,
                responseCreationOptions.Stream,
                new JsonPatch());
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        internal CreateResponseOptions GetClone()
        {
            CreateResponseOptions copiedOptions = (CreateResponseOptions)this.MemberwiseClone();
            copiedOptions.Patch = _patch;

            return copiedOptions;
        }

        public static implicit operator BinaryContent(CreateResponseOptions createResponseOptions)
        {
            if (createResponseOptions == null)
            {
                return null;
            }
            return BinaryContent.Create(createResponseOptions, ModelSerializationExtensions.WireOptions);
        }
    }
}
