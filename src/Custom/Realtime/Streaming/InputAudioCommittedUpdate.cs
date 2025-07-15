using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>input_audio_buffer.committed</c>, which is received when a preceding
/// <c>input_audio_buffer.commit</c> command
/// (<see cref="RealtimeSession.CommitPendingAudioAsync(System.Threading.CancellationToken)"/> has
/// completed submission of the user audio input buffer.
/// </summary>
[CodeGenType("RealtimeServerEventInputAudioBufferCommitted")]
public partial class InputAudioCommittedUpdate
{ }
