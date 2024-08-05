namespace OpenAI.Assistants;

/*
 * This file stubs and performs minimal customization to generated internal types for the OpenAI.Assistants namespace.
 */

[CodeGenModel("SubmitToolOutputsRunRequest")]
internal partial class InternalSubmitToolOutputsRunRequest { }

[CodeGenModel("CreateAssistantRequestModel")]
internal readonly partial struct InternalCreateAssistantRequestModel { }

[CodeGenModel("MessageContentTextObjectAnnotation")]
internal partial class InternalMessageContentTextObjectAnnotation { }

[CodeGenModel("MessageContentTextAnnotationsFileCitationObject")]
internal partial class InternalMessageContentTextAnnotationsFileCitationObject { }

[CodeGenModel("MessageContentTextAnnotationsFilePathObject")]
internal partial class InternalMessageContentTextAnnotationsFilePathObject { }

[CodeGenModel("MessageDeltaContentImageFileObjectImageFile")]
internal partial class InternalMessageDeltaContentImageFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenModel("MessageDeltaContentImageUrlObjectImageUrl")]
internal partial class InternalMessageDeltaContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenModel("MessageDeltaContentImageFileObject")]
internal partial class InternalMessageDeltaContentImageFileObject { }

[CodeGenModel("MessageDeltaContentImageUrlObject")]
internal partial class InternalMessageDeltaContentImageUrlObject { }

[CodeGenModel("MessageDeltaObjectDelta")]
internal partial class InternalMessageDeltaObjectDelta
{
    [CodeGenMember("Role")]
    internal MessageRole Role { get; }
}

[CodeGenModel("MessageDeltaContentTextObject")]
internal partial class InternalMessageDeltaContentTextObject { }

[CodeGenModel("MessageDeltaContentTextObjectText")]
internal partial class InternalMessageDeltaContentTextObjectText { }

[CodeGenModel("MessageDeltaContentTextAnnotationsFileCitationObject")]
internal partial class InternalMessageDeltaContentTextAnnotationsFileCitationObject { }

[CodeGenModel("MessageDeltaTextContentAnnotation")]
internal partial class InternalMessageDeltaTextContentAnnotation { }

[CodeGenModel("MessageDeltaContentTextAnnotationsFileCitationObjectFileCitation")]
internal partial class InternalMessageDeltaContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenModel("RunStepDeltaObject")]
internal partial class InternalRunStepDelta { private readonly object Object; }

[CodeGenModel("RunStepDeltaObjectDelta")]
internal partial class InternalRunStepDeltaObjectDelta { }

[CodeGenModel("MessageDeltaContentTextAnnotationsFilePathObject")]
internal partial class InternalMessageDeltaContentTextAnnotationsFilePathObject { }

[CodeGenModel("MessageDeltaContentTextAnnotationsFilePathObjectFilePath")]
internal partial class InternalMessageDeltaContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenModel("MessageDeltaContent")]
internal partial class InternalMessageDeltaContent { }

[CodeGenModel("DeleteAssistantResponse")]
internal partial class InternalDeleteAssistantResponse { }

[CodeGenModel("DeleteAssistantResponseObject")]
internal readonly partial struct InternalDeleteAssistantResponseObject { }

[CodeGenModel("DeleteThreadResponse")]
internal partial class InternalDeleteThreadResponse { }

[CodeGenModel("DeleteThreadResponseObject")]
internal readonly partial struct InternalDeleteThreadResponseObject { }

[CodeGenModel("DeleteMessageResponse")]
internal partial class InternalDeleteMessageResponse { }

[CodeGenModel("DeleteMessageResponseObject")]
internal readonly partial struct InternalDeleteMessageResponseObject { }

[CodeGenModel("CreateThreadAndRunRequest")]
internal partial class InternalCreateThreadAndRunRequest
{
    public string Model { get; set; }
    public ToolResources ToolResources { get; set; }
    public AssistantResponseFormat ResponseFormat { get; set; }
    public ToolConstraint ToolChoice { get; set; }
}

