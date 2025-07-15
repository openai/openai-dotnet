using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobEvent")]
public partial class FineTuningEvent
{
    [CodeGenMember("FineTuningJobEventLevel")]
    public string Level;

    [CodeGenMember("Object")]
    private string _object;
}