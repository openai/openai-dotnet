using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Globalization;

namespace OpenAI.Tests.FineTuning;

[Category("Smoke")]
[Category("FineTuning")]
public class HyperparameterCultureTests
{
    private static IEnumerable<TestCaseData> NumericCases()
    {
        foreach (string culture in new[] { "en-US", "de-DE", "fr-FR", "pl-PL" })
        foreach (string kind in new[] { "legacy", "supervised", "dpo" })
        foreach ((string json, float expected) in new[] { ("0.125", 0.125f), ("1.25", 1.25f), ("2.5e-3", 0.0025f) })
            yield return new TestCaseData(culture, kind, json, expected);
    }

    [TestCaseSource(nameof(NumericCases))]
    public void JsonNumbersHaveTheSameValueInEveryCulture(string culture, string kind, string json, float expected)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            BinaryData payload = BinaryData.FromString($"{{\"batch_size\":64,\"n_epochs\":3,\"learning_rate_multiplier\":{json},\"beta\":{json}}}");
            if (kind == "legacy")
            {
                FineTuningHyperparameters value = ModelReaderWriter.Read<FineTuningHyperparameters>(payload);
                Assert.That(value.LearningRateMultiplier, Is.EqualTo(expected));
                Assert.That(value.BatchSize, Is.EqualTo(64));
                Assert.That(value.EpochCount, Is.EqualTo(3));
            }
            else if (kind == "supervised")
            {
                HyperparametersForSupervised value = ModelReaderWriter.Read<HyperparametersForSupervised>(payload);
                Assert.That(value.LearningRateMultiplier, Is.EqualTo(expected));
                Assert.That(value.BatchSize, Is.EqualTo(64));
                Assert.That(value.EpochCount, Is.EqualTo(3));
            }
            else
            {
                HyperparametersForDPO value = ModelReaderWriter.Read<HyperparametersForDPO>(payload);
                Assert.That(value.LearningRateMultiplier, Is.EqualTo(expected));
                Assert.That(value.Beta, Is.EqualTo(expected));
                Assert.That(value.BatchSize, Is.EqualTo(64));
                Assert.That(value.EpochCount, Is.EqualTo(3));
            }
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [TestCase("en-US")]
    [TestCase("de-DE")]
    [TestCase("fr-FR")]
    public void AutoAndUnsetParametersKeepExistingBehavior(string culture)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            BinaryData automatic = BinaryData.FromString("{\"learning_rate_multiplier\":\"auto\",\"beta\":\"auto\"}");
            Assert.That(ModelReaderWriter.Read<FineTuningHyperparameters>(automatic).LearningRateMultiplier, Is.Zero);
            Assert.That(ModelReaderWriter.Read<HyperparametersForSupervised>(automatic).LearningRateMultiplier, Is.Zero);
            Assert.That(ModelReaderWriter.Read<HyperparametersForDPO>(automatic).Beta, Is.Zero);
            BinaryData missing = BinaryData.FromString("{}");
            Assert.Throws<ArgumentNullException>(() => _ = ModelReaderWriter.Read<FineTuningHyperparameters>(missing).LearningRateMultiplier);
            Assert.Throws<ArgumentNullException>(() => _ = ModelReaderWriter.Read<HyperparametersForSupervised>(missing).LearningRateMultiplier);
            Assert.Throws<ArgumentNullException>(() => _ = ModelReaderWriter.Read<HyperparametersForDPO>(missing).Beta);
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }
}