[CodeGenModel("MessageContentImageUrlObjectImageUrl")]
internal partial class InternalMessageContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenModel("MessageContentImageFileObjectImageFile")]
internal partial class InternalMessageContentItemFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenModel("MessageContentTextObjectText")]
internal partial class InternalMessageContentTextObjectText { }

[CodeGenModel("RunStepDetailsMessageCreationObjectMessageCreation")]
internal partial class InternalRunStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenModel("RunStepDetailsToolCallsFunctionObjectFunction")]
internal partial class InternalRunStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenModel("RunStepDetailsToolCallsCodeObjectCodeInterpreter")]
internal partial class InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenModel("RunStepDetailsToolCallsCodeOutputImageObjectImage")]
internal partial class InternalRunStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenModel("MessageContentTextAnnotationsFileCitationObjectFileCitation")]
internal partial class InternalMessageContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenModel("MessageContentTextAnnotationsFilePathObjectFilePath")]
internal partial class InternalMessageContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenModel("RunObjectRequiredAction")]
internal partial class InternalRunRequiredAction { private readonly object Type; }

[CodeGenModel("RunObjectRequiredActionSubmitToolOutputs")]
internal partial class InternalRunObjectRequiredActionSubmitToolOutputs { private readonly object Type; }

[CodeGenModel("RunToolCallObjectFunction")]
internal partial class InternalRunToolCallObjectFunction { }

[CodeGenModel("ListAssistantsResponse")]
internal partial class InternalListAssistantsResponse : IInternalListResponse<Assistant> { }

[CodeGenModel("ListAssistantsResponseObject")]
internal readonly partial struct InternalListAssistantsResponseObject {}

[CodeGenModel("ListThreadsResponse")]
internal partial class InternalListThreadsResponse : IInternalListResponse<AssistantThread> { }

[CodeGenModel("ListThreadsResponseObject")]
internal readonly partial struct InternalListThreadsResponseObject {}

[CodeGenModel("ListMessagesResponse")]
internal partial class InternalListMessagesResponse : IInternalListResponse<ThreadMessage> { }

[CodeGenModel("ListMessagesResponseObject")]
internal readonly partial struct InternalListMessagesResponseObject {}

[CodeGenModel("ListRunsResponse")]
internal partial class InternalListRunsResponse : IInternalListResponse<ThreadRun> { }

[CodeGenModel("ListRunsResponseObject")]
internal readonly partial struct InternalListRunsResponseObject {}

[CodeGenModel("ListRunStepsResponse")]
internal partial class InternalListRunStepsResponse : IInternalListResponse<RunStep> { }

[CodeGenModel("ListRunStepsResponseObject")]
internal readonly partial struct InternalListRunStepsResponseObject {}

[CodeGenModel("RunStepDetailsToolCallsFileSearchObject")]
internal partial class InternalRunStepFileSearchToolCallDetails { }

[CodeGenModel("TruncationObjectType")]
internal readonly partial struct InternalTruncationObjectType { }

