import "../base/typespec/assistants/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@visibility(ListAssistantsResponse.data, Lifecycle.Read);
@@visibility(AssistantObject.tools, Lifecycle.Read);
@@visibility(AssistantObject.metadata, Lifecycle.Read);

@@clientName(Assistants.listAssistants, "GetAssistants");
