using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.ClientModel.Primitives;
using System.Reflection;
using System.Text.Json;

namespace OpenAI.Tests.FineTuning;

[Category("Smoke")]
[Category("FineTuning")]
[Parallelizable(ParallelScope.All)]
public class FractionalBetaTests
{
    [TestCase("0.1", 0.1)]
    [TestCase("0.25", 0.25)]
    [TestCase("1.5", 1.5)]
    [TestCase("2e-3", 0.002)]
    [TestCase("0", 0)]
    [TestCase("2", 2)]
    public void JsonBetaSurvivesModelAndMethodSerialization(string json, double expected)
    {
        HyperparameterBetaFactor beta = ModelReaderWriter.Read<HyperparameterBetaFactor>(BinaryData.FromString(json));
        Assert.That(ModelReaderWriter.Write(beta).ToObjectFromJson<double>(), Is.EqualTo(expected));
        FineTuningTrainingMethod method = FineTuningTrainingMethod.CreateDirectPreferenceOptimization(betaFactor: beta);
        using JsonDocument body = JsonDocument.Parse(ModelReaderWriter.Write(method));
        Assert.That(body.RootElement.GetProperty("type").GetString(), Is.EqualTo("dpo"));
        Assert.That(body.RootElement.GetProperty("dpo").GetProperty("hyperparameters").GetProperty("beta").GetDouble(), Is.EqualTo(expected));
        HyperparameterBetaFactor roundtrip = ModelReaderWriter.Read<HyperparameterBetaFactor>(ModelReaderWriter.Write(beta));
        Assert.That(roundtrip, Is.EqualTo(beta));
        Assert.That(roundtrip.GetHashCode(), Is.EqualTo(beta.GetHashCode()));
    }

    [TestCase("constructor")]
    [TestCase("factory")]
    [TestCase("implicit")]
    public void FractionalValuesHavePublicConstructionPaths(string path)
    {
        // Reflection allows the baseline test to report the missing overload
        // without preventing the existing library from compiling.
        Type type = typeof(HyperparameterBetaFactor);
        HyperparameterBetaFactor beta;
        if (path == "constructor")
        {
            ConstructorInfo constructor = type.GetConstructor([typeof(double)]);
            Assert.That(constructor, Is.Not.Null);
            beta = (HyperparameterBetaFactor)constructor.Invoke([0.125]);
        }
        else
        {
            MethodInfo factory = type.GetMethod(path == "factory" ? "CreateBeta" : "op_Implicit", [typeof(double)]);
            Assert.That(factory, Is.Not.Null);
            beta = (HyperparameterBetaFactor)factory.Invoke(null, [0.125]);
        }
        Assert.That(ModelReaderWriter.Write(beta).ToObjectFromJson<double>(), Is.EqualTo(0.125));
        Assert.That(beta is IEquatable<double>, Is.True);
        Assert.That(((IEquatable<double>)(object)beta).Equals(0.125), Is.True);
        Assert.That(beta.Equals(0), Is.False);
        Assert.That(beta.Equals("auto"), Is.False);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void ExistingIntegerApiRetainsEqualityAndJsonValues(int value)
    {
        Assert.That(typeof(HyperparameterBetaFactor).GetConstructor([typeof(int)]), Is.Not.Null);
        Assert.That(typeof(HyperparameterBetaFactor).GetMethod("CreateBeta", [typeof(int)]), Is.Not.Null);
        Assert.That(typeof(HyperparameterBetaFactor).GetMethod("op_Implicit", [typeof(int)]), Is.Not.Null);
        HyperparameterBetaFactor original = new(value);
        HyperparameterBetaFactor factory = HyperparameterBetaFactor.CreateBeta(value);
        HyperparameterBetaFactor converted = value;
        HyperparameterBetaFactor read = ModelReaderWriter.Read<HyperparameterBetaFactor>(BinaryData.FromString(value.ToString()));
        Assert.That(factory == original && converted == original && read == original, Is.True);
        Assert.That(factory.GetHashCode(), Is.EqualTo(read.GetHashCode()));
        Assert.That(original.Equals(value), Is.True);
        Assert.That(ModelReaderWriter.Write(original).ToObjectFromJson<int>(), Is.EqualTo(value));
    }

    [Test]
    public void AutomaticAndNullSemanticsRemainUnchanged()
    {
        HyperparameterBetaFactor automatic = HyperparameterBetaFactor.CreateAuto();
        HyperparameterBetaFactor parsed = ModelReaderWriter.Read<HyperparameterBetaFactor>(BinaryData.FromString("\"auto\""));
        Assert.That(automatic == parsed, Is.True);
        Assert.That(automatic.Equals("auto"), Is.True);
        Assert.That(automatic.Equals(0), Is.False);
        Assert.That(ModelReaderWriter.Write(automatic).ToObjectFromJson<string>(), Is.EqualTo("auto"));
        Assert.That(automatic == null, Is.False);
        Assert.That((HyperparameterBetaFactor)null == null, Is.True);
    }
}
