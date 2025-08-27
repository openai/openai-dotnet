import "../base/typespec/responses/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateResponse.service_tier, DotNetResponseServiceTier);
@@alternateType(Response.service_tier, DotNetResponseServiceTier);
