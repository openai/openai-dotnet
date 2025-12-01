using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    public partial class ResponseResult
    {
        [Experimental("SCME0001")]
        private JsonPatch _patch;

        internal ResponseResult(IDictionary<string, string> metadata, float? temperature, float? topP, string user, string id, DateTimeOffset createdAt, ResponseError error, ResponseIncompleteStatusDetails incompleteDetails, IEnumerable<ResponseItem> output, bool parallelToolCalls, Conversation conversation = null)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            EndUserId = user;
            Tools = new ChangeTrackingList<ResponseTool>();
            Id = id;
            CreatedAt = createdAt;
            Error = error;
            IncompleteStatusDetails = incompleteDetails;
            OutputItems = output.ToList();
            ParallelToolCallsEnabled = parallelToolCalls;
            Conversation = conversation;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseResult(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ResponseServiceTier? serviceTier, string previousResponseId, string model, ResponseReasoningOptions reasoning, bool? background, int? maxOutputTokens, string instructions, ResponseTextOptions text, IList<ResponseTool> tools, ResponseToolChoice toolChoice, ResponseTruncationMode? truncation, string id, string @object, ResponseStatus? status, DateTimeOffset createdAt, ResponseError error, ResponseIncompleteStatusDetails incompleteDetails, IList<ResponseItem> output, string outputText, ResponseTokenUsage usage, bool parallelToolCalls, Conversation conversation, in JsonPatch patch)
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
            BackgroundModeEnabled = background;
            MaxOutputTokenCount = maxOutputTokens;
            Instructions = instructions;
            TextOptions = text;
            Tools = tools ?? new ChangeTrackingList<ResponseTool>();
            ToolChoice = toolChoice;
            TruncationMode = truncation;
            Id = id;
            Object = @object;
            Status = status;
            CreatedAt = createdAt;
            Error = error;
            IncompleteStatusDetails = incompleteDetails;
            OutputItems = output ?? new ChangeTrackingList<ResponseItem>();
            OutputText = outputText;
            Usage = usage;
            ParallelToolCallsEnabled = parallelToolCalls;
            Conversation = conversation;
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
        /// Gets the sampling temperature that was used, between 0 and 2. This corresponds to the "temperature" property in the JSON representation.
        /// </summary>
        public float? Temperature { get; set; }

        /// <summary>
        /// Gets the nucleus sampling parameter that was used, between 0 and 1. This corresponds to the "top_p" property in the JSON representation.
        /// </summary>
        public float? TopP { get; set; }

        /// <summary>
        /// Gets the unique identifier representing the end-user. This corresponds to the "user" property in the JSON representation.
        /// </summary>
        public string EndUserId { get; set; }

        /// <summary>
        /// Gets the service tier that was used for processing the request. This corresponds to the "service_tier" property in the JSON representation.
        /// </summary>
        public ResponseServiceTier? ServiceTier { get; set; }

        /// <summary>
        /// Gets the ID of the previous response that was continued from, if applicable. This corresponds to the "previous_response_id" property in the JSON representation.
        /// </summary>
        public string PreviousResponseId { get; set; }

        /// <summary>
        /// Gets the model name that was used for generating the response. This corresponds to the "model" property in the JSON representation.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Gets the reasoning options that were used for the response. This corresponds to the "reasoning" property in the JSON representation.
        /// </summary>
        public ResponseReasoningOptions ReasoningOptions { get; set; }

        /// <summary>
        /// Gets whether the response was run in background mode. This corresponds to the "background" property in the JSON representation.
        /// </summary>
        public bool? BackgroundModeEnabled { get; set; }

        /// <summary>
        /// Gets the maximum number of output tokens that were configured. This corresponds to the "max_output_tokens" property in the JSON representation.
        /// </summary>
        public int? MaxOutputTokenCount { get; set; }

        /// <summary>
        /// Gets the instructions that were used to guide the response generation. This corresponds to the "instructions" property in the JSON representation.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets the text format options that were used for the response. This corresponds to the "text" property in the JSON representation.
        /// </summary>
        public ResponseTextOptions TextOptions { get; set; }

        /// <summary>
        /// Gets a list of tools that were available for the response. This corresponds to the "tools" property in the JSON representation.
        /// </summary>
        public IList<ResponseTool> Tools { get; }

        /// <summary>
        /// Gets how tool calls were selected during response generation. This corresponds to the "tool_choice" property in the JSON representation.
        /// </summary>
        public ResponseToolChoice ToolChoice { get; set; }

        /// <summary>
        /// Gets the truncation mode that was used for the response. This corresponds to the "truncation" property in the JSON representation.
        /// </summary>
        public ResponseTruncationMode? TruncationMode { get; set; }

        /// <summary>
        /// Gets the unique identifier for the response. This corresponds to the "id" property in the JSON representation.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the object type identifier for the response. This corresponds to the "object" property in the JSON representation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; set; } = "ResponseResult";

        /// <summary>
        /// Gets the status of the response processing. This corresponds to the "status" property in the JSON representation.
        /// </summary>
        public ResponseStatus? Status { get; set; }

        /// <summary>
        /// Gets the timestamp when the response was created. This corresponds to the "created_at" property in the JSON representation.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets the error information if the response failed. This corresponds to the "error" property in the JSON representation.
        /// </summary>
        public ResponseError Error { get; set; }

        /// <summary>
        /// Gets the details about incomplete status if applicable. This corresponds to the "incomplete_details" property in the JSON representation.
        /// </summary>
        public ResponseIncompleteStatusDetails IncompleteStatusDetails { get; set; }

        /// <summary>
        /// Gets the output items generated by the response. This corresponds to the "output" property in the JSON representation.
        /// </summary>
        public IList<ResponseItem> OutputItems { get; }

        /// <summary>
        /// Gets the concatenated text output from the response, if any.
        /// </summary>
        internal string OutputText { get; set; }

        /// <summary>
        /// Gets the token usage statistics for the response. This corresponds to the "usage" property in the JSON representation.
        /// </summary>
        public ResponseTokenUsage Usage { get; set; }

        /// <summary>
        /// Gets whether multiple tool calls were made in parallel. This corresponds to the "parallel_tool_calls" property in the JSON representation.
        /// </summary>
        public bool ParallelToolCallsEnabled { get; set; }

        public Conversation Conversation { get; set; }

        public string GetOutputText()
        {
            IEnumerable<string> outputTextSegments = OutputItems.Where(item => item is InternalResponsesAssistantMessage)
                .Select(item => item as InternalResponsesAssistantMessage)
                .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                    .Select(outputTextPart => outputTextPart.Text));
            return outputTextSegments.Any() ? string.Concat(outputTextSegments) : null;
        }
    }
}
