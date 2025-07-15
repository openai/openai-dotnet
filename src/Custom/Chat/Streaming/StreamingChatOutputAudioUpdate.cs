namespace OpenAI.Chat;

using System;
using System.Diagnostics.CodeAnalysis;

// CUSTOM: Added Experimental attribute.
/// <summary>
/// Represents an audio update in a streaming chat response.
/// </summary>
[CodeGenType("ChatCompletionMessageAudioChunk")]
public partial class StreamingChatOutputAudioUpdate
{
    // CUSTOM: Renamed for clarity of incremental data availability while streaming.
    /// <summary>
    /// The next, incremental audio transcript part from the streaming response. <see cref="TranscriptUpdate"/> payloads
    /// across all received <see cref="StreamingChatCompletionUpdate"/> instances should be concatenated to form the
    /// full response audio transcript.
    /// </summary>
    [CodeGenMember("Transcript")]
    public string TranscriptUpdate { get; }

    // CUSTOM: Renamed for clarity of incremental data availability while streaming.
    /// <summary>
    /// The next, incremental response audio data chunk from the streaming response. <see cref="AudioBytesUpdate"/> payloads
    /// across all received <see cref="StreamingChatCompletionUpdate"/> instances should be concatenated to form the
    /// full response audio.
    /// </summary>
    [CodeGenMember("Data")]
    public BinaryData AudioBytesUpdate { get; }
}
