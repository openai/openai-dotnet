using System.ClientModel;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// Protocol version
// Note: idea here is that this type is generated to be specific to the client.
// Like PageResultEnumerator, the subtype will be generated and wrapped in an
// outer public type.
internal abstract class OperationResultPoller
{
    private const int DefaultWaitMilliseconds = 1000;

    protected OperationResultPoller(ClientResult current)
    {
        Current = current;
    }

    // TODO: Thread-safe assignment?
    public ClientResult Current { get; protected set; }

    // Service-specific methods to be generated on the subclient
    public abstract Task<ClientResult> UpdateStatusAsync();

    public abstract ClientResult UpdateStatus();

    public abstract bool HasStopped(ClientResult result);

    // TODO: how does RequestOptions/CancellationToken work?
    public async Task WaitForCompletionAsync()
    {
        bool hasStopped = HasStopped(Current);

        while (!hasStopped)
        {
            // TODO: implement an interesting wait routine
            await Task.Delay(DefaultWaitMilliseconds);

            Current = await UpdateStatusAsync().ConfigureAwait(false);
            Update();

            hasStopped = HasStopped(Current);
        }
    }

    public void WaitForCompletion()
    {
        bool hasStopped = HasStopped(Current);

        while (!hasStopped)
        {
            // TODO: implement an interesting wait routine
            Thread.Sleep(DefaultWaitMilliseconds);

            Current = UpdateStatus();
            Update();

            hasStopped = HasStopped(Current);
        }
    }

    protected virtual void Update() { }
}
