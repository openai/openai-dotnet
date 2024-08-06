using System.Collections.Generic;

namespace OpenAI.Assistants;

/// <summary>
/// Represents an item of annotated text content within an Assistants API response message.
/// </summary>
[CodeGenModel("MessageContentTextObject")]
internal partial class InternalResponseMessageTextContent
{
    /// <inheritdoc cref="InternalMessageContentTextObjectText.Value"/>
    public string InternalText => _text.Value;

    public IReadOnlyList<TextAnnotation> InternalAnnotations => _annotations ??= WrapAnnotations();

    [CodeGenMember("Type")]
    private readonly string _type;

    [CodeGenMember("Text")]
    private readonly InternalMessageContentTextObjectText _text;

    private IReadOnlyList<TextAnnotation> _annotations;

    /// <summary> Initializes a new instance of <see cref="InternalResponseMessageTextContent"/>. </summary>
    /// <param name="internalText"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="internalText"/> is null. </exception>
    internal InternalResponseMessageTextContent(InternalMessageContentTextObjectText internalText)
    {
        Argument.AssertNotNull(internalText, nameof(internalText));

        _text = internalText;
    }

    public override string ToString() => Text;

    private IReadOnlyList<TextAnnotation> WrapAnnotations()
    {
        List<TextAnnotation> annotations = [];
        foreach (InternalMessageContentTextObjectAnnotation internalAnnotation in _text?.Annotations ?? [])
        {
            annotations.Add(new(internalAnnotation));
        }
        return annotations;
    }
}
