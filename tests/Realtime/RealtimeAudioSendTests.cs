using NUnit.Framework;
using OpenAI.Realtime;
using System;
using System.Buffers;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.Realtime;

[Category("Smoke")]
public class RealtimeAudioSendTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);
    private static readonly bool UsesMemoryOverload = !typeof(RealtimeSessionClient).Assembly
        .GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName.StartsWith(".NETStandard", StringComparison.Ordinal);

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(16384)]
    [TestCase(98305)]
    public async Task StandaloneAudioSendsExactJsonForCallerMemorySlice(int length)
    {
        byte[] backing = CreateAudio(length + 17);
        BinaryData audio = BinaryData.FromBytes(backing.AsMemory(7, length));
        using RecordingWebSocket socket = new();
        using TestSession session = new(socket);
        using CancellationTokenSource cancellation = new();

        await session.SendInputAudioAsync(audio, cancellation.Token);

        Assert.That(socket.Frames, Has.Count.EqualTo(1));
        AssertAudioFrame(socket.Frames[0], backing.AsSpan(7, length), cancellation.Token);
    }

    [Test]
    public async Task StreamAudioSendsEveryShortReadWithoutStaleBytesOrExtraCommands()
    {
        byte[] audio = CreateAudio(16384 * 3 + 23);
        using ShortReadStream stream = new(audio, 16384, 3, 8001, 16384, 1, 20000);
        using RecordingWebSocket socket = new();
        using TestSession session = new(socket);
        using CancellationTokenSource cancellation = new();

        await session.SendInputAudioAsync(stream, cancellation.Token);

        Assert.That(socket.Frames.Count, Is.GreaterThan(2));
        int offset = 0;
        for (int i = 0; i < socket.Frames.Count; i++)
        {
            int count = stream.ReadLengths[i];
            AssertAudioFrame(socket.Frames[i], audio.AsSpan(offset, count), cancellation.Token);
            offset += count;
        }
        Assert.That(offset, Is.EqualTo(audio.Length));
        Assert.That(stream.ReadLengths[^1], Is.Zero);
        Assert.That(stream.WasDisposed, Is.False, "The caller owns the audio stream.");
    }

    [Test]
    public async Task EmptyStreamSendsNothingAndReleasesAudioStreamState()
    {
        using ShortReadStream stream = new(Array.Empty<byte>(), 16384);
        using RecordingWebSocket socket = new();
        using TestSession session = new(socket);

        await session.SendInputAudioAsync(stream);

        Assert.That(socket.Frames, Is.Empty);
        Assert.That(stream.ReadLengths, Is.EqualTo(new[] { 0 }));
        await AssertAudioCanBeSentAgain(session, socket);
    }

    [Test]
    public async Task RawCommandPreservesArrayIdentityOffsetLengthAndNotification()
    {
        const string json = "{\"type\":\"input_audio_buffer.append\",\"audio\":\"AAECAw==\"}";
        byte[] backing = Encoding.UTF8.GetBytes("prefix!" + json + "!suffix");
        BinaryData command = BinaryData.FromBytes(backing.AsMemory(7, Encoding.UTF8.GetByteCount(json)));
        RealtimeClient parent = new(new ApiKeyCredential("unused-test-key"));
        BinaryData notifiedData = null;
        object notifiedSender = null;
        int notifications = 0;
        parent.OnSendingCommand += (sender, data) =>
        {
            notifiedSender = sender;
            notifiedData = data;
            notifications++;
        };
        using RecordingWebSocket socket = new();
        using TestSession session = new(socket, parent);
        using CancellationTokenSource cancellation = new();

        await session.SendCommandAsync(command, new RequestOptions { CancellationToken = cancellation.Token });

        Frame frame = socket.Frames[0];
        Assert.That(MemoryMarshal.TryGetArray(frame.Memory, out ArraySegment<byte> segment), Is.True);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(segment.Array, Is.SameAs(backing));
            Assert.That(segment.Offset, Is.EqualTo(7));
            Assert.That(segment.Count, Is.EqualTo(command.ToMemory().Length));
            Assert.That(Encoding.UTF8.GetString(frame.WireBytes), Is.EqualTo(json));
            Assert.That(frame.MessageType, Is.EqualTo(WebSocketMessageType.Text));
            Assert.That(frame.EndOfMessage, Is.True);
            Assert.That(frame.CancellationToken, Is.EqualTo(cancellation.Token));
            Assert.That(frame.UsedMemoryOverload, Is.EqualTo(UsesMemoryOverload));
            Assert.That(notifiedData, Is.SameAs(command));
            Assert.That(notifiedSender, Is.SameAs(session));
            Assert.That(notifications, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task NonArrayBackedCommandUsesOriginalMemoryOrExactFallbackCopy()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("prefix{\"type\":\"input_audio_buffer.clear\"}suffix");
        using NonArrayMemoryManager manager = new(bytes);
        ReadOnlyMemory<byte> memory = manager.Memory.Slice(6, bytes.Length - 12);
        Assert.That(MemoryMarshal.TryGetArray(memory, out _), Is.False);
        BinaryData command = BinaryData.FromBytes(memory);
        using RecordingWebSocket socket = new();
        using TestSession session = new(socket);

        await session.SendCommandAsync(command, (RequestOptions)null);

        Frame frame = socket.Frames[0];
        Assert.That(frame.WireBytes, Is.EqualTo(memory.ToArray()));
        Assert.That(frame.MessageType, Is.EqualTo(WebSocketMessageType.Text));
        Assert.That(frame.EndOfMessage, Is.True);
        Assert.That(frame.UsedMemoryOverload, Is.EqualTo(UsesMemoryOverload));
        if (frame.UsedMemoryOverload)
        {
            Assert.That(frame.Memory.Equals(memory), Is.True, "Modern targets must retain the memory manager and slice.");
            Assert.That(MemoryMarshal.TryGetArray(frame.Memory, out _), Is.False);
        }
        else
        {
            Assert.That(MemoryMarshal.TryGetArray(frame.Memory, out ArraySegment<byte> segment), Is.True);
            Assert.That(segment.Array, Is.Not.SameAs(bytes));
            Assert.That(segment.Offset, Is.Zero);
            Assert.That(segment.Count, Is.EqualTo(memory.Length));
        }
    }

    [Test]
    public async Task BlockedStreamRetainsBufferAndRejectsConcurrentAudioUntilSendCompletes()
    {
        byte[] audio = CreateAudio(16384 * 3 + 11);
        using ShortReadStream stream = new(audio, 16384);
        using RecordingWebSocket socket = new();
        SendGate gate = socket.BlockNextSend();
        using TestSession session = new(socket);
        Task sending = session.SendInputAudioAsync(stream);
        await gate.Entered.Task.WaitAsync(TestTimeout);
        try
        {
            byte[] pendingWire = socket.Frames[0].Memory.ToArray();
            Assert.That(stream.ReadLengths, Is.EqualTo(new[] { 16384 }));
            Assert.That(sending.IsCompleted, Is.False);
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await session.SendInputAudioAsync(BinaryData.FromBytes(new byte[] { 9 })));
            using ShortReadStream otherStream = new(new byte[] { 8 }, 1);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await session.SendInputAudioAsync(otherStream));
            Assert.That(otherStream.ReadLengths, Is.Empty);
            Assert.That(socket.Frames[0].Memory.ToArray(), Is.EqualTo(pendingWire));
            Assert.That(stream.ReadLengths, Has.Count.EqualTo(1), "Do not overwrite/reuse the stream buffer while sending.");
        }
        finally
        {
            gate.Release.TrySetResult(true);
            await sending.WaitAsync(TestTimeout);
        }

        AssertAudioFrame(socket.Frames[0], audio.AsSpan(0, 16384), default);
        Assert.That(socket.Frames, Has.Count.EqualTo(4));
        await session.SendInputAudioAsync(BinaryData.FromBytes(new byte[] { 9 })).WaitAsync(TestTimeout);
        AssertAudioFrame(socket.Frames[^1], new byte[] { 9 }, default);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task CanceledSendReleasesProtocolSemaphoreAndAudioStreamState(bool streamAudio)
    {
        using RecordingWebSocket socket = new();
        SendGate gate = socket.BlockNextSend();
        using TestSession session = new(socket);
        using CancellationTokenSource cancellation = new();
        using ShortReadStream stream = new(CreateAudio(20000), 16384);
        Task sending = streamAudio
            ? session.SendInputAudioAsync(stream, cancellation.Token)
            : session.SendInputAudioAsync(BinaryData.FromBytes(new byte[] { 1, 2 }), cancellation.Token);
        await gate.Entered.Task.WaitAsync(TestTimeout);

        Assert.That(socket.Frames[0].CancellationToken, Is.EqualTo(cancellation.Token));
        cancellation.Cancel();
        await Assert.CatchAsync<OperationCanceledException>(async () => await sending.WaitAsync(TestTimeout));

        await AssertAudioCanBeSentAgain(session, socket);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task FailedSendReleasesProtocolSemaphoreAndAudioStreamState(bool streamAudio)
    {
        WebSocketException failure = new(WebSocketError.ConnectionClosedPrematurely);
        using RecordingWebSocket socket = new() { NextFailure = failure };
        using TestSession session = new(socket);
        using ShortReadStream stream = new(CreateAudio(20000), 16384);
        WebSocketException actual = await Assert.ThrowsAsync<WebSocketException>(async () =>
        {
            if (streamAudio)
            {
                await session.SendInputAudioAsync(stream).WaitAsync(TestTimeout);
            }
            else
            {
                await session.SendInputAudioAsync(BinaryData.FromBytes(new byte[] { 1 })).WaitAsync(TestTimeout);
            }
        });
        Assert.That(actual, Is.SameAs(failure));

        await AssertAudioCanBeSentAgain(session, socket);
    }

    [Test]
    public async Task CancellationWhileWaitingForSendSemaphoreDoesNotReleaseAnotherSend()
    {
        using RecordingWebSocket socket = new();
        SendGate gate = socket.BlockNextSend();
        using TestSession session = new(socket);
        BinaryData command = BinaryData.FromString("{\"type\":\"input_audio_buffer.clear\"}");
        Task first = session.SendCommandAsync(command, (RequestOptions)null);
        await gate.Entered.Task.WaitAsync(TestTimeout);
        using CancellationTokenSource cancellation = new();
        try
        {
            Task canceled = session.SendCommandAsync(command, new RequestOptions { CancellationToken = cancellation.Token });
            cancellation.Cancel();
            await Assert.CatchAsync<OperationCanceledException>(async () => await canceled.WaitAsync(TestTimeout));
            Task queued = session.SendCommandAsync(command, (RequestOptions)null);
            Assert.That(socket.Frames, Has.Count.EqualTo(1));
            Assert.That(queued.IsCompleted, Is.False);
            gate.Release.TrySetResult(true);
            await Task.WhenAll(first, queued).WaitAsync(TestTimeout);
            Assert.That(socket.Frames, Has.Count.EqualTo(2));
        }
        finally
        {
            gate.Release.TrySetResult(true);
            await first.WaitAsync(TestTimeout);
        }
    }

    [TestCase(true, 16384, TestName = "RepeatedStreamChunksAvoidFullCommandAllocation")]
    [TestCase(false, 98304, TestName = "RepeatedLargeStandaloneAudioSendsAvoidFullCommandAllocation")]
    public void AudioSendEliminatesAllocationProportionalToSerializedCommandSize(bool streamAudio, int audioSize)
    {
        const int iterations = 64;
        BinaryData audio = BinaryData.FromBytes(CreateAudio(audioSize));
        using MemoryStream stream = new(CreateAudio(16384 * iterations), writable: false);
        using RecordingWebSocket socket = new() { CaptureFrames = false };
        using TestSession session = new(socket);
        Action sendAudio = () =>
        {
            if (streamAudio)
            {
                stream.Position = 0;
                CompleteSynchronously(session.SendInputAudioAsync(stream));
            }
            else
            {
                for (int i = 0; i < iterations; i++)
                {
                    CompleteSynchronously(session.SendInputAudioAsync(audio));
                }
            }
        };
        for (int i = 0; i < 8; i++)
        {
            session.CopyCommands = false;
            sendAudio();
            session.CopyCommands = true;
            sendAudio();
        }

        session.CopyCommands = false;
        long commandCountBefore = session.SentCommandCount;
        long commandBytesBefore = session.SentCommandBytes;
        long before = GC.GetAllocatedBytesForCurrentThread();
        sendAudio();
        long withoutCopy = GC.GetAllocatedBytesForCurrentThread() - before;
        long commandBytes = session.SentCommandBytes - commandBytesBefore;
        Assert.That(session.SentCommandCount - commandCountBefore, Is.EqualTo(iterations));
        Assert.That(commandBytes, Is.EqualTo((long)Encoding.UTF8.GetByteCount(AudioJson(audio.ToMemory().Span)) * iterations));

        session.CopyCommands = true;
        commandCountBefore = session.SentCommandCount;
        commandBytesBefore = session.SentCommandBytes;
        before = GC.GetAllocatedBytesForCurrentThread();
        sendAudio();
        long oldPathBaseline = GC.GetAllocatedBytesForCurrentThread() - before;
        long saved = oldPathBaseline - withoutCopy;

        Assert.That(session.SentCommandCount - commandCountBefore, Is.EqualTo(iterations));
        Assert.That(session.SentCommandBytes - commandBytesBefore, Is.EqualTo(commandBytes));
        TestContext.WriteLine($"Stream={streamAudio}, audio/chunk={audioSize}, serialized bytes={commandBytes}, sends={iterations}, current={withoutCopy}, ToArray baseline={oldPathBaseline}, saved={saved}");
        Assert.That(saved, Is.GreaterThanOrEqualTo(commandBytes));
        Assert.That(saved, Is.LessThanOrEqualTo(commandBytes + 256 * iterations));
    }

    private static void CompleteSynchronously(Task sending)
    {
        if (!sending.IsCompleted)
        {
            throw new InvalidOperationException("Allocation measurements require synchronous completion on the measuring thread.");
        }
        sending.GetAwaiter().GetResult();
    }

    private static async Task AssertAudioCanBeSentAgain(TestSession session, RecordingWebSocket socket)
    {
        byte[] standalone = { 7, 8, 9 };
        await session.SendInputAudioAsync(BinaryData.FromBytes(standalone)).WaitAsync(TestTimeout);
        AssertAudioFrame(socket.Frames[^1], standalone, default);
        byte[] audio = { 10, 11, 12, 13 };
        using ShortReadStream stream = new(audio, 3);
        int previousCount = socket.Frames.Count;
        await session.SendInputAudioAsync(stream).WaitAsync(TestTimeout);
        Assert.That(socket.Frames.Count - previousCount, Is.EqualTo(2));
        AssertAudioFrame(socket.Frames[previousCount], audio.AsSpan(0, 3), default);
        AssertAudioFrame(socket.Frames[previousCount + 1], audio.AsSpan(3), default);
    }

    private static void AssertAudioFrame(Frame frame, ReadOnlySpan<byte> audio, CancellationToken cancellationToken)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Encoding.UTF8.GetString(frame.WireBytes), Is.EqualTo(AudioJson(audio)));
            Assert.That(frame.MessageType, Is.EqualTo(WebSocketMessageType.Text));
            Assert.That(frame.EndOfMessage, Is.True);
            Assert.That(frame.CancellationToken, Is.EqualTo(cancellationToken));
        }
    }

    private static string AudioJson(ReadOnlySpan<byte> audio)
        => "{\"type\":\"input_audio_buffer.append\",\"audio\":\"" + Convert.ToBase64String(audio) + "\"}";

    private static byte[] CreateAudio(int count)
    {
        byte[] bytes = new byte[count];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(i * 37 + 11);
        }
        return bytes;
    }

    private sealed class TestSession : RealtimeSessionClient
    {
        public bool CopyCommands { get; set; }
        public long SentCommandCount { get; private set; }
        public long SentCommandBytes { get; private set; }

        public TestSession(WebSocket socket, RealtimeClient parent = null)
            : base(null, new Uri("wss://localhost/realtime"), "test-model", null, parent)
        {
            WebSocket = socket;
        }

        public override Task SendCommandAsync(BinaryData data, RequestOptions options)
        {
            SentCommandCount++;
            SentCommandBytes += data.ToMemory().Length;
            // Reintroduce the old full-command copy without changing serialization or the remaining send path.
            return base.SendCommandAsync(CopyCommands ? BinaryData.FromBytes(data.ToArray()) : data, options);
        }
    }

    private sealed class Frame
    {
        public ReadOnlyMemory<byte> Memory { get; init; }
        public byte[] WireBytes { get; set; }
        public WebSocketMessageType MessageType { get; init; }
        public bool EndOfMessage { get; init; }
        public CancellationToken CancellationToken { get; init; }
        public bool UsedMemoryOverload { get; init; }
    }

    private sealed class SendGate
    {
        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed class RecordingWebSocket : WebSocket
    {
        private SendGate _nextGate;
        public List<Frame> Frames { get; } = new();
        public bool CaptureFrames { get; init; } = true;
        public Exception NextFailure { get; set; }
        public override WebSocketCloseStatus? CloseStatus => null;
        public override string CloseStatusDescription => null;
        public override WebSocketState State => WebSocketState.Open;
        public override string SubProtocol => null;

        public SendGate BlockNextSend() => _nextGate = new SendGate();

        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => SendCoreAsync(buffer.AsMemory(), messageType, endOfMessage, cancellationToken, usedMemoryOverload: false);

        public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken = default)
            => new(SendCoreAsync(buffer, messageType, endOfMessage, cancellationToken, usedMemoryOverload: true));

        private async Task SendCoreAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken, bool usedMemoryOverload)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!CaptureFrames)
            {
                return;
            }
            Frame frame = new()
            {
                Memory = buffer,
                MessageType = messageType,
                EndOfMessage = endOfMessage,
                CancellationToken = cancellationToken,
                UsedMemoryOverload = usedMemoryOverload,
            };
            Frames.Add(frame);
            SendGate gate = _nextGate;
            _nextGate = null;
            if (gate is not null)
            {
                gate.Entered.TrySetResult(true);
                await gate.Release.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            Exception failure = NextFailure;
            NextFailure = null;
            if (failure is not null)
            {
                throw failure;
            }
            // Consume only when the send completes, just like a transport borrowing caller memory.
            frame.WireBytes = buffer.ToArray();
        }

        public override void Abort() { }
        public override void Dispose() { }
        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => Task.CompletedTask;
        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
            => Task.CompletedTask;
        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
            => throw new NotSupportedException();
    }

    private sealed class NonArrayMemoryManager : MemoryManager<byte>
    {
        private readonly byte[] _bytes;
        public NonArrayMemoryManager(byte[] bytes) => _bytes = bytes;
        public override Span<byte> GetSpan() => _bytes;
        public override MemoryHandle Pin(int elementIndex = 0) => throw new NotSupportedException();
        public override void Unpin() { }
        protected override void Dispose(bool disposing) { }
    }

    private sealed class ShortReadStream : Stream
    {
        private readonly byte[] _audio;
        private readonly int[] _readSizes;
        private int _position;
        public List<int> ReadLengths { get; } = new();
        public bool WasDisposed { get; private set; }
        public ShortReadStream(byte[] audio, params int[] readSizes)
        {
            _audio = audio;
            _readSizes = readSizes;
        }
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _audio.Length;
        public override long Position { get => _position; set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int size = _readSizes[Math.Min(ReadLengths.Count, _readSizes.Length - 1)];
            int read = Math.Min(Math.Min(size, count), _audio.Length - _position);
            _audio.AsSpan(_position, read).CopyTo(buffer.AsSpan(offset, read));
            _position += read;
            ReadLengths.Add(read);
            return read;
        }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Read(buffer, offset, count));
        }
        protected override void Dispose(bool disposing)
        {
            WasDisposed = true;
            base.Dispose(disposing);
        }
        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
