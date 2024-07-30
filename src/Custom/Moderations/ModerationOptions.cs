using System;
using System.Collections.Generic;

namespace OpenAI.Moderations;

[CodeGenModel("CreateModerationRequest")]
[CodeGenSuppress("ModerationOptions", typeof(BinaryData))]
internal partial class ModerationOptions
{
    // CUSTOM:
    // - Made internal. This value comes from a parameter on the client method.
    // - Added setter.
    /// <summary>
    /// The input text to classify
    /// <para>
    /// To assign an object to this property use <see cref="BinaryData.FromObjectAsJson{T}(T, System.Text.Json.JsonSerializerOptions?)"/>.
    /// </para>
    /// <para>
    /// To assign an already formatted json string to this property use <see cref="BinaryData.FromString(string)"/>.
    /// </para>
    /// <para>
    /// <remarks>
    /// Supported types:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="string"/></description>
    /// </item>
    /// <item>
    /// <description><see cref="IList{T}"/> where <c>T</c> is of type <see cref="string"/></description>
    /// </item>
    /// </list>
    /// </remarks>
    /// Examples:
    /// <list type="bullet">
    /// <item>
    /// <term>BinaryData.FromObjectAsJson("foo")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("\"foo\"")</term>
    /// <description>Creates a payload of "foo".</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromObjectAsJson(new { key = "value" })</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// <item>
    /// <term>BinaryData.FromString("{\"key\": \"value\"}")</term>
    /// <description>Creates a payload of { "key": "value" }.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    internal BinaryData Input { get; set; }

    // CUSTOM: Made internal. The model is specified by the client.
    /// <summary>
    /// Two content moderations models are available: `text-moderation-stable` and
    /// `text-moderation-latest`. The default is `text-moderation-latest` which will be automatically
    /// upgraded over time. This ensures you are always using our most accurate model. If you use
    /// `text-moderation-stable`, we will provide advanced notice before updating the model. Accuracy
    /// of `text-moderation-stable` may be slightly lower than for `text-moderation-latest`.
    /// </summary>
    internal InternalCreateModerationRequestModel? Model { get; set; }

    // CUSTOM: Made public now that there are no required properties.
    /// <summary> Initializes a new instance of <see cref="ModerationOptions"/> for deserialization. </summary>
    public ModerationOptions()
    {
    }
}
