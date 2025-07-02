namespace OpenAI.Assistants;

/*
 * This file stubs and performs minimal customization to generated internal types for the OpenAI.Assistants namespace.
 */

[CodeGenType("SubmitToolOutputsRunRequest")]
internal partial class InternalSubmitToolOutputsRunRequest { }

[CodeGenType("MessageContentTextObjectAnnotation")]
internal partial class InternalMessageContentTextObjectAnnotation { }

[CodeGenType("MessageContentTextAnnotationsFileCitationObject")]
internal partial class InternalMessageContentTextAnnotationsFileCitationObject { }

[CodeGenType("MessageContentTextAnnotationsFilePathObject")]
internal partial class InternalMessageContentTextAnnotationsFilePathObject { }

[CodeGenType("MessageDeltaContentImageFileObjectImageFile")]
internal partial class InternalMessageDeltaContentImageFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenType("MessageDeltaContentImageUrlObjectImageUrl")]
internal partial class InternalMessageDeltaContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenType("MessageDeltaContentImageFileObject")]
internal partial class InternalMessageDeltaContentImageFileObject { }

[CodeGenType("MessageDeltaContentImageUrlObject")]
internal partial class InternalMessageDeltaContentImageUrlObject { }

[CodeGenType("MessageDeltaObjectDelta")]
internal partial class InternalMessageDeltaObjectDelta
{
    [CodeGenMember("Role")]
    internal MessageRole? Role { get; }
}

[CodeGenType("MessageDeltaContentTextObject")]
internal partial class InternalMessageDeltaContentTextObject { }

[CodeGenType("MessageDeltaContentTextObjectText")]
internal partial class InternalMessageDeltaContentTextObjectText { }

[CodeGenType("MessageDeltaContentTextAnnotationsFileCitationObject")]
internal partial class InternalMessageDeltaContentTextAnnotationsFileCitationObject { }

[CodeGenType("MessageDeltaTextContentAnnotation")]
internal partial class InternalMessageDeltaTextContentAnnotation { }

[CodeGenType("MessageDeltaContentTextAnnotationsFileCitationObjectFileCitation")]
internal partial class InternalMessageDeltaContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenType("RunStepDeltaObject")]
internal partial class InternalRunStepDelta { private readonly object Object; }

[CodeGenType("RunStepDeltaObjectDelta")]
internal partial class InternalRunStepDeltaObjectDelta { }

[CodeGenType("MessageDeltaContentTextAnnotationsFilePathObject")]
internal partial class InternalMessageDeltaContentTextAnnotationsFilePathObject { }

[CodeGenType("MessageDeltaContentTextAnnotationsFilePathObjectFilePath")]
internal partial class InternalMessageDeltaContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenType("MessageDeltaContent")]
internal partial class InternalMessageDeltaContent { }

[CodeGenType("DeleteAssistantResponseObject")]
internal readonly partial struct InternalDeleteAssistantResponseObject { }

[CodeGenType("DeleteThreadResponseObject")]
internal readonly partial struct InternalDeleteThreadResponseObject { }

[CodeGenType("DeleteMessageResponseObject")]
internal readonly partial struct InternalDeleteMessageResponseObject { }

[CodeGenType("CreateThreadAndRunRequest")]
internal partial class InternalCreateThreadAndRunRequest
{
    public string Model { get; set; }
    public ToolResources ToolResources { get; set; }
    public AssistantResponseFormat ResponseFormat { get; set; }
    public ToolConstraint ToolChoice { get; set; }
}

[CodeGenType("MessageContentImageUrlObjectImageUrl")]
internal partial class InternalMessageContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenType("MessageContentImageFileObjectImageFile")]
internal partial class InternalMessageContentItemFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenType("MessageContentRefusalObjectType")]
internal readonly partial struct InternalMessageContentRefusalObjectType { }

[CodeGenType("RunStepDetailsMessageCreationObjectMessageCreation")]
internal partial class InternalRunStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenType("RunStepDetailsToolCallsFunctionObjectFunction")]
internal partial class InternalRunStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenType("RunStepDetailsToolCallsCodeObjectCodeInterpreter")]
internal partial class InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenType("RunStepDetailsToolCallsCodeOutputImageObjectImage")]
internal partial class InternalRunStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenType("MessageContentTextAnnotationsFileCitationObjectFileCitation")]
internal partial class InternalMessageContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenType("MessageContentTextAnnotationsFilePathObjectFilePath")]
internal partial class InternalMessageContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenType("RunObjectRequiredAction1")]
internal partial class InternalRunRequiredAction { private readonly object Type; }

[CodeGenType("RunObjectRequiredActionSubmitToolOutputs")]
internal partial class InternalRunObjectRequiredActionSubmitToolOutputs { private readonly object Type; }

[CodeGenType("RunToolCallObjectFunction")]
internal partial class InternalRunToolCallObjectFunction { }

