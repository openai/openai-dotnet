using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace OpenAI.RealtimeConversation;

public partial class RealtimeConversationClient
{
    /// <summary>
    /// <para>[Protocol Method]</para>
    /// Creates a new realtime conversation operation instance, establishing the connection and optionally sending an
    /// initial configuration message payload.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual async Task<RealtimeConversationSession> StartConversationSessionAsync(RequestOptions options)
    {
        RealtimeConversationSession provisionalSession = new(this, _endpoint, _credential);
        try
        {
            await provisionalSession.ConnectAsync(options).ConfigureAwait(false);
            RealtimeConversationSession result = provisionalSession;
            provisionalSession = null;
            return result;
        }
        finally
        {
            provisionalSession?.Dispose();
        }
    }
}