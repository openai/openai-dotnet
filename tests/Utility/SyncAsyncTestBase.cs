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
    }
}
