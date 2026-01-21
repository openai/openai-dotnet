import "../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@alternateType(CreateResponse.conversation, `ConversationParam-2`);
@@alternateType(CreateResponse.`model`, string);
@@alternateType(CreateResponse.service_tier, DotNetResponseServiceTier);
@@alternateType(Response.`model`, string);
@@alternateType(Response.service_tier, DotNetResponseServiceTier);

// ------------ ItemResources ------------
@@usage(ItemResource, Usage.input | Usage.output);
@@visibility(ItemResource.id, Lifecycle.Read);

@@visibility(CodeInterpreterToolCallItemResource.status, Lifecycle.Read);
@@clientName(CodeInterpreterToolImageOutput.url, "ImageUri");

@@visibility(ComputerToolCallItemResource.status, Lifecycle.Read);

@@visibility(ComputerToolCallOutputItemResource.status, Lifecycle.Read);

@@visibility(FileSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(FunctionToolCallItemResource.status, Lifecycle.Read);
@@alternateType(FunctionToolCallItemResource.arguments, unknown);
@@clientName(FunctionToolCallItemResource.name, "FunctionName");
@@clientName(FunctionToolCallItemResource.arguments, "FunctionArguments");

@@visibility(FunctionToolCallOutputItemResource.status, Lifecycle.Read);
@@clientName(FunctionToolCallOutputItemResource.output, "FunctionOutput");

@@visibility(ReasoningItemResource.status, Lifecycle.Read);
@@clientName(ReasoningItemResource.summary, "SummaryParts");

@@visibility(WebSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(ResponsesMessageItemResource.status, Lifecycle.Read);

@@alternateType(MCPApprovalRequestItemResource.arguments, unknown);
@@clientName(MCPApprovalRequestItemResource.name, "ToolName");
@@clientName(MCPApprovalRequestItemResource.arguments, "ToolArguments");

@@clientName(MCPApprovalResponseItemResource.approve, "Approved");

@@alternateType(MCPCallItemResource.arguments, unknown);
@@alternateType(MCPCallItemResource.error, unknown | null);
@@clientName(MCPCallItemResource.name, "ToolName");
@@clientName(MCPCallItemResource.arguments, "ToolArguments");
@@clientName(MCPCallItemResource.output, "ToolOutput");

@@alternateType(MCPListToolsItemResource.error, unknown | null);
@@clientName(MCPListToolsItemResource.tools, "ToolDefinitions");

@@visibility(ImageGenToolCallItemResource.status, Lifecycle.Read);
@@clientName(ImageGenToolCallItemResource.result, "ImageResultBytes");

// ------------ Tools ------------

@@clientName(FileSearchTool.max_num_results, "MaxResultCount");

@@clientName(FunctionTool.name, "FunctionName");
@@clientName(FunctionTool.description, "FunctionDescription");
@@clientName(FunctionTool.parameters, "FunctionParameters");
@@clientName(FunctionTool.strict, "StrictModeEnabled");

@@alternateType(MCPTool.server_url, url);
@@alternateType(MCPTool.allowed_tools, MCPToolFilter | null);
@@clientName(MCPTool.server_url, "ServerUri");
@@clientName(MCPTool.authorization, "AuthorizationToken");

@@clientName(MCPToolFilter.read_only, "IsReadOnly");

@@clientName(Responses.listInputItems, "GetResponseInputItems");

// ------------ Streaming ------------

@@usage(ResponseStreamEvent, Usage.json);

@@alternateType(ResponseFunctionCallArgumentsDoneEvent.arguments, unknown);
@@clientName(ResponseFunctionCallArgumentsDoneEvent.arguments, "FunctionArguments");

@@alternateType(ResponseFunctionCallArgumentsDeltaEvent.delta, unknown);

@@alternateType(ResponseMCPCallArgumentsDoneEvent.arguments, unknown);
@@clientName(ResponseMCPCallArgumentsDoneEvent.arguments, "ToolArguments");

@@alternateType(ResponseMCPCallArgumentsDeltaEvent.delta, unknown);

@@clientName(ResponseImageGenCallPartialImageEvent.partial_image_b64, "PartialImageBytes");

// ------------ JsonPatch ------------

// models.tsp
@@dynamicModel(CreateResponse);
@@dynamicModel(Response);
@@dynamicModel(ResponseProperties);
@@dynamicModel(ItemParam);
@@dynamicModel(Tool);
@@dynamicModel(ToolChoiceObject);
@@dynamicModel(ToolChoiceObjectFileSearch);
@@dynamicModel(ToolChoiceObjectComputer);
@@dynamicModel(ToolChoiceObjectWebSearch);
@@dynamicModel(ToolChoiceObjectImageGen);
@@dynamicModel(ToolChoiceObjectCodeInterpreter);
@@dynamicModel(ToolChoiceObjectMCP);
@@dynamicModel(ToolChoiceObjectFunction);
@@dynamicModel(FunctionTool);
@@dynamicModel(FileSearchTool);
@@dynamicModel(ComputerUsePreviewTool);
@@dynamicModel(WebSearchPreviewTool);
@@dynamicModel(Location);
@@dynamicModel(ApproximateLocation);
@@dynamicModel(ComputerCallSafetyCheckParam);
@@dynamicModel(ComputerToolCallSafetyCheck);
@@dynamicModel(ComputerActionClick);
@@dynamicModel(ComputerAction);
@@dynamicModel(ComputerToolCallOutputItemOutput);
@@dynamicModel(ComputerToolCallOutputItemOutputComputerScreenshot);
@@dynamicModel(ComputerActionDoubleClick);
@@dynamicModel(ComputerActionDrag);
@@dynamicModel(ComputerActionMove);
@@dynamicModel(ComputerActionScreenshot);
@@dynamicModel(ComputerActionScroll);
@@dynamicModel(ComputerActionTypeKeys);
@@dynamicModel(ComputerActionWait);
@@dynamicModel(ComputerActionKeyPress);
@@dynamicModel(Coordinate);
@@dynamicModel(Reasoning);
@@dynamicModel(ResponseTextFormatConfiguration);
@@dynamicModel(ResponseTextFormatConfigurationText);
@@dynamicModel(ResponseTextFormatConfigurationJsonObject);
@@dynamicModel(ResponseTextFormatConfigurationJsonSchema);
@@dynamicModel(ItemContent);
@@dynamicModel(ItemContentInputAudio);
@@dynamicModel(InputMessageContentList);
@@dynamicModel(ItemContentOutputAudio);
@@dynamicModel(ItemContentRefusal);
@@dynamicModel(Annotation);
@@dynamicModel(AnnotationFileCitation);
@@dynamicModel(AnnotationUrlCitation);
@@dynamicModel(ContainerFileCitationBody);
@@dynamicModel(AnnotationFilePath);
@@dynamicModel(ItemContentInputText);
@@dynamicModel(ItemContentInputImage);
@@dynamicModel(ItemContentInputFile);
@@dynamicModel(RankingOptions);
@@dynamicModel(ItemContentOutputText);
@@dynamicModel(ResponseUsage);
@@dynamicModel(ResponseError);
@@dynamicModel(ResponseItemList);
@@dynamicModel(ResponseStreamEvent);
@@dynamicModel(ResponseCompletedEvent);
@@dynamicModel(ResponseContentPartAddedEvent);
@@dynamicModel(ResponseContentPartDoneEvent);
@@dynamicModel(ResponseCreatedEvent);
@@dynamicModel(ResponseErrorEvent);
@@dynamicModel(ResponseFileSearchCallCompletedEvent);
@@dynamicModel(ResponseFileSearchCallInProgressEvent);
@@dynamicModel(ResponseFileSearchCallSearchingEvent);
@@dynamicModel(ResponseFunctionCallArgumentsDeltaEvent);
@@dynamicModel(ResponseFunctionCallArgumentsDoneEvent);
@@dynamicModel(ResponseInProgressEvent);
@@dynamicModel(ResponseFailedEvent);
@@dynamicModel(ResponseIncompleteEvent);
@@dynamicModel(ResponseOutputItemAddedEvent);
@@dynamicModel(ResponseOutputItemDoneEvent);
@@dynamicModel(ResponseRefusalDeltaEvent);
@@dynamicModel(ResponseRefusalDoneEvent);
@@dynamicModel(ResponseTextDeltaEvent);
@@dynamicModel(ResponseTextDoneEvent);
@@dynamicModel(ResponseReasoningSummaryPartAddedEvent);
@@dynamicModel(ResponseReasoningSummaryPartDoneEvent);
@@dynamicModel(ResponseReasoningSummaryTextDeltaEvent);
@@dynamicModel(ResponseReasoningSummaryTextDoneEvent);
@@dynamicModel(ResponseWebSearchCallCompletedEvent);
@@dynamicModel(ResponseWebSearchCallInProgressEvent);
@@dynamicModel(ResponseWebSearchCallSearchingEvent);
@@dynamicModel(ItemResource);
@@dynamicModel(CodeInterpreterTool);
@@dynamicModel(CodeInterpreterToolAuto);
@@dynamicModel(ImageGenTool);
@@dynamicModel(LocalShellExecAction);
@@dynamicModel(LocalShellTool);
@@dynamicModel(MCPListToolsTool);
@@dynamicModel(MCPTool);
@@dynamicModel(MCPToolFilter);
@@dynamicModel(ResponseImageGenCallCompletedEvent);
@@dynamicModel(ResponseImageGenCallGeneratingEvent);
@@dynamicModel(ResponseImageGenCallInProgressEvent);
@@dynamicModel(ResponseImageGenCallPartialImageEvent);
@@dynamicModel(ResponseMCPCallArgumentsDeltaEvent);
@@dynamicModel(ResponseMCPCallArgumentsDoneEvent);
@@dynamicModel(ResponseMCPCallCompletedEvent);
@@dynamicModel(ResponseMCPCallFailedEvent);
@@dynamicModel(ResponseMCPCallInProgressEvent);
@@dynamicModel(ResponseMCPListToolsCompletedEvent);
@@dynamicModel(ResponseMCPListToolsFailedEvent);
@@dynamicModel(ResponseMCPListToolsInProgressEvent);
@@dynamicModel(ResponseOutputTextAnnotationAddedEvent);
@@dynamicModel(ResponseQueuedEvent);
@@dynamicModel(ResponseReasoningDeltaEvent);
@@dynamicModel(ResponseReasoningDoneEvent);
@@dynamicModel(ResponseReasoningSummaryDeltaEvent);
@@dynamicModel(ResponseReasoningSummaryDoneEvent);
@@dynamicModel(CodeInterpreterToolLogsOutput);
@@dynamicModel(CodeInterpreterToolImageOutput);
@@dynamicModel(ResponseAudioDeltaEvent);
@@dynamicModel(ResponseAudioDoneEvent);
@@dynamicModel(ResponseAudioTranscriptDeltaEvent);
@@dynamicModel(ResponseAudioTranscriptDoneEvent);
@@dynamicModel(ResponseCodeInterpreterCallCodeDeltaEvent);
@@dynamicModel(ResponseCodeInterpreterCallCodeDoneEvent);
@@dynamicModel(ResponseCodeInterpreterCallCompletedEvent);
@@dynamicModel(ResponseCodeInterpreterCallInProgressEvent);
@@dynamicModel(ResponseCodeInterpreterCallInterpretingEvent);
@@dynamicModel(TopLogProb);
@@dynamicModel(LogProb);
@@dynamicModel(WebSearchToolFilters);
// custom.tsp models
@@dynamicModel(ResponseErrorResponse);
@@dynamicModel(DeleteResponseResponse);
@@dynamicModel(ReasoningItemSummaryPart);
@@dynamicModel(ReasoningItemSummaryTextPart);
@@dynamicModel(CodeInterpreterToolOutput);
@@dynamicModel(CodeInterpreterContainerConfiguration);
@@dynamicModel(ReasoningItemSummaryPartType);
@@dynamicModel(CodeInterpreterToolOutputType);
@@dynamicModel(CodeInterpreterContainerConfigurationType);
// custom/items.tsp models
@@dynamicModel(FileSearchToolCallItemParam);
@@dynamicModel(FileSearchToolCallItemResource);
@@dynamicModel(ComputerToolCallItemParam);
@@dynamicModel(ComputerToolCallItemResource);
@@dynamicModel(ComputerToolCallOutputItemParam);
@@dynamicModel(ComputerToolCallOutputItemResource);
@@dynamicModel(WebSearchToolCallItemParam);
@@dynamicModel(WebSearchToolCallItemResource);
@@dynamicModel(FunctionToolCallItemParam);
@@dynamicModel(FunctionToolCallItemResource);
@@dynamicModel(FunctionToolCallOutputItemParam);
@@dynamicModel(FunctionToolCallOutputItemResource);
@@dynamicModel(ReasoningItemParam);
@@dynamicModel(ReasoningItemResource);
@@dynamicModel(ItemReferenceItemParam);
@@dynamicModel(ImageGenToolCallItemParam);
@@dynamicModel(ImageGenToolCallItemResource);
@@dynamicModel(CodeInterpreterToolCallItemParam);
@@dynamicModel(CodeInterpreterToolCallItemResource);
@@dynamicModel(LocalShellToolCallItemParam);
@@dynamicModel(LocalShellToolCallItemResource);
@@dynamicModel(LocalShellToolCallOutputItemParam);
@@dynamicModel(LocalShellToolCallOutputItemResource);
@@dynamicModel(MCPListToolsItemParam);
@@dynamicModel(MCPListToolsItemResource);
@@dynamicModel(MCPApprovalRequestItemParam);
@@dynamicModel(MCPApprovalRequestItemResource);
@@dynamicModel(MCPApprovalResponseItemParam);
@@dynamicModel(MCPApprovalResponseItemResource);
@@dynamicModel(MCPCallItemParam);
@@dynamicModel(MCPCallItemResource);
// custom/items.messages.tsp models
@@dynamicModel(ResponsesMessageItemParam);
@@dynamicModel(ResponsesUserMessageItemParam);
@@dynamicModel(ResponsesSystemMessageItemParam);
@@dynamicModel(ResponsesDeveloperMessageItemParam);
@@dynamicModel(ResponsesAssistantMessageItemParam);
@@dynamicModel(ResponsesMessageItemResource);
@@dynamicModel(ResponsesUserMessageItemResource);
@@dynamicModel(ResponsesSystemMessageItemResource);
@@dynamicModel(ResponsesDeveloperMessageItemResource);
@@dynamicModel(ResponsesAssistantMessageItemResource);
@@dynamicModel(ImplicitUserMessage);
// base/entrypoints/sdk.dotnet/specialized-types.tsp models
@@dynamicModel(ComparisonFilter);
@@dynamicModel(ComparisonFilterEquals);
@@dynamicModel(ComparisonFilterNotEquals);
@@dynamicModel(ComparisonFilterGreaterThan);
@@dynamicModel(ComparisonFilterGreaterThanOrEquals);
@@dynamicModel(ComparisonFilterLessThan);
@@dynamicModel(ComparisonFilterLessThanOrEquals);
@@dynamicModel(CompoundFilter);
@@dynamicModel(CompoundFilterAnd);
@@dynamicModel(CompoundFilterOr);
// client/models/responses.models.tsp models
@@dynamicModel(DotNetItemReferenceItemResource);
@@dynamicModel(DotNetResponseItemCollectionOrder);
@@dynamicModel(DotNetGetResponseOptions);
@@dynamicModel(DotNetGlobalToolCallApprovalPolicy);
@@dynamicModel(DotNetCustomToolCallApprovalPolicy);
@@dynamicModel(DotNetToolCallApprovalPolicy);
@@dynamicModel(DotNetCodeInterpreterToolContainer);
// client/models/common.models.tsp models
@@dynamicModel(DotNetResponseReasoningEffortLevel);
@@dynamicModel(DotNetResponseServiceTier);