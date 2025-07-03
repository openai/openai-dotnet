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
    public static explicit operator InternalContainerListResource(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalContainerListResource(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("ContainerResource")]
internal partial class InternalContainerResource
{
    public static explicit operator InternalContainerResource(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalContainerResource(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
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
    public static explicit operator InternalDeleteContainerResponse(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalDeleteContainerResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("CreateContainerFileBody")] internal partial class InternalCreateContainerFileBody {}
[CodeGenType("ContainerFileResource")]
internal partial class InternalContainerFileResource
{
    public static explicit operator InternalContainerFileResource(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalContainerFileResource(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("ContainerFileListResource")]
internal partial class InternalContainerFileListResource
{
    public static explicit operator InternalContainerFileListResource(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalContainerFileListResource(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}
[CodeGenType("DeleteContainerFileResponse")]
internal partial class InternalDeleteContainerFileResponse
{
    public static explicit operator InternalDeleteContainerFileResponse(ClientResult result)
    {
        using PipelineResponse response = result.GetRawResponse();
        using JsonDocument document = JsonDocument.Parse(response.Content);
        return DeserializeInternalDeleteContainerFileResponse(document.RootElement, ModelSerializationExtensions.WireOptions);
    }
}