[CodeGenType("ListAssistantsResponse")]
internal partial class InternalListAssistantsResponse : IInternalListResponse<Assistant> { }

[CodeGenType("ListAssistantsResponseObject")]
internal readonly partial struct InternalListAssistantsResponseObject { }

[CodeGenType("ListMessagesResponse")]
internal partial class InternalListMessagesResponse : IInternalListResponse<ThreadMessage> { }

[CodeGenType("ListMessagesResponseObject")]
internal readonly partial struct InternalListMessagesResponseObject { }

[CodeGenType("ListRunsResponse")]
internal partial class InternalListRunsResponse : IInternalListResponse<ThreadRun> { }

[CodeGenType("ListRunsResponseObject")]
internal readonly partial struct InternalListRunsResponseObject { }

[CodeGenType("ListRunStepsResponse")]
internal partial class InternalListRunStepsResponse : IInternalListResponse<RunStep> { }

[CodeGenType("ListRunStepsResponseObject")]
internal readonly partial struct InternalListRunStepsResponseObject { }

[CodeGenType("RunStepDetailsToolCallsCodeObject")]
internal partial class InternalRunStepDetailsToolCallsCodeObject { }

[CodeGenType("RunStepDetailsToolCallsFileSearchObject")]
internal partial class InternalRunStepDetailsToolCallsFileSearchObject { }

[CodeGenType("RunStepDetailsToolCallsFunctionObject")]
internal partial class InternalRunStepDetailsToolCallsFunctionObject { }

[CodeGenType("TruncationObjectType")]
internal readonly partial struct InternalTruncationObjectType { }

[CodeGenType("AssistantsNamedToolChoiceType")]
internal readonly partial struct InternalAssistantsNamedToolChoiceType { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputImageObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject
{
    [CodeGenMember("Logs")]
    public string InternalLogs { get; set; }
}

[CodeGenType("RunStepDeltaStepDetailsMessageCreationObject")]
internal partial class InternalRunStepDeltaStepDetailsMessageCreationObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsFileSearchObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFileSearchObject
{
    [CodeGenMember("FileSearch")]
    public InternalRunStepDetailsToolCallsFileSearchObjectFileSearch FileSearch { get; set; }
}

[CodeGenType("RunStepDeltaStepDetailsToolCallsFunctionObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsObjectToolCallsObject")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject { }

[CodeGenType("RunStepDeltaStepDetailsMessageCreationObjectMessageCreation")]
internal partial class InternalRunStepDeltaStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenType("RunStepDeltaStepDetails")]
internal partial class InternalRunStepDeltaStepDetails { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsFunctionObjectFunction")]
internal partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenType("AssistantsNamedToolChoiceFunction")]
internal partial class InternalAssistantsNamedToolChoiceFunction { }

[CodeGenType("AssistantObjectObject")]
internal readonly partial struct InternalAssistantObjectObject { }

[CodeGenType("MessageObjectObject")]
internal readonly partial struct InternalMessageObjectObject { }

[CodeGenType("RunObjectObject")]
internal readonly partial struct InternalRunObjectObject { }

[CodeGenType("RunStepObjectObject")]
internal readonly partial struct InternalRunStepObjectObject { }

[CodeGenType("ThreadObjectObject")]
internal readonly partial struct InternalThreadObjectObject { }

[CodeGenType("MessageRequestContentTextObjectType")]
internal readonly partial struct InternalMessageRequestContentTextObjectType { }

[CodeGenType("MessageContentImageUrlObjectImageUrlDetail")]
internal readonly partial struct InternalMessageContentImageUrlObjectImageUrlDetail { }

[CodeGenType("MessageContentImageFileObjectImageFileDetail")]
internal readonly partial struct InternalMessageContentImageFileObjectImageFileDetail { }

[CodeGenType("MessageDeltaContentImageFileObjectImageFileDetail")]
internal readonly partial struct InternalMessageDeltaContentImageFileObjectImageFileDetail { }

[CodeGenType("MessageDeltaContentImageUrlObjectImageUrlDetail")]
internal readonly partial struct InternalMessageDeltaContentImageUrlObjectImageUrlDetail { }

[CodeGenType("MessageDeltaObject")]
internal partial class InternalMessageDeltaObject { }

[CodeGenType("MessageDeltaObjectDeltaRole")]
internal readonly partial struct InternalMessageDeltaObjectDeltaRole { }

[CodeGenType("MessageDeltaObjectObject")]
internal readonly partial struct InternalMessageDeltaObjectObject { }

[CodeGenType("MessageObjectAttachment")]
internal partial class InternalMessageObjectAttachment { }

[CodeGenType("RunObjectRequiredAction1Type")]
internal readonly partial struct InternalRunObjectRequiredActionType { }

[CodeGenType("RunStepDeltaObjectObject")]
internal readonly partial struct InternalRunStepDeltaObjectObject { }

[CodeGenType("RunToolCallObjectType")]
internal readonly partial struct InternalRunToolCallObjectType { }

[CodeGenType("MessageObjectRole")]
internal readonly partial struct InternalMessageObjectRole { }

[CodeGenType("CreateAssistantRequestToolResources1")]
internal partial class InternalCreateAssistantRequestToolResources { }

[CodeGenType("CreateAssistantRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadAndRunRequestModel")]
internal readonly partial struct InternalCreateThreadAndRunRequestModel { }

[CodeGenType("CreateThreadAndRunRequestToolChoice1")]
internal readonly partial struct InternalCreateThreadAndRunRequestToolChoice { }

[CodeGenType("CreateThreadAndRunRequestToolResources1")]
internal partial class InternalCreateThreadAndRunRequestToolResources { }

[CodeGenType("CreateThreadAndRunRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateThreadAndRunRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadRequestToolResources1")]
internal partial class InternalCreateThreadRequestToolResources { }

[CodeGenType("CreateThreadRequestToolResourcesCodeInterpreter")]
internal partial class InternalCreateThreadRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadRequestToolResourcesFileSearchBase")]
internal partial class InternalCreateThreadRequestToolResourcesFileSearchBase { }

[CodeGenType("ModifyAssistantRequestToolResources1")]
internal partial class InternalModifyAssistantRequestToolResources { }

[CodeGenType("ModifyAssistantRequestToolResourcesCodeInterpreter")]
internal partial class InternalModifyAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenType("ModifyThreadRequestToolResources1")]
internal partial class InternalModifyThreadRequestToolResources { }

