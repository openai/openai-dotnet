using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(IEnumerable<string>))]
[JsonSerializable(typeof(IEnumerable<IEnumerable<int>>))]
[JsonSerializable(typeof(ReadOnlyMemory<float>))]
internal sealed partial class SourceGenerationContext : JsonSerializerContext;