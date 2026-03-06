/*
 * This file was automatically generated from an OpenAPI .yaml file.
 * Edits made directly to this file will be lost.
 */

import "../common";
import "./models_GA.tsp";

using Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

model RealtimeResponseCreateParamsAudio_GA {
  output?: RealtimeResponseCreateParamsAudioOutput_GA;
}

model RealtimeResponseCreateParamsAudioOutput_GA {
  format?: RealtimeAudioFormat_GA;
  voice?: VoiceIdsShared_GA;
}

model RealtimeResponseCreateParams_GA {
  /**The set of modalities the model used to respond, currently the only possible values are
  `[\"audio\"]`, `[\"text\"]`. Audio output always include a text transcript. Setting the
  output to mode `text` will disable audio output from the model.*/
  output_modalities?: RealtimeOutputModalities_GA[];

  /**The default system instructions (i.e. system message) prepended to model calls. This field allows the client to guide the model on desired responses. The model can be instructed on response content and format, (e.g. "be extremely succinct", "act friendly", "here are examples of good responses") and on audio behavior (e.g. "talk quickly", "inject emotion into your voice", "laugh frequently"). The instructions are not guaranteed to be followed by the model, but they provide guidance to the model on the desired behavior.
  Note that the server sets default instructions which will be used if this field is not set and are visible in the `session.created` event at the start of the session.*/
  instructions?: string;

  /** Configuration for audio input and output. */
  audio?: RealtimeResponseCreateParamsAudio_GA;

  /** Tools available to the model. */
  tools?: RealtimeToolBase_GA[];

  /**How the model chooses tools. Provide one of the string modes or force a specific
  function/MCP tool.*/
  tool_choice?: "none" | "auto" | "required" | ToolChoiceBase_GA = "auto";

  /**Maximum number of output tokens for a single assistant response,
  inclusive of tool calls. Provide an integer between 1 and 4096 to
  limit output tokens, or `inf` for the maximum available tokens for a
  given model. Defaults to `inf`.*/
  max_output_tokens?: integer | "inf";

  /**Controls which conversation the response is added to. Currently supports
  `auto` and `none`, with `auto` as the default value. The `auto` value
  means that the contents of the response will be added to the default
  conversation. Set this to `none` to create an out-of-band response which
  will not add items to default conversation.*/
  conversation?: string | "auto" | "none" = "auto";

  metadata?: Record<unknown> | null;

  // prompt?: Prompt;

  /**Input items to include in the prompt for the model. Using this field
  creates a new context for this Response instead of using the default
  conversation. An empty array `[]` will clear the context for this Response.
  Note that this can include references to items that previously appeared in the session
  using their id.*/
  input?: RealtimeConversationItem_GA[];
}

union RealtimeClientEventType_GA {
  string,
  conversation_item_create: "conversation.item.create",
  conversation_item_delete: "conversation.item.delete",
  conversation_item_retrieve: "conversation.item.retrieve",
  conversation_item_truncate: "conversation.item.truncate",
  input_audio_buffer_append: "input_audio_buffer.append",
  input_audio_buffer_clear: "input_audio_buffer.clear",
  output_audio_buffer_clear: "output_audio_buffer.clear",
  input_audio_buffer_commit: "input_audio_buffer.commit",
  response_cancel: "response.cancel",
  response_create: "response.create",
  session_update: "session.update",
}

/** A realtime client event. */
@discriminator("type")
model RealtimeClientEvent_GA {
  type: RealtimeClientEventType_GA;
}

