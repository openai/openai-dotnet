import "../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using OpenAI.Audio;
using Azure.ClientGenerator.Core;

@@visibility(CreateTranscriptionResponseVerboseJson.words, Lifecycle.Read);
@@visibility(CreateTranscriptionResponseVerboseJson.segments, Lifecycle.Read);

@@visibility(CreateTranslationResponseVerboseJson.segments, Lifecycle.Read);

@@visibility(CreateTranscriptionResponseDiarizedJson.segments, Lifecycle.Read);

@@alternateType(CreateTranscriptionResponseVerboseJson.usage, CreateTranscriptionResponseJsonUsage);

@@clientName(CreateTranscriptionResponseJsonUsage, "AudioTranscriptionUsage", "csharp");

@@clientName(TranscriptTextUsageTokens, "AudioTranscriptionTokenUsage", "csharp");
@@clientName(TranscriptTextUsageTokens.input_tokens, "InputTokenCount", "csharp");
@@clientName(TranscriptTextUsageTokens.output_tokens, "OutputTokenCount", "csharp");
@@clientName(TranscriptTextUsageTokens.total_tokens, "TotalTokenCount", "csharp");

@@clientName(TranscriptTextUsageTokensInputTokenDetails, "AudioTranscriptionInputTokenUsageDetails", "csharp");
@@clientName(TranscriptTextUsageTokensInputTokenDetails.text_tokens, "TextTokenCount", "csharp");
@@clientName(TranscriptTextUsageTokensInputTokenDetails.audio_tokens, "AudioTokenCount", "csharp");

@@clientName(TranscriptTextUsageDuration, "AudioTranscriptionDurationUsage", "csharp");
@@clientName(TranscriptTextUsageDuration.seconds, "Duration", "csharp");

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
