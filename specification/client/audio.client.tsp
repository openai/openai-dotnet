import "../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@visibility(CreateTranscriptionResponseVerboseJson.words, Lifecycle.Read);
@@visibility(CreateTranscriptionResponseVerboseJson.segments, Lifecycle.Read);

@@visibility(CreateTranslationResponseVerboseJson.segments, Lifecycle.Read);

@@visibility(CreateTranscriptionResponseDiarizedJson.segments, Lifecycle.Read);

@@alternateType(CreateTranscriptionResponseVerboseJson.usage, CreateTranscriptionResponseJsonUsage);

@@clientName(createSpeech, "GenerateSpeech");
@@clientName(createTranscription, "TranscribeAudio");
@@clientName(createTranslation, "TranslateAudio");

@@clientLocation(createSpeech, "Audio");
@@clientLocation(createTranscription, "Audio");
@@clientLocation(createTranslation, "Audio");
@@clientLocation(listVoiceConsents, "Audio");
@@clientLocation(createVoiceConsent, "Audio");
@@clientLocation(deleteVoiceConsent, "Audio");
@@clientLocation(getVoiceConsent, "Audio");
@@clientLocation(updateVoiceConsent, "Audio");
@@clientLocation(createVoice, "Audio");

@@convenientAPI(listVoiceConsents, false);
@@convenientAPI(createVoiceConsent, false);
@@convenientAPI(deleteVoiceConsent, false);
@@convenientAPI(getVoiceConsent, false);
@@convenientAPI(updateVoiceConsent, false);
@@convenientAPI(createVoice, false);
