namespace OpenAI.FineTuning;

// CUSTOM: Made internal.

[CodeGenType("FineTuningIntegration")]
public partial class InternalFineTuningIntegration { }

[CodeGenType("FineTuningIntegrationType")]
public readonly partial struct InternalFineTuningIntegrationType { }

[CodeGenType("FineTuningIntegrationWandb")]
public partial class InternalFineTuningIntegrationWandb { }

[CodeGenType("CreateFineTuningJobRequestWandbIntegrationWandb")]
public partial class InternalCreateFineTuningJobRequestWandbIntegrationWandb
{
    [CodeGenMember("Project")]
    public string Project { get; set; }
}

[CodeGenType("FineTuningJobObject")]
public readonly partial struct InternalFineTuningJobObject { }

[CodeGenType("FineTuningJobCheckpointObject")]
public readonly partial struct InternalFineTuningJobCheckpointObject { }

[CodeGenType("ListFineTuningJobCheckpointsResponse")]
public partial class InternalListFineTuningJobCheckpointsResponse { }

[CodeGenType("ListFineTuningJobCheckpointsResponseObject")]
public readonly partial struct InternalListFineTuningJobCheckpointsResponseObject { }

[CodeGenType("ListFineTuningJobEventsResponse")]
public partial class InternalListFineTuningJobEventsResponse { }

[CodeGenType("ListPaginatedFineTuningJobsResponse")]
public partial class InternalListPaginatedFineTuningJobsResponse { }

[CodeGenType("ListPaginatedFineTuningJobsResponseObject")]
public readonly partial struct InternalListPaginatedFineTuningJobsResponseObject { }

[CodeGenType("FineTuningIntegrationWandbWandb")]
public partial class FineTuningIntegrationWandbWandb { }

[CodeGenType("UnknownCreateFineTuningJobRequestIntegration")]
public partial class UnknownCreateFineTuningJobRequestIntegration { }

[CodeGenType("UnknownFineTuningIntegration")]
public partial class UnknownFineTuningIntegration { }

[CodeGenType("FineTuningJobEventObject")]
public readonly partial struct InternalFineTuningJobEventObject { }

[CodeGenType("ListFineTuningJobEventsResponseObject")]
public readonly partial struct InternalListFineTuningJobEventsResponseObject { }

[CodeGenType("CreateFineTuningJobRequestModel")]
public readonly partial struct InternalCreateFineTuningJobRequestModel { }

[CodeGenType("FineTuningJobEventLevel")]
internal enum FineTuningJobEventLevel
{
    Info,
    Warn,
    Error
}

[CodeGenType("FineTuneDPOMethod")]
public partial class InternalFineTuningJobRequestMethodDpo { }

[CodeGenType("FineTuneMethodType")]
public readonly partial struct InternalFineTuneMethodType { }

[CodeGenType("FineTuneSupervisedMethod")]
public partial class InternalFineTuningJobRequestMethodSupervised { }

// TODO: not yet integrated

[CodeGenType("FineTuneChatRequestInput")]
public partial class InternalTodoFineTuneChatRequestInput { }

[CodeGenType("FineTuneChatCompletionRequestAssistantMessageWeight")]
public readonly partial struct InternalFineTuneChatCompletionRequestAssistantMessageWeight { }

[CodeGenType("CreateFineTuningCheckpointPermissionRequest")] public partial class InternalCreateFineTuningCheckpointPermissionRequest { }
[CodeGenType("DeleteFineTuningCheckpointPermissionResponse")] public partial class InternalDeleteFineTuningCheckpointPermissionResponse { }
[CodeGenType("FineTuningCheckpointPermission")] public partial class InternalFineTuningCheckpointPermission { }
[CodeGenType("ListFineTuningCheckpointPermissionResponse")] public partial class InternalListFineTuningCheckpointPermissionResponse { }
[CodeGenType("CreateFineTuningJobRequestIntegrationType")] public readonly partial struct InternalCreateFineTuningJobRequestIntegrationType {}
[CodeGenType("FineTuneReinforcementHyperparametersReasoningEffort")] public readonly partial struct InternalFineTuneReinforcementHyperparametersReasoningEffort {}
[CodeGenType("FineTuneReinforcementMethod")] public partial class InternalFineTuneReinforcementMethod {}
[CodeGenType("FineTuningCheckpointPermissionObject")] public readonly partial struct InternalFineTuningCheckpointPermissionObject {}
[CodeGenType("ListFineTuningCheckpointPermissionResponseObject")] public readonly partial struct InternalListFineTuningCheckpointPermissionResponseObject {}
[CodeGenType("DeleteFineTuningCheckpointPermissionResponseObject")] public readonly partial struct InternalDeleteFineTuningCheckpointPermissionResponseObject {}