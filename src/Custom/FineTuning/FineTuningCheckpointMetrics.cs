using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobCheckpointMetrics")]
public partial class FineTuningCheckpointMetrics
{
    [CodeGenMember("Step")]
    public int StepNumber { get; }

}
