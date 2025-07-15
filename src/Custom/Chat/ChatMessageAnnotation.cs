using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Chat;

// CUSTOM: Added Experimental attribute.
[CodeGenType("ChatCompletionResponseMessageAnnotation")]
public partial class ChatMessageAnnotation
{
    public int StartIndex => UrlCitation?.StartIndex
        ?? 0;
    public int EndIndex => UrlCitation?.EndIndex
        ?? 0;
    public Uri WebResourceUri => UrlCitation?.Url;
    public string WebResourceTitle => UrlCitation?.Title;

    [CodeGenMember("UrlCitation")]
    internal InternalChatCompletionResponseMessageAnnotationUrlCitation UrlCitation { get; }

    // CUSTOM: Made internal
    [CodeGenMember("Kind")]
    internal string Kind { get; } = "url_citation";
}