using System;
using System.ClientModel;
using System.Collections.Generic;

namespace OpenAI.Moderations;

[CodeGenType("CreateModerationRequest")]
[CodeGenSuppress("ModerationOptions", typeof(BinaryData))]
internal partial class ModerationOptions
{
    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    [CodeGenMember("Input")]
    internal BinaryData Input { get; set; }

    // CUSTOM: Made internal. The model is specified by the client.
    [CodeGenMember("Model")]
    internal InternalCreateModerationRequestModel? Model { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ModerationOptions"/> for deserialization. </summary>
    public ModerationOptions()
    {
    }

    internal BinaryContent ToBinaryContent() => BinaryContent.Create(this, ModelSerializationExtensions.WireOptions);
}
