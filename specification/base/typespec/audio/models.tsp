/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

// Tool generated type. Extracts from CreateTranscriptionResponseJson.logprobs
alias CreateTranscriptionResponseJsonLogprob = {
  /** The token in the transcription. */
  token?: string;

  /** The log probability of the token. */
  logprob?: float32;

  /** The bytes of the token. */
  bytes?: float32[];
};

// Tool generated type. Extracts from TranscriptTextDeltaEvent.logprobs
alias TranscriptTextDeltaEventLogprob = {
  /** The token that was used to generate the log probability. */
  token?: string;

  /** The log probability of the token. */
  logprob?: float32;

  /** The bytes that were used to generate the log probability. */
  bytes?: int32[];
};

// Tool generated type. Extracts from TranscriptTextDoneEvent.logprobs
alias TranscriptTextDoneEventLogprob = {
  /** The token that was used to generate the log probability. */
  token?: string;

  /** The log probability of the token. */
  logprob?: float32;

  /** The bytes that were used to generate the log probability. */
  bytes?: int32[];
};

@doc("""
  The format of the output, in one of these options: `json`, `text`, `srt`, `verbose_json`, or `vtt`. For `gpt-4o-transcribe` and `gpt-4o-mini-transcribe`, the only supported format is `json`.
  """)
union AudioResponseFormat {
  "json",
  "text",
  "srt",
  "verbose_json",
  "vtt",
}

model CreateSpeechRequest {
  @doc("""
    One of the available [TTS models](/docs/models#tts): `tts-1`, `tts-1-hd` or `gpt-4o-mini-tts`.
    """)
  @extension("x-oaiTypeLabel", "string")
  `model`: string | "tts-1" | "tts-1-hd" | "gpt-4o-mini-tts";

  /** The text to generate audio for. The maximum length is 4096 characters. */
  @maxLength(4096)
  input: string;

  @doc("""
    Control the voice of your generated audio with additional instructions. Does not work with `tts-1` or `tts-1-hd`.
    """)
  @maxLength(4096)
  instructions?: string;

  @doc("""
    The voice to use when generating the audio. Supported voices are `alloy`, `ash`, `ballad`, `coral`, `echo`, `fable`, `onyx`, `nova`, `sage`, `shimmer`, and `verse`. Previews of the voices are available in the [Text to speech guide](/docs/guides/text-to-speech#voice-options).
    """)
  voice: VoiceIdsShared;

  @doc("""
    The format to audio in. Supported formats are `mp3`, `opus`, `aac`, `flac`, `wav`, and `pcm`.
    """)
  response_format?: "mp3" | "opus" | "aac" | "flac" | "wav" | "pcm" = "mp3";

  @doc("""
    The speed of the generated audio. Select a value from `0.25` to `4.0`. `1.0` is the default. Does not work with `gpt-4o-mini-tts`.
    """)
  @minValue(0.25)
  @maxValue(4)
  speed?: float32 = 1;
}

model CreateTranscriptionRequest {
  // Tool customization: Apply HttpPart for property in multipart request model
  /** The audio file object (not file name) to transcribe, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm. */
  @extension("x-oaiTypeLabel", "file")
  file: HttpPart<bytes>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    ID of the model to use. The options are `gpt-4o-transcribe`, `gpt-4o-mini-transcribe`, and `whisper-1` (which is powered by our open source Whisper V2 model).
    """)
  @extension("x-oaiTypeLabel", "string")
  `model`: HttpPart<
    | string
    | "whisper-1"
    | "gpt-4o-transcribe"
    | "gpt-4o-mini-transcribe">;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    The language of the input audio. Supplying the input language in [ISO-639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) (e.g. `en`) format will improve accuracy and latency.
    """)
  language?: HttpPart<string>;

  // Tool customization: Apply HttpPart for property in multipart request model
  /** An optional text to guide the model's style or continue a previous audio segment. The [prompt](/docs/guides/speech-to-text#prompting) should match the audio language. */
  prompt?: HttpPart<string>;

  // Tool customization: Use scenario-specific composed union and apply HttpPart for property in multipart request model
  response_format?: HttpPart<TranscriptionAudioResponseFormat>;

  // Tool customization: Use scenario-specific composed union and apply HttpPart for property in multipart request model
  /** The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use [log probability](https://en.wikipedia.org/wiki/Log_probability) to automatically increase the temperature until certain thresholds are hit. */
  temperature?: HttpPart<ZeroOrOneFloat>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    Additional information to include in the transcription response.
    `logprobs` will return the log probabilities of the tokens in the
    response to understand the model's confidence in the transcription.
    `logprobs` only works with response_format set to `json` and only with
    the models `gpt-4o-transcribe` and `gpt-4o-mini-transcribe`.
    """)
  `include[]`?: HttpPart<TranscriptionInclude[]>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    The timestamp granularities to populate for this transcription. `response_format` must be set `verbose_json` to use timestamp granularities. Either or both of these options are supported: `word`, or `segment`. Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
    """)
  `timestamp_granularities[]`?: HttpPart<("word" | "segment")[]>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    If set to true, the model response data will be streamed to the client
    as it is generated using [server-sent events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events#Event_stream_format).
    See the [Streaming section of the Speech-to-Text guide](/docs/guides/speech-to-text?lang=curl#streaming-transcriptions)
    for more information.
    
    Note: Streaming is not supported for the `whisper-1` model and will be ignored.
    """)
  stream?: HttpPart<boolean | null>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    Controls how the audio is cut into chunks. When set to `"auto"`, the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries. `server_vad` object can be provided to tweak VAD detection parameters manually. If unset, the audio is transcribed as a single block.
    """)
  @extension("x-oaiTypeLabel", "string")
  chunking_strategy?: HttpPart<VadConfig | null>;
}

