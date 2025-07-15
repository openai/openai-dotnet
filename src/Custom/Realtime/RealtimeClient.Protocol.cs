using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OpenAI.Realtime;

[CodeGenSuppress("StartRealtimeSessionAsync", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("StartRealtimeSession", typeof(BinaryContent), typeof(RequestOptions))]
[CodeGenSuppress("CreateStartRealtimeSessionRequest", typeof(BinaryContent), typeof(RequestOptions))]
public partial class RealtimeClient
{
    /// <summary>
    /// <para>[Protocol Method]</para>
    /// Creates a new realtime conversation operation instance, establishing a connection to the /realtime endpoint.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual async Task<RealtimeSession> StartConversationSessionAsync(string model, RequestOptions options)
    {
        Argument.AssertNotNull(model, nameof(model));
        return await StartSessionAsync(model, intent: null, options).ConfigureAwait(false);
    }

    /// <summary>
    /// <para>[Protocol Method]</para>
    /// Creates a new realtime transcription operation instance, establishing a connection to the /realtime endpoint.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual Task<RealtimeSession> StartTranscriptionSessionAsync(RequestOptions options)
        => StartSessionAsync(model: null, intent: "transcription", options);

    /// <summary>
    /// <para>[Protocol Method]</para>
    /// Creates a new realtime operation instance, establishing a connection to the /realtime endpoint.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="intent"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual async Task<RealtimeSession> StartSessionAsync(string model, string intent, RequestOptions options)
    {
        Uri fullEndpoint = BuildSessionEndpoint(_baseEndpoint, model, intent);
        RealtimeSession provisionalSession = new(this, fullEndpoint, _credential);
        try
        {
            await provisionalSession.ConnectAsync(options).ConfigureAwait(false);
            RealtimeSession result = provisionalSession;
            provisionalSession = null;
            return result;
        }
        finally
        {
            provisionalSession?.Dispose();
        }
    }

    private static Uri BuildSessionEndpoint(Uri baseEndpoint, string model, string intent)
    {
        ClientUriBuilder builder = new();
        builder.Reset(baseEndpoint);
        if (!string.IsNullOrEmpty(model))
        {
            builder.AppendQuery("model", model, escape: true);
        }
        if (!string.IsNullOrEmpty(intent))
        {
            builder.AppendQuery("intent", intent, escape: true);
        }
        return builder.ToUri();
    }
}