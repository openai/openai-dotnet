using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenAI.FineTuning;

// CUSTOM: Made internal.

[CodeGenType("FineTuneChatCompletionRequestAssistantMessage")]
internal partial class InternalFineTuneChatCompletionRequestAssistantMessage
{
    [CodeGenMember("Content")]
    public ChatMessageContent Content { get; set; }
}

[CodeGenType("FinetuneChatRequestInput")]
internal partial class InternalFinetuneChatRequestInput { }

[CodeGenType("FinetuneCompletionRequestInput")]
internal partial class InternalFinetuneCompletionRequestInput { }

[CodeGenType("FineTuningIntegration")]
internal partial class InternalFineTuningIntegration { }

[CodeGenType("FineTuningIntegrationType")]
internal readonly partial struct InternalFineTuningIntegrationType { }

[CodeGenType("FineTuningIntegrationWandb")]
internal partial class InternalFineTuningIntegrationWandb { }

[CodeGenType("CreateFineTuningJobRequestWandbIntegrationWandb")]
internal partial class InternalCreateFineTuningJobRequestWandbIntegrationWandb
{
    [CodeGenMember("Project")]
    public string Project { get; set; }
}

[CodeGenType("FineTuningJobObject")]
internal readonly partial struct InternalFineTuningJobObject { }

[CodeGenType("FineTuningJobCheckpoint")]
internal partial class InternalFineTuningJobCheckpoint { }

[CodeGenType("FineTuningJobCheckpointMetrics")]
internal partial class InternalFineTuningJobCheckpointMetrics { }

[CodeGenType("FineTuningJobCheckpointObject")]
internal readonly partial struct InternalFineTuningJobCheckpointObject { }

[CodeGenType("ListFineTuningJobCheckpointsResponse")]
internal partial class InternalListFineTuningJobCheckpointsResponse { }

[CodeGenType("ListFineTuningJobCheckpointsResponseObject")]
internal readonly partial struct InternalListFineTuningJobCheckpointsResponseObject { }

[CodeGenType("ListFineTuningJobEventsResponse")]
internal partial class InternalListFineTuningJobEventsResponse { }

[CodeGenType("ListPaginatedFineTuningJobsResponse")]
internal partial class InternalListPaginatedFineTuningJobsResponse { }

[CodeGenType("ListPaginatedFineTuningJobsResponseObject")]
internal readonly partial struct InternalListPaginatedFineTuningJobsResponseObject { }

[CodeGenType("FineTuningIntegrationWandbWandb")]
internal partial class FineTuningIntegrationWandbWandb { }

[CodeGenType("FineTuningJobHyperparametersBatchSizeChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersBatchSizeChoiceEnum { }

[CodeGenType("FineTuningJobHyperparametersLearningRateMultiplierChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersLearningRateMultiplierChoiceEnum { }

[CodeGenType("FineTuningJobHyperparametersNEpochsChoiceEnum")]
internal readonly partial struct FineTuningJobHyperparametersNEpochsChoiceEnum { }

[CodeGenType("UnknownCreateFineTuningJobRequestIntegration")]
internal partial class UnknownCreateFineTuningJobRequestIntegration { }

[CodeGenType("UnknownFineTuningIntegration")]
internal partial class UnknownFineTuningIntegration { }

[CodeGenType("FineTuningJobEventObject")]
internal readonly partial struct InternalFineTuningJobEventObject { }

[CodeGenType("ListFineTuningJobEventsResponseObject")]
internal readonly partial struct InternalListFineTuningJobEventsResponseObject { }

[CodeGenType("CreateFineTuningJobRequestModel")]
internal readonly partial struct InternalCreateFineTuningJobRequestModel { }

// Future public types follow

[CodeGenType("CreateFineTuningJobRequestIntegration")]
internal partial class FineTuningIntegration { }

[CodeGenType("FineTuningJob")]
internal partial class FineTuningJob { }

[CodeGenType("FineTuningJobError1")]
internal partial class FineTuningJobError { }

[CodeGenType("FineTuningJobEvent")]
internal partial class FineTuningJobEvent { }

[CodeGenType("FineTuningJobEventLevel")]
internal enum FineTuningJobEventLevel
{
    Info,
    Warn,
    Error
}

[CodeGenType("FineTuningJobHyperparameters")]
[StructLayout(LayoutKind.Auto)]
internal readonly partial struct FineTuningJobHyperparameters
{
    public BinaryData NEpochs { get; }
    public BinaryData BatchSize { get; }
    public BinaryData LearningRateMultiplier { get; }
}

[CodeGenType("FineTuningJobStatus")]
internal readonly partial struct FineTuningJobStatus { }

[CodeGenType("CreateFineTuningJobRequest")]
internal partial class FineTuningOptions { }

[CodeGenType("CreateFineTuningJobRequestHyperparametersBatchSizeChoiceEnum")]
internal readonly partial struct HyperparameterBatchSize { }

[CodeGenType("CreateFineTuningJobRequestHyperparametersNEpochsChoiceEnum")]
internal readonly partial struct HyperparameterCycleCount { }

[CodeGenType("CreateFineTuningJobRequestHyperparametersLearningRateMultiplierChoiceEnum")]
internal readonly partial struct HyperparameterLearningRate { }

[CodeGenType("CreateFineTuningJobRequestHyperparameters")]
internal partial class HyperparameterOptions { }

[CodeGenType("CreateFineTuningJobRequestWandbIntegration")]
internal partial class WeightsAndBiasesIntegration { }

// TODO: not yet integrated

[CodeGenType("FineTuneChatRequestInput")]
internal partial class InternalTodoFineTuneChatRequestInput { }

[CodeGenType("FineTuneCompletionRequestInput")]
internal partial class InternalTodoFineTuneCompletionRequestInput { }

[CodeGenType("FineTuneDPOMethod")]
internal partial class InternalTodoFineTuneDPOMethod { }

[CodeGenType("FineTuneDPOMethodHyperparameters")]
internal partial class InternalTodoFineTuneDPOMethodHyperparameters { }

[CodeGenType("FineTuneMethod")]
internal partial class InternalTodoFineTuneMethod { }

[CodeGenType("FineTuneMethodType")]
internal readonly partial struct InternalTodoFineTuneMethodType { }

[CodeGenType("FineTuneSupervisedMethod")]
internal partial class InternalTodoFineTuneSupervisedMethod { }

[CodeGenType("FineTuneSupervisedMethodHyperparameters")]
internal partial class InternalFineTuneSupervisedMethodHyperparameters { }

[CodeGenType("FineTuningJobEventType")]
internal readonly partial struct InternalFineTuningJobEventType { }

[CodeGenType("FineTuneChatCompletionRequestAssistantMessageWeight")]
internal readonly partial struct InternalFineTuneChatCompletionRequestAssistantMessageWeight { }

[CodeGenType("FineTuneChatCompletionRequestAssistantMessageRole")]
internal readonly partial struct InternalFineTuneChatCompletionRequestAssistantMessageRole { }

