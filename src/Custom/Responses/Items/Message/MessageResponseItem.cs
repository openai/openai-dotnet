using System.Collections.Generic;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponsesMessageItemResource")]
public partial class MessageResponseItem
{
    // CUSTOM:
    // - Made nullable because this is an optional property.
    // - Added setter because this is an optional property in an input/output type.
    [CodeGenMember("Status")]
    public MessageStatus? Status { get; set; }

    // CUSTOM: Expose public enum type with 'Unknown' using internal extensible role.
    [CodeGenMember("Role")]
    internal InternalResponsesMessageRole InternalRole { get; set; }
    public MessageRole Role
    {
        get => InternalRole.ToString().ToMessageRole();
        private set => InternalRole = value.ToSerialString();
    }

    // CUSTOM: Recombined content from derived types.
    public IList<ResponseContentPart> Content
        => (this as InternalResponsesUserMessage)?.InternalContent
        ?? (this as InternalResponsesDeveloperMessage)?.InternalContent
        ?? (this as InternalResponsesSystemMessage)?.InternalContent
        ?? (this as InternalResponsesAssistantMessage)?.InternalContent
        ?? [];
}
