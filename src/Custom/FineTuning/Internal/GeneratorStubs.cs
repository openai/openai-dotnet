namespace OpenAI.FineTuning;

// CUSTOM: Made internal.

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

[CodeGenType("FineTuningJobEventLevel")]
internal enum FineTuningJobEventLevel
{
    Info,
    Warn,
    Error
}

[CodeGenType("FineTuneDPOMethod")]
internal partial class InternalFineTuningJobRequestMethodDpo { }

[CodeGenType("FineTuneMethodType")]
internal readonly partial struct InternalFineTuneMethodType { }

[CodeGenType("FineTuneSupervisedMethod")]
internal partial class InternalFineTuningJobRequestMethodSupervised { }

// TODO: not yet integrated

[CodeGenType("FineTuneChatRequestInput")]
internal partial class InternalTodoFineTuneChatRequestInput { }

[CodeGenType("FineTuneChatCompletionRequestAssistantMessageWeight")]
internal readonly partial struct InternalFineTuneChatCompletionRequestAssistantMessageWeight { }

[CodeGenType("CreateFineTuningCheckpointPermissionRequest")] internal partial class InternalCreateFineTuningCheckpointPermissionRequest { }
[CodeGenType("DeleteFineTuningCheckpointPermissionResponse")] internal partial class InternalDeleteFineTuningCheckpointPermissionResponse { }
[CodeGenType("FineTuningCheckpointPermission")] internal partial class InternalFineTuningCheckpointPermission { }
[CodeGenType("ListFineTuningCheckpointPermissionResponse")] internal partial class InternalListFineTuningCheckpointPermissionResponse { }
[CodeGenType("CreateFineTuningJobRequestIntegrationType")] internal readonly partial struct InternalCreateFineTuningJobRequestIntegrationType {}
[CodeGenType("FineTuneReinforcementHyperparametersReasoningEffort")] internal readonly partial struct InternalFineTuneReinforcementHyperparametersReasoningEffort {}
[CodeGenType("FineTuneReinforcementMethod")] internal partial class InternalFineTuneReinforcementMethod {}
[CodeGenType("FineTuningCheckpointPermissionObject")] internal readonly partial struct InternalFineTuningCheckpointPermissionObject {}
[CodeGenType("ListFineTuningCheckpointPermissionResponseObject")] internal readonly partial struct InternalListFineTuningCheckpointPermissionResponseObject {}
[CodeGenType("DeleteFineTuningCheckpointPermissionResponseObject")] internal readonly partial struct InternalDeleteFineTuningCheckpointPermissionResponseObject {}