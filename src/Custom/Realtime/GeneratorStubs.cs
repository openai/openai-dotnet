using System.ClientModel;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeAudioFormat")] public readonly partial struct RealtimeAudioFormat { }
[CodeGenType("RealtimeAudioInputTranscriptionModel")] public readonly partial struct InputTranscriptionModel { }
[CodeGenType("RealtimeAudioInputTranscriptionSettings")] public partial class InputTranscriptionOptions { }
[CodeGenType("RealtimeCreateClientSecretResponse")] public partial class RealtimeCreateClientSecretResponse { }
[CodeGenType("RealtimeItemStatus")] public readonly partial struct ConversationItemStatus { }
[CodeGenType("RealtimeMessageRole")] public readonly partial struct ConversationMessageRole { }
[CodeGenType("RealtimeResponseStatus")] public readonly partial struct ConversationStatus { }
[CodeGenType("RealtimeResponseUsage")] public partial class ConversationTokenUsage { }
[CodeGenType("RealtimeSessionCreateResponseUnion")] public partial class RealtimeSessionCreateResponseUnion { }
[CodeGenType("RealtimeSessionCreateResponseUnionType")] public readonly partial struct RealtimeSessionCreateResponseUnionType { }
[CodeGenType("RealtimeToolType")] public readonly partial struct ConversationToolKind { }
[CodeGenType("UnknownRealtimeSessionCreateRequestUnion")] internal partial class UnknownRealtimeSessionCreateRequestUnion { }
[CodeGenType("UnknownRealtimeSessionCreateResponseUnion")] internal partial class UnknownRealtimeSessionCreateResponseUnion { }
[CodeGenType("DotNetRealtimeVoiceIds")] public readonly partial struct ConversationVoice { }
[CodeGenType("RealtimeSemanticVadTurnDetectionEagerness")] public readonly partial struct SemanticEagernessLevel { }

// Session update request discriminated union types (must be public because ConversationSessionOptions extends them)
[CodeGenType("RealtimeSessionType")] public readonly partial struct RealtimeSessionType { }
[CodeGenType("RealtimeRequestSessionBase")] public partial class RealtimeRequestSessionBase { }

// Session audio configuration types (need to be public for ConversationSessionOptions.Audio property)
[CodeGenType("RealtimeSessionAudioConfiguration")] public partial class RealtimeSessionAudioConfiguration { }
[CodeGenType("RealtimeSessionAudioInputConfiguration")] public partial class RealtimeSessionAudioInputConfiguration { }
[CodeGenType("RealtimeSessionAudioOutputConfiguration")] public partial class RealtimeSessionAudioOutputConfiguration { }