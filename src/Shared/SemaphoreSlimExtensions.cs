using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI;

internal static class SemaphoreSlimExtensions
{
    public static async Task<IDisposable> AutoReleaseWaitAsync(
        this SemaphoreSlim semaphore,
        CancellationToken cancellationToken = default)
    {
        Contract.Requires(semaphore != null);
        var wrapper = new ReleaseableSemaphoreSlimWrapper(semaphore);
        await semaphore.WaitAsync(cancellationToken);
        return wrapper;
    }

    public static IDisposable AutoReleaseWait(
        this SemaphoreSlim semaphore,
        CancellationToken cancellationToken = default)
    {
        Contract.Requires(semaphore != null);
        var wrapper = new ReleaseableSemaphoreSlimWrapper(semaphore);
        semaphore.Wait(cancellationToken);
        return wrapper;
    }

    private class ReleaseableSemaphoreSlimWrapper
        : IDisposable
    {
        private readonly SemaphoreSlim semaphore;
        private bool alreadyDisposed = false;

        public ReleaseableSemaphoreSlimWrapper(SemaphoreSlim semaphore)
            => this.semaphore = semaphore;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposeActuallyCalled)
        {
            if (!this.alreadyDisposed)
            {
                if (disposeActuallyCalled)
                {
                    this.semaphore?.Release();
                }

                this.alreadyDisposed = true;
            }
        }
    }
}