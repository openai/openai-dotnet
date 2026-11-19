namespace OpenAI.SpecProcessor;

/// <summary> Configuration for spec-related file names and endpoints. </summary>
public class SpecSettings
{
    /// <summary> URL to download the latest OpenAI REST spec. </summary>
    public string SourceUrl { get; set; } = "https://raw.githubusercontent.com/openai/openai-openapi/master/openapi.yaml";

    /// <summary> File name used when downloading the raw spec. </summary>
    public string RawDownloadFile { get; set; } = "openai-rest-raw.yml";

    /// <summary> File name for the generated diff report. </summary>
    public string DiffReportFile { get; set; } = "diff-report.md";
}
