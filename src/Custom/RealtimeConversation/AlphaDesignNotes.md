# Development notes for .NET `/realtime` -- alpha

This document is intended to capture some of the exploratory design choices made when exposing the `/realtime` API in the .NET library.

## Naming and structure

"Realtime" does not describe "what" the capability does, but rather "how" it does it; `RealtimeClient`, while a faithful translation from REST, would not be descriptive or idiomatic. `AudioClient` has operations that let you send or receive audio; `ChatClient` is about all about getting chat (completion) responses; EmbeddingClient generates embeddings; `${NAME}Client` does *not* let you send or receive "realtimes."

A number of names could work. `Conversation` was chosen as an expedient placeholder.

Because the `/realtime` API involves simultaneously sending and receiving data on a single WebSocket, the primary logic vehicle is an `IDisposable` `ConversationSession` type -- this is configured by its originating `ConversationClient` and manages a `ClientWebSocket` instance. `ConversationClient` then provides task-based methods like `SendText` and `SubmitToolResponse` -- methods that allow the abstraction of client-originated request messages -- while exposing an `IAsyncEnumerable` collection of (response) `ConversationMessage` instances via `ReceiveMessagesAsync`.

The initial design approach for `ConversationMessage` feature uses a "squish" strategy; the many variant concrete message types are internalized, then composed into the single wrapper that conditionally populates appropriate properties based on the underlying message. This is a reapplication of the general principles applied to Chat Completion and Assistants streaming, though it's a larger single-type "squish" than previously pursued.

This is intended to facilitate a low barrier to entry, as explicit knowledge about different message types is not necessary to work with the operation. For example, a basic "hello world" may just do something like the following:

```csharp
using ConversationSession conversation = await client.StartConversationAsync();

await conversation.SendTextAsync("Hello, world!");

await foreach (ConversationMessage message in client.ReceiveMessagesAsync())
{
    Console.Write(message.Text);
}
```

## Turn-based data buffering

A repeated piece of early alpha feedback was that a client-integrated mechanism to automatically accumulate incoming response data (not requiring manual, do-it-yourself accumulation) would be valuable.

To explore accomplishing this, `ConversationSession` includes a pair of properties, `LastTurnFullResponseText` and `LastTurnFullResponseAudio`, that will automatically be populated with accumulated data when a `turn_finished` event is received. This is consistent with the "snapshot" mechanism used in several instances within Stainless SDK libraries, which likewise feature automatically accumulated data being populated into an appropriate location.

As this requires visibility into the response body, automatic accumulation is only performed when using the convenience method variant of `ReceiveMessagesAsync`.

Because this accumulated text and (especially) audio data can quickly grow in size to hundreds of kilobytes, a client-only property for `LastTurnResponseAccumulationEnabled` is inserted into `ConversationOptions`. In contexts with many parallel operations and high sensitive to memory footprint, the setting can thus opt out of the behavior.