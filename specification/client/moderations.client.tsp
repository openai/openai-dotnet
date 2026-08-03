import "../base/typespec/moderations/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

@@clientName(createModeration, "ClassifyInputs");

@@clientLocation(createModeration, "Moderations");

@@dynamicModel(CreateModerationRequestInputType);