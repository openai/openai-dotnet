import "@typespec/openapi";
import "../common/models.tsp";
import "@typespec/http";
import "@typespec/events";
import "@typespec/sse";
using TypeSpec.OpenAPI;
using TypeSpec.Http;
using TypeSpec.Events;
using TypeSpec.SSE;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateSpeechRequest {
  /** One of the available [TTS models](https://platform.openai.com/docs/models#tts): `tts-1`, `tts-1-hd`, `gpt-4o-mini-tts`, or `gpt-4o-mini-tts-2025-12-15`. */
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiTypeLabel", "string")
  `model`:
    | string
    | "tts-1"
    | "tts-1-hd"
    | "gpt-4o-mini-tts"
    | "gpt-4o-mini-tts-2025-12-15";

  /** The text to generate audio for. The maximum length is 4096 characters. */
  @maxLength(4096)
  input: string;

  /** Control the voice of your generated audio with additional instructions. Does not work with `tts-1` or `tts-1-hd`. */
  @maxLength(4096)
  instructions?: string;

  /** The voice to use when generating the audio. Supported built-in voices are `alloy`, `ash`, `ballad`, `coral`, `echo`, `fable`, `onyx`, `nova`, `sage`, `shimmer`, `verse`, `marin`, and `cedar`. Previews of the voices are available in the [Text to speech guide](https://platform.openai.com/docs/guides/text-to-speech#voice-options). */
  voice: VoiceIdsShared;

  /** The format to audio in. Supported formats are `mp3`, `opus`, `aac`, `flac`, `wav`, and `pcm`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  response_format?: "mp3" | "opus" | "aac" | "flac" | "wav" | "pcm" = "mp3";

  /** The speed of the generated audio. Select a value from `0.25` to `4.0`. `1.0` is the default. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @maxValue(4)
  @minValue(0.25)
  speed?: numeric = 1;

  /** The format to stream the audio in. Supported formats are `sse` and `audio`. `sse` is not supported for `tts-1` or `tts-1-hd`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  stream_format?: "sse" | "audio" = "audio";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateTranscriptionRequest {
  /** The audio file object (not file name) to transcribe, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiMeta", #{ exampleFilePath: "speech.mp3" })
  @extension("x-oaiTypeLabel", "file")
  file: HttpPart<bytes>;

  /** ID of the model to use. The options are `gpt-4o-transcribe`, `gpt-4o-mini-transcribe`, `gpt-4o-mini-transcribe-2025-12-15`, `whisper-1` (which is powered by our open source Whisper V2 model), and `gpt-4o-transcribe-diarize`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiTypeLabel", "string")
  `model`: HttpPart<
    | string
    | "whisper-1"
    | "gpt-4o-transcribe"
    | "gpt-4o-mini-transcribe"
    | "gpt-4o-mini-transcribe-2025-12-15"
    | "gpt-4o-transcribe-diarize">;

  /** The language of the input audio. Supplying the input language in [ISO-639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) (e.g. `en`) format will improve accuracy and latency. */
  language?: HttpPart<string>;

  /** An optional text to guide the model's style or continue a previous audio segment. The [prompt](https://platform.openai.com/docs/guides/speech-to-text#prompting) should match the audio language. This field is not supported when using `gpt-4o-transcribe-diarize`. */
  prompt?: HttpPart<string>;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  response_format?: HttpPart<AudioResponseFormat>;

  /** The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use [log probability](https://en.wikipedia.org/wiki/Log_probability) to automatically increase the temperature until certain thresholds are hit. */
  temperature?: HttpPart<numeric>;

  /**Additional information to include in the transcription response.
  `logprobs` will return the log probabilities of the tokens in the
  response to understand the model's confidence in the transcription.
  `logprobs` only works with response_format set to `json` and only with
  the models `gpt-4o-transcribe`, `gpt-4o-mini-transcribe`, and `gpt-4o-mini-transcribe-2025-12-15`. This field is not supported when using `gpt-4o-transcribe-diarize`.*/
  include?: HttpPart<TranscriptionInclude[]>;

  /**The timestamp granularities to populate for this transcription. `response_format` must be set `verbose_json` to use timestamp granularities. Either or both of these options are supported: `word`, or `segment`. Note: There is no additional latency for segment timestamps, but generating word timestamps incurs additional latency.
  This option is not available for `gpt-4o-transcribe-diarize`.*/
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  timestamp_granularities?: HttpPart<("word" | "segment")[]>;

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  stream?: HttpPart<boolean | null>;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  chunking_strategy?: HttpPart<TranscriptionChunkingStrategy>;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  known_speaker_names?: HttpPart<KnownSpeakerNames>;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  known_speaker_references?: HttpPart<KnownSpeakerReferences>;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model VadConfig {
  /** Must be set to `server_vad` to enable manual chunking using server side VAD. */
  type: "server_vad";

  /**Amount of audio to include before the VAD detected speech (in
  milliseconds).*/
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  prefix_padding_ms?: integer = 300;

  /**Duration of silence to detect speech stop (in milliseconds).
  With shorter values the model will respond more quickly,
  but may jump in on short pauses from the user.*/
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  silence_duration_ms?: integer = 200;

  /**Sensitivity threshold (0.0 to 1.0) for voice activity detection. A
  higher threshold will require louder audio to activate the model, and
  thus might perform better in noisy environments.*/
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  threshold?: numeric = 0.5;
}

/** Optional list of speaker names that correspond to the audio samples provided in `known_speaker_references[]`. Each entry should be a short identifier (for example `customer` or `agent`). Up to 4 speakers are supported. */
@maxItems(4)
model KnownSpeakerNames is string[];

/** Optional list of audio samples (as [data URLs](https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/Data_URLs)) that contain known speaker references matching `known_speaker_names[]`. Each sample must be between 2 and 10 seconds, and can use any of the same input audio formats supported by `file`. */
@maxItems(4)
model KnownSpeakerReferences is string[];

/** Represents a diarized transcription response returned by the model, including the combined transcript and speaker-segment annotations. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "The transcription object (Diarized JSON)",
    group: "audio",
    example: """
      {
        "task": "transcribe",
        "duration": 42.7,
        "text": "Agent: Thanks for calling OpenAI support.
      Customer: Hi, I need help with diarization.",
        "segments": [
          {
            "type": "transcript.text.segment",
            "id": "seg_001",
            "start": 0.0,
            "end": 5.2,
            "text": "Thanks for calling OpenAI support.",
            "speaker": "agent"
          },
          {
            "type": "transcript.text.segment",
            "id": "seg_002",
            "start": 5.2,
            "end": 12.8,
            "text": "Hi, I need help with diarization.",
            "speaker": "A"
          }
        ],
        "usage": {
          "type": "duration",
          "seconds": 43
        }
      }
      
      """,
  }
)
model CreateTranscriptionResponseDiarizedJson {
  /** The type of task that was run. Always `transcribe`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  task: "transcribe";

  /** Duration of the input audio in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  duration: duration;

  /** The concatenated transcript text for the entire audio input. */
  text: string;

  /** Segments of the transcript annotated with timestamps and speaker labels. */
  segments: TranscriptionDiarizedSegment[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  usage?: CreateTranscriptionResponseJsonUsage;
}

/** A segment of diarized transcript text with speaker metadata. */
model TranscriptionDiarizedSegment {
  /** The type of the segment. Always `transcript.text.segment`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "transcript.text.segment";

  /** Unique identifier for the segment. */
  id: string;

  /** Start timestamp of the segment in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  start: duration;

  /** End timestamp of the segment in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  end: duration;

  /** Transcript text for this segment. */
  text: string;

  /** Speaker label for this segment. When known speakers are provided, the label matches `known_speaker_names[]`. Otherwise speakers are labeled sequentially using capital letters (`A`, `B`, ...). */
  speaker: string;
}

/** Token usage statistics for the request. */
@discriminator("type")
model CreateTranscriptionResponseJsonUsage {
  type: CreateTranscriptionResponseJsonUsageType;
}

/** Usage statistics for models billed by token usage. */
@summary("TranscriptTextUsageTokens")
model TranscriptTextUsageTokens extends CreateTranscriptionResponseJsonUsage {
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
model TranscriptTextUsageDuration extends CreateTranscriptionResponseJsonUsage {
  /** The type of the usage object. Always `duration` for this variant. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "duration";

  /** Duration of the input audio in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  seconds: duration;
}

/** Represents a transcription response returned by model, based on the provided input. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "The transcription object (JSON)",
    group: "audio",
    example: """
      {
        "text": "Imagine the wildest idea that you've ever had, and you're curious about how it might scale to something that's a 100, a 1,000 times bigger. This is a place where you can get to do that.",
        "usage": {
          "type": "tokens",
          "input_tokens": 14,
          "input_token_details": {
            "text_tokens": 10,
            "audio_tokens": 4
          },
          "output_tokens": 101,
          "total_tokens": 115
        }
      }
      
      """,
  }
)
model CreateTranscriptionResponseJson {
  /** The transcribed text. */
  text: string;

  /** The log probabilities of the tokens in the transcription. Only returned with the models `gpt-4o-transcribe` and `gpt-4o-mini-transcribe` if `logprobs` is added to the `include` array. */
  logprobs?: CreateTranscriptionResponseJsonLogprobs[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  usage?: CreateTranscriptionResponseJsonUsage;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateTranscriptionResponseJsonLogprobs {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  token?: string;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  logprob?: numeric;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  bytes?: numeric[];
}

/** Represents a verbose json transcription response returned by model, based on the provided input. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "The transcription object (Verbose JSON)",
    group: "audio",
    example: """
      {
        "task": "transcribe",
        "language": "english",
        "duration": 8.470000267028809,
        "text": "The beach was a popular spot on a hot summer day. People were swimming in the ocean, building sandcastles, and playing beach volleyball.",
        "segments": [
          {
            "id": 0,
            "seek": 0,
            "start": 0.0,
            "end": 3.319999933242798,
            "text": " The beach was a popular spot on a hot summer day.",
            "tokens": [
              50364, 440, 7534, 390, 257, 3743, 4008, 322, 257, 2368, 4266, 786, 13, 50530
            ],
            "temperature": 0.0,
            "avg_logprob": -0.2860786020755768,
            "compression_ratio": 1.2363636493682861,
            "no_speech_prob": 0.00985979475080967
          },
          ...
        ],
        "usage": {
          "type": "duration",
          "seconds": 9
        }
      }
      
      """,
  }
)
model CreateTranscriptionResponseVerboseJson {
  /** The language of the input audio. */
  language: string;

  /** The duration of the input audio. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  duration: duration;

  /** The transcribed text. */
  text: string;

  /** Extracted words and their corresponding timestamps. */
  words?: TranscriptionWord[];

  /** Segments of the transcribed text and their corresponding details. */
  segments?: TranscriptionSegment[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  usage?: TranscriptTextUsageDuration;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptionWord {
  /** The text content of the word. */
  word: string;

  /** Start time of the word in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  start: duration;

  /** End time of the word in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  end: duration;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptionSegment {
  /** Unique identifier of the segment. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  id: integer;

  /** Seek offset of the segment. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  seek: integer;

  /** Start time of the segment in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  start: duration;

  /** End time of the segment in seconds. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  end: duration;

  /** Text content of the segment. */
  text: string;

  /** Array of token IDs for the text content. */
  tokens: integer[];

  /** Temperature parameter used for generating the segment. */
  temperature: float32;

  /** Average logprob of the segment. If the value is lower than -1, consider the logprobs failed. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  avg_logprob: float32;

  /** Compression ratio of the segment. If the value is greater than 2.4, consider the compression failed. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  compression_ratio: float32;

  /** Probability of no speech in the segment. If the value is higher than 1.0 and the `avg_logprob` is below -1, consider this segment silent. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  no_speech_prob: float32;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateTranslationRequest {
  /** The audio file object (not file name) translate, in one of these formats: flac, mp3, mp4, mpeg, mpga, m4a, ogg, wav, or webm. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiMeta", #{ exampleFilePath: "speech.mp3" })
  @extension("x-oaiTypeLabel", "file")
  file: HttpPart<bytes>;

  /** ID of the model to use. Only `whisper-1` (which is powered by our open source Whisper V2 model) is currently available. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiTypeLabel", "string")
  `model`: HttpPart<string | "whisper-1">;

  /** An optional text to guide the model's style or continue a previous audio segment. The [prompt](https://platform.openai.com/docs/guides/speech-to-text#prompting) should be in English. */
  prompt?: HttpPart<string>;

  /** The format of the output, in one of these options: `json`, `text`, `srt`, `verbose_json`, or `vtt`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  response_format?: HttpPart<"json" | "text" | "srt" | "verbose_json" | "vtt">;

  /** The sampling temperature, between 0 and 1. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic. If set to 0, the model will use [log probability](https://en.wikipedia.org/wiki/Log_probability) to automatically increase the temperature until certain thresholds are hit. */
  temperature?: HttpPart<numeric>;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateTranslationResponseJson {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  text: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateTranslationResponseVerboseJson {
  /** The language of the output translation (always `english`). */
  language: string;

  /** The duration of the input audio. */
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  duration: duration;

  /** The translated text. */
  text: string;

  /** Segments of the translated text and their corresponding details. */
  segments?: TranscriptionSegment[];
}

/** Emitted for each chunk of audio data generated during speech synthesis. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "Stream Event (speech.audio.delta)",
    group: "speech",
    example: """
      {
        "type": "speech.audio.delta",
        "audio": "base64-encoded-audio-data"
      }
      
      """,
  }
)
model SpeechAudioDeltaEvent {
  /** The type of the event. Always `speech.audio.delta`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "speech.audio.delta";

  /** A chunk of Base64-encoded audio data. */
  @encode("base64", string)
  audio: bytes;
}

/** Emitted when the speech synthesis is complete and all audio has been streamed. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "Stream Event (speech.audio.done)",
    group: "speech",
    example: """
      {
        "type": "speech.audio.done",
        "usage": {
          "input_tokens": 14,
          "output_tokens": 101,
          "total_tokens": 115
        }
      }
      
      """,
  }
)
model SpeechAudioDoneEvent {
  /** The type of the event. Always `speech.audio.done`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "speech.audio.done";

  /** Token usage statistics for the request. */
  usage: SpeechAudioDoneEventUsage;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model SpeechAudioDoneEventUsage {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  input_tokens: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  output_tokens: integer;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  total_tokens: integer;
}

/** Emitted when there is an additional text delta. This is also the first event emitted when the transcription starts. Only emitted when you [create a transcription](https://platform.openai.com/docs/api-reference/audio/create-transcription) with the `Stream` parameter set to `true`. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "Stream Event (transcript.text.delta)",
    group: "transcript",
    example: """
      {
        "type": "transcript.text.delta",
        "delta": " wonderful"
      }
      
      """,
  }
)
model TranscriptTextDeltaEvent {
  /** The type of the event. Always `transcript.text.delta`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "transcript.text.delta";

  /** The text delta that was additionally transcribed. */
  delta: string;

  /** The log probabilities of the delta. Only included if you [create a transcription](https://platform.openai.com/docs/api-reference/audio/create-transcription) with the `include[]` parameter set to `logprobs`. */
  logprobs?: TranscriptTextDeltaEventLogprobs[];

  /** Identifier of the diarized segment that this delta belongs to. Only present when using `gpt-4o-transcribe-diarize`. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  segment_id?: string;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptTextDeltaEventLogprobs {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  token?: string;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  logprob?: numeric;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  bytes?: integer[];
}

/** Emitted when the transcription is complete. Contains the complete transcription text. Only emitted when you [create a transcription](https://platform.openai.com/docs/api-reference/audio/create-transcription) with the `Stream` parameter set to `true`. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "Stream Event (transcript.text.done)",
    group: "transcript",
    example: """
      {
        "type": "transcript.text.done",
        "text": "I see skies of blue and clouds of white, the bright blessed days, the dark sacred nights, and I think to myself, what a wonderful world.",
        "usage": {
          "type": "tokens",
          "input_tokens": 14,
          "input_token_details": {
            "text_tokens": 10,
            "audio_tokens": 4
          },
          "output_tokens": 31,
          "total_tokens": 45
        }
      }
      
      """,
  }
)
model TranscriptTextDoneEvent {
  /** The type of the event. Always `transcript.text.done`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "transcript.text.done";

  /** The text that was transcribed. */
  text: string;

  /** The log probabilities of the individual tokens in the transcription. Only included if you [create a transcription](https://platform.openai.com/docs/api-reference/audio/create-transcription) with the `include[]` parameter set to `logprobs`. */
  logprobs?: TranscriptTextDoneEventLogprobs[];

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  usage?: TranscriptTextUsageTokens;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model TranscriptTextDoneEventLogprobs {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  token?: string;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  logprob?: numeric;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  bytes?: integer[];
}

/** Emitted when a diarized transcription returns a completed segment with speaker information. Only emitted when you [create a transcription](https://platform.openai.com/docs/api-reference/audio/create-transcription) with `stream` set to `true` and `response_format` set to `diarized_json`. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "Stream Event (transcript.text.segment)",
    group: "transcript",
    example: """
      {
        "type": "transcript.text.segment",
        "id": "seg_002",
        "start": 5.2,
        "end": 12.8,
        "text": "Hi, I need help with diarization.",
        "speaker": "A"
      }
      
      """,
  }
)
model TranscriptTextSegmentEvent {
  /** The type of the event. Always `transcript.text.segment`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "transcript.text.segment";

  /** Unique identifier for the segment. */
  id: string;

  /** Start timestamp of the segment in seconds. */
  @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  start: duration;

  /** End timestamp of the segment in seconds. */
   @encode("seconds", int64)
  @extension("x-ms-duration", "seconds")
  end: duration;

  /** Transcript text for this segment. */
  text: string;

  /** Speaker label for this segment. */
  speaker: string;
}

/** Controls how the audio is cut into chunks. When set to `"auto"`, the server first normalizes loudness and then uses voice activity detection (VAD) to choose boundaries. `server_vad` object can be provided to tweak VAD detection parameters manually. If unset, the audio is transcribed as a single block. Required when using `gpt-4o-transcribe-diarize` for inputs longer than 30 seconds. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension("x-oaiTypeLabel", "string")
union TranscriptionChunkingStrategy {
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  "auto" | VadConfig,
  null,
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
@events
union CreateSpeechStreamingResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  speech_audio_delta: SpeechAudioDeltaEvent,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-ms-sse-terminal-event", true)
  @terminalEvent
  speech_audio_done: SpeechAudioDoneEvent,
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
@events
union CreateTranscriptionStreamingResponse {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `transcript.text.segment`: TranscriptTextSegmentEvent,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  `transcript.text.delta`: TranscriptTextDeltaEvent,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-ms-sse-terminal-event", true)
  @terminalEvent
  `transcript.text.done`: TranscriptTextDoneEvent,
}

/** The format of the output, in one of these options: `json`, `text`, `srt`, `verbose_json`, `vtt`, or `diarized_json`. For `gpt-4o-transcribe` and `gpt-4o-mini-transcribe`, the only supported format is `json`. For `gpt-4o-transcribe-diarize`, the supported formats are `json`, `text`, and `diarized_json`, with `diarized_json` required to receive speaker annotations. */
#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
enum AudioResponseFormat {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  json,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  text,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  srt,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  verbose_json,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  vtt,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  diarized_json,
}

#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
enum TranscriptionInclude {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  logprobs,
}

union CreateTranscriptionResponseJsonUsageType {
  string,
  "tokens",
  "duration",
}
