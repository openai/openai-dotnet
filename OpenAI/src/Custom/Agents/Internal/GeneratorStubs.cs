using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Agents;

[CodeGenType("AgentObject")] internal readonly partial struct InternalAgentObject {}
[CodeGenType("AgentDeletedObject")] internal readonly partial struct InternalAgentDeletedObject {}
[CodeGenType("UnknownPersistedAgentToolConfigParam")] internal partial class InternalUnknownPersistedAgentToolConfigParam {}
[CodeGenType("UnknownPersistedAgentToolResource")] internal partial class InternalUnknownPersistedAgentToolResource {}
[CodeGenType("UnknownPersistedMcpTransportConfigParam")] internal partial class InternalUnknownPersistedMcpTransportConfigParam {}
[CodeGenType("UnknownPersistedMcpTransportResource")] internal partial class InternalUnknownPersistedMcpTransportResource {}
[CodeGenType("UnknownTextFormatParam")] internal partial class InternalUnknownTextFormatParam {}
[CodeGenType("UnknownTextFormatResource")] internal partial class InternalUnknownTextFormatResource {}

// ------------ Agent sessions ------------
[CodeGenType("DeletedSessionObject")] internal readonly partial struct InternalDeletedSessionObject {}
[CodeGenType("InputMessageParamType")] internal readonly partial struct InternalInputMessageParamType {}
[CodeGenType("SessionObject")] internal readonly partial struct InternalSessionObject {}
[CodeGenType("UnknownAgentToolConfigParam")] internal partial class InternalUnknownAgentToolConfigParam {}
[CodeGenType("UnknownAgentToolResource")] internal partial class InternalUnknownAgentToolResource {}
[CodeGenType("UnknownInputContentParam")] internal partial class InternalUnknownInputContentParam {}
[CodeGenType("UnknownMcpTransportConfigParam")] internal partial class InternalUnknownMcpTransportConfigParam {}
[CodeGenType("UnknownMcpTransportResource")] internal partial class InternalUnknownMcpTransportResource {}
[CodeGenType("UnknownSessionRequiredActionResource")] internal partial class InternalUnknownSessionRequiredActionResource {}

// ------------ Session artifacts ------------
[CodeGenType("DeletedSessionArtifactObject")] internal readonly partial struct InternalDeletedSessionArtifactObject {}
[CodeGenType("SessionArtifactObject")] internal readonly partial struct InternalSessionArtifactObject {}

// ------------ Session turns ------------
[CodeGenType("TurnObjectResource")] internal readonly partial struct InternalTurnObjectResource {}

// ------------ Session items ------------
[CodeGenType("SummaryTextResourceType")] internal readonly partial struct InternalSummaryTextResourceType {}
[CodeGenType("UnknownAgentContentResource")] internal partial class InternalUnknownAgentContentResource {}
[CodeGenType("UnknownAgentSessionItem")] internal partial class InternalUnknownAgentSessionItem {}
[CodeGenType("UnknownInputContentResource")] internal partial class InternalUnknownInputContentResource {}
[CodeGenType("UnknownMessageContentResource")] internal partial class InternalUnknownMessageContentResource {}
[CodeGenType("UnknownWebSearchActionResource")] internal partial class InternalUnknownWebSearchActionResource {}

// ------------ Session subagents ------------
[CodeGenType("SubagentObjectResource")] internal readonly partial struct InternalSubagentObjectResource {}
