using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace OpenAI.Assistants;

public partial class AssistantRunOperation : ResultValueOperation
{
    private class AssistantRunOperationToken
    {
        public string ThreadId { get; }
        public string RunId { get; }

        public AssistantRunOperationToken(string threadId, string runId)
        {
            ThreadId = threadId;
            RunId = runId;
        }

        public static BinaryData ToBytes(string threadId, string runId)
        {
            using MemoryStream stream = new();
            using Utf8JsonWriter writer = new(stream);
            writer.WriteString("threadId", threadId);
            writer.WriteString("runId", runId);
            writer.Flush();
            stream.Position = 0;
            return BinaryData.FromStream(stream);
        }

        public static AssistantRunOperationToken FromBytes(BinaryData data)
        {
            Utf8JsonReader reader = new(data);

            Debug.Assert(reader.Read());
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString() == "threadId");
            Debug.Assert(reader.Read());
            Debug.Assert(reader.TokenType == JsonTokenType.String);
            string threadId = reader.GetString();

            Debug.Assert(reader.Read());
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            Debug.Assert(reader.GetString() == "threadId");
            Debug.Assert(reader.Read());
            Debug.Assert(reader.TokenType == JsonTokenType.String);
            string runId = reader.GetString();

            return new(threadId, runId);
        }
    }
}
