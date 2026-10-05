using System;
using System.ClientModel.Primitives;

namespace OpenAI;

internal abstract class SseLifecycle<T>
{
    public abstract IDisposable Enter();
    public virtual void OnResponse(PipelineResponse response) { }
    public virtual void OnTypedResponse() { }
    public abstract void OnEvent();
    public abstract void OnUpdate(T update);
    public abstract void OnException(Exception exception);
    public abstract void Complete(SseCompletionKind completionKind);
}

internal enum SseCompletionKind
{
    EndOfStream,
    Disposed,
    RawResponse,
}
