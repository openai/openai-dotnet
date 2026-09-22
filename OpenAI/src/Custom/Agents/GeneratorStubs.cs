using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Agents;

[CodeGenType("Agent")] public partial class Agent {}
[CodeGenType("AgentCollection")] public partial class AgentCollectionPage {}
[CodeGenType("AgentCollectionOptions")] public partial class AgentCollectionOptions {}
[CodeGenType("AgentCollectionOrder")] public readonly partial struct AgentCollectionOrder {}
[CodeGenType("AgentCreationOptions")] public partial class AgentCreationOptions {}
[CodeGenType("AgentDeletionResult")] public partial class AgentDeletionResult {}
[CodeGenType("AgentModificationOptions")] public partial class AgentModificationOptions {}
[CodeGenType("McpConnectionOriginParam")] public readonly partial struct McpConnectionOriginParam {}
[CodeGenType("McpConnectionOriginResource")] public readonly partial struct McpConnectionOriginResource {}
[CodeGenType("MultiAgentConfigCurrentParam")] public partial class MultiAgentConfigCurrentParam {}
[CodeGenType("MultiAgentConfigResource")] public partial class MultiAgentConfigResource {}
[CodeGenType("PersistedAgentToolConfigParam")] public partial class PersistedAgentToolConfigParam {}
[CodeGenType("PersistedAgentToolConfigParamFunction")] public partial class PersistedAgentToolConfigParamFunction {}
[CodeGenType("PersistedAgentToolConfigParamMcp")] public partial class PersistedAgentToolConfigParamMcp {}
[CodeGenType("PersistedAgentToolConfigParamProgrammaticToolCalling")] public partial class PersistedAgentToolConfigParamProgrammaticToolCalling {}
[CodeGenType("PersistedAgentToolConfigParamToolSearch")] public partial class PersistedAgentToolConfigParamToolSearch {}
[CodeGenType("PersistedAgentToolConfigParamWebSearch")] public partial class PersistedAgentToolConfigParamWebSearch {}
[CodeGenType("PersistedAgentToolResource")] public partial class PersistedAgentToolResource {}
[CodeGenType("PersistedAgentToolResourceFunction")] public partial class PersistedAgentToolResourceFunction {}
[CodeGenType("PersistedAgentToolResourceMcp")] public partial class PersistedAgentToolResourceMcp {}
[CodeGenType("PersistedAgentToolResourceProgrammaticToolCalling")] public partial class PersistedAgentToolResourceProgrammaticToolCalling {}
[CodeGenType("PersistedAgentToolResourceToolSearch")] public partial class PersistedAgentToolResourceToolSearch {}
[CodeGenType("PersistedAgentToolResourceWebSearch")] public partial class PersistedAgentToolResourceWebSearch {}
[CodeGenType("PersistedAgentToolType")] public readonly partial struct PersistedAgentToolType {}
[CodeGenType("PersistedMcpTransportConfigParam")] public partial class PersistedMcpTransportConfigParam {}
[CodeGenType("PersistedMcpTransportConfigParamHttp")] public partial class PersistedMcpTransportConfigParamHttp {}
[CodeGenType("PersistedMcpTransportConfigParamStdio")] public partial class PersistedMcpTransportConfigParamStdio {}
[CodeGenType("PersistedMcpTransportResource")] public partial class PersistedMcpTransportResource {}
[CodeGenType("PersistedMcpTransportResourceHttp")] public partial class PersistedMcpTransportResourceHttp {}
[CodeGenType("PersistedMcpTransportResourceStdio")] public partial class PersistedMcpTransportResourceStdio {}
[CodeGenType("PersistedMcpTransportType")] public readonly partial struct PersistedMcpTransportType {}
[CodeGenType("ReasoningEffortParam")] public readonly partial struct ReasoningEffortParam {}
[CodeGenType("ReasoningEffortResource")] public readonly partial struct ReasoningEffortResource {}
[CodeGenType("ReasoningParam")] public partial class ReasoningParam {}
[CodeGenType("ReasoningResource")] public partial class ReasoningResource {}
[CodeGenType("ReasoningSummaryParam")] public readonly partial struct ReasoningSummaryParam {}
[CodeGenType("ReasoningSummaryResource")] public readonly partial struct ReasoningSummaryResource {}
[CodeGenType("ServiceTierParam")] public readonly partial struct ServiceTierParam {}
[CodeGenType("ServiceTierResource")] public readonly partial struct ServiceTierResource {}
[CodeGenType("TextFormatParam")] public partial class TextFormatParam {}
[CodeGenType("TextFormatParamJsonSchema")] public partial class TextFormatParamJsonSchema {}
[CodeGenType("TextFormatParamText")] public partial class TextFormatParamText {}
[CodeGenType("TextFormatResource")] public partial class TextFormatResource {}
[CodeGenType("TextFormatResourceJsonSchema")] public partial class TextFormatResourceJsonSchema {}
[CodeGenType("TextFormatResourceText")] public partial class TextFormatResourceText {}
[CodeGenType("TextFormatType")] public readonly partial struct TextFormatType {}
[CodeGenType("TextParam")] public partial class TextParam {}
[CodeGenType("TextResource")] public partial class TextResource {}
[CodeGenType("VerbosityParam")] public readonly partial struct VerbosityParam {}
[CodeGenType("VerbosityResource")] public readonly partial struct VerbosityResource {}
[CodeGenType("WebSearchContextSizeParam")] public readonly partial struct WebSearchContextSizeParam {}
[CodeGenType("WebSearchContextSizeResource")] public readonly partial struct WebSearchContextSizeResource {}
[CodeGenType("WebSearchLocationParam")] public partial class WebSearchLocationParam {}
[CodeGenType("WebSearchLocationResource")] public partial class WebSearchLocationResource {}
[CodeGenType("WebSearchModeParam")] public readonly partial struct WebSearchModeParam {}
[CodeGenType("WebSearchModeResource")] public readonly partial struct WebSearchModeResource {}