[CodeGenModel("AssistantsNamedToolChoiceType")]
internal readonly partial struct InternalAssistantsNamedToolChoiceType { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsCodeObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeObject { }

[CodeGenModel("RunStepUpdateCodeInterpreterOutput")]
internal abstract partial class InternalRunStepUpdateCodeInterpreterOutput { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsCodeOutputImageObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObject { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject
{
    [CodeGenMember("Logs")]
    public string InternalLogs { get; set; }
}

[CodeGenModel("RunStepDeltaStepDetailsMessageCreationObject")]
internal partial class InternalRunStepDeltaStepDetailsMessageCreationObject { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsObject { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsFileSearchObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFileSearchObject { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsFunctionObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObject { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsObjectToolCallsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject { }

[CodeGenModel("RunStepDeltaStepDetailsMessageCreationObjectMessageCreation")]
internal partial class InternalRunStepDeltaStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenModel("RunStepDeltaStepDetails")]
internal partial class InternalRunStepDeltaStepDetails { }

[CodeGenModel("RunStepDeltaStepDetailsToolCallsFunctionObjectFunction")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenModel("AssistantsApiResponseFormat")]
internal partial class InternalAssistantsApiResponseFormat { }

[CodeGenModel("AssistantsApiResponseFormatType")]
internal readonly partial struct InternalAssistantsApiResponseFormatType { }

[CodeGenModel("AssistantsNamedToolChoiceFunction")]
internal partial class InternalAssistantsNamedToolChoiceFunction { }

[CodeGenModel("AssistantObjectObject")]
internal readonly partial struct InternalAssistantObjectObject { }

[CodeGenModel("MessageObjectObject")]
internal readonly partial struct InternalMessageObjectObject { }

[CodeGenModel("RunObjectObject")]
internal readonly partial struct InternalRunObjectObject { }

[CodeGenModel("RunStepObjectObject")]
internal readonly partial struct InternalRunStepObjectObject { }

[CodeGenModel("ThreadObjectObject")]
internal readonly partial struct InternalThreadObjectObject { }

[CodeGenModel("MessageRequestContentTextObjectType")]
internal readonly partial struct InternalMessageRequestContentTextObjectType { }

[CodeGenModel("MessageContentImageUrlObjectImageUrlDetail")]
internal readonly partial struct InternalMessageContentImageUrlObjectImageUrlDetail { }

[CodeGenModel("MessageContentImageFileObjectImageFileDetail")]
internal readonly partial struct InternalMessageContentImageFileObjectImageFileDetail { }

[CodeGenModel("MessageDeltaContentImageFileObjectImageFileDetail")]
internal readonly partial struct InternalMessageDeltaContentImageFileObjectImageFileDetail { }

[CodeGenModel("MessageDeltaContentImageUrlObjectImageUrlDetail")]
internal readonly partial struct InternalMessageDeltaContentImageUrlObjectImageUrlDetail { }

[CodeGenModel("MessageDeltaObject")]
internal partial class InternalMessageDeltaObject { }

[CodeGenModel("MessageDeltaObjectDeltaRole")]
internal readonly partial struct InternalMessageDeltaObjectDeltaRole { }

[CodeGenModel("MessageDeltaObjectObject")]
internal readonly partial struct InternalMessageDeltaObjectObject { }

[CodeGenModel("MessageObjectAttachment")]
internal partial class InternalMessageObjectAttachment { }

[CodeGenModel("MessageContentImageFileObjectType")]
internal readonly partial struct InternalMessageContentImageFileObjectType { }

[CodeGenModel("MessageContentImageUrlObjectType")]
internal readonly partial struct InternalMessageContentImageUrlObjectType { }

[CodeGenModel("MessageContentTextObjectType")]
internal readonly partial struct InternalMessageContentTextObjectType { }

[CodeGenModel("RunObjectRequiredActionType")]
internal readonly partial struct InternalRunObjectRequiredActionType { }

[CodeGenModel("RunStepDeltaObjectObject")]
internal readonly partial struct InternalRunStepDeltaObjectObject { }

[CodeGenModel("RunToolCallObjectType")]
internal readonly partial struct InternalRunToolCallObjectType { }

[CodeGenModel("MessageObjectRole")]
internal readonly partial struct InternalMessageObjectRole { }

[CodeGenModel("CreateRunRequestModel")]
internal readonly partial struct InternalCreateRunRequestModel { }

[CodeGenModel("CreateRunRequestToolChoice")]
internal readonly partial struct InternalCreateRunRequestToolChoice { }

[CodeGenModel("CreateAssistantRequestResponseFormat1")]
internal readonly partial struct InternalCreateAssistantRequestResponseFormat { }

[CodeGenModel("CreateAssistantRequestToolResources")]
internal partial class InternalCreateAssistantRequestToolResources { }

[CodeGenModel("CreateAssistantRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenModel("CreateAssistantRequestToolResourcesFileSearchBase")]
internal partial class InternalCreateAssistantRequestToolResourcesFileSearchBase { }

[CodeGenModel("CreateAssistantRequestToolResourcesFileSearchVectorStoreCreationHelpers")]
internal partial class InternalCreateAssistantRequestToolResourcesFileSearchVectorStoreCreationHelpers { }

[CodeGenModel("CreateAssistantRequestToolResourcesFileSearchVectorStoreIdReferences")]
internal partial class InternalCreateAssistantRequestToolResourcesFileSearchVectorStoreIdReferences { }

[CodeGenModel("CreateThreadAndRunRequestModel")]
internal readonly partial struct InternalCreateThreadAndRunRequestModel { }

[CodeGenModel("CreateThreadAndRunRequestResponseFormat")]
internal readonly partial struct InternalCreateThreadAndRunRequestResponseFormat { }

[CodeGenModel("CreateThreadAndRunRequestToolChoice")]
internal readonly partial struct InternalCreateThreadAndRunRequestToolChoice { }

[CodeGenModel("CreateThreadAndRunRequestToolResources")]
internal partial class InternalCreateThreadAndRunRequestToolResources { }

[CodeGenModel("CreateThreadAndRunRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateThreadAndRunRequestToolResourcesCodeInterpreter { }

[CodeGenModel("CreateThreadAndRunRequestToolResourcesFileSearch")]
internal partial class InternalCreateThreadAndRunRequestToolResourcesFileSearch { }

[CodeGenModel("CreateThreadRequestToolResources")]
internal partial class InternalCreateThreadRequestToolResources { }

[CodeGenModel("CreateThreadRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateThreadRequestToolResourcesCodeInterpreter { }

[CodeGenModel("CreateThreadRequestToolResourcesFileSearchBase")]
internal partial class InternalCreateThreadRequestToolResourcesFileSearchBase { }

[CodeGenModel("CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpers")]
internal partial class InternalCreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpers { }

[CodeGenModel("CreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpersVectorStore")]
internal partial class InternalCreateThreadRequestToolResourcesFileSearchVectorStoreCreationHelpersVectorStore { }

[CodeGenModel("CreateThreadRequestToolResourcesFileSearchVectorStoreIdReferences")]
internal partial class InternalCreateThreadRequestToolResourcesFileSearchVectorStoreIdReferences { }

[CodeGenModel("ModifyAssistantRequestToolResources")]
internal partial class InternalModifyAssistantRequestToolResources { }

[CodeGenModel("ModifyAssistantRequestToolResourcesCodeInterpreter")]
internal partial class InternalModifyAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenModel("ModifyAssistantRequestToolResourcesFileSearch")]
internal partial class InternalModifyAssistantRequestToolResourcesFileSearch { }

[CodeGenModel("ModifyThreadRequestToolResources")]
internal partial class InternalModifyThreadRequestToolResources { }

[CodeGenModel("ModifyThreadRequestToolResourcesCodeInterpreter")]
internal partial class InternalModifyThreadRequestToolResourcesCodeInterpreter { }

[CodeGenModel("ModifyThreadRequestToolResourcesFileSearch")]
internal partial class InternalModifyThreadRequestToolResourcesFileSearch { }

[CodeGenModel("ThreadObjectToolResources")]
internal partial class InternalThreadObjectToolResources { }

[CodeGenModel("ThreadObjectToolResourcesCodeInterpreter")]
internal partial class InternalThreadObjectToolResourcesCodeInterpreter { }

[CodeGenModel("ThreadObjectToolResourcesFileSearch")]
internal partial class InternalThreadObjectToolResourcesFileSearch { }

[CodeGenModel("AssistantToolsFileSearchTypeOnly")]
internal partial class InternalAssistantToolsFileSearchTypeOnly { }

[CodeGenModel("AssistantToolsFileSearchTypeOnlyType")]
internal readonly partial struct InternalAssistantToolsFileSearchTypeOnlyType { }
