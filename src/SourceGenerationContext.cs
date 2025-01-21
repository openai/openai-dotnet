using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<ReadOnlyMemory<int>>))]
[JsonSerializable(typeof(ReadOnlyMemory<float>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;