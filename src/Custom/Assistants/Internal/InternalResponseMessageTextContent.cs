using System.Collections.Generic;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of annotated text content within an Assistants API response message.
/// </summary>
[CodeGenModel("MessageContentTextObject")]
internal partial class InternalResponseMessageTextContent
{
    /// <inheritdoc cref="MessageContentTextObjectText.Value"/>
    public string InternalText => _text.Value;

    public IReadOnlyList<TextAnnotation> InternalAnnotations => _annotations ??= WrapAnnotations();

    [CodeGenMember("Type")]
    private readonly string _type;

    [CodeGenMember("Text")]
    private readonly MessageContentTextObjectText _text;

    private IReadOnlyList<TextAnnotation> _annotations;

    /// <summary> Initializes a new instance of <see cref="InternalResponseMessageTextContent"/>. </summary>
    /// <param name="internalText"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="internalText"/> is null. </exception>
    internal InternalResponseMessageTextContent(MessageContentTextObjectText internalText)
    {
        Argument.AssertNotNull(internalText, nameof(internalText));

        _text = internalText;
    }

    public override string ToString() => Text;

    private IReadOnlyList<TextAnnotation> WrapAnnotations()
    {
        List<TextAnnotation> annotations = [];
        foreach (MessageContentTextObjectAnnotation internalAnnotation in _text?.Annotations ?? [])
        {
            annotations.Add(new(internalAnnotation));
        }
        return annotations;
    }
}
