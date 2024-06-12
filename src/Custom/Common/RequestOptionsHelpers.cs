using System.ClientModel.Primitives;
using System.Threading;

public  static class RequestOptionsHelpers
{
    public static RequestOptions ToRequestOptions(this CancellationToken cancellationToken)
    {
        if (cancellationToken == null) return null;
        return new RequestOptions() { CancellationToken = cancellationToken };
}