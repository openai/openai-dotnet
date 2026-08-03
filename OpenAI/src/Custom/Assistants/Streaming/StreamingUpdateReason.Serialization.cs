namespace OpenAI.Assistants;

internal static class StreamingUpdateReasonExtensions
{
    internal static string ToSseEventLabel(this StreamingUpdateReason value) => value.ToString();

    internal static StreamingUpdateReason FromSseEventLabel(string label) => new StreamingUpdateReason(label);
}
