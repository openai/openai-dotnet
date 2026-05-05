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

namespace OpenAI.Realtime;

[Experimental("OPENAI002")]
public partial class RealtimeSessionClient : IDisposable
{
    public WebSocket WebSocket { get; protected set; }

    private readonly ApiKeyCredential _keyCredential;
    private readonly Uri _endpoint;
    private readonly string _model;
    private readonly string _intent;
    private readonly RealtimeClient _parentClient;

    private readonly SemaphoreSlim _audioSendSemaphore = new(1, 1);
    private bool _isSendingAudioStream = false;

    internal bool ShouldBufferTurnResponseData { get; set; }

    protected internal RealtimeSessionClient(ApiKeyCredential credential, Uri endpoint, string model, string intent, RealtimeClient parentClient)
    {
        _keyCredential = credential;
        _endpoint = endpoint;
        _model = model;
        _intent = intent;
        _parentClient = parentClient;
    }

    public void Dispose()
    {
        WebSocket?.Dispose();
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

        using (await _audioSendSemaphore.AutoReleaseWaitAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_isSendingAudioStream)
            {
                throw new InvalidOperationException($"Only one stream of audio may be sent at once.");
            }
            _isSendingAudioStream = true;
        }

        byte[] buffer = null;

        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(1024 * 16);
            while (true)
            {
                int bytesRead = await audio.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
                if (bytesRead == 0)
                {
                    break;
                }

                ReadOnlyMemory<byte> audioMemory = buffer.AsMemory(0, bytesRead);
                BinaryData audioData = BinaryData.FromBytes(audioMemory);

                RealtimeClientCommandInputAudioBufferAppend internalCommand = new(audioData);
                await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            if (buffer is not null)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            using (await _audioSendSemaphore.AutoReleaseWaitAsync(CancellationToken.None).ConfigureAwait(false))
            {
                _isSendingAudioStream = false;
            }
        }
    }

    public virtual void SendInputAudio(Stream audio, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(audio, nameof(audio));
        using (_audioSendSemaphore.AutoReleaseWait(cancellationToken))
        {
            if (_isSendingAudioStream)
            {
                throw new InvalidOperationException($"Only one stream of audio may be sent at once.");
            }
            _isSendingAudioStream = true;
        }

        byte[] buffer = null;
        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(1024 * 16);
            while (true)
            {
                int bytesRead = audio.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                ReadOnlyMemory<byte> audioMemory = buffer.AsMemory(0, bytesRead);
                BinaryData audioData = BinaryData.FromBytes(audioMemory);

                RealtimeClientCommandInputAudioBufferAppend internalCommand = new(audioData);
                SendCommand(internalCommand, cancellationToken);
            }
        }
        finally
        {
            if (buffer is not null)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
            using (_audioSendSemaphore.AutoReleaseWait(CancellationToken.None))
            {
                _isSendingAudioStream = false;
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
        using (await _audioSendSemaphore.AutoReleaseWaitAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_isSendingAudioStream)
            {
                throw new InvalidOperationException($"Cannot send a standalone audio chunk while a stream is already in progress.");
            }
            // TODO: consider automatically limiting/breaking size of chunk (as with streaming)
            RealtimeClientCommandInputAudioBufferAppend internalCommand = new(audio);
            await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
        }
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
        using (_audioSendSemaphore.AutoReleaseWait(cancellationToken))
        {
            if (_isSendingAudioStream)
            {
                throw new InvalidOperationException($"Cannot send a standalone audio chunk while a stream is already in progress.");
            }
            // TODO: consider automatically limiting/breaking size of chunk (as with streaming)
            RealtimeClientCommandInputAudioBufferAppend internalCommand = new(audio);
            SendCommand(internalCommand, cancellationToken);
        }
    }

    public virtual async Task ClearInputAudioAsync(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandInputAudioBufferClear internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void ClearInputAudio(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandInputAudioBufferClear internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task ConfigureConversationSessionAsync(RealtimeConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        RealtimeClientCommandSessionUpdate internalCommand = new(sessionOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void ConfigureConversationSession(RealtimeConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        RealtimeClientCommandSessionUpdate internalCommand = new(sessionOptions);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task ConfigureTranscriptionSessionAsync(RealtimeTranscriptionSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        RealtimeClientCommandSessionUpdate internalCommand = new(sessionOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void ConfigureTranscriptionSession(RealtimeTranscriptionSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(sessionOptions, nameof(sessionOptions));
        RealtimeClientCommandSessionUpdate internalCommand = new(sessionOptions);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task AddItemAsync(RealtimeItem item, CancellationToken cancellationToken = default)
    {
        await AddItemAsync(item, previousItemId: null, cancellationToken).ConfigureAwait(false);
    }

    public virtual void AddItem(RealtimeItem item, CancellationToken cancellationToken = default)
    {
        AddItem(item, previousItemId: null, cancellationToken);
    }

    public virtual async Task AddItemAsync(RealtimeItem item, string previousItemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(item, nameof(item));
        RealtimeClientCommandConversationItemCreate internalCommand = new(item)
        {
            PreviousItemId = previousItemId,
        };
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void AddItem(RealtimeItem item, string previousItemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(item, nameof(item));
        RealtimeClientCommandConversationItemCreate internalCommand = new(item)
        {
            PreviousItemId = previousItemId,
        };
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task RequestItemRetrievalAsync(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemRetrieve internalCommand = new(itemId);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void RequestItemRetrieval(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemRetrieve internalCommand = new(itemId);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task DeleteItemAsync(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemDelete internalCommand = new(itemId);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void DeleteItem(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemDelete internalCommand = new(itemId);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task TruncateItemAsync(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemTruncate internalCommand = new(
            itemId: itemId,
            contentIndex: contentPartIndex,
            audioEndTime: audioDuration);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void TruncateItem(string itemId, int contentPartIndex, TimeSpan audioDuration, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        RealtimeClientCommandConversationItemTruncate internalCommand = new(
            itemId: itemId,
            contentIndex: contentPartIndex,
            audioEndTime: audioDuration);
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task CommitPendingAudioAsync(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandInputAudioBufferCommit internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void CommitPendingAudio(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandInputAudioBufferCommit internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task StartResponseAsync(CancellationToken cancellationToken = default)
    {
        await StartResponseAsync(new RealtimeResponseOptions(), cancellationToken).ConfigureAwait(false);
    }

    public virtual void StartResponse(CancellationToken cancellationToken = default)
    {
        StartResponse(new RealtimeResponseOptions(), cancellationToken);
    }

    public virtual async Task StartResponseAsync(RealtimeResponseOptions responseOptions, CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandResponseCreate internalCommand = new()
        {
            ResponseOptions = responseOptions,
        };
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void StartResponse(RealtimeResponseOptions responseOptions, CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandResponseCreate internalCommand = new()
        {
            ResponseOptions = responseOptions,
        };
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async Task CancelResponseAsync(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandResponseCancel internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public virtual void CancelResponse(CancellationToken cancellationToken = default)
    {
        RealtimeClientCommandResponseCancel internalCommand = new();
        SendCommand(internalCommand, cancellationToken);
    }

    public virtual async IAsyncEnumerable<RealtimeServerUpdate> ReceiveUpdatesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (ClientResult protocolEvent in ReceiveUpdatesAsync(cancellationToken.ToRequestOptions()))
        {
            using PipelineResponse response = protocolEvent.GetRawResponse();
            RealtimeServerUpdate nextUpdate = ModelReaderWriter.Read<RealtimeServerUpdate>(response.Content, ModelReaderWriterOptions.Json, OpenAIContext.Default);
            // Skip null updates (e.g., conversation.item.done events that are intentionally ignored)
            if (nextUpdate is not null)
            {
                yield return nextUpdate;
            }
        }
    }

    public virtual IEnumerable<RealtimeServerUpdate> ReceiveUpdates(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public virtual async Task SendCommandAsync(RealtimeClientCommand command, CancellationToken cancellationToken = default)
    {
        BinaryData requestData = ModelReaderWriter.Write(command, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        await SendCommandAsync(requestData, cancellationOptions).ConfigureAwait(false);
    }

    public virtual void SendCommand(RealtimeClientCommand command, CancellationToken cancellationToken = default)
    {
        BinaryData requestData = ModelReaderWriter.Write(command, ModelReaderWriterOptions.Json, OpenAIContext.Default);
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        SendCommand(requestData, cancellationOptions);
    }
}