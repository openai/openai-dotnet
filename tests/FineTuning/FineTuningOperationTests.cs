using NUnit.Framework;
using OpenAI.FineTuning;

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
        Assert.That(success.InProgress);

        var fail = FineTuningStatus.Failed;
        Assert.That(fail.InProgress, Is.False);
    }
}