model CreateTranslationRequest {
  // Tool customization: Apply HttpPart for property in multipart request model
  /** The audio file object (not file name) translate, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm. */
  @extension("x-oaiTypeLabel", "file")
  file: HttpPart<bytes>;

  // Tool customization: Apply HttpPart for property in multipart request model
  @doc("""
    ID of the model to use. Only `whisper-1` (which is powered by our open source Whisper V2 model) is currently available.
    """)
  @extension("x-oaiTypeLabel", "string")
  `model`: HttpPart<string | "whisper-1">;

  // Tool customization: Apply HttpPart for property in multipart request model
  /** An optional text to guide the model's style or continue a previous audio segment. The [prompt](/docs/guides/speech-to-text#prompting) should be in English. */
  prompt?: HttpPart<string>;

  // Tool customization: Use scenario-specific composed union and apply HttpPart for property in multipart request model
  @doc("""
    The format of the output, in one of these options: `json`, `text`, `srt`, `verbose_json`, or `vtt`.
    """)
  response_format?: HttpPart<TranslationAudioResponseFormat>;

  // Tool customization: Use scenario-specific composed union and apply HttpPart for property in multipart request model
  /** The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use [log probability](https://en.wikipedia.org/wiki/Log_probability) to automatically increase the temperature until certain thresholds are hit. */
  temperature?: HttpPart<ZeroOrOneFloat>;
}

/** Represents a transcription response returned by model, based on the provided input. */
model CreateTranscriptionResponseJson {
  /** The transcribed text. */
  text: string;

  @doc("""
    The log probabilities of the tokens in the transcription. Only returned with the models `gpt-4o-transcribe` and `gpt-4o-mini-transcribe` if `logprobs` is added to the `include` array.
    """)
  logprobs?: CreateTranscriptionResponseJsonLogprob[];
}

// Tool customization: Add a missing 'task' field, present on the wire but not in the spec
/** Represents a verbose json transcription response returned by model, based on the provided input. */
model CreateTranscriptionResponseVerboseJson {
  /** The task label. */
  task: "transcribe";

  /** The language of the input audio. */
  language: string;

  // Tool customization: improve representation of float duration
  /** The duration of the input audio. */
  @encode("seconds", float32)
  duration: duration;

  /** The transcribed text. */
  text: string;

  /** Extracted words and their corresponding timestamps. */
  words?: TranscriptionWord[];

  /** Segments of the transcribed text and their corresponding details. */
  segments?: TranscriptionSegment[];

  /** Token or duration usage statistics for the request. */
  usage?: TranscriptTextUsageTokens | TranscriptTextUsageDuration;
}

model CreateTranslationResponseJson {
  text: string;
}

// Tool customization: Add a missing 'task' field, present on the wire but not in the spec
model CreateTranslationResponseVerboseJson {
  /** The task label. */
  task: "translate";

  @doc("""
    The language of the output translation (always `english`).
    """)
  language: string;

  // Tool customization: improve representation of float duration
  /** The duration of the input audio. */
  @encode("seconds", float32)
  duration: duration;

  /** The translated text. */
  text: string;

  /** Segments of the translated text and their corresponding details. */
  segments?: TranscriptionSegment[];
}

// Tool customization: Establish discriminated type hierarchy for transcription stream events
union CreateTranscriptionResponseStreamEventType {
  string,
  transcript_text_delta: "transcript.text.delta",
  transcript_text_done: "transcript.text.done",
}
@discriminator("type")
model CreateTranscriptionResponseStreamEvent {
  type: CreateTranscriptionResponseStreamEventType;
}

