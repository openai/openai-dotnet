using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel.Primitives;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Tests.FineTuning;


[Parallelizable(ParallelScope.Fixtures)]
[Category("FineTuning")]
internal class FineTuningOperationTests
{
    [Test]
    [Parallelizable]
    [Category("Smoke")]
    public void TestInProgress()
    {
        var success = FineTuningStatus.Queued;
        Assert.IsTrue(success.InProgress);

        var fail = FineTuningStatus.Failed;
        Assert.IsFalse(fail.InProgress);
    }
}