// 01
/**Send this event to update the session’s configuration.
The client may send this event at any time to update any field
except for `voice` and `model`. `voice` can be updated only if there have been no other audio outputs yet.
When the server receives a `session.update`, it will respond
with a `session.updated` event showing the full, effective configuration.
Only the fields that are present in the `session.update` are updated. To clear a field like
`instructions`, pass an empty string. To clear a field like `tools`, pass an empty array.
To clear a field like `turn_detection`, pass `null`.*/
@extension(
  "x-oaiMeta",
  #{
    name: "session.update",
    group: "realtime",
    example: """
      {
        "type": "session.update",
        "session": {
          "type": "realtime",
          "instructions": "You are a creative assistant that helps with design tasks.",
          "tools": [
            {
              "type": "function",
              "name": "display_color_palette",
              "description": "Call this function when a user asks for a color palette.",
              "parameters": {
                "type": "object",
                "properties": {
                  "theme": {
                    "type": "string",
                    "description": "Description of the theme for the color scheme."
                  },
                  "colors": {
                    "type": "array",
                    "description": "Array of five hex color codes based on the theme.",
                    "items": {
                      "type": "string",
                      "description": "Hex color code"
                    }
                  }
                },
                "required": [
                  "theme",
                  "colors"
                ]
              }
            }
          ],
          "tool_choice": "auto"
        }
      }
      
      """,
  }
)
model RealtimeClientEventSessionUpdate_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. This is an arbitrary string that a client may assign. It will be passed back if there is an error with the event, but the corresponding `session.updated` event will not include it. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `session.update`. */
  type: RealtimeClientEventType_GA.session_update;

  /**Update the Realtime session. Choose either a realtime
  session or a transcription session.*/
  session: RealtimeSessionCreateRequestGABase_GA;
}

// 02
/**Send this event to append audio bytes to the input audio buffer. The audio
buffer is temporary storage you can write to and later commit. A "commit" will create a new
user message item in the conversation history from the buffer content and clear the buffer.
Input audio transcription (if enabled) will be generated when the buffer is committed.
If VAD is enabled the audio buffer is used to detect speech and the server will decide
when to commit. When Server VAD is disabled, you must commit the audio buffer
manually. Input audio noise reduction operates on writes to the audio buffer.
The client may choose how much audio to place in each event up to a maximum
of 15 MiB, for example streaming smaller chunks from the client may allow the
VAD to be more responsive. Unlike most other client events, the server will
not send a confirmation response to this event.*/
@extension(
  "x-oaiMeta",
  #{
    name: "input_audio_buffer.append",
    group: "realtime",
    example: """
      {
          "event_id": "event_456",
          "type": "input_audio_buffer.append",
          "audio": "Base64EncodedAudioData"
      }
      
      """,
  }
)
model RealtimeClientEventInputAudioBufferAppend_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `input_audio_buffer.append`. */
  type: RealtimeClientEventType_GA.input_audio_buffer_append;

  /**Base64-encoded audio bytes. This must be in the format specified by the
  `input_audio_format` field in the session configuration.*/
  @encode("base64", string)
  audio: bytes;
}

// 03
/**Send this event to commit the user input audio buffer, which will create a  new user message item in the conversation. This event will produce an error  if the input audio buffer is empty. When in Server VAD mode, the client does  not need to send this event, the server will commit the audio buffer  automatically.
Committing the input audio buffer will trigger input audio transcription  (if enabled in session configuration), but it will not create a response  from the model. The server will respond with an `input_audio_buffer.committed` event.*/
@extension(
  "x-oaiMeta",
  #{
    name: "input_audio_buffer.commit",
    group: "realtime",
    example: """
      {
          "event_id": "event_789",
          "type": "input_audio_buffer.commit"
      }
      
      """,
  }
)
model RealtimeClientEventInputAudioBufferCommit_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `input_audio_buffer.commit`. */
  type: RealtimeClientEventType_GA.input_audio_buffer_commit;
}

// 04
/**Send this event to clear the audio bytes in the buffer. The server will
respond with an `input_audio_buffer.cleared` event.*/
@extension(
  "x-oaiMeta",
  #{
    name: "input_audio_buffer.clear",
    group: "realtime",
    example: """
      {
          "event_id": "event_012",
          "type": "input_audio_buffer.clear"
      }
      
      """,
  }
)
model RealtimeClientEventInputAudioBufferClear_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `input_audio_buffer.clear`. */
  type: RealtimeClientEventType_GA.input_audio_buffer_clear;
}

