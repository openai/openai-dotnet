import "../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateResponse.service_tier, DotNetResponseServiceTier);
@@alternateType(Response.service_tier, DotNetResponseServiceTier);

@@visibility(ItemResource.id, Lifecycle.Read);
@@usage(ItemResource, Usage.input | Usage.output);

@@visibility(ComputerToolCallItemResource.status, Lifecycle.Read);

@@visibility(ComputerToolCallOutputItemResource.status, Lifecycle.Read);

@@visibility(FileSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(FunctionToolCallItemResource.status, Lifecycle.Read);
@@clientName(FunctionToolCallItemResource.name, "FunctionName");
@@clientName(FunctionToolCallItemResource.arguments, "FunctionArguments");
@@alternateType(FunctionToolCallItemResource.arguments, unknown);

@@visibility(FunctionToolCallOutputItemResource.status, Lifecycle.Read);
@@clientName(FunctionToolCallOutputItemResource.output, "FunctionOutput");

@@visibility(ReasoningItemResource.status, Lifecycle.Read);
@@clientName(ReasoningItemResource.summary, "SummaryParts");

@@visibility(WebSearchToolCallItemResource.status, Lifecycle.Read);

@@visibility(ResponsesMessageItemResource.status, Lifecycle.Read);
