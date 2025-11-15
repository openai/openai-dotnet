import "../base/typespec/audio/main.tsp";
import "./models/audio.models.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;
using Azure.ClientGenerator.Core.Legacy;

@@visibility(CreateTranscriptionResponseVerboseJson.words, Lifecycle.Read);
@@visibility(CreateTranscriptionResponseVerboseJson.segments, Lifecycle.Read);

@@visibility(CreateTranslationResponseVerboseJson.segments, Lifecycle.Read);

@@clientName(Audio.createSpeech, "GenerateSpeech");
@@clientName(Audio.createTranscription, "TranscribeAudio");
@@clientName(Audio.createTranslation, "TranslateAudio");

@@alternateType(TranscriptTextUsageTokens.type, DotNetTranscriptTextUsageType.tokens);
@@alternateType(TranscriptTextUsageDuration.type, DotNetTranscriptTextUsageType.duration);
@@alternateType(CreateTranscriptionResponseVerboseJson.usage, DotNetTranscriptTextUsage);
@@hierarchyBuilding(TranscriptTextUsageTokens, DotNetTranscriptTextUsage);
@@hierarchyBuilding(TranscriptTextUsageDuration, DotNetTranscriptTextUsage);
@@usage(TranscriptTextUsageTokens, Usage.output);
@@usage(TranscriptTextUsageDuration, Usage.output);