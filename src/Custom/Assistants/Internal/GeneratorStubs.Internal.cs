namespace OpenAI.Assistants;

/*
 * This file stubs and performs minimal customization to generated internal types for the OpenAI.Assistants namespace.
 */

[CodeGenType("SubmitToolOutputsRunRequest")]
public partial class InternalSubmitToolOutputsRunRequest { }

[CodeGenType("MessageContentTextObjectAnnotation")]
public partial class InternalMessageContentTextObjectAnnotation { }

[CodeGenType("MessageContentTextAnnotationsFileCitationObject")]
public partial class InternalMessageContentTextAnnotationsFileCitationObject { }

[CodeGenType("MessageContentTextAnnotationsFilePathObject")]
public partial class InternalMessageContentTextAnnotationsFilePathObject { }

[CodeGenType("MessageDeltaContentImageFileObjectImageFile")]
public partial class InternalMessageDeltaContentImageFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenType("MessageDeltaContentImageUrlObjectImageUrl")]
public partial class InternalMessageDeltaContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenType("MessageDeltaContentImageFileObject")]
public partial class InternalMessageDeltaContentImageFileObject { }

[CodeGenType("MessageDeltaContentImageUrlObject")]
public partial class InternalMessageDeltaContentImageUrlObject { }

[CodeGenType("MessageDeltaObjectDelta")]
public partial class InternalMessageDeltaObjectDelta
{
    [CodeGenMember("Role")]
    internal MessageRole? Role { get; }
}

[CodeGenType("MessageDeltaContentTextObject")]
public partial class InternalMessageDeltaContentTextObject { }

[CodeGenType("MessageDeltaContentTextObjectText")]
public partial class InternalMessageDeltaContentTextObjectText { }

[CodeGenType("MessageDeltaContentTextAnnotationsFileCitationObject")]
public partial class InternalMessageDeltaContentTextAnnotationsFileCitationObject { }

[CodeGenType("MessageDeltaTextContentAnnotation")]
public partial class InternalMessageDeltaTextContentAnnotation { }

[CodeGenType("MessageDeltaContentTextAnnotationsFileCitationObjectFileCitation")]
public partial class InternalMessageDeltaContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenType("RunStepDeltaObject")]
public partial class InternalRunStepDelta { private readonly object Object; }

[CodeGenType("RunStepDeltaObjectDelta")]
public partial class InternalRunStepDeltaObjectDelta { }

[CodeGenType("MessageDeltaContentTextAnnotationsFilePathObject")]
public partial class InternalMessageDeltaContentTextAnnotationsFilePathObject { }

[CodeGenType("MessageDeltaContentTextAnnotationsFilePathObjectFilePath")]
public partial class InternalMessageDeltaContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenType("MessageDeltaContent")]
public partial class InternalMessageDeltaContent { }

[CodeGenType("DeleteAssistantResponseObject")]
public readonly partial struct InternalDeleteAssistantResponseObject { }

[CodeGenType("DeleteThreadResponseObject")]
public readonly partial struct InternalDeleteThreadResponseObject { }

[CodeGenType("DeleteMessageResponseObject")]
public readonly partial struct InternalDeleteMessageResponseObject { }

[CodeGenType("CreateThreadAndRunRequest")]
public partial class InternalCreateThreadAndRunRequest
{
    public string Model { get; set; }
    public ToolResources ToolResources { get; set; }
    public AssistantResponseFormat ResponseFormat { get; set; }
    public ToolConstraint ToolChoice { get; set; }
}

[CodeGenType("MessageContentImageUrlObjectImageUrl")]
public partial class InternalMessageContentImageUrlObjectImageUrl
{
    [CodeGenMember("Detail")]
    internal string Detail { get; }
}

[CodeGenType("MessageContentImageFileObjectImageFile")]
public partial class InternalMessageContentItemFileObjectImageFile
{
    [CodeGenMember("Detail")]
    internal string Detail { get; set; }
}

