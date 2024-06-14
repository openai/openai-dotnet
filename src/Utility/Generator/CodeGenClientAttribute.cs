#nullable enable

using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class CodeGenClientAttribute : CodeGenTypeAttribute
{
    public Type? ParentClient { get; set; }

    public CodeGenClientAttribute(string originalName) : base(originalName)
    {
    }
}