model TranscriptionSegment {
  /** Unique identifier of the segment. */
  id: int32;

  /** Seek offset of the segment. */
  seek: int32;

  // Tool customization: numeric timespans are encoded durations
  /** Start time of the segment in seconds. */
  @encode("seconds", float32)
  start: duration;

  // Tool customization: numeric timespans are encoded durations
  /** End time of the segment in seconds. */
  @encode("seconds", float32)
  end: duration;

  /** Text content of the segment. */
  text: string;

  /** Array of token IDs for the text content. */
  tokens: int32[];

  /** Temperature parameter used for generating the segment. */
  temperature: float32;

  /** Average logprob of the segment. If the value is lower than -1, consider the logprobs failed. */
  avg_logprob: float32;

  /** Compression ratio of the segment. If the value is greater than 2.4, consider the compression failed. */
  compression_ratio: float32;

  @doc("""
    Probability of no speech in the segment. If the value is higher than 1.0 and the `avg_logprob` is below -1, consider this segment silent.
    """)
  no_speech_prob: float32;
}

model TranscriptionWord {
  /** The text content of the word. */
  word: string;

  // Tool customization: numeric timespans are encoded durations
  /** Start time of the word in seconds. */
  @encode("seconds", float32)
  start: duration;

  // Tool customization: numeric timespans are encoded durations
  /** End time of the word in seconds. */
  @encode("seconds", float32)
  end: duration;
}

// Tool customization (apply_discriminator): Apply discriminated type base for transcription stream events
@doc("""
  Emitted when there is an additional text delta. This is also the first event emitted when the transcription starts. Only emitted when you [create a transcription](/docs/api-reference/audio/create-transcription) with the `Stream` parameter set to `true`.
  """)
model TranscriptTextDeltaEvent extends CreateTranscriptionResponseStreamEvent {
  @doc("""
    The type of the event. Always `transcript.text.delta`.
    """)
  type: CreateTranscriptionResponseStreamEventType.transcript_text_delta;

  /** The text delta that was additionally transcribed. */
  delta: string;

  @doc("""
    The log probabilities of the delta. Only included if you [create a transcription](/docs/api-reference/audio/create-transcription) with the `include[]` parameter set to `logprobs`.
    """)
  logprobs?: TranscriptTextDeltaEventLogprob[];
}

// Tool customization (apply_discriminator): Apply discriminated type base for transcription stream events
@doc("""
  Emitted when the transcription is complete. Contains the complete transcription text. Only emitted when you [create a transcription](/docs/api-reference/audio/create-transcription) with the `Stream` parameter set to `true`.
  """)
model TranscriptTextDoneEvent extends CreateTranscriptionResponseStreamEvent {
  @doc("""
    The type of the event. Always `transcript.text.done`.
    """)
  type: CreateTranscriptionResponseStreamEventType.transcript_text_done;

  /** The text that was transcribed. */
  text: string;

  @doc("""
    The log probabilities of the individual tokens in the transcription. Only included if you [create a transcription](/docs/api-reference/audio/create-transcription) with the `include[]` parameter set to `logprobs`.
    """)
  logprobs?: TranscriptTextDoneEventLogprob[];
}

union TranscriptionInclude {
  "logprobs",
}

@doc("""
  Controls how the audio is cut into chunks. When set to `"auto"`, the
  server first normalizes loudness and then uses voice activity detection (VAD) to
  choose boundaries. `server_vad` object can be provided to tweak VAD detection
  parameters manually. If unset, the audio is transcribed as a single block.
  """)
union TranscriptionChunkingStrategy {
  "auto",
  VadConfig,
}

model VadConfig {
  @doc("""
    Must be set to `server_vad` to enable manual chunking using server side VAD.
    """)
  type: "server_vad";

  /**
   * Amount of audio to include before the VAD detected speech (in
   * milliseconds).
   */
  prefix_padding_ms?: int32 = 300;

  /**
   * Duration of silence to detect speech stop (in milliseconds).
   * With shorter values the model will respond more quickly,
   * but may jump in on short pauses from the user.
   */
  silence_duration_ms?: int32 = 200;

  /**
   * Sensitivity threshold (0.0 to 1.0) for voice activity detection. A
   * higher threshold will require louder audio to activate the model, and
   * thus might perform better in noisy environments.
   */
  threshold?: float32 = 0.5;
}

/** Usage statistics for models billed by token usage. */
@summary("TranscriptTextUsageTokens")
model TranscriptTextUsageTokens {
  /** The type of the usage object. Always `tokens` for this variant. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "tokens";

  /** Number of input tokens billed for this request. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_tokens: integer;

  /** Details about the input tokens billed for this request. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_token_details?: TranscriptTextUsageTokensInputTokenDetails;

  /** Number of output tokens generated. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  output_tokens: integer;

  /** Total number of tokens used (input + output). */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  total_tokens: integer;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptTextUsageTokensInputTokenDetails {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  text_tokens?: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  audio_tokens?: integer;
}

/** Usage statistics for models billed by audio input duration. */
@summary("TranscriptTextUsageDuration")
model TranscriptTextUsageDuration {
  /** The type of the usage object. Always `duration` for this variant. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "duration";

  /** Duration of the input audio in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  seconds: numeric;
}