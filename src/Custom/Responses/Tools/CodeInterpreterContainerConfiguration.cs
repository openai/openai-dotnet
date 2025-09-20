using System.Collections.Generic;

namespace OpenAI.Responses;

/// <summary>
/// Represents the configuration for a Code Interpreter container.
/// </summary>
public class CodeInterpreterContainerConfiguration
{

    /// <summary>
    /// Creates a new instance of the <see cref="AutomaticCodeInterpreterContainerConfiguration"/> class.
    /// </summary>
    /// <param name="fileIds">The list of file IDs associated with the container.</param>
    /// <returns>A new instance of the <see cref="AutomaticCodeInterpreterContainerConfiguration"/> class.</returns>
    public static AutomaticCodeInterpreterContainerConfiguration CreateAutomaticConfiguration(IEnumerable<string> fileIds = null) =>
        new AutomaticCodeInterpreterContainerConfiguration(fileIds);
}