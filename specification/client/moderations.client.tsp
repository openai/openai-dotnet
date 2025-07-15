import "../base/typespec/moderations/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(Moderations.createModeration, "ClassifyText");
