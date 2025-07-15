using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
// - Suppressed constructor in favor of custom default constructor.
[CodeGenType("CreateResponse")]
[CodeGenVisibility(nameof(ResponseCreationOptions), CodeGenVisibility.Public)]
[CodeGenSuppress(nameof(ResponseCreationOptions), typeof(IEnumerable<ResponseItem>))]
public partial class ResponseCreationOptions
{
    // CUSTOM: Temporarily made internal.
    [CodeGenMember("Include")]
    internal IList<InternalIncludable> Include { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Model")]
    internal string Model { get; set; }

    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Input")]
    internal IList<ResponseItem> Input { get; set; }

    // CUSTOM: Made internal. This value comes from a parameter on the client method.
    internal bool? Stream { get; set; }

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
    public IList<ResponseTool> Tools { get; }

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

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
