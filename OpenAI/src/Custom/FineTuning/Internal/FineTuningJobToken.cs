using System;
using System.ClientModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
internal class FineTuningJobToken : ContinuationToken
{
    public FineTuningJobToken(string jobId)
    {
        JobId = jobId;
    }

    public string JobId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartObject();

        writer.WriteString("jobId", JobId);

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }

    public static FineTuningJobToken FromToken(ContinuationToken continuationToken)
    {
        if (continuationToken is FineTuningJobToken token)
        {
            return token;
        }

        BinaryData data = continuationToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            throw new ArgumentException("Failed to create " + nameof(FineTuningJobToken) + " from provided continuationToken.", nameof(continuationToken));
        }

        Utf8JsonReader reader = new(data);

        string jobId = null!;

        reader.Read();

        Debug.Assert(reader.TokenType == JsonTokenType.StartObject);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);

            string propertyName = reader.GetString()!;

            switch (propertyName)
            {
                case "jobId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    jobId = reader.GetString()!;
                    break;

                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (jobId is null)
        {
            throw new ArgumentException("Failed to create " + nameof(FineTuningJobToken) + " from provided continuationToken.", nameof(continuationToken));
        }

        return new(jobId);
    }
}