[CodeGenType("MessageContentRefusalObjectType")]
public readonly partial struct InternalMessageContentRefusalObjectType { }

[CodeGenType("RunStepDetailsMessageCreationObjectMessageCreation")]
public partial class InternalRunStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenType("RunStepDetailsToolCallsFunctionObjectFunction")]
public partial class InternalRunStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenType("RunStepDetailsToolCallsCodeObjectCodeInterpreter")]
public partial class InternalRunStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenType("RunStepDetailsToolCallsCodeOutputImageObjectImage")]
public partial class InternalRunStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenType("MessageContentTextAnnotationsFileCitationObjectFileCitation")]
public partial class InternalMessageContentTextAnnotationsFileCitationObjectFileCitation { }

[CodeGenType("MessageContentTextAnnotationsFilePathObjectFilePath")]
public partial class InternalMessageContentTextAnnotationsFilePathObjectFilePath { }

[CodeGenType("RunObjectRequiredAction1")]
public partial class InternalRunRequiredAction { private readonly object Type; }

[CodeGenType("RunObjectRequiredActionSubmitToolOutputs")]
public partial class InternalRunObjectRequiredActionSubmitToolOutputs { private readonly object Type; }

[CodeGenType("RunToolCallObjectFunction")]
public partial class InternalRunToolCallObjectFunction { }

[CodeGenType("ListAssistantsResponse")]
public partial class InternalListAssistantsResponse : IInternalListResponse<Assistant> { }

[CodeGenType("ListAssistantsResponseObject")]
public readonly partial struct InternalListAssistantsResponseObject { }

[CodeGenType("ListMessagesResponse")]
public partial class InternalListMessagesResponse : IInternalListResponse<ThreadMessage> { }

[CodeGenType("ListMessagesResponseObject")]
public readonly partial struct InternalListMessagesResponseObject { }

[CodeGenType("ListRunsResponse")]
public partial class InternalListRunsResponse : IInternalListResponse<ThreadRun> { }

[CodeGenType("ListRunsResponseObject")]
public readonly partial struct InternalListRunsResponseObject { }

[CodeGenType("ListRunStepsResponse")]
public partial class InternalListRunStepsResponse : IInternalListResponse<RunStep> { }

[CodeGenType("ListRunStepsResponseObject")]
public readonly partial struct InternalListRunStepsResponseObject { }

[CodeGenType("RunStepDetailsToolCallsCodeObject")]
public partial class InternalRunStepDetailsToolCallsCodeObject { }

[CodeGenType("RunStepDetailsToolCallsFileSearchObject")]
public partial class InternalRunStepDetailsToolCallsFileSearchObject { }

[CodeGenType("RunStepDetailsToolCallsFunctionObject")]
public partial class InternalRunStepDetailsToolCallsFunctionObject { }

[CodeGenType("TruncationObjectType")]
public readonly partial struct InternalTruncationObjectType { }

