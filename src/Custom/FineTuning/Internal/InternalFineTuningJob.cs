using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.FineTuning;

/**
 * Represents a fine-tuning job as returned by the OpenAI API.
 * This is separate from the <see cref="FineTuningJob" /> class, which is a Long Running Operation.
 */

[Experimental("OPENAI001")]
[CodeGenType("FineTuningJob")]
internal partial class InternalFineTuningJob
{
    [CodeGenMember("Id")]
    public string JobId { get; }

    [CodeGenMember("Model")]
    public string BaseModel { get; }

    [CodeGenMember("EstimatedFinish")]
    public DateTimeOffset? EstimatedFinishAt { get; }

    [CodeGenMember("ValidationFile")]
    public string ValidationFileId { get; }

    [CodeGenMember("TrainingFile")]
    public string TrainingFileId { get; }

    [CodeGenMember("ResultFiles")]
    public IReadOnlyList<string> ResultFileIds { get; }

    [CodeGenMember("Status")]
    public FineTuningStatus Status { get; }

    [CodeGenMember("Object")]
    internal string _object { get; }

    [CodeGenMember("Hyperparameters")]
    public FineTuningHyperparameters Hyperparameters { get; } = default;

    [CodeGenMember("Integrations")]
    public IReadOnlyList<FineTuningIntegration> Integrations { get; }

    [CodeGenMember("TrainedTokens")]
    public int? BillableTrainedTokenCount { get; set; }

    [CodeGenMember("UserProvidedSuffix")]
    public string UserProvidedSuffix { get; }

    [CodeGenMember("CreatedAt")]
    public DateTimeOffset CreatedAt { get; }

    [CodeGenMember("Error")]
    public FineTuningError Error { get; }

    [CodeGenMember("FineTunedModel")]
    public string FineTunedModel { get; }

    [CodeGenMember("FinishedAt")]
    public DateTimeOffset? FinishedAt { get; }

    [CodeGenMember("OrganizationId")]
    public string OrganizationId { get; }

    [CodeGenMember("Seed")]
    public int Seed { get; }

    [CodeGenMember("Method")]
    public FineTuningTrainingMethod Method { get; }

    public override string ToString()
    {
        return $"FineTuningJob<{JobId}, {Status}, {CreatedAt}>";
    }

    internal static InternalFineTuningJob FromClientResult(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalFineTuningJob(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}