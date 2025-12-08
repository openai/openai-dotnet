using System.ComponentModel;

namespace OpenAI.Responses
{
    [CodeGenType("ResponseItemList")]
    public partial class ResponseItemCollectionPage
    {
        [CodeGenMember("Object")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Object { get; } = "list";
    }
}
