using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Containers;

// CUSTOM: Renamed.
[CodeGenType("ContainerNetworkPolicy")]
[CodeGenVisibility(nameof(ContainerNetworkPolicy), CodeGenVisibility.ProtectedInternal, typeof(ContainerNetworkPolicyKind))]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class ContainerNetworkPolicy
{
}
