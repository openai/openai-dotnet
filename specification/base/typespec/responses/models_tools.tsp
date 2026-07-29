import "@typespec/openapi";
import "@azure-tools/typespec-client-generator-core";
import "./models.tsp";

using Azure.ClientGenerator.Core;
using TypeSpec.OpenAPI;

namespace OpenAI;

// --------- Custom Tool ---------

model CustomTool extends Tool {
  /** The type of the custom tool. Always `custom`. */
  type: ToolType.custom;

  /** The name of the custom tool, used to identify it in tool calls. */
  name: string;

  /** Optional description of the custom tool, used to provide more context. */
  description?: string;

  /** The input format for the custom tool. Default is unconstrained text. */
  format?: CustomToolFormat;

  // WIP
  // /** Whether this tool should be deferred and discovered via tool search. */
  // defer_loading?: boolean;

  // WIP
  // allowed_callers?: CallableToolAllowedCaller[] | null;
}

union CustomToolFormatType {
  string,
  text: "text",
  grammar: "grammar",
}

@discriminator("type")
model CustomToolFormat {
  type: CustomToolFormatType;
}

model CustomToolTextFormat extends CustomToolFormat {
  /** Unconstrained text format. Always `text`. */
  type: CustomToolFormatType.text;
}

model CustomToolGrammarFormat extends CustomToolFormat {
  /** Grammar format. Always `grammar`. */
  type: CustomToolFormatType.grammar;

  /** The syntax of the grammar definition. One of `lark` or `regex`. */
  syntax: CustomToolGrammarFormatSyntax;

  /** The grammar definition. */
  definition: string;
}

union CustomToolGrammarFormatSyntax {
  string,
  lark: "lark",
  regex: "regex",
}