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
