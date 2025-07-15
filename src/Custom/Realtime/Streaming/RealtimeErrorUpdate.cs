using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>error</c>, which is received when a problem is encountered while
/// processing a request command or generating another response command.
/// </summary>
[CodeGenType("RealtimeServerEventError")]
public partial class RealtimeErrorUpdate
{
    [CodeGenMember("Error")]
    private readonly InternalRealtimeServerEventErrorError _error;

    public string ErrorCode => _error?.Code;
    /// <summary>
    /// Gets the identifier that correlates to a prior event associated with this error.
    /// </summary>
    public string ErrorEventId => _error?.EventId;
    public string Message => _error?.Message;
    public string ParameterName => _error?.Param;
}