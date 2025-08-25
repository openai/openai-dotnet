using System.ClientModel;

namespace OpenAI.Containers;

[CodeGenType("ContainerResource")] public partial class ContainerResource { }
[CodeGenType("ContainerResourceExpiresAfter")] public partial class ContainerResourceExpiresAfter { }
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
[CodeGenType("CreateContainerBodyExpiresAfter")] public partial class CreateContainerBodyExpiresAfter { }
[CodeGenType("DeleteContainerResponse")] public partial class DeleteContainerResponse { }
[CodeGenType("CreateContainerFileBody")] public partial class CreateContainerFileBody { }
[CodeGenType("ContainerFileResource")] public partial class ContainerFileResource { }
[CodeGenType("DeleteContainerFileResponse")] public partial class DeleteContainerFileResponse { }
