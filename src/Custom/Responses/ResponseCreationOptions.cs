using System;
using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM:
// - Renamed.
// - Suppressed constructor in favor of custom default constructor.
[CodeGenType("CreateResponsesRequest")]
[CodeGenSuppress(nameof(ResponseCreationOptions), typeof(InternalCreateResponsesRequestModel), typeof(IEnumerable<ResponseItem>))]
public partial class ResponseCreationOptions
{
    // CUSTOM: Temporarily made internal.
    [CodeGenMember("Include")]
    internal IList<InternalCreateResponsesRequestIncludable> Include { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Model")]
    internal InternalCreateResponsesRequestModel Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Input")]
    internal IList<ResponseItem> Input { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    internal bool? Stream { get; set; }

    // CUSTOM: Added public default constructor now that there are no required properties.
    public ResponseCreationOptions()
    {
        Input =  new ChangeTrackingList<ResponseItem>();
        Metadata = new ChangeTrackingDictionary<string, string>();
        Tools = new ChangeTrackingList<ResponseTool>();
        Include = new ChangeTrackingList<InternalCreateResponsesRequestIncludable>();
    }

    // CUSTOM: Renamed.
    [CodeGenMember("User")]
    public string EndUserId { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Reasoning")]
    public ResponseReasoningOptions ReasoningOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("MaxOutputTokens")]
    public int? MaxOutputTokenCount { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Text")]
    public ResponseTextOptions TextOptions { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Truncation")]
    public ResponseTruncationMode? TruncationMode { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("ParallelToolCalls")]
    public bool? ParallelToolCallsEnabled { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Store")]
    public bool? StoredOutputEnabled { get; set; }

    // CUSTOM: Using convenience type.
    [CodeGenMember("ToolChoice")]
    public ResponseToolChoice ToolChoice { get; set; }

    // CUSTOM: Apply get-only collection pattern
    [CodeGenMember("Tools")]
    public IList<ResponseTool> Tools { get; set; }

    internal ResponseCreationOptions GetClone()
    {
        ResponseCreationOptions copiedOptions = (ResponseCreationOptions)this.MemberwiseClone();

        if (SerializedAdditionalRawData is not null)
        {
            copiedOptions.SerializedAdditionalRawData = new ChangeTrackingDictionary<string, BinaryData>();
            foreach (KeyValuePair<string, BinaryData> sourcePair in SerializedAdditionalRawData)
            {
                copiedOptions.SerializedAdditionalRawData[sourcePair.Key] = sourcePair.Value;
            }
        }

        return copiedOptions;
    }
}
