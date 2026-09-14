import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

// A credential-free tool available to a reusable agent. The `*ConfigParam` models are used on
// requests (e.g. create/update agent) and the `*Resource` models are returned on responses.

/** The type of a persisted agent tool. */
union PersistedAgentToolType {
  string,
  function: "function",
  tool_search: "tool_search",
  programmatic_tool_calling: "programmatic_tool_calling",
  mcp: "mcp",
  web_search: "web_search",
}

/** A credential-free transport used to connect to an MCP server. */
union PersistedMcpTransportType {
  string,
  http: "http",
  stdio: "stdio",
}

/** Where outbound MCP HTTP connections originate. */
union McpConnectionOriginParam {
  string,

  /** Uses the Managed Agents service network. */
  service: "service",

  /** Uses the session's execution environment. */
  environment: "environment",
}

/** Where outbound MCP HTTP connections originate. */
union McpConnectionOriginResource {
  string,
  service: "service",
  environment: "environment",
}

/** The source used for web search results. */
union WebSearchModeParam {
  string,

  /** Disables web search. */
  disabled: "disabled",

  /** Uses cached search results. */
  cached: "cached",

  /** Searches the live web. */
  live: "live",
}

/** The source used for web search results. */
union WebSearchModeResource {
  string,
  disabled: "disabled",
  cached: "cached",
  live: "live",
}

/** The amount of web search context made available to the model. */
union WebSearchContextSizeParam {
  string,
  low: "low",
  medium: "medium",
  high: "high",
}

/** The amount of web search context made available to the model. */
union WebSearchContextSizeResource {
  string,
  low: "low",
  medium: "medium",
  high: "high",
}

/** Approximate user location used to localize web search results. */
model WebSearchLocationParam {
  /** The two-letter ISO country code, such as `US`. */
  country?: string | null;

  /** The region or state name. */
  region?: string | null;

  /** The city name. */
  city?: string | null;

  /** The IANA timezone, such as `America/Los_Angeles`. */
  timezone?: string | null;
}

/** Approximate user location used to localize web search results. */
model WebSearchLocationResource {
  /** The two-letter ISO country code, such as `US`. */
  country: string | null;

  /** The region or state name. */
  region: string | null;

  /** The city name. */
  city: string | null;

  /** The IANA timezone, such as `America/Los_Angeles`. */
  timezone: string | null;
}

// --------- Tool config params (request) ---------

@discriminator("type")
model PersistedAgentToolConfigParam {
  type: PersistedAgentToolType;
}

/** A function defined by the application. */
model PersistedAgentToolConfigParamFunction
  extends PersistedAgentToolConfigParam {
  /** The type of the object. Always `function`. */
  type: PersistedAgentToolType.function;

  /** The name of the function. */
  name: string;

  /** A description of what the function does. */
  description: string;

  /** A JSON Schema object describing the function's arguments. */
  parameters: Record<unknown>;

  /** Whether this function is deferred and discovered through tool search. Defaults to `false`. */
  defer_loading?: boolean = false;
}

/** Discovers deferred function tools and loads them into the model context. */
model PersistedAgentToolConfigParamToolSearch
  extends PersistedAgentToolConfigParam {
  /** The type of the object. Always `tool_search`. */
  type: PersistedAgentToolType.tool_search;
}

/** Enables calling tools from model-generated code. */
model PersistedAgentToolConfigParamProgrammaticToolCalling
  extends PersistedAgentToolConfigParam {
  /** The type of the object. Always `programmatic_tool_calling`. */
  type: PersistedAgentToolType.programmatic_tool_calling;

  /** Whether tools can be called from model-generated code. Defaults to `true`. */
  enabled?: boolean = true;
}

@discriminator("type")
model PersistedMcpTransportConfigParam {
  type: PersistedMcpTransportType;
}

/** Connects to an MCP server over HTTP. */
model PersistedMcpTransportConfigParamHttp
  extends PersistedMcpTransportConfigParam {
  /** The type of the object. Always `http`. */
  type: PersistedMcpTransportType.http;

  /** The URL of the MCP server. */
  server_url: string;

  /** Non-secret HTTP headers sent to the MCP server. */
  headers?: Record<string> | null;
}

/** Starts an MCP server as a local process. */
model PersistedMcpTransportConfigParamStdio
  extends PersistedMcpTransportConfigParam {
  /** The type of the object. Always `stdio`. */
  type: PersistedMcpTransportType.stdio;

  /** The command used to start the MCP server. */
  command: string;

  /** Arguments passed to the MCP server command. */
  @maxItems(16384)
  args?: string[] | null;

  /** The working directory used to start the MCP server. */
  cwd: string;

  /** Environment variable names to inherit from the selected execution environment. */
  @maxItems(16384)
  env_vars?: string[] | null;
}

