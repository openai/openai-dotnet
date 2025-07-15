using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

/// <summary>
/// The update (response command) of type <c>response.done</c>, which is received when a model response turn has
/// completed and no further content part or item information will be transmitted.
/// </summary>
[CodeGenType("RealtimeServerEventResponseDone")]
public partial class ResponseFinishedUpdate
{
    [CodeGenMember("Response")]
    internal readonly InternalRealtimeResponse _internalResponse;

    public string ResponseId => _internalResponse?.Id;

    public ConversationStatus? Status => _internalResponse?.Status;

    public ConversationStatusDetails StatusDetails => _internalResponse.StatusDetails;

    [CodeGenMember("Output")]
    public IReadOnlyList<RealtimeItem> CreatedItems => _internalResponse?.Output ?? [];

    public ConversationTokenUsage Usage => _internalResponse.Usage;
}
