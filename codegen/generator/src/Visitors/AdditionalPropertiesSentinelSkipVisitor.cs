using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Skips sentinel-valued entries when serializing additional properties.
/// </summary>
public class AdditionalPropertiesSentinelSkipVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Name != SerializationVisitorHelpers.JsonModelWriteCoreMethodName
            || method.BodyStatements is not MethodBodyStatements statements)
        {
            return method;
        }

        List<MethodBodyStatement> flattenedStatements = SerializationVisitorHelpers.FlattenTopLevelStatements(statements);

        foreach (MethodBodyStatement statement in flattenedStatements)
        {
            if (statement is IfStatement ifStatement
                && SerializationVisitorHelpers.GetWritePropertyNameTargetFromStatement(ifStatement) is null
                && ifStatement.Body.FirstOrDefault() is ForEachStatement foreachStatement)
            {
                foreachStatement.Body.Insert(
                    0,
                    new IfStatement(
                        Static(new ModelSerializationExtensionsDefinition().Type).Invoke(
                            SerializationVisitorHelpers.IsSentinelValueMethodName,
                            foreachStatement.ItemVariable.Property("Value")))
                    {
                        Continue,
                    });
            }
        }

        method.Update(bodyStatements: flattenedStatements);
        return method;
    }
}
