import "../base/typespec/audio/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(CreateTranscriptionResponseJsonLogprobs.bytes, float64[]);
@@alternateType(CreateTranscriptionRequest.temperature, float32);
@@alternateType(CreateTranslationRequest.temperature, float32);

@@alternateType(CreateSpeechRequest.speed, float32);
@@alternateType(VadConfig.threshold, float32);
@@alternateType(TranscriptTextUsageTokens.input_tokens, int32);
@@alternateType(TranscriptTextUsageTokens.output_tokens, int32);
@@alternateType(TranscriptTextUsageTokens.total_tokens, int32);
@@alternateType(TranscriptTextUsageTokensInputTokenDetails.text_tokens, int32);
@@alternateType(TranscriptTextUsageTokensInputTokenDetails.audio_tokens, int32);
@@alternateType(CreateTranscriptionResponseJsonLogprobs.logprob, float32);
@@alternateType(TranscriptionSegment.id, int32);
@@alternateType(TranscriptionSegment.seek, int32);
@@alternateType(SpeechAudioDoneEventUsage.input_tokens, int32);
@@alternateType(SpeechAudioDoneEventUsage.output_tokens, int32);
@@alternateType(SpeechAudioDoneEventUsage.total_tokens, int32);
@@alternateType(TranscriptTextDeltaEventLogprobs.logprob, float32);
@@alternateType(TranscriptTextDoneEventLogprobs.logprob, float32);
@@alternateType(listVoiceConsents::parameters.limit, int32);

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
