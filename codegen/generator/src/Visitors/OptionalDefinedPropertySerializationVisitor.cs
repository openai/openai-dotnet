using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Applies per-type Optional.IsDefined/IsCollectionDefined serialization guards to selected properties.
/// </summary>
public class OptionalDefinedPropertySerializationVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Name != SerializationVisitorHelpers.JsonModelWriteCoreMethodName
            || method.BodyStatements is not MethodBodyStatements statements)
        {
            return method;
        }

        IReadOnlyList<WritePropertyNameConditionInfo> conditions = OptionalDefinedSerializationConfiguration.GetConditions(method.EnclosingType.Name);
        if (conditions.Count == 0)
        {
            return method;
        }

        List<MethodBodyStatement> flattenedStatements = SerializationVisitorHelpers.FlattenTopLevelStatements(statements);
        List<MethodBodyStatement> updatedStatements = [];

        foreach (MethodBodyStatement statement in flattenedStatements)
        {
            string? writePropertyNameTarget = SerializationVisitorHelpers.GetWritePropertyNameTargetFromStatement(statement);

            if (statement is IfStatement ifStatement
                && writePropertyNameTarget is not null
                && conditions.FirstOrDefault(condition => condition.JsonName == writePropertyNameTarget) is WritePropertyNameConditionInfo matchingCondition
                && (SerializationVisitorHelpers.GetPatchContainsExpression(ifStatement.Condition) != null
                    || SerializationVisitorHelpers.IsContainsKeyCondition(ifStatement.Condition, writePropertyNameTarget)))
            {
                updatedStatements.Add(OptionalDefinedSerializationConfiguration.OptionalDefinedCheckComment);
                ifStatement.Update(condition: OptionalDefinedSerializationConfiguration.GetOptionalDefinedCondition(matchingCondition).And(ifStatement.Condition));
                updatedStatements.Add(ifStatement);
                continue;
            }

            if (statement is IfElseStatement ifElseStatement
                && ifElseStatement.Else is not null
                && writePropertyNameTarget is not null
                && SerializationVisitorHelpers.GetPatchContainsExpression(ifElseStatement.If.Condition) != null
                && conditions.FirstOrDefault(condition => condition.JsonName == writePropertyNameTarget) is WritePropertyNameConditionInfo replacementInfo)
            {
                IfStatement updatedCondition = new(OptionalDefinedSerializationConfiguration.GetOptionalDefinedCondition(replacementInfo))
                {
                    ifElseStatement.Else,
                };

                ifElseStatement.Update(elseStatement: new MethodBodyStatements([OptionalDefinedSerializationConfiguration.OptionalDefinedCheckComment, updatedCondition]));
            }

            updatedStatements.Add(statement);
        }

        method.Update(bodyStatements: updatedStatements);
        return method;
    }
}
