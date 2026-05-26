using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.LegacyCompletions;

[CodeGenType("CreateCompletionRequest")]
internal partial class InternalCreateCompletionRequest
{
    public static implicit operator BinaryContent(InternalCreateCompletionRequest internalCreateCompletionRequest)
    {
        if (internalCreateCompletionRequest == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateCompletionRequest, ModelSerializationExtensions.WireOptions);
    }
}
