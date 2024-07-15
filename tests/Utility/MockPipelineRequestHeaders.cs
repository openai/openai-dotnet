using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;

#nullable enable

namespace OpenAI.Tests;

public class MockPipelineRequestHeaders : PipelineRequestHeaders
{
    private readonly Dictionary<string, string> _headers = [];
    public override void Add(string name, string value) => _headers[name] = value;
    public override IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _headers.GetEnumerator();
    public override bool Remove(string name) => _headers.Remove(name);
    public override void Set(string name, string value) => _headers[name] = value;
    public override bool TryGetValue(string name, out string value) => _headers.TryGetValue(name, out value!);
    public override bool TryGetValues(string name, out IEnumerable<string> values)
    {
        throw new NotImplementedException();
    }
}