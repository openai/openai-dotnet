using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;
using static OpenAILibraryPlugin.Visitors.VisitorHelpers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor that adds the OpenAIContext.Default parameter to ModelReaderWriter.Write calls
/// that are missing it (2-arg calls), ensuring AOT compatibility on .NET 10+.
/// The upstream generator emits ModelReaderWriter.Write(collection, options) without the
/// required ModelReaderWriterContext parameter in TryResolve*Array helper methods.
/// </summary>
public class ModelReaderWriterContextVisitor : ScmLibraryVisitor
{
    private static readonly ValueExpression OpenAIContextDefault =
        new MemberExpression(new MemberExpression(null, "OpenAIContext"), "Default");

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        if (method.BodyStatements is not MethodBodyStatements methodBodyStatements)
        {
            return method;
        }

        List<MethodBodyStatement> statements = methodBodyStatements.ToList();
        bool modified = false;

        VisitExplodedMethodBodyStatements(
            statements!,
            statement =>
            {
                if (statement is ExpressionStatement expressionStatement
                    && expressionStatement.Expression is AssignmentExpression assignmentExpression
                    && IsModelReaderWriterWriteWithoutContext(assignmentExpression.Value, out var invokeExpr))
                {
                    var newArgs = new List<ValueExpression>(invokeExpr!.Arguments)
                    {
                        OpenAIContextDefault
                    };

                    var newInvoke = invokeExpr.InstanceReference!.Invoke(
                        invokeExpr.MethodName!,
                        [.. newArgs]);

                    modified = true;
                    return new ExpressionStatement(new AssignmentExpression(assignmentExpression.Variable, newInvoke));
                }

                return statement;
            });

        if (modified)
        {
            method.Update(bodyStatements: statements);
        }

        return method;
    }

    private static bool IsModelReaderWriterWriteWithoutContext(ValueExpression? expression, out InvokeMethodExpression? invokeExpr)
    {
        invokeExpr = null;

        // Unwrap ScopedApi<T> if present
        if (expression is ScopedApi<System.BinaryData> scopedApi)
        {
            expression = scopedApi.Original;
        }

        if (expression is InvokeMethodExpression invoke
            && invoke.MethodName == "Write"
            && invoke.Arguments.Count == 2
            && invoke.InstanceReference is TypeReferenceExpression typeRef
            && typeRef.Type?.Name == "ModelReaderWriter")
        {
            invokeExpr = invoke;
            return true;
        }

        return false;
    }
}
