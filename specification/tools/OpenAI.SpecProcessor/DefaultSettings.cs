namespace OpenAI.SpecProcessor;

/// <summary> Default values for CLI options when not explicitly provided. </summary>
public class DefaultSettings
{
    /// <summary> Default output directory for processed per-feature spec files. </summary>
    public string OutputDirectory { get; set; } = ".";

    /// <summary> Default directory for the diff report. </summary>
    public string ReportDirectory { get; set; } = ".";

    /// <summary> Default directory containing previous per-feature spec files. </summary>
    public string PreviousSpecDirectory { get; set; } = "./previous";
}
