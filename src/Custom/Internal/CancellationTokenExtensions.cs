using System.ClientModel.Primitives;
using System.Threading;

namespace OpenAI;

internal static class CancellationTokenExtensions
{
    public static RequestOptions ToRequestOptions(this CancellationToken cancellationToken, bool streaming = false)
    {
        if (cancellationToken == default)
        {
            if (!streaming) return null;
            return StreamRequestOptions;
        }

        return new RequestOptions() { 
            CancellationToken = cancellationToken,
            BufferResponse = !streaming,
        };
    }

    private static RequestOptions StreamRequestOptions => _streamRequestOptions ??= new() { BufferResponse = false };
    private static RequestOptions _streamRequestOptions;
}
