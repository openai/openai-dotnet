using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenAI.Chat;

[CodeGenModel("ChatMessageContent")]
public partial class ChatMessageContent : Collection<ChatMessageContentPart>
{
    public ChatMessageContent()
        : this([])
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

    internal bool IsInnerCollectionDefined()
    {
        return !(Items is ChangeTrackingList<ChatMessageContentPart> changeTrackingList && changeTrackingList.IsUndefined);
    }
}
