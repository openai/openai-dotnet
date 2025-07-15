using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuneSupervisedHyperparameters")]
public partial class HyperparametersForSupervised : MethodHyperparameters
{
    [CodeGenMember("BatchSize")]
    internal BinaryData _BatchSize { get; set; }

    [CodeGenMember("NEpochs")]
    internal BinaryData _NEpochs { get; set; }

    [CodeGenMember("LearningRateMultiplier")]
    internal BinaryData _LearningRateMultiplier { get; set; }
    public int BatchSize => (int)HandleDefaults(_BatchSize);
    public int EpochCount => (int)HandleDefaults(_NEpochs);
    public float LearningRateMultiplier => HandleDefaults(_LearningRateMultiplier);

}
