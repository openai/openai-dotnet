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

// ------------ Model names ------------

@@clientName(CreateAgentParams, "AgentCreationOptions");
@@clientName(UpdateAgentParams, "AgentModificationOptions");
@@clientName(AgentResource, "Agent");
@@clientName(DeletedAgentResource, "AgentDeletionResult");
@@clientName(DeletedAgentResource.id, "AgentId");
@@clientName(AgentCollectionOrder.asc, "Ascending");
@@clientName(AgentCollectionOrder.desc, "Descending");

// ------------ Free-form JSON payloads ------------

// Function schemas and MCP metadata are arbitrary JSON; surface them as BinaryData.
@@alternateType(PersistedAgentToolConfigParamFunction.parameters, unknown);
@@alternateType(PersistedAgentToolResourceFunction.parameters, unknown);
@@alternateType(TextFormatParamJsonSchema.schema, unknown);
@@alternateType(TextFormatResourceJsonSchema.schema, unknown);
@@alternateType(PersistedAgentToolConfigParamMcp.request_metadata, unknown | null);
@@alternateType(PersistedAgentToolResourceMcp.request_metadata, unknown);

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
