using System;
using System.Collections.Generic;

namespace OpenAI.FineTuning;

// CUSTOM: Made internal.

[CodeGenModel("FineTuneChatCompletionRequestAssistantMessage")]
internal partial class InternalFineTuneChatCompletionRequestAssistantMessage { }

[CodeGenModel("FinetuneChatRequestInput")]
internal partial class InternalFinetuneChatRequestInput { }

[CodeGenModel("FinetuneCompletionRequestInput")]
internal partial class InternalFinetuneCompletionRequestInput { }

[CodeGenModel("FineTuningIntegration")]
internal partial class InternalFineTuningIntegration { }

[CodeGenModel("FineTuningIntegrationType")]
internal readonly partial struct InternalFineTuningIntegrationType { }

[CodeGenModel("FineTuningIntegrationWandb")]
internal partial class InternalFineTuningIntegrationWandb { }

[CodeGenModel("CreateFineTuningJobRequestWandbIntegrationWandb")]
internal partial class InternalCreateFineTuningJobRequestWandbIntegrationWandb
{
    [CodeGenMember("Project")]
    public string Project { get; set; }
}

[CodeGenModel("FineTuningJobObject")]
internal readonly partial struct InternalFineTuningJobObject { }

[CodeGenModel("FineTuningJobCheckpoint")]
internal partial class InternalFineTuningJobCheckpoint { }

[CodeGenModel("FineTuningJobCheckpointMetrics")]
internal partial class InternalFineTuningJobCheckpointMetrics { }

[CodeGenModel("FineTuningJobCheckpointObject")]
internal readonly partial struct InternalFineTuningJobCheckpointObject { }

[CodeGenModel("ListFineTuningJobCheckpointsResponse")]
internal partial class InternalListFineTuningJobCheckpointsResponse { }

[CodeGenModel("ListFineTuningJobCheckpointsResponseObject")]
internal readonly partial struct InternalListFineTuningJobCheckpointsResponseObject { }

[CodeGenModel("ListFineTuningJobEventsResponse")]
internal partial class InternalListFineTuningJobEventsResponse { }

[CodeGenModel("ListPaginatedFineTuningJobsResponse")]
internal partial class InternalListPaginatedFineTuningJobsResponse { }

[CodeGenModel("ListPaginatedFineTuningJobsResponseObject")]
internal readonly partial struct InternalListPaginatedFineTuningJobsResponseObject { }

[CodeGenModel("FineTuningIntegrationWandbWandb")]
internal partial class FineTuningIntegrationWandbWandb { }

[CodeGenModel("FineTuningJobHyperparametersBatchSizeChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersBatchSizeChoiceEnum { }

[CodeGenModel("FineTuningJobHyperparametersLearningRateMultiplierChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersLearningRateMultiplierChoiceEnum { }

[CodeGenModel("FineTuningJobHyperparametersNEpochsChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersNEpochsChoiceEnum { }

[CodeGenModel("UnknownCreateFineTuningJobRequestIntegration")]
internal partial class UnknownCreateFineTuningJobRequestIntegration { }

[CodeGenModel("UnknownFineTuningIntegration")]
internal partial class UnknownFineTuningIntegration { }

[CodeGenModel("FineTuningJobEventObject")]
internal readonly partial struct InternalFineTuningJobEventObject { }

[CodeGenModel("ListFineTuningJobEventsResponseObject")]
internal readonly partial struct InternalListFineTuningJobEventsResponseObject { }

[CodeGenModel("CreateFineTuningJobRequestModel")]
internal readonly partial struct InternalCreateFineTuningJobRequestModel { }

// Future public types follow

[CodeGenModel("CreateFineTuningJobRequestIntegration")]
internal partial class FineTuningIntegration { }

[CodeGenModel("FineTuningJob")]
internal partial class FineTuningJob { }

[CodeGenModel("FineTuningJobError")]
internal partial class FineTuningJobError { }

[CodeGenModel("FineTuningJobEvent")]
internal partial class FineTuningJobEvent { }

[CodeGenModel("FineTuningJobEventLevel")]
internal enum FineTuningJobEventLevel
{
    Info,
    Warn,
    Error
}

[CodeGenModel("FineTuningJobHyperparameters")]
internal readonly partial struct FineTuningJobHyperparameters
{
    internal readonly IDictionary<string, BinaryData> SerializedAdditionalRawData { get; }
    public BinaryData NEpochs { get; }
    public BinaryData BatchSize { get; }
    public BinaryData LearningRateMultiplier { get; }
}

[CodeGenModel("FineTuningJobStatus")]
internal readonly partial struct FineTuningJobStatus { }

[CodeGenModel("CreateFineTuningJobRequest")]
internal partial class FineTuningOptions { }

[CodeGenModel("CreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum")]
internal readonly partial struct HyperparameterBatchSize { }

[CodeGenModel("CreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum")]
internal readonly partial struct HyperparameterCycleCount { }

[CodeGenModel("CreateFineTuningJobRequestHyperparametersLearningRateMultiplierChoiceEnum")]
internal readonly partial struct HyperparameterLearningRate { }

[CodeGenModel("CreateFineTuningJobRequestHyperparameters")]
internal partial class HyperparameterOptions { }

[CodeGenModel("CreateFineTuningJobRequestWandbIntegration")]
internal partial class WeightsAndBiasesIntegration { }
