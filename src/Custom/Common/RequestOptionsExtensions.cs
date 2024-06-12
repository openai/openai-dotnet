
using System.ClientModel.Primitives;
using System.Threading;

internal static class RequestOptionsExtensions
{
    public static RequestOptions ToRequestOptions(this CancellationToken cancellationToken)
    {
        if (cancellationToken == default) return null;
        return new RequestOptions() { CancellationToken = cancellationToken };
    }
}