import "../base/typespec/moderations/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateModerationResponseResultsCategoryScores.hate, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`hate/threatening`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.harassment, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`harassment/threatening`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.illicit, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`illicit/violent`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`self-harm`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`self-harm/intent`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`self-harm/instructions`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.sexual, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`sexual/minors`, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.violence, float32);
@@alternateType(CreateModerationResponseResultsCategoryScores.`violence/graphic`, float32);

@@clientName(createModeration, "ClassifyInputs");

@@clientLocation(createModeration, "Moderations");
