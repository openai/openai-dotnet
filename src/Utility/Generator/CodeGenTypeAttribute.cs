#nullable enable

using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class)]
internal class CodeGenTypeAttribute(string? originalName) : Attribute
{
    public string? OriginalName { get; } = originalName;
}