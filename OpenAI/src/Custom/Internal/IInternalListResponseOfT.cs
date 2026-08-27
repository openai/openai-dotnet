using System.Collections.Generic;

namespace OpenAI;

internal interface IInternalListResponse<T>
{
    IReadOnlyList<T> Data { get; }
    string FirstId { get; }
    string LastId { get; }
    bool HasMore { get; }
}