/** Tools provided by a remote MCP server without stored credentials. */
model PersistedAgentToolConfigParamMcp extends PersistedAgentToolConfigParam {
  /** The type of the object. Always `mcp`. */
  type: PersistedAgentToolType.mcp;

  /** A label used to identify the MCP server in tool calls. */
  server_label: string;

  /**
   * The vault credential selected for this MCP server. Optional when exactly one attached
   * credential matches the server URL.
   */
  credential_id?: string | null;

  /** The credential-free transport used to connect to the MCP server. */
  transport: PersistedMcpTransportConfigParam;

  /** Metadata included with requests to this MCP server. */
  request_metadata?: Record<unknown> | null;

  /** The MCP tools the agent may call. All server tools are allowed when omitted. */
  @maxItems(16384)
  allowed_tools?: string[] | null;

  /** Whether this MCP server must initialize before the first turn. Defaults to `false`. */
  required?: boolean = false;

  /** Selects where outbound MCP HTTP connections originate. */
  connection_origin?: McpConnectionOriginParam | null;
}

/** Web search. */
model PersistedAgentToolConfigParamWebSearch
  extends PersistedAgentToolConfigParam {
  /** The type of the object. Always `web_search`. */
  type: PersistedAgentToolType.web_search;

  /** The source used for web search results. Defaults to `live`. */
  mode?: WebSearchModeParam | null;

  /** The amount of search context made available to the model. Defaults to `medium`. */
  context_size?: WebSearchContextSizeParam | null;

  /** Domains the search may include. */
  @maxItems(16384)
  allowed_domains?: string[] | null;

  /** Approximate location used to localize search results. */
  location?: WebSearchLocationParam | null;
}

// --------- Tool resources (response) ---------

@discriminator("type")
model PersistedAgentToolResource {
  type: PersistedAgentToolType;
}

/** A function defined by the application. */
model PersistedAgentToolResourceFunction extends PersistedAgentToolResource {
  /** The type of the object. Always `function`. */
  type: PersistedAgentToolType.function;

  /** The name of the function. */
  name: string;

  /** A description of what the function does. */
  description: string;

  /** A JSON Schema object describing the function's arguments. */
  parameters: Record<unknown>;

  /** Whether the function is deferred and discovered through tool search. */
  defer_loading: boolean;
}

/** Discovers deferred function tools and loads them into the model context. */
model PersistedAgentToolResourceToolSearch extends PersistedAgentToolResource {
  /** The type of the object. Always `tool_search`. */
  type: PersistedAgentToolType.tool_search;
}

/** Enables calling tools from model-generated code. */
model PersistedAgentToolResourceProgrammaticToolCalling
  extends PersistedAgentToolResource {
  /** The type of the object. Always `programmatic_tool_calling`. */
  type: PersistedAgentToolType.programmatic_tool_calling;

  /** Whether tools can be called from model-generated code. */
  enabled: boolean;
}

@discriminator("type")
model PersistedMcpTransportResource {
  type: PersistedMcpTransportType;
}

/** Connects to an MCP server over HTTP. */
model PersistedMcpTransportResourceHttp extends PersistedMcpTransportResource {
  /** The type of the object. Always `http`. */
  type: PersistedMcpTransportType.http;

  /** The URL of the MCP server. */
  server_url: string;

  /** Non-secret HTTP headers sent to the MCP server. */
  headers: Record<string>;
}

/** Starts an MCP server as a local process. */
model PersistedMcpTransportResourceStdio extends PersistedMcpTransportResource {
  /** The type of the object. Always `stdio`. */
  type: PersistedMcpTransportType.stdio;

  /** The command used to start the MCP server. */
  command: string;

  /** Arguments passed to the MCP server command. */
  @maxItems(2000)
  args: string[];

  /** The working directory used to start the MCP server. */
  cwd: string;

  /** Environment variable names inherited from the execution environment. */
  @maxItems(2000)
  env_vars: string[];
}

/** Tools provided by a remote MCP server without stored credentials. */
model PersistedAgentToolResourceMcp extends PersistedAgentToolResource {
  /** The type of the object. Always `mcp`. */
  type: PersistedAgentToolType.mcp;

  /** A label used to identify the MCP server in tool calls. */
  server_label: string;

  /** The vault credential selected for this MCP server, if any. */
  credential_id: string | null;

  /** The credential-free transport used to connect to the MCP server. */
  transport: PersistedMcpTransportResource;

  /** Metadata included with requests to this MCP server. */
  request_metadata: Record<unknown>;

  /** The MCP tools the agent may call, or null when all server tools are allowed. */
  @maxItems(2000)
  allowed_tools: string[] | null;

  /** Whether this MCP server must initialize before the first turn. */
  required: boolean;

  /** Where outbound MCP HTTP connections originate. */
  connection_origin: McpConnectionOriginResource;
}

/** Web search. */
model PersistedAgentToolResourceWebSearch extends PersistedAgentToolResource {
  /** The type of the object. Always `web_search`. */
  type: PersistedAgentToolType.web_search;

  /** The source used for web search results. */
  mode: WebSearchModeResource;

  /** The amount of search context made available to the model. Defaults to `medium`. */
  context_size: WebSearchContextSizeResource;

  /** Allowed search domains, or `null` when the search is unrestricted. */
  @maxItems(2000)
  allowed_domains: string[] | null;

  /** Approximate location used to localize search results, if provided. */
  location: WebSearchLocationResource | null;
}