// ------------ Agent sessions ------------
[CodeGenType("AgentSession")] public partial class AgentSession {}
[CodeGenType("AgentSessionCollectionOptions")] public partial class AgentSessionCollectionOptions {}
[CodeGenType("AgentSessionCollectionOrder")] public readonly partial struct AgentSessionCollectionOrder {}
[CodeGenType("AgentSessionCollectionPage")] public partial class AgentSessionCollectionPage {}
[CodeGenType("AgentSessionCreationOptions")] public partial class AgentSessionCreationOptions {}
[CodeGenType("AgentSessionDeletionResult")] public partial class AgentSessionDeletionResult {}
[CodeGenType("AgentSessionModificationOptions")] public partial class AgentSessionModificationOptions {}
[CodeGenType("AgentToolConfigParam")] public partial class AgentToolConfigParam {}
[CodeGenType("AgentToolConfigParamFunction")] public partial class AgentToolConfigParamFunction {}
[CodeGenType("AgentToolConfigParamMcp")] public partial class AgentToolConfigParamMcp {}
[CodeGenType("AgentToolConfigParamProgrammaticToolCalling")] public partial class AgentToolConfigParamProgrammaticToolCalling {}
[CodeGenType("AgentToolConfigParamToolSearch")] public partial class AgentToolConfigParamToolSearch {}
[CodeGenType("AgentToolConfigParamWebSearch")] public partial class AgentToolConfigParamWebSearch {}
[CodeGenType("AgentToolResource")] public partial class AgentToolResource {}
[CodeGenType("AgentToolResourceFunction")] public partial class AgentToolResourceFunction {}
[CodeGenType("AgentToolResourceMcp")] public partial class AgentToolResourceMcp {}
[CodeGenType("AgentToolResourceProgrammaticToolCalling")] public partial class AgentToolResourceProgrammaticToolCalling {}
[CodeGenType("AgentToolResourceWebSearch")] public partial class AgentToolResourceWebSearch {}
[CodeGenType("AgentToolType")] public readonly partial struct AgentToolType {}
[CodeGenType("InputContentParam")] public partial class InputContentParam {}
[CodeGenType("InputContentParamInputImage")] public partial class InputContentParamInputImage {}
[CodeGenType("InputContentParamInputText")] public partial class InputContentParamInputText {}
[CodeGenType("InputContentType")] public readonly partial struct InputContentType {}
[CodeGenType("InputMessageParam")] public partial class InputMessageParam {}
[CodeGenType("InputTokensDetailsResource")] public partial class InputTokensDetailsResource {}
[CodeGenType("McpTransportConfigParam")] public partial class McpTransportConfigParam {}
[CodeGenType("McpTransportConfigParamHttp")] public partial class McpTransportConfigParamHttp {}
[CodeGenType("McpTransportConfigParamStdio")] public partial class McpTransportConfigParamStdio {}
[CodeGenType("McpTransportResource")] public partial class McpTransportResource {}
[CodeGenType("McpTransportResourceHttp")] public partial class McpTransportResourceHttp {}
[CodeGenType("McpTransportResourceStdio")] public partial class McpTransportResourceStdio {}
[CodeGenType("McpTransportType")] public readonly partial struct McpTransportType {}
[CodeGenType("OutputTokensDetailsResource")] public partial class OutputTokensDetailsResource {}
[CodeGenType("SessionAgentConfigParam")] public partial class SessionAgentConfigParam {}
[CodeGenType("SessionAgentResource")] public partial class SessionAgentResource {}
[CodeGenType("SessionRequiredActionResource")] public partial class SessionRequiredActionResource {}
[CodeGenType("SessionRequiredActionResourceEnvironmentConnection")] public partial class SessionRequiredActionResourceEnvironmentConnection {}
[CodeGenType("SessionRequiredActionResourceFunctionCall")] public partial class SessionRequiredActionResourceFunctionCall {}
[CodeGenType("SessionRequiredActionType")] public readonly partial struct SessionRequiredActionType {}
[CodeGenType("SessionStatusResource")] public readonly partial struct SessionStatusResource {}
[CodeGenType("TokenUsageResource")] public partial class TokenUsageResource {}

