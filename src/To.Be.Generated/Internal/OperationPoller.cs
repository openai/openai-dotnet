//using System.ClientModel;

//#nullable enable

//namespace OpenAI;

//// Convenience version
//// Note: Right now, this just inherits from the protocol poller and adds a T.
//// Do we need more than that?
//internal abstract class OperationPoller<T> : OperationResultPoller
//{
//    private T? _value;

//    protected OperationPoller(ClientResult current) : base(current)
//    {
//    }

//    public T Value => _value ??= GetValueFromResult(Current);

//    public abstract T GetValueFromResult(ClientResult result);

//    protected override void Update() => _value = GetValueFromResult(Current);
//}
