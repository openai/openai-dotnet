using System.Collections.Generic;

namespace OpenAI.Responses;

/// <summary>
/// Represents a configuration for an automatic Code Interpreter container.
/// </summary>
public class AutomaticCodeInterpreterContainerConfiguration : CodeInterpreterContainerConfiguration
{

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomaticCodeInterpreterContainerConfiguration"/> class.
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

    /// <summary>
    /// Gets the list of file IDs associated with the container.
    /// </summary>
    public IList<string> FileIds { get; }
}