// ------------ Session artifacts ------------
[CodeGenType("AgentSessionArtifact")] public partial class AgentSessionArtifact {}
[CodeGenType("AgentSessionArtifactCollectionOptions")] public partial class AgentSessionArtifactCollectionOptions {}
[CodeGenType("AgentSessionArtifactCollectionOrder")] public readonly partial struct AgentSessionArtifactCollectionOrder {}
[CodeGenType("AgentSessionArtifactCollectionPage")] public partial class AgentSessionArtifactCollectionPage {}
[CodeGenType("AgentSessionArtifactDeletionResult")] public partial class AgentSessionArtifactDeletionResult {}

// ------------ Session turns ------------
[CodeGenType("AgentSessionTurn")] public partial class AgentSessionTurn {}
[CodeGenType("AgentSessionTurnCollectionOptions")] public partial class AgentSessionTurnCollectionOptions {}
[CodeGenType("AgentSessionTurnCollectionOrder")] public readonly partial struct AgentSessionTurnCollectionOrder {}
[CodeGenType("AgentSessionTurnCollectionPage")] public partial class AgentSessionTurnCollectionPage {}
[CodeGenType("SessionTurnErrorCodeResource")] public readonly partial struct SessionTurnErrorCodeResource {}
[CodeGenType("SessionTurnErrorResource")] public partial class SessionTurnErrorResource {}
[CodeGenType("TurnStatusResource")] public readonly partial struct TurnStatusResource {}

