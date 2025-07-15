using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[CodeGenType("FineTuningJobStatus")]
public readonly partial struct FineTuningStatus : IEquatable<string>
{
    public bool InProgress =>
        _value == FineTuningStatus.ValidatingFiles ||
        _value == FineTuningStatus.Queued ||
        _value == FineTuningStatus.Running;

    public bool Equals(string other)
    {
        return string.Equals(_value.ToString(), other);
    }
}
