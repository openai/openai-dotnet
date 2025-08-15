import "@typespec/openapi";
import "../../entrypoints/sdk.dotnet/specialized-types.tsp";
import "../common";
import "./custom/items.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

@error
model ResponseErrorResponse {
  error: ResponseError;
}

union ReasoningItemSummaryPartType {
  string,
  summary_text: "summary_text",
}

@discriminator("type")
model ReasoningItemSummaryPart {
  type: ReasoningItemSummaryPartType;
}

model ReasoningItemSummaryTextPart extends ReasoningItemSummaryPart {
  type: ReasoningItemSummaryPartType.summary_text;
  text: string;
}
