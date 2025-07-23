using System.ClientModel;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OpenAI.Containers;

[CodeGenType("ContainerListResourceObject")] internal readonly partial struct InternalContainerListResourceObject {}
[CodeGenType("ContainerResourceExpiresAfterAnchor")] internal readonly partial struct InternalContainerResourceExpiresAfterAnchor {}
[CodeGenType("CreateContainerBodyExpiresAfterAnchor")] internal readonly partial struct InternalCreateContainerBodyExpiresAfterAnchor {}
[CodeGenType("DeleteContainerResponseObject")] internal readonly partial struct InternalDeleteContainerResponseObject {}
[CodeGenType("ContainerFileListResourceObject")] internal readonly partial struct InternalContainerFileListResourceObject {}
[CodeGenType("DeleteContainerFileResponseObject")] internal readonly partial struct InternalDeleteContainerFileResponseObject {}
[CodeGenType("ContainerListResource")]
internal partial class InternalContainerListResource
{
}
[CodeGenType("ContainerResource")]
internal partial class InternalContainerResource
{
}
[CodeGenType("ContainerResourceExpiresAfter")] internal partial class InternalContainerResourceExpiresAfter {}
[CodeGenType("CreateContainerBody")] internal partial class InternalCreateContainerBody
{
    public static implicit operator BinaryContent(InternalCreateContainerBody internalCreateContainerBody)
    {
        if (internalCreateContainerBody == null)
        {
            return null;
        }
        return BinaryContent.Create(internalCreateContainerBody, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("CreateContainerBodyExpiresAfter")] internal partial class InternalCreateContainerBodyExpiresAfter {}
[CodeGenType("DeleteContainerResponse")]
internal partial class InternalDeleteContainerResponse
{
}
[CodeGenType("CreateContainerFileBody")] internal partial class InternalCreateContainerFileBody {}
[CodeGenType("ContainerFileResource")]
internal partial class InternalContainerFileResource
{
}
[CodeGenType("ContainerFileListResource")]
internal partial class InternalContainerFileListResource
{
}
[CodeGenType("DeleteContainerFileResponse")]
internal partial class InternalDeleteContainerFileResponse
{
}