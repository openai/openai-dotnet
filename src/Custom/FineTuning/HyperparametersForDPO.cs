using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;


[CodeGenType("FineTuneDPOHyperparameters")]
public partial class HyperparametersForDPO : MethodHyperparameters
{
    [CodeGenMember("BatchSize")]
    internal BinaryData _BatchSize { get; set; }

    [CodeGenMember("NEpochs")]
    internal BinaryData _NEpochs { get; set; }

    [CodeGenMember("LearningRateMultiplier")]
    internal BinaryData _LearningRateMultiplier { get; set; }

    [CodeGenMember("Beta")]
    internal BinaryData _Beta { get; set; }


    public int BatchSize => (int)HandleDefaults(_BatchSize);
    public int EpochCount => (int)HandleDefaults(_NEpochs);
    public float LearningRateMultiplier => HandleDefaults(_LearningRateMultiplier);
    public float Beta => HandleDefaults(_Beta);
}
