using System.Collections.Generic;

namespace OpenAI.Chat;

internal static partial class ChatResponseModalitiesExtensions
{
    internal static IList<InternalCreateChatCompletionRequestModality> ToInternalModalities(this ChatResponseModalities modalities)
    {
        ChangeTrackingList<InternalCreateChatCompletionRequestModality> internalModalities = new();
        if (modalities.HasFlag(ChatResponseModalities.Text))
        {
            internalModalities.Add(InternalCreateChatCompletionRequestModality.Text);
        }
        if (modalities.HasFlag(ChatResponseModalities.Audio))
        {
            internalModalities.Add(InternalCreateChatCompletionRequestModality.Audio);
        }
        return internalModalities;
    }

    internal static ChatResponseModalities FromInternalModalities(IEnumerable<InternalCreateChatCompletionRequestModality> internalModalities)
    {
        ChatResponseModalities result = 0;
        foreach (InternalCreateChatCompletionRequestModality internalModality in internalModalities ?? [])
        {
            if (internalModality == InternalCreateChatCompletionRequestModality.Text)
            {
                result |= ChatResponseModalities.Text;
            }
            else if (internalModality == InternalCreateChatCompletionRequestModality.Audio)
            {
                result |= ChatResponseModalities.Audio;
            }
        }
        return result;
    }
}