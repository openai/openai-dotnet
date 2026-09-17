# Responses over WebSocket

`ResponsesClient.ConnectWebSocketAsync` opens an owned, reusable connection to the Responses API. It uses the client's endpoint, organization, project, authentication policy, and request policies. `OpenAIClient.GetResponsesClient()` carries the same configuration. Authentication is evaluated for each connection, including an explicit reconnect.

```csharp
ResponsesClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
ResponseWebSocketOptions options = new();
options.Headers["X-Application-Name"] = "responses-example";
await using ResponseWebSocketConnection connection = await client.ConnectWebSocketAsync(options);

var command = new ResponseWebSocketCreateCommand { Model = "gpt-5" };
command.InputItems.Add(ResponseItem.CreateUserMessageItem("Say hello."));
await connection.SendAsync(command);
ResponseResult response = await connection.ReceiveResponseAsync();
Console.WriteLine(response.GetOutputText());

var followup = new ResponseWebSocketCreateCommand
{
    Model = "gpt-5",
    PreviousResponseId = response.Id,
};
followup.InputItems.Add(ResponseItem.CreateUserMessageItem("Now say it in French."));
await connection.SendAsync(followup);
Console.WriteLine((await connection.ReceiveResponseAsync()).GetOutputText());
```

`ResponseWebSocketOptions.Headers` adds custom headers to the upgrade request and
treats header names case-insensitively. These headers pass through the client's
request and authentication policies, which may replace a header they own. The
same options work with a client returned by `OpenAIClient.GetResponsesClient()`.
Each connection snapshots its settings. An explicit reconnect reads the current
headers and settings from the original options again.

WebSocket upgrades do not use HTTP retries. A custom `ClientRetryPolicy` subclass
is rejected before authentication because its hooks cannot be preserved while
disabling retries. Add custom header logic through `AddPolicy` instead; these
request policies run for direct and factory clients.

These APIs are experimental (`OPENAI001`). A terminal completed, failed, or incomplete response ends one response, not its connection. `ReceiveResponseAsync` returns the complete terminal response snapshot with its status and all output items intact; it does not execute tools. A premature socket close throws rather than fabricating a completed response.

For individual events, use `ReceiveAsync` or `GetEventsAsync`. Each event exposes its complete original `RawData`, including unknown event types and fields. Normal response events also expose a typed `Update`. A nested protocol error is a `ResponseWebSocketErrorEvent`; receiving one does not close the socket. The final-response helper throws `ResponseWebSocketException` for that error and preserves the typed error in its `Error` property. Only one receive operation may be active on each event stream.

## Lanes and warmup

Reserve a lane before sending a command for its stream identifier. One reader routes every event to its lane, or to the connection's default stream if no lane is registered.

```csharp
using ResponseWebSocketLane lane = connection.OpenLane("answer");
await lane.SendAsync(new ResponseWebSocketCreateCommand
{
    Model = "gpt-5",
    Generate = false,
});
```

`Generate = false` sends a warmup command. Lane sends add `stream_id` to `response.create` without modifying the supplied command. An existing different stream identifier is rejected. Steering commands use `previous_response_id` to select the target response and its lane; lane sends preserve these commands without adding `stream_id`. `previous_response_id` controls continuation independently of lane routing. Canceling a receive wait leaves the socket and other lanes open and does not consume an event. Disposing a lane discards its buffered events and detaches it; later events for that identifier reach the default stream. Disposing the connection aborts all operations.

## Transport and limits

The default upgrade uses HTTP/1.1 and rejects redirects. Configure proxy, client certificates, or TLS settings through `ResponseWebSocketOptions.ConfigureTransport`. The default adapter is available on .NET 8 and .NET 10. When using the `netstandard2.0` assembly, it requires a runtime that provides `ClientWebSocket.ConnectAsync(Uri, HttpMessageInvoker, CancellationToken)`; older runtimes must supply an explicit `Connector`.

An opaque custom HTTP transport also requires `Connector`, since its settings cannot safely be inferred. The connector receives the final URI and authenticated headers and must return an open `WebSocket` that honors cancellation and abort. It must not forward credentials across origins. The connection owns that socket, but it does not dispose a supplied shared HTTP client or other shared transport resources. Clients constructed from an opaque prebuilt pipeline cannot open a connection; construct them using the authentication policy and client options instead.

Default limits are configurable per connection:

| Limit | Default |
| --- | --- |
| Incoming or outgoing UTF-8 message | No byte limit (`0`) |
| Buffered events across all lanes | 128 |
| Buffered UTF-8 event bytes across all lanes | No byte limit (`0`) |
| Concurrently admitted sends, including the active send | 16 |
| Registered lanes | 64 |
| Close handshake timeout | 2 seconds |

Message and buffered-byte limits are opt-in: set `MaxMessageBytes` or `MaxBufferedBytes` to a positive value to enable a byte limit. An enabled message limit is checked while assembling fragments. Event count and enabled byte limits bound stored wire payloads; decoded model objects have additional overhead. Overflow fails the connection explicitly. A send rejected by the admission limit was not sent. Cancellation while waiting for send admission leaves the socket open. A failed or canceled physical write aborts the connection because delivery may be uncertain. Successful send completion means the command was written locally, not acknowledged by the service.

## Explicit recovery

Recovery is opt-in. `ReconnectAsync` disposes the old connection, opens a fresh one, and calls the supplied restoration callback before returning it. `HasConnectionStateLoss` is true on the replacement. Connection-local service cache is lost, lane registrations are not copied, and no command is replayed automatically. Reconstruct any required application state in the callback. `maxAttempts` retries only opening the replacement; restoration is never retried automatically. Cancellation stops opening or cooperative restoration. If restoration fails, its replacement connection is disposed and the original restoration error is preserved.
