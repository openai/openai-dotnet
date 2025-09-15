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
[CodeGenType("ContainerCollectionOptions")] public partial class ContainerCollectionOptions { }
[CodeGenType("ContainerCollectionOrder")] public readonly partial struct ContainerCollectionOrder { }
[CodeGenType("ContainerFileCollectionOptions")] public partial class ContainerFileCollectionOptions { }
[CodeGenType("ContainerFileResource")] public partial class ContainerFileResource { }
[CodeGenType("CreateContainerBodyExpiresAfter")] public partial class CreateContainerBodyExpiresAfter { }
[CodeGenType("CreateContainerFileBody")] public partial class CreateContainerFileBody { }
[CodeGenType("DeleteContainerFileResponse")] public partial class DeleteContainerFileResponse { }
[CodeGenType("DeleteContainerResponse")] public partial class DeleteContainerResponse { }
