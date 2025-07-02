using NUnit.Framework;
using OpenAI.FineTuning;
using System;

namespace OpenAI.Tests.FineTuning;

[Parallelizable(ParallelScope.Fixtures)]
[Category("FineTuning")]
[Category("Smoke")]
class HyperparameterOptionsTests
{
    [Test]
    [Parallelizable]
    public void OptionsCanEasilyCompare()
    {
        Assert.AreEqual(HyperparameterEpochCount.CreateAuto(), "auto");
        Assert.AreEqual(HyperparameterBatchSize.CreateAuto(), "auto");
        Assert.AreEqual(HyperparameterLearningRate.CreateAuto(), "auto");
        Assert.AreEqual(HyperparameterBetaFactor.CreateAuto(), "auto");

        Assert.AreEqual(new HyperparameterEpochCount(1), 1);
        Assert.AreEqual(new HyperparameterBatchSize(1), 1);
        Assert.AreEqual(new HyperparameterLearningRate(1), 1);
        Assert.AreEqual(new HyperparameterBetaFactor(1), 1);

        Assert.AreEqual(1, new HyperparameterEpochCount(1));
        Assert.AreEqual(1, new HyperparameterBatchSize(1));
        Assert.AreEqual(1.0, new HyperparameterLearningRate(1));
        Assert.AreEqual(1, new HyperparameterBetaFactor(1));

        Assert.That(1 == new HyperparameterEpochCount(1));
        Assert.That(1 == new HyperparameterBatchSize(1));
        Assert.That(0.5 == new HyperparameterLearningRate(0.5));

        Assert.That(new HyperparameterBatchSize(1) == 1);
        Assert.That(new HyperparameterEpochCount(1) == 1);
        Assert.That(new HyperparameterLearningRate(1) == 1);
        Assert.That(new HyperparameterBetaFactor(1) == 1);
    }

    [Test]
    [Parallelizable]
    public void OptionsCanEasilySet()
    {
        FineTuningTrainingMethod supervisedMethod = FineTuningTrainingMethod.CreateSupervised();
        // TODO: Add more tests here
    }
}
