using System.ClientModel;

#nullable enable

namespace OpenAI;

// Convenience version
// Note: Right now, this just inherits from the protocol poller and adds a T.
// Do we need more than that?
internal abstract class OperationPoller<T> : OperationResultPoller
{
    protected OperationPoller(ClientResult current) : base(current)
    {
    }

    public ClientResult<T> Value => GetValueFromResult(Current);

    public abstract ClientResult<T> GetValueFromResult(ClientResult result);
}
