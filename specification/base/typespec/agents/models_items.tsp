import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

// ------------ Session input ------------

/** The type of content included in an input message. */
union InputContentType {
  string,
  input_text: "input_text",
  input_image: "input_image",
}

@discriminator("type")
model InputContentParam {
  type: InputContentType;
}

/** Text input to the model. */
model InputContentParamInputText extends InputContentParam {
  /** The type of the object. Always `input_text`. */
  type: InputContentType.input_text;

  /** The text sent to the model. */
  text: string;
}

/** Image input to the model. */
model InputContentParamInputImage extends InputContentParam {
  /** The type of the object. Always `input_image`. */
  type: InputContentType.input_image;

  /** The URL of the image sent to the model. */
  image_url: string;
}

/** A user message submitted to a session. */
model InputMessageParam {
  /** The type of the input item. Always `message`. */
  type?: "message";

  /** The role of the message author. Always `user`. */
  role: "user";

  /** The content of the message. */
  @maxItems(16384)
  content: InputContentParam[];
}

// ------------ Session item content ------------

/** The author of a session message. */
union SessionMessageRoleResource {
  user: "user",
  assistant: "assistant",
}

/** The status of an agent output item. */
union OutputItemStatusResource {
  in_progress: "in_progress",
  completed: "completed",
  incomplete: "incomplete",
}

/** The phase of an assistant message. */
union MessagePhaseResource {
  /** Commentary produced while the agent works. */
  commentary: "commentary",

  /** The agent's final answer. */
  final_answer: "final_answer",
}

/** The status of a tool call. */
union FunctionCallStatusResource {
  in_progress: "in_progress",
  completed: "completed",
  failed: "failed",
  incomplete: "incomplete",
}

/** The type of content part in a session message. */
union AgentSessionMessageContentType {
  string,
  input_text: "input_text",
  input_image: "input_image",
  output_text: "output_text",
}

@discriminator("type")
model MessageContentResource {
  type: AgentSessionMessageContentType;
}

/** Text supplied by the user. */
model MessageContentResourceInputText extends MessageContentResource {
  /** The type of the object. Always `input_text`. */
  type: AgentSessionMessageContentType.input_text;

  /** The text supplied by the user. */
  text: string;
}

/** An image supplied by the user. */
model MessageContentResourceInputImage extends MessageContentResource {
  /** The type of the object. Always `input_image`. */
  type: AgentSessionMessageContentType.input_image;

  /** The URL of the image supplied by the user, which may be a base64-encoded data URL. */
  image_url: string;
}

/** Text produced by the assistant. */
model MessageContentResourceOutputText extends MessageContentResource {
  /** The type of the object. Always `output_text`. */
  type: AgentSessionMessageContentType.output_text;

  /** The text produced by the assistant. */
  text: string;
}

/** The type of content part exchanged between agents. */
union AgentContentType {
  string,
  output_text: "output_text",
  encrypted_content: "encrypted_content",
}

@discriminator("type")
model AgentContentResource {
  type: AgentContentType;
}

/** A text content part produced by the agent. */
model OutputTextResource extends AgentContentResource {
  /** The content type. Always `output_text`. */
  type: AgentContentType.output_text;

  /** The text produced by the agent. */
  text: string;
}

/** Encrypted content exchanged between agents. */
model EncryptedContentResource extends AgentContentResource {
  /** The content type. Always `encrypted_content`. */
  type: AgentContentType.encrypted_content;

  /** The encrypted content payload. */
  encrypted_content: string;
}

@discriminator("type")
model InputContentResource {
  type: InputContentType;
}

/** Text input recorded in a session item. */
model InputContentResourceInputText extends InputContentResource {
  /** The type of the object. Always `input_text`. */
  type: InputContentType.input_text;

  /** The text supplied to the agent. */
  text: string;
}

/** Image input recorded in a session item. */
model InputContentResourceInputImage extends InputContentResource {
  /** The type of the object. Always `input_image`. */
  type: InputContentType.input_image;

  /** The URL of the image supplied to the agent, which may be a base64-encoded data URL. */
  image_url: string;
}

/** A reasoning summary content part. */
model SummaryTextResource {
  /** The content type. Always `summary_text`. */
  type?: "summary_text";

  /** The reasoning summary text. */
  text: string;
}

// ------------ Web search action ------------

