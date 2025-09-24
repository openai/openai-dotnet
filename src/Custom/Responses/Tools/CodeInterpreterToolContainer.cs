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
        Container = containerConfiguration;
    }

    // CUSTOM: Removed setter.
    public string ContainerId { get; }

    // CUSTOM: Removed setter.
    public CodeInterpreterToolContainerConfiguration Container { get; }
}