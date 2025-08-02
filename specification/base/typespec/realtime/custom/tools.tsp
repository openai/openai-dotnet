using TypeSpec.OpenAPI;

namespace OpenAI;

/**
 * The supported tool type discriminators for realtime tools.
 * Currently, only 'function' tools are supported.
 */
union RealtimeToolType {
  string,
  function: "function",
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
 * The combined set of available representations for a realtime tool_choice parameter, encompassing both string
 * literal options like 'auto' as well as structured references to defined tools.
 */
union RealtimeToolChoice {
  RealtimeToolChoiceLiteral,
  RealtimeToolChoiceObject,
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