// ------------ Session items ------------
[CodeGenType("AgentContentResource")] public partial class AgentContentResource {}
[CodeGenType("AgentContentType")] public readonly partial struct AgentContentType {}
[CodeGenType("AgentMessageItemResource")] public partial class AgentMessageItemResource {}
[CodeGenType("AgentSessionItem")] public partial class AgentSessionItem {}
[CodeGenType("AgentSessionItemCollectionOptions")] public partial class AgentSessionItemCollectionOptions {}
[CodeGenType("AgentSessionItemCollectionOrder")] public readonly partial struct AgentSessionItemCollectionOrder {}
[CodeGenType("AgentSessionItemCollectionPage")] public partial class AgentSessionItemCollectionPage {}
[CodeGenType("AgentSessionMessageContentType")] public readonly partial struct AgentSessionMessageContentType {}
[CodeGenType("AgentSessionReasoningItemResource")] public partial class AgentSessionReasoningItemResource {}
[CodeGenType("AgentSessionWebSearchActionType")] public readonly partial struct AgentSessionWebSearchActionType {}
[CodeGenType("CloseSubagentCallItemResource")] public partial class CloseSubagentCallItemResource {}
[CodeGenType("CommandExecutionItemResource")] public partial class CommandExecutionItemResource {}
[CodeGenType("CreateSubagentCallItemResource")] public partial class CreateSubagentCallItemResource {}
[CodeGenType("EncryptedContentResource")] public partial class EncryptedContentResource {}
[CodeGenType("FunctionCallItemResource")] public partial class FunctionCallItemResource {}
[CodeGenType("FunctionCallOutputItemResource")] public partial class FunctionCallOutputItemResource {}
[CodeGenType("FunctionCallStatusResource")] public readonly partial struct FunctionCallStatusResource {}
[CodeGenType("InputContentResource")] public partial class InputContentResource {}
[CodeGenType("InputContentResourceInputImage")] public partial class InputContentResourceInputImage {}
[CodeGenType("InputContentResourceInputText")] public partial class InputContentResourceInputText {}
[CodeGenType("InterruptSubagentCallItemResource")] public partial class InterruptSubagentCallItemResource {}
[CodeGenType("McpCallItemResource")] public partial class McpCallItemResource {}
[CodeGenType("MessageContentResource")] public partial class MessageContentResource {}
[CodeGenType("MessageContentResourceInputImage")] public partial class MessageContentResourceInputImage {}
[CodeGenType("MessageContentResourceInputText")] public partial class MessageContentResourceInputText {}
[CodeGenType("MessageContentResourceOutputText")] public partial class MessageContentResourceOutputText {}
[CodeGenType("MessageItemResource")] public partial class MessageItemResource {}
[CodeGenType("MessagePhaseResource")] public readonly partial struct MessagePhaseResource {}
[CodeGenType("OutputItemStatusResource")] public readonly partial struct OutputItemStatusResource {}
[CodeGenType("OutputTextResource")] public partial class OutputTextResource {}
[CodeGenType("ResumeSubagentCallItemResource")] public partial class ResumeSubagentCallItemResource {}
[CodeGenType("SendSubagentInputCallItemResource")] public partial class SendSubagentInputCallItemResource {}
[CodeGenType("SessionMessageRoleResource")] public readonly partial struct SessionMessageRoleResource {}
[CodeGenType("SessionTurnItemType")] public readonly partial struct SessionTurnItemType {}
[CodeGenType("SummaryTextResource")] public partial class SummaryTextResource {}
[CodeGenType("WaitForSubagentsCallItemResource")] public partial class WaitForSubagentsCallItemResource {}
[CodeGenType("WebSearchActionResource")] public partial class WebSearchActionResource {}
[CodeGenType("WebSearchActionResourceFindInPage")] public partial class WebSearchActionResourceFindInPage {}
[CodeGenType("WebSearchActionResourceOpenPage")] public partial class WebSearchActionResourceOpenPage {}
[CodeGenType("WebSearchActionResourceOther")] public partial class WebSearchActionResourceOther {}
[CodeGenType("WebSearchActionResourceSearch")] public partial class WebSearchActionResourceSearch {}
[CodeGenType("WebSearchCallItemResource")] public partial class WebSearchCallItemResource {}

