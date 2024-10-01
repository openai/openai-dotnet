using System;
using System.Buffers;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")]
public partial class RealtimeConversationSession : IDisposable
{
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
        _clientWebSocket = new ClientWebSocket();

        _credential.Deconstruct(out string dangerousCredential);
        _clientWebSocket.Options.SetRequestHeader("openai-beta", $"realtime=v1");
        _clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {dangerousCredential}");
    }

    /// <summary>
    /// Transmits audio data from a stream, ending the client turn once the stream is complete.
    /// </summary>
    /// <param name="audio"> The audio stream to transmit. </param>
    /// <param name="cancellationToken"> An optional cancellation token. </param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SendAudioAsync(Stream audio, CancellationToken cancellationToken = default)
    {
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
                InternalRealtimeRequestInputAudioBufferAppendCommand internalCommand = new(audioData);
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

    /// <summary>
    /// Transmits a single chunk of audio.
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SendAudioAsync(BinaryData audio, CancellationToken cancellationToken = default)
    {
        lock (_sendingAudioLock)
        {
            if (_isSendingAudio)
            {
                throw new InvalidOperationException($"Cannot send a standalone audio chunk while a stream is already in progress.");
            }
            _isSendingAudio = true;
        }
        // TODO: consider automatically limiting/breaking size of chunk (as with streaming)
        InternalRealtimeRequestInputAudioBufferAppendCommand internalCommand = new(audio);
        BinaryData requestData = ModelReaderWriter.Write(internalCommand);
        await SendCommandAsync(requestData, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    public async Task ConfigureSessionAsync(ConversationSessionOptions sessionOptions, CancellationToken cancellationToken = default)
    {
        InternalRealtimeRequestSessionUpdateCommand internalCommand = new(sessionOptions);
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public async Task AddItemAsync(ConversationItem item, CancellationToken cancellationToken = default)
        => await AddItemAsync(item, null, cancellationToken).ConfigureAwait(false);

    public async Task AddItemAsync(ConversationItem item, string previousItemId, CancellationToken cancellationToken = default)
    {
        InternalRealtimeRequestItemCreateCommand internalCommand = new(item)
        {
            PreviousItemId = previousItemId,
        };
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteItemAsync(string itemId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(itemId, nameof(itemId));
        await SendCommandAsync(new InternalRealtimeRequestItemDeleteCommand(itemId), cancellationToken).ConfigureAwait(false);
    }

    public async Task CommitPendingAudioAsync(CancellationToken cancellationToken = default)
    {
        await SendCommandAsync(new InternalRealtimeRequestInputAudioBufferCommitCommand(), cancellationToken).ConfigureAwait(false);
    }

    public async Task InterruptTurnAsync(CancellationToken cancellationToken = default)
    {
        await SendCommandAsync(new InternalRealtimeRequestResponseCancelCommand(), cancellationToken).ConfigureAwait(false);
    }

    public async Task StartResponseTurnAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeRequestResponseCreateCommand internalCommand = new()
        {
            Response = new(commit: true, cancelPrevious: true)
        };
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    public async Task CancelResponseTurnAsync(CancellationToken cancellationToken = default)
    {
        InternalRealtimeRequestResponseCancelCommand internalCommand = new();
        await SendCommandAsync(internalCommand, cancellationToken).ConfigureAwait(false);
    }

    internal virtual async Task SendCommandAsync(InternalRealtimeRequestCommand command, CancellationToken cancellationToken = default)
    {
        BinaryData requestData = ModelReaderWriter.Write(command);
        RequestOptions cancellationOptions = cancellationToken.ToRequestOptions();
        await SendCommandAsync(requestData, cancellationOptions).ConfigureAwait(false);
    }

    public async IAsyncEnumerable<ConversationUpdate> ReceiveUpdatesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (ClientResult protocolEvent in ReceiveUpdatesAsync(cancellationToken.ToRequestOptions()))
        {
            ConversationUpdate nextUpdate = ConversationUpdate.FromResponse(protocolEvent.GetRawResponse());
            yield return nextUpdate;
        }
    }

    public void Dispose()
    {
        _clientWebSocket?.Dispose();
    }
}