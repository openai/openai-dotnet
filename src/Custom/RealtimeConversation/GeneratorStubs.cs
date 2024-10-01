using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioFormat")] public readonly partial struct ConversationAudioFormat { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioInputTranscriptionModel")] public readonly partial struct ConversationTranscriptionModel { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioInputTranscriptionSettings")] public partial class ConversationInputTranscriptionOptions { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeContentPartType")] public readonly partial struct ConversationContentPartKind { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeItemStatus")] public readonly partial struct ConversationItemStatus { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeMessageRole")] public readonly partial struct ConversationMessageRole { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseAudioDeltaCommand")] public partial class ConversationAudioDeltaUpdate{ }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseAudioDoneCommand")] public partial class ConversationAudioDoneUpdate{ }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseAudioTranscriptDeltaCommand")] public partial class ConversationOutputTranscriptionDeltaUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseAudioTranscriptDoneCommand")] public partial class ConversationOutputTranscriptionFinishedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseFunctionCallArgumentsDeltaCommand")] public partial class ConversationFunctionCallArgumentsDeltaUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseFunctionCallArgumentsDoneCommand")] public partial class ConversationFunctionCallArgumentsDoneUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseInputAudioBufferClearedCommand")] public partial class ConversationInputAudioBufferClearedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseInputAudioBufferCommittedCommand")] public partial class ConversationInputAudioBufferCommittedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseInputAudioBufferSpeechStartedCommand")] public partial class ConversationInputSpeechStartedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseInputAudioBufferSpeechStoppedCommand")] public partial class ConversationInputSpeechFinishedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseItemDeletedCommand")] public partial class ConversationItemDeletedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseItemInputAudioTranscriptionCompletedCommand")] public partial class ConversationInputTranscriptionFinishedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseItemTruncatedCommand")] public partial class ConversationItemTruncatedUpdate { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseRateLimitDetailsItem")] public partial class ConversationRateLimitDetailsItem { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseRateLimitsUpdatedCommand")] public partial class ConversationRateLimitsUpdatedUpdate{ }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseStatus")] public readonly partial struct ConversationStatus { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseTextDeltaCommand")] public partial class ConversationTextDeltaUpdate{ }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseTextDoneCommand")] public partial class ConversationTextDoneUpdate{ }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsage")] public partial class ConversationTokenUsage { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsageInputTokenDetails")] public partial class ConversationInputTokenUsageDetails { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsageOutputTokenDetails")] public partial class ConversationOutputTokenUsageDetails { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeToolType")] public readonly partial struct ConversationToolKind { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeVoice")] public readonly partial struct ConversationVoice { }
