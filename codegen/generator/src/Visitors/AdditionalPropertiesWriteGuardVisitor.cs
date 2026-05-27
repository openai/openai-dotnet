using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Wraps model serialization property writes with checks against additional-properties overrides.
/// </summary>
public class AdditionalPropertiesWriteGuardVisitor : ScmLibraryVisitor
{
    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Name != SerializationVisitorHelpers.JsonModelWriteCoreMethodName
            || method.BodyStatements is not MethodBodyStatements statements)
        {
            return method;
        }

        List<MethodBodyStatement> flattenedStatements = SerializationVisitorHelpers.FlattenTopLevelStatements(statements);
        List<MethodBodyStatement> updatedStatements = [];

        for (int line = 0; line < flattenedStatements.Count; line++)
        {
            MethodBodyStatement statement = flattenedStatements[line];
            string? writePropertyNameTarget = SerializationVisitorHelpers.GetWritePropertyNameTargetFromStatement(statement);

            if (statement is IfStatement ifStatement)
            {
                if (writePropertyNameTarget is not null
                    && SerializationVisitorHelpers.GetPatchContainsExpression(ifStatement.Condition) is null)
                {
                    ifStatement.Update(condition: ifStatement.Condition.As<bool>().And(SerializationVisitorHelpers.GetContainsKeyCondition(writePropertyNameTarget)));
                }

                updatedStatements.Add(ifStatement);
                continue;
            }

            if (statement is IfElseStatement ifElseStatement
                && SerializationVisitorHelpers.GetPatchContainsExpression(ifElseStatement.If.Condition) != null)
            {
                updatedStatements.Add(statement);
                continue;
            }

            if (writePropertyNameTarget is not null)
            {
                line = WrapPropertyWriteStatement(statement, writePropertyNameTarget, flattenedStatements, line, updatedStatements);
                continue;
            }

            updatedStatements.Add(statement);
        }

        method.Update(bodyStatements: updatedStatements);
        return method;
    }

    private static int WrapPropertyWriteStatement(
        MethodBodyStatement statement,
        string writePropertyNameTarget,
        List<MethodBodyStatement> flattenedStatements,
        int currentLine,
        List<MethodBodyStatement> updatedStatements)
    {
        int line = currentLine;
        IfStatement ifStatement = new(SerializationVisitorHelpers.GetContainsKeyCondition(writePropertyNameTarget))
        {
            statement,
        };

        if (statement is ExpressionStatement)
        {
            ifStatement.Add(flattenedStatements[++line]);
            if (line + 1 < flattenedStatements.Count && flattenedStatements[line + 1] is ForEachStatement)
            {
                ifStatement.Add(flattenedStatements[++line]);
                ifStatement.Add(flattenedStatements[++line]);
            }
        }

        updatedStatements.Add(ifStatement);
        return line;
    }
}
