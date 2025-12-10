import "@typespec/openapi";
import "../../entrypoints/sdk.dotnet/specialized-types.tsp";
import "../common";
import "./custom/items.tsp";

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

union CodeInterpreterToolOutputType {
  string,
  logs: "logs",
  image: "image",
}

@discriminator("type")
model CodeInterpreterToolOutput {
  @doc("The type of the output.")
  type: CodeInterpreterToolOutputType;
}

union CodeInterpreterContainerConfigurationType {
  string,
  auto: "auto",
}

@discriminator("type")
model CodeInterpreterContainerConfiguration {  
  @doc("The type of the output.")  
  type: CodeInterpreterContainerConfigurationType;  
}

model DeleteResponseResponse {
    id: string;
    object: "response.deleted";
    deleted: boolean;
};