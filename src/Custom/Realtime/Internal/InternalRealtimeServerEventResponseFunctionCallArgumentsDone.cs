using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>response.function_call_arguments.done</c>, which is received after all
/// associated <see cref="InternalConversationFunctionCallArgumentsDeltaUpdate"/> commands have finished incrementally
/// streaming the arguments to a conversation function call item. This update contains the complete body of the
/// function call arguments and is typically interpreted as a JSON document with parameterization matching the
/// associated function definition.
/// </summary>
[CodeGenType("RealtimeServerEventResponseFunctionCallArgumentsDone")]
internal partial class InternalRealtimeServerEventResponseFunctionCallArgumentsDone
{ }
