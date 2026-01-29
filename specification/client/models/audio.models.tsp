import "../../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI;

@access(Access.public)
@usage(Usage.output)
model DotNetCombinedJsonTranscriptionResponse {
  ...CreateTranscriptionResponseVerboseJson;
  logprobs?: LogProbProperties[];
}

// Remove this after https://github.com/microsoft/openai-openapi-pr/issues/4785 is fixed
@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetCreateTranscriptionStreamingResponse {
  type: DotNetCreateTranscriptionStreamingResponseType;
}

union DotNetCreateTranscriptionStreamingResponseType {
  `transcript.text.segment`: "transcript.text.segment",
  `transcript.text.delta`: "transcript.text.delta",
  `transcript.text.done`: "transcript.text.done",
  string
}

model DotNetTranscriptTextSegmentEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextSegmentEvent;
}

model DotNetTranscriptTextDeltaEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextDeltaEvent;
}

model DotNetTranscriptTextDoneEvent extends DotNetCreateTranscriptionStreamingResponse {
  ...TranscriptTextDoneEvent;
}

@usage(Usage.output | Usage.json)
@discriminator("type")
model DotNetCreateSpeechStreamingResponse {
  type: DotNetCreateSpeechStreamingResponseType;
}

union DotNetCreateSpeechStreamingResponseType {
  `speech.audio.delta`: "speech.audio.delta",
  `speech.audio.done`: "speech.audio.done",
  string
}

model DotNetSpeechAudioDeltaEvent extends DotNetCreateSpeechStreamingResponse {
  ...SpeechAudioDeltaEvent;
}

model DotNetSpeechAudioDoneEvent extends DotNetCreateSpeechStreamingResponse {
  ...SpeechAudioDoneEvent;
}
