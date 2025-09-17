import "../base/typespec/models/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientLocation(listModels, "Models");
@@clientName(listModels, "GetModels");

@@clientLocation(deleteModel, "Models");

@@clientLocation(retrieveModel, "Models");
@@clientName(retrieveModel, "GetModel");