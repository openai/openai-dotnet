import "../base/typespec/agents/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

// ------------ Client and method names ------------

@@clientLocation(createAgent, "Agents");
@@clientLocation(retrieveAgent, "Agents");
@@clientLocation(updateAgent, "Agents");
@@clientLocation(deleteAgent, "Agents");
@@clientLocation(listAgents, "Agents");
@@clientLocation(createAgentSession, "Agents");
@@clientLocation(retrieveAgentSession, "Agents");
@@clientLocation(updateAgentSession, "Agents");
@@clientLocation(deleteAgentSession, "Agents");
@@clientLocation(listAgentSessions, "Agents");
@@clientLocation(listAgentSessionArtifacts, "Agents");
@@clientLocation(retrieveAgentSessionArtifact, "Agents");
@@clientLocation(deleteAgentSessionArtifact, "Agents");
@@clientLocation(retrieveAgentSessionArtifactContent, "Agents");
@@clientLocation(listAgentSessionTurns, "Agents");
@@clientLocation(retrieveAgentSessionTurn, "Agents");
@@clientLocation(listAgentSessionItems, "Agents");
@@clientLocation(listAgentSessionSubagents, "Agents");
@@clientLocation(retrieveAgentSessionSubagent, "Agents");
@@clientLocation(listAgentSessionSubagentItems, "Agents");
@@clientLocation(listAgentSessionSubagentTurns, "Agents");
@@clientLocation(retrieveAgentSessionSubagentTurn, "Agents");
@@clientLocation(listAgentSessionSubagentTurnItems, "Agents");
@@clientLocation(listAgentSessionEvents, "Agents");
@@clientLocation(createAgentSessionEvents, "Agents");

// ------------ Model names ------------

@@clientName(CreateAgentParams, "AgentCreationOptions");
@@clientName(UpdateAgentParams, "AgentModificationOptions");
@@clientName(AgentResource, "Agent");
@@clientName(DeletedAgentResource, "AgentDeletionResult");
@@clientName(DeletedAgentResource.id, "AgentId");
@@clientName(AgentCollectionOrder.asc, "Ascending");
@@clientName(AgentCollectionOrder.desc, "Descending");

@@clientName(SessionResource, "AgentSession");
@@clientName(CreateAgentSessionParams, "AgentSessionCreationOptions");
@@clientName(UpdateAgentSessionParams, "AgentSessionModificationOptions");
@@clientName(DeletedSessionResource, "AgentSessionDeletionResult");
@@clientName(DeletedSessionResource.id, "AgentSessionId");
@@clientName(SessionListResource, "AgentSessionCollectionPage");
@@clientName(AgentSessionCollectionOrder.asc, "Ascending");
@@clientName(AgentSessionCollectionOrder.desc, "Descending");

@@clientName(SessionArtifactResource, "AgentSessionArtifact");
@@clientName(DeletedSessionArtifactResource, "AgentSessionArtifactDeletionResult");
@@clientName(DeletedSessionArtifactResource.id, "AgentSessionArtifactId");
@@clientName(SessionArtifactListResource, "AgentSessionArtifactCollectionPage");
@@clientName(AgentSessionArtifactCollectionOrder.asc, "Ascending");
@@clientName(AgentSessionArtifactCollectionOrder.desc, "Descending");

@@clientName(TurnResource, "AgentSessionTurn");
@@clientName(SessionTurnListResource, "AgentSessionTurnCollectionPage");
@@clientName(AgentSessionTurnCollectionOrder.asc, "Ascending");
@@clientName(AgentSessionTurnCollectionOrder.desc, "Descending");

@@clientName(SessionTurnItemResource, "AgentSessionItem");
@@clientName(SessionItemListResource, "AgentSessionItemCollectionPage");
@@clientName(AgentSessionItemCollectionOrder.asc, "Ascending");
@@clientName(AgentSessionItemCollectionOrder.desc, "Descending");

@@clientName(SubagentResource, "AgentSessionSubagent");
@@clientName(SubagentListResource, "AgentSessionSubagentCollectionPage");
@@clientName(AgentSessionSubagentCollectionOrder.asc, "Ascending");
@@clientName(AgentSessionSubagentCollectionOrder.desc, "Descending");

// ------------ Free-form JSON payloads ------------

// Function schemas and MCP metadata are arbitrary JSON; surface them as BinaryData.
@@alternateType(PersistedAgentToolConfigParamFunction.parameters, unknown);
@@alternateType(PersistedAgentToolResourceFunction.parameters, unknown);
@@alternateType(TextFormatParamJsonSchema.schema, unknown);
@@alternateType(TextFormatResourceJsonSchema.schema, unknown);
@@alternateType(PersistedAgentToolConfigParamMcp.request_metadata, unknown | null);
@@alternateType(PersistedAgentToolResourceMcp.request_metadata, unknown);
@@alternateType(AgentToolConfigParamFunction.parameters, unknown);
@@alternateType(AgentToolResourceFunction.parameters, unknown);
@@alternateType(AgentToolConfigParamMcp.request_metadata, unknown | null);
@@alternateType(AgentToolResourceMcp.request_metadata, unknown);
@@alternateType(SessionRequiredActionResourceFunctionCall.arguments, unknown);

