namespace OpenAI.Responses;

// CUSTOM: This type represents a non-discriminated union of the following components:
// * A container ID defined as a string.
// * A container configuration defined as an object.
/// <summary>
/// Represents a container for the code interpreter tool.
/// </summary>
[CodeGenType("DotNetCodeInterpreterToolContainer")]
public partial class CodeInterpreterToolContainer
{
    // CUSTOM: Made internal.
    internal CodeInterpreterToolContainer()
    {
    }

    // CUSTOM: Added to support the corresponding component of the union.
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreterContainer"/> class.
    /// </summary>
    /// <param name="containerId">The ID of the container.</param>
    public CodeInterpreterToolContainer(string containerId)
    {
        ContainerId = containerId;
    }

    // CUSTOM: Added to support the corresponding component of the union.
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreterContainer"/> class.
    /// </summary>
    /// <param name="containerConfiguration">The configuration of the container.</param>
    public CodeInterpreterToolContainer(CodeInterpreterToolContainerConfiguration containerConfiguration)
    {
        ContainerConfiguration = containerConfiguration;
    }

    // CUSTOM: Removed setter.
    [CodeGenMember("ContainerId")]
    public string ContainerId { get; }

    // CUSTOM:
    // - Renamed.
    // - Removed setter.
    [CodeGenMember("Container")]
    public CodeInterpreterToolContainerConfiguration ContainerConfiguration { get; }
}