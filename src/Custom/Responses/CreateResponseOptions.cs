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
            User = user;
            ServiceTier = serviceTier;
            PreviousResponseId = previousResponseId;
            Model = model;
            Reasoning = reasoning;
            Background = background;
            MaxOutputTokens = maxOutputTokens;
            Instructions = instructions;
            Text = text;
            Tools = tools ?? new ChangeTrackingList<ResponseTool>();
            ToolChoice = toolChoice;
            Truncation = truncation;
            Input = input;
            Include = include ?? new ChangeTrackingList<Includable>();
            ParallelToolCalls = parallelToolCalls;
            Store = store;
            Stream = stream;
            _patch = patch;
            _patch.SetPropagators(PropagateSet, PropagateGet);
        }
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates.

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Experimental("SCME0001")]
        public ref JsonPatch Patch => ref _patch;

        public IDictionary<string, string> Metadata { get; }

        public float? Temperature { get; set; }

        public float? TopP { get; set; }

        public string User { get; set; }

        public ResponseServiceTier? ServiceTier { get; set; }

        public string PreviousResponseId { get; set; }

        public ModelIdsResponses? Model { get; set; }

        public ResponseReasoningOptions Reasoning { get; set; }

        public bool? Background { get; set; }

        public int? MaxOutputTokens { get; set; }

        public string Instructions { get; set; }

        public ResponseTextOptions Text { get; set; }

        public IList<ResponseTool> Tools { get; }

        public ResponseToolChoice ToolChoice { get; set; }

        public ResponseTruncationMode? Truncation { get; set; }

        public IList<ResponseItem> Input { get; internal set; }

        public IList<Includable> Include { get; set; }

        public bool? ParallelToolCalls { get; set; }

        public bool? Store { get; set; }

        public bool? Stream { get; set; }

        internal static CreateResponseOptions Create(IEnumerable<ResponseItem> inputItems, OpenAIResponseClient client, ResponseCreationOptions options = null, bool isStreaming = false)
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
