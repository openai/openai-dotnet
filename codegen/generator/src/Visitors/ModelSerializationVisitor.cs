using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor to add MRW serialization to models.
/// </summary>
public class ModelSerializationVisitor : ScmLibraryVisitor
{
    protected override ModelProvider? PreVisitModel(InputModelType model, ModelProvider? type)
    {
        // Ensure multipart form data models also get JSON usage so that MRW serialization is generated.
        if (model.Usage.HasFlag(InputModelTypeUsage.MultipartFormData) && !model.Usage.HasFlag(InputModelTypeUsage.Json))
        {
            model.Update(usage: model.Usage | InputModelTypeUsage.Json);
        }

        if (type is null || model.Usage.HasFlag(InputModelTypeUsage.Json))
        {
            return base.PreVisitModel(model, type);
        }

        foreach (var provider in type.SerializationProviders)
        {
            if (provider is MrwSerializationTypeDefinition)
            {
                return base.PreVisitModel(model, type);
            }
        }

        var serializations = type.SerializationProviders.ToList();
        serializations.Add(new MrwSerializationTypeDefinition(model, type));
        type.Update(serializations: serializations);

        return base.PreVisitModel(model, type);
    }

    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Name is ("PropagateGet" or "PropagateSet")
            && method.EnclosingType.Name is ("CreateResponseOptions" or "ResponseResult" or "InternalTokenCountsBody")
            && method.BodyStatements is not null)
        {
            foreach (var statement in method.BodyStatements.OfType<IfStatement>())
            {
                var condition = (statement.Condition as ScopedApi<bool>)?.Original ?? statement.Condition;
                if (condition is InvokeMethodExpression { MethodName: "StartsWith", InstanceReference: { } path } startsWith
                    && startsWith.Arguments.Count == 1
                    && startsWith.Arguments[0].ToDisplayString() == "\"tool_choice\"u8")
                {
                    // The containing model must handle whole-property edits, including removal.
                    statement.Update(condition: startsWith.As<bool>().And(Not(path.Invoke("SequenceEqual", startsWith.Arguments[0]))));
                    break;
                }
            }
        }

        return method;
    }
}
