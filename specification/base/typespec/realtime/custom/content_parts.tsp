using TypeSpec.OpenAPI;

namespace OpenAI;

union RealtimeContentPartType {
  string,
  input_text: "input_text",
  input_audio: "input_audio",
  text: "text",
  audio: "audio",
}

@discriminator("type")
model RealtimeContentPart {
  type: RealtimeContentPartType;
}

model RealtimeRequestTextContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.input_text;
  text: string;
}

model RealtimeRequestAudioContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.input_audio;
  transcript?: string;
}

model RealtimeResponseTextContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.text;
  text: string;
}

model RealtimeResponseAudioContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.audio;
  transcript: string | null;
}
