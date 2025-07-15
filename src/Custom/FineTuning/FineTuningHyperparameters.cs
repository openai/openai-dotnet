using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobHyperparameters")]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct FineTuningHyperparameters
{

    private static readonly BinaryData Auto = new("\"auto\"");

    [CodeGenMember("NEpochs")]
    internal BinaryData _EpochCount { get; }

    [CodeGenMember("BatchSize")]
    internal BinaryData _BatchSize { get; }

    [CodeGenMember("LearningRateMultiplier")]
    internal BinaryData _LearningRateMultiplier { get; }

    private float HandleDefaults(BinaryData data)
    {
        if (data is null)
        {
            throw new ArgumentNullException("This hyperparameter is not set. Values for BatchSize and LearningRateMultiplier are not available in responses.");
        }

        if (data.ToString() == Auto.ToString())
        {
            return 0;
        }

        try
        {
            return float.Parse(data.ToString());
        }
        catch (FormatException)
        {
            throw new FormatException($"Hyperparameter expected a number or \"auto\" string. Got {data}");
        }
    }

    public int EpochCount => (int)HandleDefaults(_EpochCount);
    public int BatchSize => (int)HandleDefaults(_BatchSize);
    public float LearningRateMultiplier => HandleDefaults(_LearningRateMultiplier);
}
