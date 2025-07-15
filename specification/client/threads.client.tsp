import "../base/typespec/threads/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@access(CreateThreadRequestToolResourcesFileSearchBase, Access.public);
@@usage(CreateThreadRequestToolResourcesFileSearchBase, Usage.input);

@@visibility(ThreadObject.metadata, Lifecycle.Read);
