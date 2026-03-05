using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Audio;

[CodeGenType("CreateSpeechRequestModel")] internal readonly partial struct InternalCreateSpeechRequestModel { }
[CodeGenType("CreateResponseRequestAccept")] internal readonly partial struct InternalCreateTranscriptionRequestAccept {}
[CodeGenType("CreateTranscriptionRequestModel")] internal readonly partial struct InternalCreateTranscriptionRequestModel { }
[CodeGenType("CreateTranscriptionRequestTimestampGranularity")] internal readonly partial struct InternalCreateTranscriptionRequestTimestampGranularities { }
[CodeGenType("CreateTranscriptionResponseJson")] internal partial class InternalCreateTranscriptionResponseJson { }
[CodeGenType("CreateTranscriptionResponseVerboseJson")] internal partial class InternalCreateTranscriptionResponseVerboseJson { }
[CodeGenType("DotNetCombinedJsonTranscriptionResponseTask")] internal readonly partial struct InternalDotNetCombinedJsonTranscriptionResponseTask { }
[CodeGenType("CreateTranslationRequestModel")] internal readonly partial struct InternalCreateTranslationRequestModel { }
[CodeGenType("CreateTranslationResponseJson")] internal partial class InternalCreateTranslationResponseJson { }
[CodeGenType("TranscriptionInclude")] internal readonly partial struct InternalTranscriptionInclude { }
[CodeGenType("UnknownDotNetCreateTranscriptionStreamingResponse")] internal partial class InternalUnknownCreateTranscriptionResponseStreamEvent { }
[CodeGenType("CreateTranscriptionResponseJsonLogprobs")] internal partial class InternalCreateTranscriptionResponseJsonLogprob { }
[CodeGenType("CreateSpeechRequestStreamFormat")] internal readonly partial struct InternalCreateSpeechRequestStreamFormat { }
[CodeGenType("CreateTranscriptionResponseJsonUsageType")] internal readonly partial struct InternalCreateTranscriptionResponseJsonUsageType{ }
[CodeGenType("CreateTranscriptionResponseJsonUsage")] internal partial class InternalCreateTranscriptionResponseJsonUsage { }
[CodeGenType("UnknownCreateTranscriptionResponseJsonUsage")] internal partial class InternalUnknownCreateTranscriptionResponseJsonUsage { }
[CodeGenType("TranscriptTextUsageTokens")] internal partial class InternalTranscriptTextUsageTokens { }
[CodeGenType("TranscriptTextUsageTokensInputTokenDetails")] internal partial class InternalTranscriptTextUsageTokensInputTokenDetails { }
[CodeGenType("TranscriptTextUsageDuration")] internal partial class InternalTranscriptTextUsageDuration { }
[CodeGenType("CreateTranscriptionResponseDiarizedJson")] internal partial class InternalCreateTranscriptionResponseDiarizedJson { }
[CodeGenType("TranscriptionDiarizedSegment")] internal partial class InternalTranscriptionDiarizedSegment { }
[CodeGenType("VadConfig")] internal partial class InternalVadConfig { }

// Remove these after https://github.com/microsoft/openai-openapi-pr/issues/478 is fixed
[CodeGenType("TranscriptTextDeltaEvent")] internal partial class InternalTranscriptTextDeltaEvent { }
[CodeGenType("TranscriptTextDeltaEventLogprobs")] internal partial class InternalTranscriptTextDeltaEventLogprobs { }
[CodeGenType("TranscriptTextDoneEvent")] internal partial class InternalTranscriptTextDoneEvent { }
[CodeGenType("TranscriptTextDoneEventLogprobs")] internal partial class InternalTranscriptTextDoneEventLogprobs { }
[CodeGenType("TranscriptTextSegmentEvent")] internal partial class InternalTranscriptTextSegmentEvent { }

// Remove these after streaming events are implemented
[CodeGenType("DotNetCreateSpeechStreamingResponseType")] internal readonly partial struct InternalDotNetCreateSpeechStreamingResponseType { }
[CodeGenType("SpeechAudioDoneEventUsage")] internal partial class InternalSpeechAudioDoneEventUsage { }
[CodeGenType("DotNetCreateSpeechStreamingResponse")] internal partial class InternalDotNetCreateSpeechStreamingResponse { }
[CodeGenType("UnknownDotNetCreateSpeechStreamingResponse")] internal partial class InternalUnknownDotNetCreateSpeechStreamingResponse { }
[CodeGenType("DotNetSpeechAudioDeltaEvent")] internal partial class InternalDotNetSpeechAudioDeltaEvent { }
[CodeGenType("DotNetSpeechAudioDoneEvent")] internal partial class InternalDotNetSpeechAudioDoneEvent { }