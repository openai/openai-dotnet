using System;

namespace OpenAI;

public partial class OpenApiTool
{
    public string Name
    {
        get => Openapi?.Name;
        set => Openapi.Name = value;
    }

    public string Description
    {
        get => Openapi?.Description;
        set => Openapi.Description = value;
    }

    public BinaryData Specification
    {
        get => Openapi?.Spec;
        set => Openapi.Spec = value;
    }

    public BinaryData Authentication
    {
        get => Openapi?.Auth;
        set => Openapi.Auth = value;
    }
}
