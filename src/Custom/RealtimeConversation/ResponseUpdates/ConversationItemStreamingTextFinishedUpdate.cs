using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

/// <summary>
/// The update (response command) of type <c>response.text.done</c>, which is received after correlated
/// <see cref="ConversationItemStreamingStartedUpdate"/> (<c>response.output_item.added</c>),
/// <see cref="ConversationItemCreatedUpdate"/> (<c>conversation.item.created</c>),
/// <see cref="InternalConversationContentPartStartedUpdate"/> (<c>response.content_part.added)</c>, and
/// <see cref="InternalConversationTextContentDeltaUpdate"/> commands. This update indicates that all streamed <c>delta</c> content
/// has completed and the associated content part will soon be completed.
/// </summary>
[Experimental("OPENAI002")]
[CodeGenModel("RealtimeServerEventResponseTextDone")]
public partial class ConversationItemStreamingTextFinishedUpdate
{ }
