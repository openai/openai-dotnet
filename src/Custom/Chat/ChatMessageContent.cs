using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenAI.Chat;

[CodeGenType("ChatMessageContent")]
public partial class ChatMessageContent : Collection<ChatMessageContentPart>
{
    public ChatMessageContent()
        : base(new ChangeTrackingList<ChatMessageContentPart>())
    {
    }

    public ChatMessageContent(string content)
        : this([ChatMessageContentPart.CreateTextPart(content)])
    {
    }

    public ChatMessageContent(IEnumerable<ChatMessageContentPart> contentParts)
        : base(new ChangeTrackingList<ChatMessageContentPart>((IList<ChatMessageContentPart>)contentParts.ToList()))
    {
    }

    public ChatMessageContent(params ChatMessageContentPart[] contentParts)
        : base(new ChangeTrackingList<ChatMessageContentPart>((IList<ChatMessageContentPart>)[.. contentParts]))
    {
    }

    internal ChatMessageContent(IDictionary<string, BinaryData> additionalBinaryDataProperties)
        : this()
    { }

    internal bool IsInnerCollectionDefined()
    {
        return !(Items is ChangeTrackingList<ChatMessageContentPart> changeTrackingList && changeTrackingList.IsUndefined);
    }
}