// 05
/**Add a new Item to the Conversation's context, including messages, function
calls, and function call responses. This event can be used both to populate a
"history" of the conversation and to add new items mid-stream, but has the
current limitation that it cannot populate assistant audio messages.
If successful, the server will respond with a `conversation.item.created`
event, otherwise an `error` event will be sent.*/
@extension(
  "x-oaiMeta",
  #{
    name: "conversation.item.create",
    group: "realtime",
    example: """
      {
        "type": "conversation.item.create",
        "item": {
          "type": "message",
          "role": "user",
          "content": [
            {
              "type": "input_text",
              "text": "hi"
            }
          ]
        },
        "event_id": "b904fba0-0ec4-40af-8bbb-f908a9b26793",
      }
      
      """,
  }
)
model RealtimeClientEventConversationItemCreate_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  event_id?: string;

  /** The event type, must be `conversation.item.create`. */
  type: RealtimeClientEventType_GA.conversation_item_create;

  /**The ID of the preceding item after which the new item will be inserted.
  If not set, the new item will be appended to the end of the conversation.
  If set to `root`, the new item will be added to the beginning of the conversation.
  If set to an existing ID, it allows an item to be inserted mid-conversation. If the
  ID cannot be found, an error will be returned and the item will not be added.*/
  previous_item_id?: string;

  item: RealtimeConversationItem_GA;
}

// 06
/**Send this event when you want to retrieve the server's representation of a specific item in the conversation history. This is useful, for example, to inspect user audio after noise cancellation and VAD.
The server will respond with a `conversation.item.retrieved` event,
unless the item does not exist in the conversation history, in which case the
server will respond with an error.*/
@extension(
  "x-oaiMeta",
  #{
    name: "conversation.item.retrieve",
    group: "realtime",
    example: """
      {
          "event_id": "event_901",
          "type": "conversation.item.retrieve",
          "item_id": "item_003"
      }
      
      """,
  }
)
model RealtimeClientEventConversationItemRetrieve_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `conversation.item.retrieve`. */
  type: RealtimeClientEventType_GA.conversation_item_retrieve;

  /** The ID of the item to retrieve. */
  item_id: string;
}

// 07
/**Send this event to truncate a previous assistant message’s audio. The server
will produce audio faster than realtime, so this event is useful when the user
interrupts to truncate audio that has already been sent to the client but not
yet played. This will synchronize the server's understanding of the audio with
the client's playback.
Truncating audio will delete the server-side text transcript to ensure there
is not text in the context that hasn't been heard by the user.
If successful, the server will respond with a `conversation.item.truncated`
event.*/
@extension(
  "x-oaiMeta",
  #{
    name: "conversation.item.truncate",
    group: "realtime",
    example: """
      {
          "event_id": "event_678",
          "type": "conversation.item.truncate",
          "item_id": "item_002",
          "content_index": 0,
          "audio_end_ms": 1500
      }
      
      """,
  }
)
model RealtimeClientEventConversationItemTruncate_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `conversation.item.truncate`. */
  type: RealtimeClientEventType_GA.conversation_item_truncate;

  /**The ID of the assistant message item to truncate. Only assistant message
  items can be truncated.*/
  item_id: string;

  /** The index of the content part to truncate. Set this to `0`. */
  content_index: integer;

  /**Inclusive duration up to which audio is truncated, in milliseconds. If
  the audio_end_ms is greater than the actual audio duration, the server
  will respond with an error.*/
  @encode(DurationKnownEncoding.milliseconds, integer)
  audio_end_ms: duration;
}

// 08
/**Send this event when you want to remove any item from the conversation
history. The server will respond with a `conversation.item.deleted` event,
unless the item does not exist in the conversation history, in which case the
server will respond with an error.*/
@extension(
  "x-oaiMeta",
  #{
    name: "conversation.item.delete",
    group: "realtime",
    example: """
      {
          "event_id": "event_901",
          "type": "conversation.item.delete",
          "item_id": "item_003"
      }
      
      """,
  }
)
model RealtimeClientEventConversationItemDelete_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `conversation.item.delete`. */
  @extension("x-stainless-const", true)
  type: RealtimeClientEventType_GA.conversation_item_delete;

  /** The ID of the item to delete. */
  item_id: string;
}

