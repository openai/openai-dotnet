using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

    /// <summary>
    /// Returns a string representation of the chat message content by iterating through all content parts.
    /// </summary>
    /// <returns>A formatted string representation of all content parts.</returns>
    public override string ToString()
    {
        StringBuilder builder = new();
        
        if (Count == 0)
        {
            return "<empty content>";
        }
        
        for (int i = 0; i < Count; i++)
        {
            if (i > 0) {
                builder.AppendLine();
            }
            
            var part = this[i];
            switch (part.Kind)
            {
                case ChatMessageContentPartKind.Text:
                    builder.Append(part.Text);
                    break;
                    
                case ChatMessageContentPartKind.Image:
                    builder.Append("<image>");
                    break;
                    
                case ChatMessageContentPartKind.InputAudio:
                    builder.Append("<audio>");
                    break;
                    
                case ChatMessageContentPartKind.File:
                    builder.Append($"<file: {part.Filename}>");
                    break;
                    
                case ChatMessageContentPartKind.Refusal:
                    var refusal = part.Refusal;
                    builder.Append($"<refusal: {refusal}>");
                    break;
                    
                default:
                    builder.Append("<unknown content kind>");
                    break;
            }
        }
        
        return builder.ToString();
    }
}
