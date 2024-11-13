using System;

namespace OpenAI.Assistants;

internal static partial class RunStepToolCallKindExtensions
{
    public static string ToSerialString(this RunStepToolCallKind value) => value switch
    {
        RunStepToolCallKind.CodeInterpreter => "code_interpreter",
        RunStepToolCallKind.FileSearch => "file_search",
        RunStepToolCallKind.Function => "function",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(RunStepToolCallKind)} value: {value}")
    };

    public static RunStepToolCallKind ToRunStepToolCallKind(this string value)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "code_interpreter")) return RunStepToolCallKind.CodeInterpreter;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "file_search")) return RunStepToolCallKind.FileSearch;
        if (StringComparer.OrdinalIgnoreCase.Equals(value, "function")) return RunStepToolCallKind.Function;
        throw new ArgumentOutOfRangeException(nameof(value), value, $"Unknown {nameof(RunStepToolCallKind)} value: {value}");
    }
}
