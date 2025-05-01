namespace OpenAI;

/// <summary>
/// Options for finding related tools.
/// </summary>
public class ToolSelectionOptions
{
    /// <summary>
    /// Gets or sets the maximum number of tools to return. Default is 3.
    /// </summary>
    public int MaxTools { get; set; } = 3;

    /// <summary>
    /// Gets or sets the similarity threshold for including tools. Default is 0.29.
    /// </summary>
    public float MinVectorDistance { get; set; } = 0.29f;
}