// ------------ Session subagents ------------
[CodeGenType("AgentSessionSubagent")] public partial class AgentSessionSubagent {}
[CodeGenType("AgentSessionSubagentCollectionOptions")] public partial class AgentSessionSubagentCollectionOptions {}
[CodeGenType("AgentSessionSubagentCollectionOrder")] public readonly partial struct AgentSessionSubagentCollectionOrder {}
[CodeGenType("AgentSessionSubagentCollectionPage")] public partial class AgentSessionSubagentCollectionPage {}
[CodeGenType("AgentSessionSubagentItemCollectionOptions")] public partial class AgentSessionSubagentItemCollectionOptions {}
[CodeGenType("AgentSessionSubagentTurnCollectionOptions")] public partial class AgentSessionSubagentTurnCollectionOptions {}
[CodeGenType("AgentSessionSubagentTurnItemCollectionOptions")] public partial class AgentSessionSubagentTurnItemCollectionOptions {}
[CodeGenType("SubagentStatusResource")] public readonly partial struct SubagentStatusResource {}

// ------------ Session events ------------
[CodeGenType("CreateSessionEventsParams")] public partial class CreateSessionEventsParams {}
[CodeGenType("SessionEnvironmentErrorResource")] public partial class SessionEnvironmentErrorResource {}
[CodeGenType("SessionEnvironmentStateResource")] public partial class SessionEnvironmentStateResource {}
[CodeGenType("SessionEnvironmentStatusResource")] public readonly partial struct SessionEnvironmentStatusResource {}
[CodeGenType("SessionErrorResource")] public partial class SessionErrorResource {}
[CodeGenType("SessionEventAgentOutputCommandExecutionOutputDelta")] public partial class SessionEventAgentOutputCommandExecutionOutputDelta {}
[CodeGenType("SessionEventAgentSessionCreated")] public partial class SessionEventAgentSessionCreated {}
[CodeGenType("SessionEventAgentSessionEnvironmentConnected")] public partial class SessionEventAgentSessionEnvironmentConnected {}
[CodeGenType("SessionEventAgentSessionEnvironmentDisconnected")] public partial class SessionEventAgentSessionEnvironmentDisconnected {}
[CodeGenType("SessionEventAgentSessionEnvironmentFailed")] public partial class SessionEventAgentSessionEnvironmentFailed {}
[CodeGenType("SessionEventAgentSessionEnvironmentPending")] public partial class SessionEventAgentSessionEnvironmentPending {}
[CodeGenType("SessionEventAgentSessionEnvironmentReady")] public partial class SessionEventAgentSessionEnvironmentReady {}
[CodeGenType("SessionEventAgentSessionFailed")] public partial class SessionEventAgentSessionFailed {}
[CodeGenType("SessionEventAgentSessionIdle")] public partial class SessionEventAgentSessionIdle {}
[CodeGenType("SessionEventAgentSessionInProgress")] public partial class SessionEventAgentSessionInProgress {}
[CodeGenType("SessionEventAgentSessionRequiresAction")] public partial class SessionEventAgentSessionRequiresAction {}
[CodeGenType("SessionEventAgentSessionSubagentActive")] public partial class SessionEventAgentSessionSubagentActive {}
[CodeGenType("SessionEventAgentSessionSubagentClosed")] public partial class SessionEventAgentSessionSubagentClosed {}
[CodeGenType("SessionEventAgentSessionSubagentCreated")] public partial class SessionEventAgentSessionSubagentCreated {}
[CodeGenType("SessionEventAgentSessionTurnCancelled")] public partial class SessionEventAgentSessionTurnCancelled {}
[CodeGenType("SessionEventAgentSessionTurnCompleted")] public partial class SessionEventAgentSessionTurnCompleted {}
[CodeGenType("SessionEventAgentSessionTurnContentPartAdded")] public partial class SessionEventAgentSessionTurnContentPartAdded {}
[CodeGenType("SessionEventAgentSessionTurnContentPartDone")] public partial class SessionEventAgentSessionTurnContentPartDone {}
[CodeGenType("SessionEventAgentSessionTurnCreated")] public partial class SessionEventAgentSessionTurnCreated {}
[CodeGenType("SessionEventAgentSessionTurnFailed")] public partial class SessionEventAgentSessionTurnFailed {}
[CodeGenType("SessionEventAgentSessionTurnInProgress")] public partial class SessionEventAgentSessionTurnInProgress {}
[CodeGenType("SessionEventAgentSessionTurnItemAdded")] public partial class SessionEventAgentSessionTurnItemAdded {}
[CodeGenType("SessionEventAgentSessionTurnItemDone")] public partial class SessionEventAgentSessionTurnItemDone {}
[CodeGenType("SessionEventAgentSessionTurnOutputTextDelta")] public partial class SessionEventAgentSessionTurnOutputTextDelta {}
[CodeGenType("SessionEventAgentSessionTurnOutputTextDone")] public partial class SessionEventAgentSessionTurnOutputTextDone {}
[CodeGenType("SessionEventAgentSessionTurnReasoningSummaryPartAdded")] public partial class SessionEventAgentSessionTurnReasoningSummaryPartAdded {}
[CodeGenType("SessionEventAgentSessionTurnReasoningSummaryPartDone")] public partial class SessionEventAgentSessionTurnReasoningSummaryPartDone {}
[CodeGenType("SessionEventAgentSessionTurnReasoningSummaryTextDelta")] public partial class SessionEventAgentSessionTurnReasoningSummaryTextDelta {}
[CodeGenType("SessionEventAgentSessionTurnReasoningSummaryTextDone")] public partial class SessionEventAgentSessionTurnReasoningSummaryTextDone {}
[CodeGenType("SessionEventError")] public partial class SessionEventError {}
[CodeGenType("SessionInputParam")] public partial class SessionInputParam {}
[CodeGenType("SessionInputParamAgentSessionInputCancel")] public partial class SessionInputParamAgentSessionInputCancel {}
[CodeGenType("SessionInputParamAgentSessionInputMessage")] public partial class SessionInputParamAgentSessionInputMessage {}
[CodeGenType("SessionInputParamAgentSessionInputToolResult")] public partial class SessionInputParamAgentSessionInputToolResult {}
[CodeGenType("SessionInputType")] public readonly partial struct SessionInputType {}

