using System;
using System.Buffers;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
public partial class RealtimeConversationSession : IDisposable
{
    public WebSocket WebSocket { get; protected set; }

    private readonly RealtimeConversationClient _parentClient;
    private readonly Uri _endpoint;
    private readonly ApiKeyCredential _credential;
    private readonly object _sendingAudioLock = new();
    private bool _isSendingAudio = false;

    internal bool ShouldBufferTurnResponseData { get; set; }

    protected internal RealtimeConversationSession(
        RealtimeConversationClient parentClient,
        Uri endpoint,
        ApiKeyCredential credential)
    {
        Argument.AssertNotNull(endpoint, nameof(endpoint));
        Argument.AssertNotNull(credential, nameof(credential));

        _parentClient = parentClient;
        _endpoint = endpoint;
        _credential = credential;
    }

    /// <summary>
    /// Transmits audio data from a stream, ending the client turn once the stream is complete.
    /// </summary>
    /// <param name="audio"> The audio stream to transmit. </param>
    /// <param name="cancellationToken"> An optional cancellation token. </param>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task SendInputAudioAsync(Stream audio, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        lock (_sendingAudioLock)
        {
            if (_isSendingAudio)
            {
                throw new InvalidOperationException($"Only one stream of audio may be sent at once.");
            }
            _isSendingAudio = true;
        }
        try
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 16);
            while (true)
            {
                int bytesRead = await audio.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    break;
                }

                ReadOnlyMemory<byte> audioMemory = buffer.AsMemory(0, bytesRead);
                BinaryData audioData = BinaryData.FromBytes(audioMemory);
                InternalRealtimeClientEventInputAudioBufferAppend internalCommand = new(audioData);
                BinaryData requestData = ModelReaderWriter.Write(internalCommand);
                await SendCommandAsync(requestData, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
            }
        }
        finally
        {
            lock (_sendingAudioLock)
            {
                _isSendingAudio = false;
            }
        }
    }

    public virtual void SendInputAudio(Stream audio, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        lock (_sendingAudioLock)
        {
            if (_isSendingAudio)
            {
                throw new InvalidOperationException($"Only one stream of audio may be sent at once.");
            }
            _isSendingAudio = true;
        }
        try
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 16);
            while (true)
            {
                int bytesRead = audio.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                ReadOnlyMemory<byte> audioMemory = buffer.AsMemory(0, bytesRead);
                BinaryData audioData = BinaryData.FromBytes(audioMemory);
                InternalRealtimeClientEventInputAudioBufferAppend internalCommand = new(audioData);
                BinaryData requestData = ModelReaderWriter.Write(internalCommand);
                SendCommand(requestData, cancellationToken.ToRequestOptions());
            }
        }
        finally
        {
            lock (_sendingAudioLock)
            {
                _isSendingAudio = false;
            }
        }
    }

    /// <summary>
    /// Transmits a single chunk of audio.
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task SendInputAudioAsync(BinaryData audio, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        lock (_sendingAudioLock)
        {
            if (_isSendingAudio)
            {
                throw new InvalidOperationException($"Cannot send a standalone audio chunk while a stream is already in progress.");
            }
            _isSendingAudio = true;
        }
        // TODO: consider automatically limiting/breaking size of chunk (as with streaming)
        InternalRealtimeClientEventInputAudioBufferAppend internalCommand = new(audio);
        BinaryData requestData = ModelReaderWriter.Write(internalCommand);
        await SendCommandAsync(requestData, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary>
    /// Transmits a single chunk of audio.
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual void SendInputAudio(BinaryData audio, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        lock (_sendingAudioLock)
        {
            if (_isSendingAudio)
            {
                throw new InvalidOperationException($"Cannot send a standalone audio chunk while a stream is already in progress.");
            }
            _isSendingAudio = true;
        }
        // TODO: consider automatically limiting/breaking size of chunk (as with streaming)
        InternalRealtimeClientEventInputAudioBufferAppend internalCommand = new(audio);
        BinaryData requestData = ModelReaderWriter.Write(internalCommand);
        SendCommand(requestData, cancellationToken.ToRequestOptions());
    }

    public virtual async Task ClearInputAudioAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventInputAudioBufferClear internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void ClearInputAudio(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventInputAudioBufferClear internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task ConfigureSessionAsync(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        InternalRealtimeClientEventSessionUpdate internalCommand = new(sessionOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void ConfigureSession(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        InternalRealtimeClientEventSessionUpdate internalCommand = new(sessionOptions);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task AddItemAsync(ConversationItem item, CancellationToken cancellationToken = default)
        => await AddItemAsync(item, previousItemId: null, cancellationToken).ConfigureAwait(false);

    public virtual void AddItem(ConversationItem item, CancellationToken cancellationToken = default)
        => AddItem(item, previousItemId: null, cancellationToken);

    public virtual async Task AddItemAsync(ConversationItem item, string previousItemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(item, nameof(item));
        InternalRealtimeClientEventConversationItemCreate internalCommand = new(item)
        {
            PreviousItemId = previousItemId,
        };
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void AddItem(ConversationItem item, string previousItemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(item, nameof(item));
        InternalRealtimeClientEventConversationItemCreate internalCommand = new(item)
        {
            PreviousItemId = previousItemId,
        };
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task DeleteItemAsync(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        InternalRealtimeClientEventConversationItemDelete internalCommand = new(itemId);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void DeleteItem(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        InternalRealtimeClientEventConversationItemDelete internalCommand = new(itemId);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task TruncateItemAsync(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        InternalRealtimeClientEventConversationItemTruncate internalCommand = new(
            itemId: itemId,
            contentIndex: contentPartIndex,
            audioEndMs: (int)audioDuration.TotalMilliseconds);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void TruncateItem(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        InternalRealtimeClientEventConversationItemTruncate internalCommand = new(
            itemId: itemId,
            contentIndex: contentPartIndex,
            audioEndMs: (int)audioDuration.TotalMilliseconds);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task CommitPendingAudioAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventInputAudioBufferCommit internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void CommitPendingAudio(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventInputAudioBufferCommit internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task InterruptResponseAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCancel internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void InterruptResponse(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCancel internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task StartResponseAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCreateResponse internalOptions = new();
        InternalRealtimeClientEventResponseCreate internalCommand = new(internalOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void StartResponse(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCreateResponse internalOptions = new();
        InternalRealtimeClientEventResponseCreate internalCommand = new(internalOptions);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task StartResponseAsync(ConversationSessionOptions sessionOptionOverrides, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptionOverrides, nameof(sessionOptionOverrides));
        InternalRealtimeClientEventResponseCreateResponse internalOptions
            = InternalRealtimeClientEventResponseCreateResponse.FromSessionOptions(sessionOptionOverrides);
        InternalRealtimeClientEventResponseCreate internalCommand = new(internalOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void StartResponse(ConversationSessionOptions sessionOptionOverrides, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptionOverrides, nameof(sessionOptionOverrides));
        InternalRealtimeClientEventResponseCreateResponse internalOptions
            = InternalRealtimeClientEventResponseCreateResponse.FromSessionOptions(sessionOptionOverrides);
        InternalRealtimeClientEventResponseCreate internalCommand = new(internalOptions);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task CancelResponseAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCancel internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void CancelResponse(CancellationToken cancellationToken = default)
    {
        InternalRealtimeClientEventResponseCancel internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async IAsyncEnumerable<ConversationUpdate> ReceiveUpdatesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (ClientResult protocolEvent in ReceiveUpdatesAsync(cancellationToken.ToRequestOptions()))
        {
            ConversationUpdate nextUpdate = ConversationUpdate.FromResponse(protocolEvent.GetRawResponse());
            yield return nextUpdate;
        }
    }

    public virtual IEnumerable<ConversationUpdate> ReceiveUpdates(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    internal virtual async Task SendCommandAsync(InternalRealtimeClientEvent command, CancellationToken cancellationToken = default)
    {
        BinaryData requestData = ModelReaderWriter.Write(command);
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        await SendCommandAsync(requestData, cancellationOptions).ConfigureAwait(false);
    }

    internal virtual void SendCommand(InternalRealtimeClientEvent command, CancellationToken cancellationToken = default)
    {
        BinaryData requestData = ModelReaderWriter.Write(command);
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        SendCommand(requestData, cancellationOptions);
    }

    public void Dispose()
    {
        WebSocket?.Dispose();
    }
}