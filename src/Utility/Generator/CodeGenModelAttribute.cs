#nullable enable

using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
internal sealed class CodeGenModelAttribute : CodeGenTypeAttribute
{
    /// <summary>
    /// Gets or sets a coma separated list of additional model usage modes. Allowed values: model, error, intput, output.
    /// </summary>
    public string[]? Usage { get; set; }

    /// <summary>
    /// Gets or sets a coma separated list of additional model serialization formats.
    /// </summary>
    public string[]? Formats { get; set; }

    public CodeGenModelAttribute() : base(null)
    {
    }

    public CodeGenModelAttribute(string originalName) : base(originalName)
    {
    }
}