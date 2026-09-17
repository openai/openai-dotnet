using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses;

/// <summary>Transport and resource limits for a Responses connection.</summary>
[Experimental("OPENAI001")]
public class ResponseWebSocketOptions
{
    /// <summary>Maximum UTF-8 bytes in an incoming or outgoing message. Zero (the default) means no byte limit.</summary>
    public int MaxMessageBytes { get; set; }
    /// <summary>Maximum total buffered events across all lanes. Defaults to <see cref="int.MaxValue"/>.</summary>
    public int MaxBufferedEvents { get; set; } = int.MaxValue;
    /// <summary>Maximum total UTF-8 bytes buffered across all lanes. Zero (the default) means no byte limit.</summary>
    public int MaxBufferedBytes { get; set; }
    /// <summary>Maximum concurrently admitted sends, including the active send. Defaults to 16.</summary>
    public int MaxPendingSends { get; set; } = 16;
    /// <summary>Maximum explicitly registered lanes. Defaults to 64.</summary>
    public int MaxLanes { get; set; } = 64;
    /// <summary>Maximum time for a close handshake. Defaults to two seconds.</summary>
    public TimeSpan CloseTimeout { get; set; } = TimeSpan.FromSeconds(2);
    /// <summary>Additional upgrade request headers.</summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    /// <summary>Configures proxy, TLS, or client certificates for the default HTTP/1.1 upgrade transport. Redirects remain disabled.</summary>
    public Action<HttpClientHandler> ConfigureTransport { get; set; }
    /// <summary>
    /// Optional connector for a custom HTTP transport. It receives the URI and headers after client policies
    /// and authentication have run. It must not forward credentials across origins. The returned open socket
    /// is owned by the connection; other transport resources remain owned by the caller.
    /// </summary>
    public Func<Uri, IReadOnlyDictionary<string, string>, CancellationToken, Task<WebSocket>> Connector { get; set; }

    internal ResponseWebSocketOptions Snapshot()
    {
        if (MaxMessageBytes < 0 || MaxBufferedEvents < 1 || MaxBufferedBytes < 0 || MaxPendingSends < 1 || MaxLanes < 1
            || CloseTimeout <= TimeSpan.Zero || CloseTimeout.TotalMilliseconds > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(ResponseWebSocketOptions), "Byte limits must not be negative, count limits must be positive, and the positive close timeout must fit in milliseconds.");
        }
        var copy = new ResponseWebSocketOptions
        {
            MaxMessageBytes = MaxMessageBytes,
            MaxBufferedEvents = MaxBufferedEvents,
            MaxBufferedBytes = MaxBufferedBytes,
            MaxPendingSends = MaxPendingSends,
            MaxLanes = MaxLanes,
            CloseTimeout = CloseTimeout,
            ConfigureTransport = ConfigureTransport,
            Connector = Connector,
        };
        foreach (var header in Headers) copy.Headers.Add(header);
        return copy;
    }
}
