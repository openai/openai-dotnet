using TypeSpec.OpenAPI;

namespace OpenAI;

union RealtimeContentPartType {
  string,
  /** Input text content from the user or system. */
  input_text: "input_text",
  /** Input audio content from the user. */
  input_audio: "input_audio",
  /** Output text content from the assistant (GA format). */
  output_text: "output_text",
  /** Output audio content from the assistant (GA format). */
  output_audio: "output_audio",
}

@discriminator("type")
model RealtimeContentPart {
  type: RealtimeContentPartType;
}

/** Text content part for input (user or system messages). */
model RealtimeRequestTextContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.input_text;
  text: string;
}

/** Audio content part for input (user messages). */
model RealtimeRequestAudioContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.input_audio;
  transcript?: string;
}

/** Text content part for output (assistant messages) - GA format. */
model RealtimeResponseTextContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.output_text;
  text: string;
}

/** Audio content part for output (assistant messages) - GA format. */
model RealtimeResponseAudioContentPart extends RealtimeContentPart {
  type: RealtimeContentPartType.output_audio;
  transcript: string | null;
}