// ------------ Vaults ------------
[CodeGenType("CreateMcpOauthRefreshParam")] public partial class CreateMcpOauthRefreshParam {}
[CodeGenType("CreateMcpOauthTokenEndpointAuthParam")] public partial class CreateMcpOauthTokenEndpointAuthParam {}
[CodeGenType("CreateMcpOauthTokenEndpointAuthParamClientSecretBasic")] public partial class CreateMcpOauthTokenEndpointAuthParamClientSecretBasic {}
[CodeGenType("CreateMcpOauthTokenEndpointAuthParamClientSecretPost")] public partial class CreateMcpOauthTokenEndpointAuthParamClientSecretPost {}
[CodeGenType("CreateMcpOauthTokenEndpointAuthParamNone")] public partial class CreateMcpOauthTokenEndpointAuthParamNone {}
[CodeGenType("CreateVaultCredentialAuthParam")] public partial class CreateVaultCredentialAuthParam {}
[CodeGenType("CreateVaultCredentialAuthParamEnvironmentVariable")] public partial class CreateVaultCredentialAuthParamEnvironmentVariable {}
[CodeGenType("CreateVaultCredentialAuthParamMcpOauth")] public partial class CreateVaultCredentialAuthParamMcpOauth {}
[CodeGenType("CreateVaultCredentialAuthParamStaticBearer")] public partial class CreateVaultCredentialAuthParamStaticBearer {}
[CodeGenType("McpOauthRefreshResource")] public partial class McpOauthRefreshResource {}
[CodeGenType("McpOauthTokenEndpointAuthResource")] public partial class McpOauthTokenEndpointAuthResource {}
[CodeGenType("McpOauthTokenEndpointAuthResourceClientSecretBasic")] public partial class McpOauthTokenEndpointAuthResourceClientSecretBasic {}
[CodeGenType("McpOauthTokenEndpointAuthResourceClientSecretPost")] public partial class McpOauthTokenEndpointAuthResourceClientSecretPost {}
[CodeGenType("McpOauthTokenEndpointAuthResourceNone")] public partial class McpOauthTokenEndpointAuthResourceNone {}
[CodeGenType("McpOauthTokenEndpointAuthType")] public readonly partial struct McpOauthTokenEndpointAuthType {}
[CodeGenType("RotateMcpOauthRefreshParam")] public partial class RotateMcpOauthRefreshParam {}
[CodeGenType("RotateMcpOauthTokenEndpointAuthParam")] public partial class RotateMcpOauthTokenEndpointAuthParam {}
[CodeGenType("RotateMcpOauthTokenEndpointAuthParamClientSecretBasic")] public partial class RotateMcpOauthTokenEndpointAuthParamClientSecretBasic {}
[CodeGenType("RotateMcpOauthTokenEndpointAuthParamClientSecretPost")] public partial class RotateMcpOauthTokenEndpointAuthParamClientSecretPost {}
[CodeGenType("RotateVaultCredentialAuthParam")] public partial class RotateVaultCredentialAuthParam {}
[CodeGenType("RotateVaultCredentialAuthParamEnvironmentVariable")] public partial class RotateVaultCredentialAuthParamEnvironmentVariable {}
[CodeGenType("RotateVaultCredentialAuthParamMcpOauth")] public partial class RotateVaultCredentialAuthParamMcpOauth {}
[CodeGenType("RotateVaultCredentialAuthParamStaticBearer")] public partial class RotateVaultCredentialAuthParamStaticBearer {}
[CodeGenType("Vault")] public partial class Vault {}
[CodeGenType("VaultCollectionOptions")] public partial class VaultCollectionOptions {}
[CodeGenType("VaultCollectionOrder")] public readonly partial struct VaultCollectionOrder {}
[CodeGenType("VaultCollectionPage")] public partial class VaultCollectionPage {}
[CodeGenType("VaultCreationOptions")] public partial class VaultCreationOptions {}
[CodeGenType("VaultCredential")] public partial class VaultCredential {}
[CodeGenType("VaultCredentialAuthResource")] public partial class VaultCredentialAuthResource {}
[CodeGenType("VaultCredentialAuthResourceEnvironmentVariable")] public partial class VaultCredentialAuthResourceEnvironmentVariable {}
[CodeGenType("VaultCredentialAuthResourceMcpOauth")] public partial class VaultCredentialAuthResourceMcpOauth {}
[CodeGenType("VaultCredentialAuthResourceStaticBearer")] public partial class VaultCredentialAuthResourceStaticBearer {}
[CodeGenType("VaultCredentialAuthType")] public readonly partial struct VaultCredentialAuthType {}
[CodeGenType("VaultCredentialCollectionOptions")] public partial class VaultCredentialCollectionOptions {}
[CodeGenType("VaultCredentialCollectionPage")] public partial class VaultCredentialCollectionPage {}
[CodeGenType("VaultCredentialCreationOptions")] public partial class VaultCredentialCreationOptions {}
[CodeGenType("VaultCredentialDeletionResult")] public partial class VaultCredentialDeletionResult {}
[CodeGenType("VaultCredentialNetworkingParam")] public partial class VaultCredentialNetworkingParam {}
[CodeGenType("VaultCredentialNetworkingParamLimited")] public partial class VaultCredentialNetworkingParamLimited {}
[CodeGenType("VaultCredentialNetworkingParamUnrestricted")] public partial class VaultCredentialNetworkingParamUnrestricted {}
[CodeGenType("VaultCredentialNetworkingResource")] public partial class VaultCredentialNetworkingResource {}
[CodeGenType("VaultCredentialNetworkingResourceLimited")] public partial class VaultCredentialNetworkingResourceLimited {}
[CodeGenType("VaultCredentialNetworkingResourceUnrestricted")] public partial class VaultCredentialNetworkingResourceUnrestricted {}
[CodeGenType("VaultCredentialNetworkingType")] public readonly partial struct VaultCredentialNetworkingType {}
[CodeGenType("VaultCredentialRotationOptions")] public partial class VaultCredentialRotationOptions {}
[CodeGenType("VaultDeletionResult")] public partial class VaultDeletionResult {}
[CodeGenType("VaultStatusParam")] public readonly partial struct VaultStatusParam {}
