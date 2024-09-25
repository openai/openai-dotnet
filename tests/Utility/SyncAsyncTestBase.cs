using NUnit.Framework;

namespace OpenAI.Tests.Utility
{
    public class SyncAsyncTestBase(bool isAsync)
    {
        public bool IsAsync { get; } = isAsync;

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
