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
        Assert.That(HyperparameterEpochCount.CreateAuto(), Is.EqualTo("auto"));
        Assert.That(HyperparameterBatchSize.CreateAuto(), Is.EqualTo("auto"));
        Assert.That(HyperparameterLearningRate.CreateAuto(), Is.EqualTo("auto"));
        Assert.That(HyperparameterBetaFactor.CreateAuto(), Is.EqualTo("auto"));

        Assert.That(new HyperparameterEpochCount(1), Is.EqualTo(1));
        Assert.That(new HyperparameterBatchSize(1), Is.EqualTo(1));
        Assert.That(new HyperparameterLearningRate(1), Is.EqualTo(1));
        Assert.That(new HyperparameterBetaFactor(1), Is.EqualTo(1));

        Assert.That(new HyperparameterEpochCount(1), Is.EqualTo(1));
        Assert.That(new HyperparameterBatchSize(1), Is.EqualTo(1));
        Assert.That(new HyperparameterLearningRate(1), Is.EqualTo(1.0));
        Assert.That(new HyperparameterBetaFactor(1), Is.EqualTo(1));

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
