/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "./custom.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

/** Emitted when there is a partial audio response. */
model ResponseAudioDeltaEvent {
  @doc("""
    The type of the event. Always `response.audio.delta`.
    """)
  type: "response.audio.delta";

  /** A chunk of Base64 encoded response audio bytes. */
  delta: string;
}

/** Emitted when the audio response is complete. */
model ResponseAudioDoneEvent {
  @doc("""
    The type of the event. Always `response.audio.done`.
    """)
  type: "response.audio.done";
}

/** Emitted when there is a partial transcript of audio. */
model ResponseAudioTranscriptDeltaEvent {
  @doc("""
    The type of the event. Always `response.audio.transcript.delta`.
    """)
  type: "response.audio.transcript.delta";

  /** The partial transcript of the audio response. */
  delta: string;
}

/** Emitted when the full audio transcript is completed. */
model ResponseAudioTranscriptDoneEvent {
  @doc("""
    The type of the event. Always `response.audio.transcript.done`.
    """)
  type: "response.audio.transcript.done";
}
