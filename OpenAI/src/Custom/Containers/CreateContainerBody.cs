using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("CreateContainerBody")]
public partial class CreateContainerBody
{ 
    public static implicit operator BinaryContent(CreateContainerBody createContainerBody)
    {
        if (createContainerBody == null)
        {
            return null;
        }
        return BinaryContent.Create(createContainerBody, ModelSerializationExtensions.WireOptions);
    }
}
