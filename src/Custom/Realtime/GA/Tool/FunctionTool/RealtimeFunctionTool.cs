using Microsoft.TypeSpec.Generator.Customizations;
using System;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeFunctionToolGA")]
public partial class GARealtimeFunctionTool
{
    // CUSTOM: Renamed.
    [CodeGenMember("Name")]
    public string FunctionName { get; }

    // CUSTOM: Renamed.
    [CodeGenMember("Description")]
    public string FunctionDescription { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Parameters")]
    public BinaryData FunctionParameters { get; set; }
}