[CodeGenType("ModifyThreadRequestToolResourcesCodeInterpreter")]
internal partial class InternalModifyThreadRequestToolResourcesCodeInterpreter { }

[CodeGenType("ThreadObjectToolResources1")]
internal partial class InternalThreadObjectToolResources { }

[CodeGenType("ThreadObjectToolResourcesCodeInterpreter")]
internal partial class InternalThreadObjectToolResourcesCodeInterpreter { }

[CodeGenType("ThreadObjectToolResourcesFileSearch")]
internal partial class InternalThreadObjectToolResourcesFileSearch { }

[CodeGenType("AssistantToolsFileSearchTypeOnly")]
internal partial class InternalAssistantToolsFileSearchTypeOnly { }

[CodeGenType("AssistantToolsFileSearchTypeOnlyType")]
internal readonly partial struct InternalAssistantToolsFileSearchTypeOnlyType { }

[CodeGenType("DotNetAssistantResponseFormatText")]
internal partial class InternalDotNetAssistantResponseFormatText { }

[CodeGenType("DotNetAssistantResponseFormatJsonObject")]
internal partial class InternalDotNetAssistantResponseFormatJsonObject { }

[CodeGenType("DotNetAssistantResponseFormatJsonSchema")]
internal partial class InternalDotNetAssistantResponseFormatJsonSchema { }

[CodeGenType("DotNetAssistantResponseFormatJsonSchemaJsonSchema")]
internal partial class InternalDotNetAssistantResponseFormatJsonSchemaJsonSchema { }

[CodeGenType("UnknownDotNetAssistantResponseFormat")]
internal partial class InternalUnknownDotNetAssistantResponseFormat { }

[CodeGenType("MessageDeltaContentRefusalObject")]
internal partial class InternalMessageDeltaContentRefusalObject { }

[CodeGenType("ToolResourcesFileSearchIdsOnly")]
internal partial class InternalToolResourcesFileSearchIdsOnly { }

[CodeGenType("RunStepDetailsToolCallsFileSearchRankingOptionsObject")]
internal partial class InternalRunStepDetailsToolCallsFileSearchRankingOptionsObject { }

[CodeGenType("IncludedRunStepProperty")]
internal readonly partial struct InternalIncludedRunStepProperty { }

[CodeGenType("AssistantSupportedModels")] internal readonly partial struct InternalAssistantSupportedModels { }
[CodeGenType("AssistantToolDefinitionType")] internal readonly partial struct InternalAssistantToolDefinitionType {}
[CodeGenType("RunStepDetailsType")] internal readonly partial struct InternalRunStepDetailsType { }
[CodeGenType("RunStepDetailsCodeInterpreterOutputType")] internal readonly partial struct InternalRunStepDetailsCodeInterpreterOutputType { }
[CodeGenType("MessageContentTextAnnotationType")] internal readonly partial struct InternalMessageContentTextAnnotationType { }
[CodeGenType("MessageContentType")] internal readonly partial struct InternalMessageContentType { }
[CodeGenType("UnknownMessageContent")] internal partial class InternalUnknownMessageContent { }
[CodeGenType("MessageContentTextObjectText1")] internal partial class InternalMessageContentTextObjectText1 { }