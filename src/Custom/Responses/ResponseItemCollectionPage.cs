using System.ComponentModel;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("ResponseItemList")]
public partial class ResponseItemCollectionPage
{
    // CUSTOM: Applied EditorBrowsableState.Never.
    [CodeGenMember("Object")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string Object { get; set; } = "list";
}
