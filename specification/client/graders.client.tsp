import "../base/typespec/graders/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@convenientAPI(Graders.runGrader, false);
@@convenientAPI(Graders.validateGrader, false);
