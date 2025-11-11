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

        internal ResponseResult(IDictionary<string, string> metadata, float? temperature, float? topP, string user, string id, DateTimeOffset createdAt, ResponseError error, ResponseIncompleteStatusDetails incompleteDetails, IEnumerable<ResponseItem> output, bool parallelToolCalls)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            User = user;
            Tools = new ChangeTrackingList<ResponseTool>();
            Id = id;
            CreatedAt = createdAt;
            Error = error;
            IncompleteDetails = incompleteDetails;
            Output = output.ToList();
            ParallelToolCalls = parallelToolCalls;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.
        internal ResponseResult(IDictionary<string, string> metadata, float? temperature, float? topP, string user, ResponseServiceTier? serviceTier, string previousResponseId, ModelIdsResponses? model, ResponseReasoningOptions reasoning, bool? background, int? maxOutputTokens, string instructions, ResponseTextOptions text, IList<ResponseTool> tools, ResponseToolChoice toolChoice, ResponseTruncationMode? truncation, string id, string @object, ResponseStatus? status, DateTimeOffset createdAt, ResponseError error, ResponseIncompleteStatusDetails incompleteDetails, IList<ResponseItem> output, string outputText, ResponseTokenUsage usage, bool parallelToolCalls, in JsonPatch patch)
        {
            // Plugin customization: ensure initialization of collections
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            TopP = topP;
            User = user;
            ServiceTier = serviceTier;
            PreviousResponseId = previousResponseId;
            InternalModel = model;
            Reasoning = reasoning;
            Background = background;
            MaxOutputTokens = maxOutputTokens;
            Instructions = instructions;
            Text = text;
            Tools = tools ?? new ChangeTrackingList<ResponseTool>();
            ToolChoice = toolChoice;
            Truncation = truncation;
            Id = id;
            Object = @object;
            Status = status;
            CreatedAt = createdAt;
            Error = error;
            IncompleteDetails = incompleteDetails;
            Output = output ?? new ChangeTrackingList<ResponseItem>();
            OutputText = outputText;
            Usage = usage;
            ParallelToolCalls = parallelToolCalls;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        public IDictionary<string, string> Metadata { get; }

        public float? Temperature { get; }

        public float? TopP { get; }

        public string User { get; }

        public ResponseServiceTier? ServiceTier { get; }

        public string PreviousResponseId { get; }

        public ModelIdsResponses? InternalModel { get; }

        public string Model => InternalModel?.ToString();

        public ResponseReasoningOptions Reasoning { get; }

        public bool? Background { get; }

        public int? MaxOutputTokens { get; }

        public string Instructions { get; }

        public ResponseTextOptions Text { get; }

        public IList<ResponseTool> Tools { get; }

        public ResponseToolChoice ToolChoice { get; }

        public ResponseTruncationMode? Truncation { get; }

        public string Id { get; }

        public string Object { get; } = "ResponseResult";

        public ResponseStatus? Status { get; }

        public DateTimeOffset CreatedAt { get; }

        public ResponseError Error { get; }

        public ResponseIncompleteStatusDetails IncompleteDetails { get; }

        public IList<ResponseItem> Output { get; }

        public string OutputText { get; }

        public ResponseTokenUsage Usage { get; }

        public bool ParallelToolCalls { get; }

        public string GetOutputText()
        {
            IEnumerable<string> outputTextSegments = Output.Where(item => item is InternalResponsesAssistantMessage)
                .Select(item => item as InternalResponsesAssistantMessage)
                .SelectMany(message => message.Content.Where(contentPart => contentPart.Kind == ResponseContentPartKind.OutputText)
                    .Select(outputTextPart => outputTextPart.Text));
            return outputTextSegments.Any() ? string.Concat(outputTextSegments) : null;
        }
    }
}
