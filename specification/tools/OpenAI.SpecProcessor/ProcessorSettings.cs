namespace OpenAI.SpecProcessor;

/// <summary> Root configuration for the spec processor tool, bound from appsettings.json. </summary>
public class ProcessorSettings
{
    /// <summary> Settings related to spec file names and URLs. </summary>
    public SpecSettings Spec { get; set; } = new();

    /// <summary> Default values for command-line options. </summary>
    public DefaultSettings Defaults { get; set; } = new();
}