// ------------ Dynamic models ------------

@@dynamicModel(CreateAgentParams);
@@dynamicModel(UpdateAgentParams);
@@dynamicModel(AgentResource);
@@dynamicModel(DeletedAgentResource);
@@dynamicModel(AgentCollection);
@@dynamicModel(ReasoningParam);
@@dynamicModel(ReasoningResource);
@@dynamicModel(TextParam);
@@dynamicModel(TextResource);
@@dynamicModel(TextFormatParam);
@@dynamicModel(TextFormatParamText);
@@dynamicModel(TextFormatParamJsonSchema);
@@dynamicModel(TextFormatResource);
@@dynamicModel(TextFormatResourceText);
@@dynamicModel(TextFormatResourceJsonSchema);
@@dynamicModel(MultiAgentConfigCurrentParam);
@@dynamicModel(MultiAgentConfigResource);
@@dynamicModel(WebSearchLocationParam);
@@dynamicModel(WebSearchLocationResource);
@@dynamicModel(PersistedAgentToolConfigParam);
@@dynamicModel(PersistedAgentToolConfigParamFunction);
@@dynamicModel(PersistedAgentToolConfigParamToolSearch);
@@dynamicModel(PersistedAgentToolConfigParamProgrammaticToolCalling);
@@dynamicModel(PersistedAgentToolConfigParamMcp);
@@dynamicModel(PersistedAgentToolConfigParamWebSearch);
@@dynamicModel(PersistedAgentToolResource);
@@dynamicModel(PersistedAgentToolResourceFunction);
@@dynamicModel(PersistedAgentToolResourceToolSearch);
@@dynamicModel(PersistedAgentToolResourceProgrammaticToolCalling);
@@dynamicModel(PersistedAgentToolResourceMcp);
@@dynamicModel(PersistedAgentToolResourceWebSearch);
@@dynamicModel(PersistedMcpTransportConfigParam);
@@dynamicModel(PersistedMcpTransportConfigParamHttp);
@@dynamicModel(PersistedMcpTransportConfigParamStdio);
@@dynamicModel(PersistedMcpTransportResource);
@@dynamicModel(PersistedMcpTransportResourceHttp);
@@dynamicModel(PersistedMcpTransportResourceStdio);

@@dynamicModel(SessionResource);
@@dynamicModel(CreateAgentSessionParams);
@@dynamicModel(UpdateAgentSessionParams);
@@dynamicModel(DeletedSessionResource);
@@dynamicModel(SessionListResource);
@@dynamicModel(SessionAgentResource);
@@dynamicModel(SessionAgentConfigParam);
@@dynamicModel(SessionRequiredActionResource);
@@dynamicModel(SessionRequiredActionResourceFunctionCall);
@@dynamicModel(SessionRequiredActionResourceEnvironmentConnection);
@@dynamicModel(TokenUsageResource);
@@dynamicModel(InputTokensDetailsResource);
@@dynamicModel(OutputTokensDetailsResource);
@@dynamicModel(AgentToolConfigParam);
@@dynamicModel(AgentToolConfigParamFunction);
@@dynamicModel(AgentToolConfigParamToolSearch);
@@dynamicModel(AgentToolConfigParamProgrammaticToolCalling);
@@dynamicModel(AgentToolConfigParamMcp);
@@dynamicModel(AgentToolConfigParamWebSearch);
@@dynamicModel(AgentToolResource);
@@dynamicModel(AgentToolResourceFunction);
@@dynamicModel(AgentToolResourceProgrammaticToolCalling);
@@dynamicModel(AgentToolResourceMcp);
@@dynamicModel(AgentToolResourceWebSearch);
@@dynamicModel(McpTransportConfigParam);
@@dynamicModel(McpTransportConfigParamHttp);
@@dynamicModel(McpTransportConfigParamStdio);
@@dynamicModel(McpTransportResource);
@@dynamicModel(McpTransportResourceHttp);
@@dynamicModel(McpTransportResourceStdio);
@@dynamicModel(InputContentParam);
@@dynamicModel(InputContentParamInputText);
@@dynamicModel(InputContentParamInputImage);
@@dynamicModel(InputMessageParam);

@@dynamicModel(SessionArtifactResource);
@@dynamicModel(DeletedSessionArtifactResource);
@@dynamicModel(SessionArtifactListResource);

