using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenVisibility(nameof(ContainerNetworkPolicy), CodeGenVisibility.ProtectedInternal, typeof(ContainerNetworkPolicyKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
[CodeGenType("ContainerNetworkPolicy")]
public partial class ContainerNetworkPolicy
{
}
