using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

/// <summary>
/// Settings for fine tuning integration with Weights and Biases (https://wandb.ai).
/// </summary>
[CodeGenType("CreateFineTuningJobRequestWandbIntegration")]
[CodeGenSuppress(nameof(WeightsAndBiasesIntegration), typeof(InternalCreateFineTuningJobRequestWandbIntegrationWandb))]
[CodeGenSuppress(nameof(WeightsAndBiasesIntegration), typeof(string), typeof(IDictionary<string, BinaryData>), typeof(InternalCreateFineTuningJobRequestWandbIntegrationWandb))]
public partial class WeightsAndBiasesIntegration
{
    [CodeGenMember("Wandb")]
    private InternalCreateFineTuningJobRequestWandbIntegrationWandb _innerWandb { get; }

    /// <summary>
    /// The Weights &amp; Biases <c>project</c> name that the run will be created under.
    /// </summary>
    public string ProjectName
    {
        get => _innerWandb.Project;
        set => _innerWandb.Project = value;
    }

    /// <summary>
    /// The friendly <c>name</c> to associate with the run. If not specified, the job ID will be used.
    /// </summary>
    public string DisplayName
    {
        get => _innerWandb.Name;
        set => _innerWandb.Name = value;
    }

    /// <summary>
    /// The Weights &amp; Biases <c>entity</c> to associate with the run, specified as a team or user name.
    /// </summary>
    public string EntityName
    {
        get => _innerWandb.Entity;
        set => _innerWandb.Entity = value;
    }

    public IList<string> Tags
    {
        get => _innerWandb.Tags;
    }

    public WeightsAndBiasesIntegration(string projectName)
        : base("wandb", null)
    {
        _innerWandb = new(projectName);
    }

    internal WeightsAndBiasesIntegration(string type, IDictionary<string, BinaryData> serializedAdditionalRawData, InternalCreateFineTuningJobRequestWandbIntegrationWandb wandb)
        : base(type, serializedAdditionalRawData)
    {
        _innerWandb = wandb;
    }
}
