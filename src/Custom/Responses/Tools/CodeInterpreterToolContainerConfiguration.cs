using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAI.Responses;

/// <summary>
/// Represents the configuration for a Code Interpreter container.
/// </summary>
[CodeGenType("CodeInterpreterContainerConfiguration")]
public partial class CodeInterpreterToolContainerConfiguration
{
    /// <summary>
    /// Creates a new instance of the <see cref="AutomaticCodeInterpreterToolContainerConfiguration"/> class.
    /// </summary>
    /// <param name="fileIds">The list of file IDs associated with the container.</param>
    /// <returns>A new instance of the <see cref="AutomaticCodeInterpreterToolContainerConfiguration"/> class.</returns>
    public static AutomaticCodeInterpreterToolContainerConfiguration CreateAutomaticContainerConfiguration(IEnumerable<string> fileIds = null)
    {
        return new AutomaticCodeInterpreterToolContainerConfiguration(
            kind: InternalCodeInterpreterContainerConfigurationType.Auto,
            additionalBinaryDataProperties: null,
            fileIds: fileIds?.ToList());
    }
}