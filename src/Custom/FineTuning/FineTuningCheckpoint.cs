using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobCheckpoint")]
public partial class FineTuningCheckpoint
{
    [CodeGenMember("Id")]
    public string Id { get; }

    [CodeGenMember("CreatedAt")]
    public DateTimeOffset CreatedAt { get; }

    [CodeGenMember("FineTunedModelCheckpoint")]
    public string ModelId { get; }

    [CodeGenMember("StepNumber")]
    public int StepNumber { get; }

    [CodeGenMember("FineTuningJobCheckpointMetrics")]
    public FineTuningCheckpointMetrics Metrics { get; }

    [CodeGenMember("FineTuningJobId")]  
    public string JobId { get; }

    [CodeGenMember("Object")]
    private string _object { get; }

    public override string ToString()
    {
        return $"FineTuningJobCheckpoint<{Id}, {StepNumber}, {ModelId}>";
    }

}
