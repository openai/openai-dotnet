using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace OpenAI.FineTuning;

/// <summary>
/// Parent class for all fine-tuning integrations.
/// Use <see cref="WeightsAndBiasesIntegration" /> to create a Weights & Biases integration.
/// </summary>
[CodeGenType("CreateFineTuningJobRequestIntegration")]
public partial class FineTuningIntegration 
{
}