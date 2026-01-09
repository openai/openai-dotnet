import "../base/typespec/moderations/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(createModeration, "ClassifyText");

@@clientLocation(createModeration, "Moderations");

@@alternateType(CreateModerationResponseResultsCategories.illicit, boolean);
@@alternateType(CreateModerationResponseResultsCategories.`illicit/violent`, boolean);