@@dynamicModel(TurnResource);
@@dynamicModel(SessionTurnErrorResource);
@@dynamicModel(SessionTurnListResource);

@@dynamicModel(SessionTurnItemResource);
@@dynamicModel(SessionItemListResource);
@@dynamicModel(MessageItemResource);
@@dynamicModel(AgentSessionReasoningItemResource);
@@dynamicModel(FunctionCallItemResource);
@@dynamicModel(FunctionCallOutputItemResource);
@@dynamicModel(AgentMessageItemResource);
@@dynamicModel(McpCallItemResource);
@@dynamicModel(WebSearchCallItemResource);
@@dynamicModel(CommandExecutionItemResource);
@@dynamicModel(CreateSubagentCallItemResource);
@@dynamicModel(SendSubagentInputCallItemResource);
@@dynamicModel(ResumeSubagentCallItemResource);
@@dynamicModel(WaitForSubagentsCallItemResource);
@@dynamicModel(InterruptSubagentCallItemResource);
@@dynamicModel(CloseSubagentCallItemResource);
@@dynamicModel(MessageContentResource);
@@dynamicModel(MessageContentResourceInputText);
@@dynamicModel(MessageContentResourceInputImage);
@@dynamicModel(MessageContentResourceOutputText);
@@dynamicModel(AgentContentResource);
@@dynamicModel(OutputTextResource);
@@dynamicModel(EncryptedContentResource);
@@dynamicModel(InputContentResource);
@@dynamicModel(InputContentResourceInputText);
@@dynamicModel(InputContentResourceInputImage);
@@dynamicModel(SummaryTextResource);
@@dynamicModel(WebSearchActionResource);
@@dynamicModel(WebSearchActionResourceSearch);
@@dynamicModel(WebSearchActionResourceOpenPage);
@@dynamicModel(WebSearchActionResourceFindInPage);
@@dynamicModel(WebSearchActionResourceOther);

@@dynamicModel(SubagentResource);
@@dynamicModel(SubagentListResource);

// Session events (streamed as raw SSE; models are deserialization targets that keep spec names).
@@dynamicModel(SessionErrorResource);
@@dynamicModel(SessionEnvironmentErrorResource);
@@dynamicModel(SessionEnvironmentStateResource);
@@dynamicModel(SessionEventError);
@@dynamicModel(SessionEventAgentSessionEnvironmentReady);
@@dynamicModel(SessionEventAgentOutputCommandExecutionOutputDelta);
@@dynamicModel(SessionEventAgentSessionCreated);
@@dynamicModel(SessionEventAgentSessionTurnCreated);
@@dynamicModel(SessionEventAgentSessionTurnInProgress);
@@dynamicModel(SessionEventAgentSessionTurnCompleted);
@@dynamicModel(SessionEventAgentSessionTurnFailed);
@@dynamicModel(SessionEventAgentSessionTurnCancelled);
@@dynamicModel(SessionEventAgentSessionTurnItemAdded);
@@dynamicModel(SessionEventAgentSessionIdle);
@@dynamicModel(SessionEventAgentSessionInProgress);
@@dynamicModel(SessionEventAgentSessionRequiresAction);
@@dynamicModel(SessionEventAgentSessionFailed);
@@dynamicModel(SessionEventAgentSessionEnvironmentPending);
@@dynamicModel(SessionEventAgentSessionEnvironmentConnected);
@@dynamicModel(SessionEventAgentSessionEnvironmentDisconnected);
@@dynamicModel(SessionEventAgentSessionEnvironmentFailed);
@@dynamicModel(SessionEventAgentSessionSubagentCreated);
@@dynamicModel(SessionEventAgentSessionSubagentActive);
@@dynamicModel(SessionEventAgentSessionSubagentClosed);
@@dynamicModel(SessionEventAgentSessionTurnItemDone);
@@dynamicModel(SessionEventAgentSessionTurnContentPartAdded);
@@dynamicModel(SessionEventAgentSessionTurnContentPartDone);
@@dynamicModel(SessionEventAgentSessionTurnOutputTextDelta);
@@dynamicModel(SessionEventAgentSessionTurnOutputTextDone);
@@dynamicModel(SessionEventAgentSessionTurnReasoningSummaryPartAdded);
@@dynamicModel(SessionEventAgentSessionTurnReasoningSummaryPartDone);
@@dynamicModel(SessionEventAgentSessionTurnReasoningSummaryTextDelta);
@@dynamicModel(SessionEventAgentSessionTurnReasoningSummaryTextDone);

// Session input events.
@@dynamicModel(SessionInputParam);
@@dynamicModel(SessionInputParamAgentSessionInputMessage);
@@dynamicModel(SessionInputParamAgentSessionInputCancel);
@@dynamicModel(SessionInputParamAgentSessionInputToolResult);
@@dynamicModel(CreateSessionEventsParams);
