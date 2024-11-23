using System.Diagnostics.CodeAnalysis;

namespace OpenAI.RealtimeConversation;

[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioFormat")] public readonly partial struct ConversationAudioFormat { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioInputTranscriptionModel")] public readonly partial struct ConversationTranscriptionModel { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeAudioInputTranscriptionSettings")] public partial class ConversationInputTranscriptionOptions { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeItemStatus")] public readonly partial struct ConversationItemStatus { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeMessageRole")] public readonly partial struct ConversationMessageRole { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseStatus")] public readonly partial struct ConversationStatus { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsage")] public partial class ConversationTokenUsage { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsageInputTokenDetails")] public partial class ConversationInputTokenUsageDetails { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeResponseUsageOutputTokenDetails")] public partial class ConversationOutputTokenUsageDetails { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeToolType")] public readonly partial struct ConversationToolKind { }
[Experimental("OPENAI002")][CodeGenModel("RealtimeVoice")] public readonly partial struct ConversationVoice { }
