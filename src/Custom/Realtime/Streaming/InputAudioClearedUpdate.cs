using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>input_audio_buffer.cleared</c>, which is received when a preceding
/// <c>input_audio_buffer.clear</c> command
/// (<see cref="RealtimeSession.ClearInputAudioAsync(System.Threading.CancellationToken)"/> has completed
/// purging the user audio input buffer.
/// </summary>
[CodeGenType("RealtimeServerEventInputAudioBufferCleared")]
public partial class InputAudioClearedUpdate
{ }
