using System;
using System.Diagnostics.CodeAnalysis;
using OpenAI.Internal;

namespace OpenAI
{
    [Experimental("OPENAI001")]
    public enum ResponseFormatType2
    {
        Text,
        JsonObject,
        JsonSchema
    }
}