[CodeGenType("AssistantsNamedToolChoiceType")]
public readonly partial struct InternalAssistantsNamedToolChoiceType { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsCodeObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputImageObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputLogsObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputLogsObject
{
    [CodeGenMember("Logs")]
    public string InternalLogs { get; set; }
}

[CodeGenType("RunStepDeltaStepDetailsMessageCreationObject")]
public partial class InternalRunStepDeltaStepDetailsMessageCreationObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsFileSearchObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsFileSearchObject
{
    [CodeGenMember("FileSearch")]
    public InternalRunStepDetailsToolCallsFileSearchObjectFileSearch FileSearch { get; set; }
}

[CodeGenType("RunStepDeltaStepDetailsToolCallsFunctionObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObject { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsObjectToolCallsObject")]
public partial class InternalRunStepDeltaStepDetailsToolCallsObjectToolCallsObject { }

[CodeGenType("RunStepDeltaStepDetailsMessageCreationObjectMessageCreation")]
public partial class InternalRunStepDeltaStepDetailsMessageCreationObjectMessageCreation { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter")]
public partial class InternalRunStepDeltaStepDetailsToolCallsCodeObjectCodeInterpreter { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage")]
public partial class InternalRunStepDeltaStepDetailsToolCallsCodeOutputImageObjectImage { }

[CodeGenType("RunStepDeltaStepDetails")]
public partial class InternalRunStepDeltaStepDetails { }

[CodeGenType("RunStepDeltaStepDetailsToolCallsFunctionObjectFunction")]
public partial class InternalRunStepDeltaStepDetailsToolCallsFunctionObjectFunction { }

[CodeGenType("AssistantsNamedToolChoiceFunction")]
public partial class InternalAssistantsNamedToolChoiceFunction { }

[CodeGenType("AssistantObjectObject")]
public readonly partial struct InternalAssistantObjectObject { }

[CodeGenType("MessageObjectObject")]
public readonly partial struct InternalMessageObjectObject { }

[CodeGenType("RunObjectObject")]
public readonly partial struct InternalRunObjectObject { }

[CodeGenType("RunStepObjectObject")]
public readonly partial struct InternalRunStepObjectObject { }

[CodeGenType("ThreadObjectObject")]
public readonly partial struct InternalThreadObjectObject { }

[CodeGenType("MessageRequestContentTextObjectType")]
public readonly partial struct InternalMessageRequestContentTextObjectType { }

[CodeGenType("MessageContentImageUrlObjectImageUrlDetail")]
public readonly partial struct InternalMessageContentImageUrlObjectImageUrlDetail { }

[CodeGenType("MessageContentImageFileObjectImageFileDetail")]
public readonly partial struct InternalMessageContentImageFileObjectImageFileDetail { }

[CodeGenType("MessageDeltaContentImageFileObjectImageFileDetail")]
public readonly partial struct InternalMessageDeltaContentImageFileObjectImageFileDetail { }

[CodeGenType("MessageDeltaContentImageUrlObjectImageUrlDetail")]
public readonly partial struct InternalMessageDeltaContentImageUrlObjectImageUrlDetail { }

[CodeGenType("MessageDeltaObject")]
public partial class InternalMessageDeltaObject { }

[CodeGenType("MessageDeltaObjectDeltaRole")]
public readonly partial struct InternalMessageDeltaObjectDeltaRole { }

[CodeGenType("MessageDeltaObjectObject")]
public readonly partial struct InternalMessageDeltaObjectObject { }

[CodeGenType("MessageObjectAttachment")]
public partial class InternalMessageObjectAttachment { }

[CodeGenType("RunObjectRequiredAction1Type")]
public readonly partial struct InternalRunObjectRequiredActionType { }

[CodeGenType("RunStepDeltaObjectObject")]
public readonly partial struct InternalRunStepDeltaObjectObject { }

[CodeGenType("RunToolCallObjectType")]
public readonly partial struct InternalRunToolCallObjectType { }

[CodeGenType("MessageObjectRole")]
public readonly partial struct InternalMessageObjectRole { }

[CodeGenType("CreateAssistantRequestToolResources1")]
public partial class InternalCreateAssistantRequestToolResources { }

[CodeGenType("CreateAssistantRequestToolResourcesCodeInterpreter")]
public partial class InternalCreateAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadAndRunRequestModel")]
public readonly partial struct InternalCreateThreadAndRunRequestModel { }

[CodeGenType("CreateThreadAndRunRequestToolChoice1")]
public readonly partial struct InternalCreateThreadAndRunRequestToolChoice { }

[CodeGenType("CreateThreadAndRunRequestToolResources1")]
public partial class InternalCreateThreadAndRunRequestToolResources { }

[CodeGenType("CreateThreadAndRunRequestToolResourcesCodeInterpreter")]
public partial class InternalCreateThreadAndRunRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadRequestToolResources1")]
public partial class InternalCreateThreadRequestToolResources { }

[CodeGenType("CreateThreadRequestToolResourcesCodeInterpreter")]
public partial class InternalCreateThreadRequestToolResourcesCodeInterpreter { }

[CodeGenType("CreateThreadRequestToolResourcesFileSearchBase")]
public partial class InternalCreateThreadRequestToolResourcesFileSearchBase { }

[CodeGenType("ModifyAssistantRequestToolResources1")]
public partial class InternalModifyAssistantRequestToolResources { }

[CodeGenType("ModifyAssistantRequestToolResourcesCodeInterpreter")]
public partial class InternalModifyAssistantRequestToolResourcesCodeInterpreter { }

[CodeGenType("ModifyThreadRequestToolResources1")]
public partial class InternalModifyThreadRequestToolResources { }

[CodeGenType("ModifyThreadRequestToolResourcesCodeInterpreter")]
public partial class InternalModifyThreadRequestToolResourcesCodeInterpreter { }

[CodeGenType("ThreadObjectToolResources1")]
public partial class InternalThreadObjectToolResources { }

[CodeGenType("ThreadObjectToolResourcesCodeInterpreter")]
public partial class InternalThreadObjectToolResourcesCodeInterpreter { }

[CodeGenType("ThreadObjectToolResourcesFileSearch")]
public partial class InternalThreadObjectToolResourcesFileSearch { }

[CodeGenType("AssistantToolsFileSearchTypeOnly")]
public partial class InternalAssistantToolsFileSearchTypeOnly { }

[CodeGenType("AssistantToolsFileSearchTypeOnlyType")]
public readonly partial struct InternalAssistantToolsFileSearchTypeOnlyType { }

[CodeGenType("DotNetAssistantResponseFormatText")]
public partial class InternalDotNetAssistantResponseFormatText { }

[CodeGenType("DotNetAssistantResponseFormatJsonObject")]
public partial class InternalDotNetAssistantResponseFormatJsonObject { }

[CodeGenType("DotNetAssistantResponseFormatJsonSchema")]
public partial class InternalDotNetAssistantResponseFormatJsonSchema { }

[CodeGenType("DotNetAssistantResponseFormatJsonSchemaJsonSchema")]
public partial class InternalDotNetAssistantResponseFormatJsonSchemaJsonSchema { }

[CodeGenType("UnknownDotNetAssistantResponseFormat")]
public partial class InternalUnknownDotNetAssistantResponseFormat { }

[CodeGenType("MessageDeltaContentRefusalObject")]
public partial class InternalMessageDeltaContentRefusalObject { }

[CodeGenType("ToolResourcesFileSearchIdsOnly")]
public partial class InternalToolResourcesFileSearchIdsOnly { }

[CodeGenType("RunStepDetailsToolCallsFileSearchRankingOptionsObject")]
public partial class InternalRunStepDetailsToolCallsFileSearchRankingOptionsObject { }

[CodeGenType("IncludedRunStepProperty")]
public readonly partial struct InternalIncludedRunStepProperty { }

[CodeGenType("AssistantSupportedModels")] public readonly partial struct InternalAssistantSupportedModels { }
[CodeGenType("AssistantToolDefinitionType")] public readonly partial struct InternalAssistantToolDefinitionType {}
[CodeGenType("RunStepDetailsType")] public readonly partial struct InternalRunStepDetailsType { }
[CodeGenType("RunStepDetailsCodeInterpreterOutputType")] public readonly partial struct InternalRunStepDetailsCodeInterpreterOutputType { }
[CodeGenType("MessageContentTextAnnotationType")] public readonly partial struct InternalMessageContentTextAnnotationType { }
[CodeGenType("MessageContentType")] public readonly partial struct InternalMessageContentType { }
[CodeGenType("UnknownMessageContent")] public partial class InternalUnknownMessageContent { }
[CodeGenType("MessageContentTextObjectText1")] public partial class InternalMessageContentTextObjectText1 { }