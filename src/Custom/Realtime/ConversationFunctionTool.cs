using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Realtime;

[CodeGenType("RealtimeFunctionTool")]
public partial class ConversationFunctionTool : ConversationTool
{
    [CodeGenMember("Name")]
    private string _name;
    public string Name
    {
        get => _name;
        set => _name = value;
    }

    [CodeGenMember("Description")]
    private string _description;

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    [CodeGenMember("Parameters")]
    private BinaryData _parameters;

    public BinaryData Parameters
    {
        get => _parameters;
        set => _parameters = value;
    }
}