// 09
/**This event instructs the server to create a Response, which means triggering
model inference. When in Server VAD mode, the server will create Responses
automatically.
A Response will include at least one Item, and may have two, in which case
the second will be a function call. These Items will be appended to the
conversation history by default.
The server will respond with a `response.created` event, events for Items
and content created, and finally a `response.done` event to indicate the
Response is complete.
The `response.create` event includes inference configuration like
`instructions` and `tools`. If these are set, they will override the Session's
configuration for this Response only.
Responses can be created out-of-band of the default Conversation, meaning that they can
have arbitrary input, and it's possible to disable writing the output to the Conversation.
Only one Response can write to the default Conversation at a time, but otherwise multiple
Responses can be created in parallel. The `metadata` field is a good way to disambiguate
multiple simultaneous Responses.
Clients can set `conversation` to `none` to create a Response that does not write to the default
Conversation. Arbitrary input can be provided with the `input` field, which is an array accepting
raw Items and references to existing Items.*/
@extension(
  "x-oaiMeta",
  #{
    name: "response.create",
    group: "realtime",
    example: """
      // Trigger a response with the default Conversation and no special parameters
      {
        "type": "response.create",
      }
      
      // Trigger an out-of-band response that does not write to the default Conversation
      {
        "type": "response.create",
        "response": {
          "instructions": "Provide a concise answer.",
          "tools": [], // clear any session tools
          "conversation": "none",
          "output_modalities": ["text"],
          "metadata": {
            "response_purpose": "summarization"
          },
          "input": [
            {
              "type": "item_reference",
              "id": "item_12345",
            },
            {
              "type": "message",
              "role": "user",
              "content": [
                {
                  "type": "input_text",
                  "text": "Summarize the above message in one sentence."
                }
              ]
            }
          ],
        }
      }
      
      """,
  }
)
model RealtimeClientEventResponseCreate_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `response.create`. */
  type: RealtimeClientEventType_GA.response_create;

  response?: RealtimeResponseCreateParams_GA;
}

// 10
/**Send this event to cancel an in-progress response. The server will respond
with a `response.done` event with a status of `response.status=cancelled`. If
there is no response to cancel, the server will respond with an error. It's safe
to call `response.cancel` even if no response is in progress, an error will be
returned the session will remain unaffected.*/
@extension(
  "x-oaiMeta",
  #{
    name: "response.cancel",
    group: "realtime",
    example: """
      {
          "type": "response.cancel"
          "response_id": "resp_12345",
      }
      
      """,
  }
)
model RealtimeClientEventResponseCancel_GA extends RealtimeClientEvent_GA {
  /** Optional client-generated ID used to identify this event. */
  @maxLength(512)
  event_id?: string;

  /** The event type, must be `response.cancel`. */
  type: RealtimeClientEventType_GA.response_cancel;

  /**A specific response ID to cancel - if not provided, will cancel an
  in-progress response in the default conversation.*/
  response_id?: string;
}

// 11
/****WebRTC/SIP Only:** Emit to cut off the current audio response. This will trigger the server to
stop generating audio and emit a `output_audio_buffer.cleared` event. This
event should be preceded by a `response.cancel` client event to stop the
generation of the current response.
[Learn more](https://platform.openai.com/docs/guides/realtime-conversations#client-and-server-events-for-audio-in-webrtc).*/
@extension(
  "x-oaiMeta",
  #{
    name: "output_audio_buffer.clear",
    group: "realtime",
    example: """
      {
          "event_id": "optional_client_event_id",
          "type": "output_audio_buffer.clear"
      }
      
      """,
  }
)
model RealtimeClientEventOutputAudioBufferClear_GA extends RealtimeClientEvent_GA {
  /** The unique ID of the client event used for error handling. */
  event_id?: string;

  /** The event type, must be `output_audio_buffer.clear`. */
  type: RealtimeClientEventType_GA.output_audio_buffer_clear;
}