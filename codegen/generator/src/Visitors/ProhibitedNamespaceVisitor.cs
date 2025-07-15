using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.EmitterRpc;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System.Collections.Generic;
using System.Text;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor that throws an exception if a generated namespace is prohibited. This happens when a newly introduced
/// type isn't emplaced into its appropriate namespace. Custom code definitions, including stubs that only emplace in
/// namespaces, are exempt and will cause the generated code to be accepted.
/// </summary>
public class ProhibitedNamespaceVisitor : ScmLibraryVisitor
{
    protected override TypeProvider VisitType(TypeProvider type)
    {
        HashSet<TypeProvider> violatingTypes = [];

        bool isPublicType = type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public);
        bool isUnknownPrefixedType = type.Type.Name.StartsWith("Unknown");

        if ((isPublicType || isUnknownPrefixedType)
            && (type.Type.Namespace == "OpenAI" || type.Type.Namespace == "OpenAI.Models")
            && string.IsNullOrEmpty(type.CustomCodeView?.Type.Namespace)
            && type.Type.Name != "OpenAIContext"
            && !violatingTypes.Contains(type))
        {
            violatingTypes.Add(type);
            StringBuilder builder = new();
            builder.Append($"[CodeGenType(\"{type.Name}\")] internal ");
            builder.Append(type.Type.IsValueType
                ? "readonly partial struct "
                : "partial class ");
            builder.AppendLine($"Internal{type.Name} {{}}");
            OpenAILibraryGenerator.Instance.Emitter.ReportDiagnostic("prohibited-namespace", builder.ToString(), severity: EmitterDiagnosticSeverity.Error);
        }
        return type;
    }
}