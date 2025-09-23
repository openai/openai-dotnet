using System.Collections.Generic;

namespace OpenAI.Responses;

/// <summary>
/// Represents a configuration for an automatic Code Interpreter container.
/// </summary>
public partial class AutomaticCodeInterpreterContainerConfiguration : CodeInterpreterContainerConfiguration
{

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreterToolAuto"/> class.
    /// </summary>
    /// <param name="fileIds"></param>
    public AutomaticCodeInterpreterContainerConfiguration(IEnumerable<string> fileIds = null)
    {
        if (fileIds != null)
        {
            FileIds = [.. fileIds];
        }
        else
        {
            FileIds = new List<string>();
        }
    }
}