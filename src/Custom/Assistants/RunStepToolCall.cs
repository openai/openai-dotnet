using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

[CodeGenType("RunStepDetailsToolCallsObjectToolCallsObject")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class RunStepToolCall
{
    private IReadOnlyList<RunStepCodeInterpreterOutput> _codeInterpreterOutputs;
    private IReadOnlyList<RunStepFileSearchResult> _fileSearchResults;

    // CUSTOM: Made internal.
    internal RunStepToolCall()
    {
    }

    // CUSTOM: Made internal.
    internal RunStepToolCall(string id)
    {
        Argument.AssertNotNull(id, nameof(id));

        Id = id;
    }

    #region Code Interpreter

    // CUSTOM: Spread.
    /// <summary> The input of the code interpreter. </summary>
    public string CodeInterpreterInput => (this as InternalRunStepDetailsToolCallsCodeObject)?.CodeInterpreter.Input;

    // CUSTOM: Spread.
    /// <summary> The outputs of the code interpreter. </summary>
    public IReadOnlyList<RunStepCodeInterpreterOutput> CodeInterpreterOutputs =>
        _codeInterpreterOutputs
            ??= (this as InternalRunStepDetailsToolCallsCodeObject)?.CodeInterpreter?.Outputs
                ?? new ChangeTrackingList<RunStepCodeInterpreterOutput>();

    #endregion

    #region File Search

    // CUSTOM: Spread.
    public FileSearchRankingOptions FileSearchRankingOptions => (this as InternalRunStepDetailsToolCallsFileSearchObject)?.FileSearch?.RankingOptions;

    // CUSTOM: Spread.
    /// <summary> The results of the file search. </summary>
    public IReadOnlyList<RunStepFileSearchResult> FileSearchResults =>
        _fileSearchResults
            ??= (this as InternalRunStepDetailsToolCallsFileSearchObject)?.FileSearch?.Results
                ?? new ChangeTrackingList<RunStepFileSearchResult>();

    #endregion

    #region Function

    // CUSTOM: Spread.
    /// <summary> The name of the function. </summary>
    public string FunctionName => (this as InternalRunStepDetailsToolCallsFunctionObject)?.Function?.Name;

    // CUSTOM: Spread.
    /// <summary> The arguments passed to the function. </summary>
    public string FunctionArguments => (this as InternalRunStepDetailsToolCallsFunctionObject)?.Function?.Arguments;

    // CUSTOM: Spread.
    /// <summary> The output of the function, which will be null if not submitted yet. </summary>
    public string FunctionOutput => (this as InternalRunStepDetailsToolCallsFunctionObject)?.Function?.Output;

    #endregion
}