/** The type of action performed by the web search tool. */
union AgentSessionWebSearchActionType {
  string,
  search: "search",
  open_page: "open_page",
  find_in_page: "find_in_page",
  other: "other",
}

@discriminator("type")
model WebSearchActionResource {
  type: AgentSessionWebSearchActionType;
}

/** A search query or group of search queries. */
model WebSearchActionResourceSearch extends WebSearchActionResource {
  /** The type of the object. Always `search`. */
  type: AgentSessionWebSearchActionType.search;

  /** The search query, when a single query was used. */
  query: string | null;

  /** The search queries, when multiple queries were used. */
  @maxItems(2000)
  queries: string[] | null;
}

/** Opens a web page. */
model WebSearchActionResourceOpenPage extends WebSearchActionResource {
  /** The type of the object. Always `open_page`. */
  type: AgentSessionWebSearchActionType.open_page;

  /** The URL of the page that was opened. */
  url: string | null;
}

/** Finds text within a web page. */
model WebSearchActionResourceFindInPage extends WebSearchActionResource {
  /** The type of the object. Always `find_in_page`. */
  type: AgentSessionWebSearchActionType.find_in_page;

  /** The URL of the page that was searched. */
  url: string | null;

  /** The text pattern that was searched for. */
  pattern: string | null;
}

/** Another web search action. */
model WebSearchActionResourceOther extends WebSearchActionResource {
  /** The type of the object. Always `other`. */
  type: AgentSessionWebSearchActionType.other;
}

// ------------ Session turn items ------------

/** The type of an item associated with a session turn. */
union SessionTurnItemType {
  string,
  message: "message",
  reasoning: "reasoning",
  function_call: "function_call",
  function_call_output: "function_call_output",
  agent_message: "agent_message",
  mcp_call: "mcp_call",
  web_search_call: "web_search_call",
  command_execution: "command_execution",
  create_subagent_call: "create_subagent_call",
  send_subagent_input_call: "send_subagent_input_call",
  resume_subagent_call: "resume_subagent_call",
  wait_for_subagents_call: "wait_for_subagents_call",
  interrupt_subagent_call: "interrupt_subagent_call",
  close_subagent_call: "close_subagent_call",
}

@discriminator("type")
model SessionTurnItemResource {
  type: SessionTurnItemType;
}

/** A user or assistant message recorded in a session. */
model MessageItemResource extends SessionTurnItemResource {
  /** The item type. Always `message`. */
  type: SessionTurnItemType.message;

  /** The ID of this item, or null for legacy user messages whose ID was not recorded. */
  id: string | null;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The role of the message author. */
  role: SessionMessageRoleResource;

  /** The content of the message. */
  @maxItems(2000)
  content: MessageContentResource[];

  /** The status of the message. User messages are always `completed`. */
  status: OutputItemStatusResource;

  /** The phase of an assistant message. Null for user messages. */
  phase: MessagePhaseResource | null;
}

/** A reasoning item produced by the agent. */
model AgentSessionReasoningItemResource extends SessionTurnItemResource {
  /** The item type. Always `reasoning`. */
  type: SessionTurnItemType.reasoning;

  /** The ID of the reasoning item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The reasoning summaries produced by the agent. */
  @maxItems(2000)
  summary: SummaryTextResource[];

  /** The status of the reasoning item. */
  status: OutputItemStatusResource | null;
}

/** A function call produced by the agent. */
model FunctionCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `function_call`. */
  type: SessionTurnItemType.function_call;

  /** The ID of the function call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The ID used to submit the function result. */
  call_id: string;

  /** The name of the function to call. */
  name: string;

  /** The arguments to pass to the function. */
  arguments: unknown;

  /** The status of the function call. */
  status: FunctionCallStatusResource;
}

/** The result supplied for a function call. */
model FunctionCallOutputItemResource extends SessionTurnItemResource {
  /** The item type. Always `function_call_output`. */
  type: SessionTurnItemType.function_call_output;

  /** The ID of the function call output item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The ID of the function call that produced this output. */
  call_id: string;

  /** The status of the function call. */
  status: FunctionCallStatusResource;

  /** The function result, if the call succeeded. */
  output: (string | InputContentResource[]) | null;

  /** The error message, if the call failed. */
  error: string | null;
}

