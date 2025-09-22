using System;
using System.Linq;

namespace OpenAI.Responses;

/// <summary>
/// Represents a container for the code interpreter tool.
/// </summary>
public class CodeInterpreterContainer
{
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
        ContainerConfiguration = containerConfiguration;
    }

    /// <summary>
    /// Gets the ID of the container.
    /// </summary>
    public string ContainerId { get; }

    /// <summary>
    /// Gets the configuration of the container.
    /// </summary>
    public CodeInterpreterContainerConfiguration ContainerConfiguration { get; }

    internal BinaryData AsBinaryData()
    {
        return this.ContainerId != null
            ? new BinaryData($"\"{ContainerId}\"")
            : ContainerConfiguration is CodeInterpreterToolAuto autoConfig && autoConfig.FileIds?.Any() == true ?
            new BinaryData($"{{\"type\": \"auto\", \"file_ids\": [{string.Join(", ", autoConfig.FileIds.Select(id => $"\"{id}\""))}]}}") :
            new BinaryData("{\"type\": \"auto\"}");
    }
}