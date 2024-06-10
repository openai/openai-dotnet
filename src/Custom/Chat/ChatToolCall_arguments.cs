using System;
using System.Text;
using System.Text.Json;

namespace OpenAI.Chat;

/// <summary>
/// A base representation of an item in an <c>assistant</c> role response's <c>tool_calls</c> that specifies
/// parameterized resolution against a previously defined tool that is needed for the model to continue the logical
/// conversation.
/// </summary>
public partial class ChatToolCall
{
    public T GetFunctionArgument<T>(string parameterName, T defaultValue)
    {
        if (TryReadArgumentValue<T>(parameterName, out var value))
        {
            return value;
        }
        return defaultValue;
    }

    public T GetFunctionArgument<T>(string parameterName)
    {
        if (TryReadArgumentValue<T>(parameterName, out var value))
        {
            return value;
        }
        throw new ArgumentOutOfRangeException(nameof(parameterName));
    }

    private bool TryReadArgumentValue<T>(string propertyName, out T value)
    {
        // TODO: FunctionArguments should return BinaryData. Once it does, eliminate the transcoding call. 
        Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(FunctionArguments));

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(propertyName))
            {
                if (!reader.Read()) throw new InvalidOperationException("Malformed JSON");
                return TryReadValue<T>(ref reader, out value);
            }
        }
        value = default;
        return false;
    }
    private static bool TryReadValue<T>(ref Utf8JsonReader reader, out T value)
    {
        object? readValue = null;
        if (typeof(T).IsEnum)
        {
            var enumValueName = reader.GetString();
            readValue = Enum.Parse(typeof(T), enumValueName, ignoreCase: true);
        }
        else if (typeof(T) == typeof(string))
        {
            readValue = (object)reader.GetString();
        }
        else if (typeof(T) == typeof(bool))
        {
            readValue = (object)reader.GetBoolean();
        }
        else if (typeof(T) == typeof(double))
        {
            readValue = (object)reader.GetDouble();
        }
        else if (typeof(T) == typeof(float))
        {
            readValue = (object)reader.GetSingle();
        }
        else if (typeof(T) == typeof(int))
        {
            readValue = (object)reader.GetInt32();
        }
        else if (typeof(T) == typeof(long))
        {
            readValue = (T)(object)reader.GetInt64();
        }
        else if (typeof(T) == typeof(short))
        {
            readValue = (object)reader.GetInt16();
        }
        else if (typeof(T) == typeof(sbyte))
        {
            readValue = (object)reader.GetSByte();
        }
        else if (typeof(T) == typeof(ulong))
        {
            readValue = (object)reader.GetUInt64();
        }
        else if (typeof(T) == typeof(uint))
        {
            readValue = (object)reader.GetUInt32();
        }
        else if (typeof(T) == typeof(ushort))
        {
            readValue = (object)reader.GetUInt16();
        }
        else if (typeof(T) == typeof(byte))
        {
            readValue = (object)reader.GetByte();
        }
        else
        {
            value = default(T);
            return false;
        }

        value = (T)readValue;
        return true;
    }
}