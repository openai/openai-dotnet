using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Files;

[CodeGenType("OpenAIFile")]
public partial class OpenAIFile
{
    // CUSTOM: Made private. This property does not add value in the context of a strongly-typed class.
    /// <summary> The object type, which is always "file". </summary>
    private string Object { get; } = "file";

    // CUSTOM:
    // - Added Experimental attribute.
    // - Renamed.
    /// <summary> The size of the file, in bytes. </summary> 
    [Experimental("OPENAI001")]
    [CodeGenMember("Bytes")]
    public long? SizeInBytesLong { get; }

    /// <summary>
    /// <para>
    /// <b>Please use <see cref="SizeInBytesLong"/> instead of this property.</b>
    /// </para>
    /// A backwards-compatible facade for <see cref="SizeInBytesLong"/>.
    /// </summary>
    /// <remarks>
    /// This property will throw an <see cref="OverflowException"/> if the reported file size exceeds the maximum value
    /// of a 32-bit integer, approximately 2GB. The newer <see cref="SizeInBytesLong"/> property addresses this
    /// limitation and should always be used.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int? SizeInBytes => SizeInBytesLong is null ? null : checked((int)SizeInBytesLong.Value);

    // CUSTOM: Added the Obsolete attribute.
    [Obsolete($"This property is obsolete. If this is a fine-tuning training file, it may take some time to process"
        + $" after it has been uploaded. While the file is processing, you can still create a fine-tuning job but it"
        + $" will not start until the file processing has completed.")]
    public FileStatus Status { get; }

    // CUSTOM: Added the Obsolete attribute.
    [Obsolete($"This property is obsolete. For details on why a fine-tuning training file failed validation, see the"
        + $" `error` field on the fine-tuning job.")]
    public string StatusDetails { get; }

    internal static OpenAIFile FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeOpenAIFile(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
