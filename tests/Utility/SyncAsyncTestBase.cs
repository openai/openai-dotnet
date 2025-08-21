using System;
using System.ClientModel;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OpenAI.Tests.Utility
{
    public class SyncAsyncTestBase
    {
        public bool IsAsync { get; }

        public SyncAsyncTestBase(bool isAsync)
        {
            IsAsync = isAsync;
        }

        protected void AssertAsyncOnly()
        {
            if (!IsAsync)
            {
                Assert.Ignore("Test is async-only.");
            }
        }

        protected void AssertSyncOnly()
        {
            if (IsAsync)
            {
                Assert.Ignore("Test is sync-only.");
            }
        }

        protected static async Task RetryWithExponentialBackoffAsync(Func<Task> action, int maxRetries = 5, int initialWaitMs = 750)
        {
            int waitDuration = initialWaitMs;
            int retryCount = 0;
            bool successful = false;

            while (retryCount < maxRetries && !successful)
            {
                try
                {
                    await action();
                    successful = true;
                }
                catch (ClientResultException ex) when (ex.Status == 404)
                {
                    await Task.Delay(waitDuration);
                    waitDuration *= 2;
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
