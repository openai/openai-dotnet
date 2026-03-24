import "./models.tsp";

namespace OpenAI;

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** Defines a function in your own code the model can choose to call. Learn more about [function calling](https://platform.openai.com/docs/guides/function-calling). */
model FunctionTool extends Tool {
  @doc("""
    The type of the function tool. Always `function`.
    """)
  type: ToolType.function;

  /** The name of the function to call. */
  name: string;

  /** A description of the function. Used by the model to determine whether or not to call the function. */
  description?: string | null;

  /** A JSON schema object describing the parameters of the function. */
  parameters: unknown | null;

  @doc("""
    Whether to enforce strict parameter validation. Default `true`.
    """)
  strict: boolean | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that searches for relevant content from uploaded files. Learn more about the [file search tool](https://platform.openai.com/docs/guides/tools-file-search). */
model FileSearchTool extends Tool {
  @doc("""
    The type of the file search tool. Always `file_search`.
    """)
  type: ToolType.file_search;

  /** The IDs of the vector stores to search. */
  vector_store_ids: string[];

  /** The maximum number of results to return. This number should be between 1 and 50 inclusive. */
  max_num_results?: int32;

  /** Ranking options for search. */
  ranking_options?: RankingOptions;

  /** A filter to apply. */
  filters?: Filters | null;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that controls a virtual computer. Learn more about the [computer tool](https://platform.openai.com/docs/guides/tools-computer-use). */
model ComputerUsePreviewTool extends Tool {
  @doc("""
    The type of the computer use tool. Always `computer_use_preview`.
    """)
  type: ToolType.computer_use_preview;

  /** The type of computer environment to control. */
  environment: "windows" | "mac" | "linux" | "ubuntu" | "browser";

  /** The width of the computer display. */
  display_width: int32;

  /** The height of the computer display. */
  display_height: int32;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** This tool searches the web for relevant results to use in a response. Learn more about the [web search tool](https://platform.openai.com/docs/guides/tools-web-search). */
model WebSearchPreviewTool extends Tool {
  @doc("""
    The type of the web search tool. One of `web_search_preview` or `web_search_preview_2025_03_11`.
    """)
  type: ToolType.web_search_preview;

  // Tool customization: Use a custom discriminated type basis for web search locations
  /** The user's location. */
  user_location?: Location | null;

  @doc("""
    High level guidance for the amount of context window space to use for the search. One of `low`, `medium`, or `high`. `medium` is the default.
    """)
  search_context_size?: SearchContextSize;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** This tool searches the web for relevant results to use in a response. Learn more about the [web search tool](https://platform.openai.com/docs/guides/tools-web-search). */
model WebSearchTool extends Tool {
  /** The type of the web search tool. One of `web_search` or `web_search_2025_08_26`. */
  type: ToolType.web_search;

  filters?: WebSearchToolFilters | null;
  
  user_location?: Location | null;

  /** High level guidance for the amount of context window space to use for the search. One of `low`, `medium`, or `high`. `medium` is the default. */
  search_context_size?: SearchContextSize;
}

enum SearchContextSize {
  low,
  medium,
  high,
}

model WebSearchToolFilters {
  allowed_domains?: string[] | null;
}

// Tool customization: Establish a discriminated base for location approximation
union LocationType {
  string,
  approximate: "approximate",
}
@discriminator("type")
model Location {
  type: LocationType;
}
model ApproximateLocation extends Location {
  type: LocationType.approximate;
  country?: string | null;
  region?: string | null;
  city?: string | null;
  timezone?: string | null;
}

model RankingOptions {
  /** The ranker to use for the file search. */
  ranker?: "auto" | "default-2024-11-15";

  /** The score threshold for the file search, a number between 0 and 1. Numbers closer to 1 will attempt to return only the most relevant results, but may return fewer results. */
  score_threshold?: float32;
}

union Filters {
  ComparisonFilter,
  CompoundFilter,
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that runs Python code to help generate a response to a prompt. */
model CodeInterpreterTool extends Tool {
  @doc("""
    The type of the code interpreter tool. Always `code_interpreter`.
    """)
  type: ToolType.code_interpreter;

  /**
   * The code interpreter container. Can be a container ID or an object that
   * specifies uploaded file IDs to make available to your code.
   */
  container: string | CodeInterpreterToolAuto;
}

/**
 * Configuration for a code interpreter container. Optionally specify the IDs
 * of the files to run the code on.
 */
model CodeInterpreterToolAuto extends CodeInterpreterContainerConfiguration {
  @doc("Always `auto`.")
  type: CodeInterpreterContainerConfigurationType.auto;

  /** An optional list of uploaded files to make available to your code. */
  file_ids?: string[];
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/** A tool that allows the model to execute shell commands in a local environment. */
model LocalShellTool extends Tool {
  @doc("""
    The type of the local shell tool. Always `local_shell`.
    """)
  type: ToolType.local_shell;
}

// Tool customization (apply_discriminator): Apply discriminated type base for tools
/**
 * Give the model access to additional tools via remote Model Context Protocol
 * (MCP) servers. [Learn more about MCP](/docs/guides/tools-remote-mcp).
 */
model MCPTool extends Tool {
  @doc("""
    The type of the MCP tool. Always `mcp`.
    """)
  type: ToolType.mcp;

 /** A label for this MCP server, used to identify it in tool calls. */
  server_label: string;

  /**The URL for the MCP server. One of `server_url` or `connector_id` must be
  provided.*/
  server_url?: string;

  /**Identifier for service connectors, like those available in ChatGPT. One of
  `server_url` or `connector_id` must be provided. Learn more about service
  connectors [here](https://platform.openai.com/docs/guides/tools-remote-mcp#connectors).

  Currently supported `connector_id` values are:

  - Dropbox: `connector_dropbox`
  - Gmail: `connector_gmail`
  - Google Calendar: `connector_googlecalendar`
  - Google Drive: `connector_googledrive`
  - Microsoft Teams: `connector_microsoftteams`
  - Outlook Calendar: `connector_outlookcalendar`
  - Outlook Email: `connector_outlookemail`
  - SharePoint: `connector_sharepoint`*/
  connector_id?:
    | "connector_dropbox"
    | "connector_gmail"
    | "connector_googlecalendar"
    | "connector_googledrive"
    | "connector_microsoftteams"
    | "connector_outlookcalendar"
    | "connector_outlookemail"
    | "connector_sharepoint";

  /**An OAuth access token that can be used with a remote MCP server, either
  with a custom MCP server URL or a service connector. Your application
  must handle the OAuth authorization flow and provide the token here.*/
  authorization?: string;

  /** Optional description of the MCP server, used to provide more context. */
  server_description?: string;

  /**Optional HTTP headers to send to the MCP server. Use for authentication
  or other purposes.*/
  headers?: Record<string> | null;

  /** List of allowed tool names or a filter object. */
  allowed_tools?: string[] | MCPToolFilter | null;

  /** Specify which of the MCP server's tools require approval. */
  require_approval?:
    | {
        always?: MCPToolFilter;
        `never`?: MCPToolFilter;
      }
    | "always"
    | "never"
    | null;
}

/** A filter object to specify which tools are allowed. */
@summary("MCP tool filter")
model MCPToolFilter {
  /** List of allowed tool names. */
  @summary("MCP allowed tools")
  tool_names?: string[];

  /**Indicates whether or not a tool modifies data or is read-only. If an
  MCP server is [annotated with `readOnlyHint`](https://modelcontextprotocol.io/specification/2025-06-18/schema#toolannotations-readonlyhint),
  it will match this filter.*/
  read_only?: boolean;
}
