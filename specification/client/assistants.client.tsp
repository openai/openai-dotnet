import "../base/typespec/assistants/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@visibility(ListAssistantsResponse.data, Lifecycle.Read);
@@visibility(AssistantObject.tools, Lifecycle.Read);
@@visibility(AssistantObject.metadata, Lifecycle.Read);

@@clientName(Assistants.listAssistants, "GetAssistants");

@@dynamicModel(AssistantMessageRole);
