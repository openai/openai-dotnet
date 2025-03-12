using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenAI.Chat;

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

    [CodeGenMember("Type")]
    internal InternalChatCompletionResponseMessageAnnotationType Type { get; } = "url_citation";

}