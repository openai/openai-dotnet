using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ResponsesMessageItemResource")]
public partial class MessageResponseItem
{
    // CUSTOM: Expose public enum type with 'Unknown' using internal extensible role.
    [CodeGenMember("Role")]
    internal InternalResponsesMessageRole InternalRole { get; set; }
    public MessageRole Role
    {
        get => InternalRole.ToString().ToMessageRole();
        private set => InternalRole = value.ToSerialString();
    }

    // CUSTOM: Retain optionality of OpenAPI read-only property value
    [CodeGenMember("Status")]
    public MessageStatus? Status { get; }

    // CUSTOM: Recombined content from derived types.
    public IList<ResponseContentPart> Content
        => (this as InternalResponsesUserMessage)?.InternalContent
        ?? (this as InternalResponsesDeveloperMessage)?.InternalContent
        ?? (this as InternalResponsesSystemMessage)?.InternalContent
        ?? (this as InternalResponsesAssistantMessage)?.InternalContent
        ?? [];

    // CUSTOM: For reuse as an input model base
    internal MessageResponseItem(InternalResponsesMessageRole internalRole)
        : this(id: null, internalRole, status: null)
    { }
}
