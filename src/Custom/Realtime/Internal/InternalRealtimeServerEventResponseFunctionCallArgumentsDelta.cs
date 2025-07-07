using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>response.function_call_arguments.delta</c>, which is received after correlated
/// <see cref="OutputStreamingStartedUpdate"/> (<c>response.output_item.added</c>) and
/// <see cref="ItemCreatedUpdate"/> (<c>conversation.item.created</c>) commands that initialize
/// a conversation function call item. This and other related delta events append function arguments to the item as
/// incremental JSON.
/// </summary>
/// <remarks>
/// Each <c>delta</c> payload only contains a small, incremental portion of the overall function call argument payload
/// and is not a valid JSON document on its own. For the complete JSON arguments, refer to
/// <see cref="InternalConversationFunctionCallArgumentsFinishedUpdate"/> commands or
/// <see cref="OutputStreamingFinishedUpdate.FunctionCallArguments"/>. Using this incremental JSON deltas requires the
/// use of a compatible, incremental parser.
/// </remarks>
[Experimental("OPENAI002")]
[CodeGenType("RealtimeServerEventResponseFunctionCallArgumentsDelta")]
internal partial class InternalRealtimeServerEventResponseFunctionCallArgumentsDelta
{ }
