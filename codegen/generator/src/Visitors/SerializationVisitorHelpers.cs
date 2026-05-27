using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

internal static class SerializationVisitorHelpers
{
    internal const string AdditionalPropertiesFieldName = "_additionalBinaryDataProperties";
    internal const string JsonModelWriteCoreMethodName = "JsonModelWriteCore";
    internal const string IsSentinelValueMethodName = "IsSentinelValue";
    internal const string ModelSerializationExtensionsTypeName = "ModelSerializationExtensions";

    internal static List<MethodBodyStatement> FlattenTopLevelStatements(MethodBodyStatements statements)
    {
        List<MethodBodyStatement> flattenedStatements = [];

        foreach (MethodBodyStatement statement in statements)
        {
            if (statement is SuppressionStatement { Inner: not null } suppressionStatement)
            {
                flattenedStatements.Add(suppressionStatement.DisableStatement);
                flattenedStatements.AddRange(suppressionStatement.Inner);
                flattenedStatements.Add(suppressionStatement.RestoreStatement);
            }
            else
            {
                flattenedStatements.Add(statement);
            }
        }

        return flattenedStatements;
    }

    internal static ScopedApi<bool> GetContainsKeyCondition(string propertyName)
    {
        return This.Property(AdditionalPropertiesFieldName)
            .NullConditional()
            .Invoke("ContainsKey", Literal(propertyName))
            .NotEqual(True);
    }

    internal static bool IsContainsKeyCondition(ValueExpression expression, string propertyName)
        => expression.ToDisplayString() == GetContainsKeyCondition(propertyName).ToDisplayString();

    internal static string? GetWritePropertyNameTargetFromStatement(MethodBodyStatement? statement)
    {
        if (statement is ExpressionStatement expressionStatement
            && expressionStatement.Expression is InvokeMethodExpression expressionMethodInvocation
            && expressionMethodInvocation.MethodName == "WritePropertyName"
            && expressionMethodInvocation.Arguments.Count == 1
            && expressionMethodInvocation.Arguments[0] is ScopedApi<string> scopedStringApi
            && scopedStringApi.Original is UnaryOperatorExpression stringUnaryTargetExpression
            && stringUnaryTargetExpression.Operator == "u8"
            && stringUnaryTargetExpression.Operand is LiteralExpression stringLiteralExpression)
        {
            return stringLiteralExpression.Literal?.ToString();
        }

        if (statement is SuppressionStatement { Inner: not null } suppressionStatement)
        {
            return GetWritePropertyNameTargetFromStatements(suppressionStatement.Inner.SelectMany(bodyStatement => bodyStatement));
        }

        if (statement is MethodBodyStatements compoundStatements)
        {
            return GetWritePropertyNameTargetFromStatements(compoundStatements.Statements);
        }
        else if (statement is IfStatement ifStatement)
        {
            return GetWritePropertyNameTargetFromStatement(ifStatement.Body);
        }
        else if (statement is IfElseStatement ifElseStatement)
        {
            return GetWritePropertyNameTargetFromStatement(ifElseStatement.If);
        }

        return null;
    }

    private static string? GetWritePropertyNameTargetFromStatements(IEnumerable<MethodBodyStatement> statements)
    {
        foreach (MethodBodyStatement innerStatement in statements)
        {
            if (GetWritePropertyNameTargetFromStatement(innerStatement) is string innerTarget)
            {
                return innerTarget;
            }
        }

        return null;
    }

    /// <summary>
    /// Recursively checks if the given expression or any of its sub-expressions is a call to Patch.Contains().
    /// Handles various wrapping scenarios including unary operators, binary operators, and nested expressions.
    /// </summary>
    internal static ValueExpression? GetPatchContainsExpression(ValueExpression? expression)
    {
        if (expression is null)
        {
            return null;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return expression switch
        {
            ScopedApi<bool> { Original: InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } } => expression,
            ScopedApi<bool> { Original: UnaryOperatorExpression { Operator: "!", Operand: ScopedApi<bool> { Original: InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } } } } => expression,
            ScopedApi<bool> { Original: BinaryOperatorExpression binaryExpr } =>
                GetPatchContainsExpression(binaryExpr.Left) ?? GetPatchContainsExpression(binaryExpr.Right),
            BinaryOperatorExpression binaryExpr =>
                GetPatchContainsExpression(binaryExpr.Left) ?? GetPatchContainsExpression(binaryExpr.Right),
            UnaryOperatorExpression { Operator: "!" } unaryExpr =>
                GetPatchContainsExpression(unaryExpr.Operand) != null ? expression : null,
            InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } => expression,
            _ => null
        };
#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
