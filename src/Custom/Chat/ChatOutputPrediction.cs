using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

[CodeGenType("ChatOutputPrediction")]
public partial class ChatOutputPrediction
{
    public static ChatOutputPrediction CreateStaticContentPrediction(IEnumerable<ChatMessageContentPart> staticContentParts)
        => new InternalChatOutputPredictionContent(new ChatMessageContent(staticContentParts));

    public static ChatOutputPrediction CreateStaticContentPrediction(string staticContent)
        => new InternalChatOutputPredictionContent([ChatMessageContentPart.CreateTextPart(staticContent)]);
}