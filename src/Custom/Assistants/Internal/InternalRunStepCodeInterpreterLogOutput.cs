namespace OpenAI.Assistants
{
    /// <summary> Text output from the Code Interpreter tool call as part of a run step. </summary>
    [CodeGenType("RunStepDetailsToolCallsCodeOutputLogsObject")]
    internal partial class InternalRunStepCodeInterpreterLogOutput : RunStepCodeInterpreterOutput
    {
        /// <summary> The text output from the Code Interpreter tool call. </summary>
        [CodeGenMember("Logs")]
        public string InternalLogs { get; }
    }
}
