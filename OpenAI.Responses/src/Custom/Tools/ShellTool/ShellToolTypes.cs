using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses;

// CUSTOM: Anchor public Shell tool types in the OpenAI.Responses namespace so the code
// generator emits them under that namespace and routes the generated files into the
// OpenAI.Responses project.
[CodeGenType("ShellTool")] public partial class ShellTool { }
[CodeGenType("ShellAction")] public partial class ShellAction { }
[CodeGenType("ShellCallStatus")] public readonly partial struct ShellCallStatus { }

[CodeGenType("ShellToolEnvironment")] public partial class ShellToolEnvironment { }
[CodeGenType("ShellToolContainerAutoEnvironment")] public partial class ShellToolContainerAutoEnvironment { }
[CodeGenType("ShellToolContainerReferenceEnvironment")] public partial class ShellToolContainerReferenceEnvironment { }
[CodeGenType("ShellToolLocalEnvironment")] public partial class ShellToolLocalEnvironment { }

[CodeGenType("ShellToolContainerMemoryLimit")] public readonly partial struct ShellToolContainerMemoryLimit { }

[CodeGenType("ShellToolContainerNetworkPolicy")] public partial class ShellToolContainerNetworkPolicy { }
[CodeGenType("ShellToolDisabledContainerNetworkPolicy")] public partial class ShellToolDisabledContainerNetworkPolicy { }
[CodeGenType("ShellToolAllowlistContainerNetworkPolicy")] public partial class ShellToolAllowlistContainerNetworkPolicy { }
[CodeGenType("ShellToolDomainSecret")] public partial class ShellToolDomainSecret { }

[CodeGenType("ShellToolSkill")] public partial class ShellToolSkill { }
[CodeGenType("ShellToolSkillReference")] public partial class ShellToolSkillReference { }
[CodeGenType("ShellToolInlineSkill")] public partial class ShellToolInlineSkill { }
[CodeGenType("ShellToolInlineSkillSource")] public partial class ShellToolInlineSkillSource { }
[CodeGenType("ShellToolLocalSkill")] public partial class ShellToolLocalSkill { }

[CodeGenType("ShellCallOutputContent")] public partial class ShellCallOutputContent { }
[CodeGenType("ShellCallOutcome")] public partial class ShellCallOutcome { }
[CodeGenType("ShellCallExitOutcome")] public partial class ShellCallExitOutcome { }
[CodeGenType("ShellCallTimeoutOutcome")] public partial class ShellCallTimeoutOutcome { }

