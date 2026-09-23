import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";
import "./models.tsp";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

// --------- Programmatic Tool Calling ---------

union CallableToolAllowedCaller {
  string,
  direct: "direct",
  programmatic: "programmatic",
}

// --------- Custom Tool ---------

model CustomTool extends Tool {
  /** The type of the custom tool. Always `custom`. */
  type: ToolType.custom;

  /** The name of the custom tool, used to identify it in tool calls. */
  name: string;

  // feat: programmatic tool calling
  allowed_callers?: CallableToolAllowedCaller[] | null;

  // feat: tool search
  /** Whether this tool should be deferred and discovered via tool search. */
  defer_loading?: boolean;

  /** Optional description of the custom tool, used to provide more context. */
  description?: string;

  /** The input format for the custom tool. Default is unconstrained text. */
  format?: CustomToolFormat;
}

@discriminator("type")
model CustomToolFormat {
  type: CustomToolFormatType;
}

union CustomToolFormatType {
  string,
  grammar: "grammar",
  text: "text",
}

model CustomToolGrammarFormat extends CustomToolFormat {
  /** Grammar format. Always `grammar`. */
  type: CustomToolFormatType.grammar;

  /** The grammar definition. */
  definition: string;

  /** The syntax of the grammar definition. One of `lark` or `regex`. */
  syntax: CustomToolGrammarFormatSyntax;
}

union CustomToolGrammarFormatSyntax {
  string,
  lark: "lark",
  regex: "regex",
}

model CustomToolTextFormat extends CustomToolFormat {
  /** Unconstrained text format. Always `text`. */
  type: CustomToolFormatType.text;
}