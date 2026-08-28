using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Renamed.
[CodeGenType("DotNetGetResponseOptions")]
[CodeGenSuppress("GetResponseOptions")]
public partial class GetResponseOptions
{
    public GetResponseOptions() : this(null, default, default, null, default, default)
    {
    }
}
