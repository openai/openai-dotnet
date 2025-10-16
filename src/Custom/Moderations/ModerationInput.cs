namespace OpenAI.Moderations;

/// <summary>
/// Represents a moderation input item which can be either text or an image URL (including data URLs).
/// </summary>
public readonly struct ModerationInput
{
    public ModerationInputType Type { get; }
    public string Text { get; }
    public string ImageUrl { get; }

    private ModerationInput(ModerationInputType type, string text, string imageUrl)
    {
        Type = type;
        Text = text;
        ImageUrl = imageUrl;
    }

    /// <summary>Create a text input: {"type":"text","text": "..."}.</summary>
    public static ModerationInput FromText(string text)
    {
        Argument.AssertNotNullOrEmpty(text, nameof(text));
        return new ModerationInput(ModerationInputType.Text, text, null);
    }

    /// <summary>
    /// Create an image URL input: {"type":"image_url","image_url":{"url":"..."}}.
    /// The URL can also be a data URL (e.g., "data:image/jpeg;base64,...").
    /// </summary>
    public static ModerationInput FromImageUrl(string url)
    {
        Argument.AssertNotNullOrEmpty(url, nameof(url));
        return new ModerationInput(ModerationInputType.ImageUrl, null, url);
    }
}
