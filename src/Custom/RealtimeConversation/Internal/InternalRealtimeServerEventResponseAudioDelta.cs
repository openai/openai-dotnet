using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>response.audio.delta</c>, which is received after correlated
/// <see cref="ConversationItemStreamingStartedUpdate"/> (<c>response.output_item.added</c>),
/// <see cref="ConversationItemCreatedUpdate"/> (<c>conversation.item.created</c>), and
/// <see cref="InternalConversationContentPartStartedUpdate"/> (<c>response.content_part.added)</c> commands that initialize
/// an audio content part. This and other related delta events append audio data into the associated content part.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventResponseAudioDelta")]
internal partial class InternalRealtimeServerEventResponseAudioDelta
{ }
