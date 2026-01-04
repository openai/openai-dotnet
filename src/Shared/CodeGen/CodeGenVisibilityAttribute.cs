
using System;

namespace OpenAI;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public partial class CodeGenVisibilityAttribute : Attribute
{
    public string MemberName { get; }
    public CodeGenVisibility Visibility { get; }
    public Type[] Parameters { get; }

    public CodeGenVisibilityAttribute(string memberName, CodeGenVisibility visibility, params Type[] parameters)
    {
        MemberName = memberName;
        Visibility = visibility;
        Parameters = parameters;
    }
}

public enum CodeGenVisibility
{
    Internal,
    Public,
}