/** A message exchanged between agent threads. */
model AgentMessageItemResource extends SessionTurnItemResource {
  /** The item type. Always `agent_message`. */
  type: SessionTurnItemType.agent_message;

  /** The ID of the message. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The ID or name of the sending agent. */
  sender_agent_id: string;

  /** The ID or name of the receiving agent. */
  recipient_agent_id: string;

  /** The content exchanged between the agents. */
  @maxItems(2000)
  content: AgentContentResource[];
}

/** A call to a tool on an MCP server. */
model McpCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `mcp_call`. */
  type: SessionTurnItemType.mcp_call;

  /** The ID of the MCP call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The label of the MCP server. */
  server_label: string;

  /** The name of the MCP tool. */
  name: string;

  /** The arguments passed to the MCP tool. */
  arguments: unknown;

  /** The status of the MCP tool call. */
  status: FunctionCallStatusResource;

  /** The output returned by the MCP tool, if any. */
  output: unknown | null;

  /** The error returned by the MCP tool, if any. */
  error: unknown | null;
}

/** A web search call produced by the agent. */
model WebSearchCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `web_search_call`. */
  type: SessionTurnItemType.web_search_call;

  /** The ID of the web search call. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the web search call. */
  status: OutputItemStatusResource;

  /** The action performed by the web search tool. */
  action: WebSearchActionResource | null;
}

/** A command execution produced by the agent. */
model CommandExecutionItemResource extends SessionTurnItemResource {
  /** The item type. Always `command_execution`. */
  type: SessionTurnItemType.command_execution;

  /** The ID of the command execution item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The command that was executed. */
  command: string;

  /** The working directory used to execute the command. */
  cwd: string | null;

  /** The status of the command execution. */
  status: FunctionCallStatusResource;

  /** The command output, if available. */
  output: string | null;

  /** The process exit code, if the command completed. */
  exit_code: int64 | null;

  /** The command duration in milliseconds. */
  duration_ms: int64 | null;
}

/** A request to spawn a subagent. */
model CreateSubagentCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `create_subagent_call`. */
  type: SessionTurnItemType.create_subagent_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent that requested the subagent. */
  agent_id: string;

  /** The task given to the spawned agent. */
  @maxItems(2000)
  content: AgentContentResource[];

  /** The model requested for the spawned agent. */
  `model`: string | null;

  /** The reasoning effort requested for the spawned agent. */
  reasoning_effort: string | null;
}

/** A request to send input to another agent. */
model SendSubagentInputCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `send_subagent_input_call`. */
  type: SessionTurnItemType.send_subagent_input_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent sending the input. */
  sender_agent_id: string;

  /** The ID of the agent receiving the input. */
  recipient_agent_id: string;

  /** The input sent to the receiving agent. */
  @maxItems(2000)
  content: AgentContentResource[];
}

/** A request to resume a subagent. */
model ResumeSubagentCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `resume_subagent_call`. */
  type: SessionTurnItemType.resume_subagent_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent requesting the resume. */
  sender_agent_id: string;

  /** The ID of the agent to resume. */
  recipient_agent_id: string;
}

/** A request to wait for one or more subagents. */
model WaitForSubagentsCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `wait_for_subagents_call`. */
  type: SessionTurnItemType.wait_for_subagents_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent waiting for results. */
  sender_agent_id: string;

  /** The IDs of the agents to wait for. */
  @maxItems(2000)
  recipient_agent_ids: string[];
}

/** A request to interrupt a subagent's current turn. The subagent remains available. */
model InterruptSubagentCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `interrupt_subagent_call`. */
  type: SessionTurnItemType.interrupt_subagent_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent requesting the interrupt. */
  sender_agent_id: string;

  /** The ID of the agent to interrupt. */
  recipient_agent_id: string;
}

/** A request to close a subagent. */
model CloseSubagentCallItemResource extends SessionTurnItemResource {
  /** The item type. Always `close_subagent_call`. */
  type: SessionTurnItemType.close_subagent_call;

  /** The ID of the tool call item. */
  id: string;

  /** The ID of the turn that contains this item. */
  turn_id: string;

  /** The status of the tool call. */
  status: FunctionCallStatusResource;

  /** The ID of the agent requesting the close. */
  sender_agent_id: string;

  /** The ID of the agent to close. */
  recipient_agent_id: string;
}
