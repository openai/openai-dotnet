using System;
using System.Linq;

namespace OpenAI.Responses;

/// <summary>
/// Represents a container for the code interpreter tool.
/// </summary>
[CodeGenType("DotNetCodeInterpreterContainer")]
public partial class CodeInterpreterContainer
{
    internal CodeInterpreterContainer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreterContainer"/> class.
    /// </summary>
    /// <param name="containerId">The ID of the container.</param>
    public CodeInterpreterContainer(string containerId)
    {
        ContainerId = containerId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreterContainer"/> class.
    /// </summary>
    /// <param name="containerConfiguration">The configuration of the container.</param>
    public CodeInterpreterContainer(CodeInterpreterContainerConfiguration containerConfiguration)
    {
        Container = containerConfiguration;
    }

    internal BinaryData AsBinaryData()
    {
        return this.ContainerId != null
            ? new BinaryData($"\"{ContainerId}\"")
            : Container is AutomaticCodeInterpreterContainerConfiguration autoConfig && autoConfig.FileIds?.Any() == true ?
            new BinaryData($"{{\"type\": \"auto\", \"file_ids\": [{string.Join(", ", autoConfig.FileIds.Select(id => $"\"{id}\""))}]}}") :
            new BinaryData("{\"type\": \"auto\"}");
    }
}