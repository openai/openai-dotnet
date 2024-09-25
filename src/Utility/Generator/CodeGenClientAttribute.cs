#nullable enable

using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class CodeGenClientAttribute(string originalName) : CodeGenTypeAttribute(originalName)
{
    public Type? ParentClient { get; set; }
}