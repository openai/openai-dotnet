namespace OpenAI;

/// <summary>
/// The kind of line or field received over an SSE stream.
/// See SSE specification: https://html.spec.whatwg.org/multipage/server-sent-events.html
/// </summary>
internal enum ServerSentEventFieldKind
{
    Ignore,
    Event,
    Data,
    Id,
    Retry,
}
