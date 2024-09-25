using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = true)]
internal sealed class CodeGenSuppressAttribute(string member, params Type[] parameters) : Attribute
{
    public string Member { get; } = member;
    public Type[] Parameters { get; } = parameters;
}