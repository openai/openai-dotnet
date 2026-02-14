using TypeSpec.OpenAPI;

namespace OpenAI;

/**
 * The supported tool type discriminators for realtime tools.
 * Supports 'function' tools and 'mcp' (Model Context Protocol) tools.
 */
union RealtimeToolType {
  string,
  function: "function",
  mcp: "mcp",
}

/**
 * The base representation of a realtime tool definition.
 */
@discriminator("type")
model RealtimeTool {
  type: RealtimeToolType;
}

/**
 * The definition of a function tool as used by the realtime endpoint.
 */
model RealtimeFunctionTool extends RealtimeTool {
  type: RealtimeToolType.function;
  name: string;
  description?: string;
  parameters?: unknown;
}

/**
 * Give the model access to additional tools via remote Model Context Protocol
 * (MCP) servers for the realtime endpoint. [Learn more about MCP](/docs/guides/tools-remote-mcp).
 */
model RealtimeMCPTool extends RealtimeTool {
  /** The type of the MCP tool. Always `mcp`. */
  type: RealtimeToolType.mcp;

  /** A label for this MCP server, used to identify it in tool calls. */
  server_label: string;

  /** The URL for the MCP server. */
  server_url?: string;

  /** An OAuth access token for the MCP server. */
  authorization?: string;

  /** Optional description of the MCP server. */
  server_description?: string;

  /** Optional HTTP headers to send to the MCP server. */
  headers?: Record<string> | null;

  /** List of allowed tool names. */
  allowed_tools?: string[];

  /** Specify which of the MCP server's tools require approval. */
  require_approval?: "always" | "never" | null;
}

/**
 * The combined set of available representations for a realtime tool_choice parameter, encompassing both string
 * literal options like 'auto' as well as structured references to defined tools.
 */
union RealtimeToolChoice {
  RealtimeToolChoiceLiteral,
  RealtimeToolChoiceObject,
  RealtimeToolChoiceMCPObject,
}

/**
 * The available set of mode-level, string literal tool_choice options for the realtime endpoint.
 */
union RealtimeToolChoiceLiteral {
  string,

  /** Specifies that the model should freely determine which tool or tools, if any, to call. */
  auto: "auto",

  /** Specifies that the model should call no tools whatsoever. */
  none: "none",

  /** Specifies that the model should call at least one tool. */
  required: "required",
}

/**
 * A base representation for a realtime tool_choice selecting a named tool.
 */
@discriminator("type")
model RealtimeToolChoiceObject {
  type: RealtimeToolType;
}

/**
 * The representation of a realtime tool_choice selecting a named function tool.
 */
model RealtimeToolChoiceFunctionObject extends RealtimeToolChoiceObject {
  type: RealtimeToolType.function;
  function: {
    name: string;
  };
}

/**
 * The representation of a realtime tool_choice selecting a named MCP tool.
 */
model RealtimeToolChoiceMCPObject {
  type: RealtimeToolType.mcp;
  mcp: {
    server_label: string;
    name: string;
  };
}
