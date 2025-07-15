using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

internal static class VisitorHelpers
{
    internal static void VisitExplodedMethodBodyStatements(
        IList<MethodBodyStatement?> statements,
        Func<MethodBodyStatement?, MethodBodyStatement?> visitorFunc)
    {
        for (int i = 0; i < statements.Count; i++)
        {
            statements[i] = visitorFunc.Invoke(statements[i]);
            
            if (statements[i] is ForEachStatement foreachStatement)
            {
                List<MethodBodyStatement> foreachBodyStatements
                    = foreachStatement.Body
                        .SelectMany(bodyStatement => bodyStatement)
                        .ToList();
                VisitExplodedMethodBodyStatements(foreachBodyStatements!, visitorFunc);
                foreachStatement.Body.Clear();
                foreachStatement.Body.Add(new MethodBodyStatements(foreachBodyStatements));
            }
            else if (statements[i] is IfStatement ifStatement)
            {
                // To do: traverse inside of "if"
            }
            else if (statements[i] is ForStatement forStatement)
            {
                // To do: traverse inside of "for"
            }
        }
    }

    internal static MethodBodyStatement? GetUpdatedIfStatement(
        MethodBodyStatement? statement,
        Func<ValueExpression, ValueExpression?> conditionUpdateFunc,
        string comment)
    {
        if (statement is not IfStatement ifStatement)
        {
            return statement;
        }

        List<MethodBodyStatement> statements =
        [
            new SingleLineCommentStatement(comment)
        ];

        ValueExpression? newCondition = GetRecursivelyUpdatedCondition(ifStatement.Condition, conditionUpdateFunc);
        if (newCondition == ifStatement.Condition)
        {
            return ifStatement;
        }
        else if (newCondition is null)
        {
            statements.Add(ifStatement.Body);
        }
        else
        {
            var updatedIfStatement = new IfStatement(newCondition, ifStatement.Inline, ifStatement.AddBraces)
            {
                ifStatement.Body,
            };

            statements.Add(updatedIfStatement);
        }

        return new MethodBodyStatements(statements);
    }

    private static ValueExpression? GetRecursivelyUpdatedCondition(
        ValueExpression expression,
        Func<ValueExpression, ValueExpression?> visitorFunc)
    {
        ValueExpression? newExpression = visitorFunc.Invoke(expression);
        BinaryOperatorExpression? binaryOperatorExpression
            = expression as BinaryOperatorExpression ?? (expression as ScopedApi<bool>)?.Original as BinaryOperatorExpression;

        if (newExpression == expression && binaryOperatorExpression is not null)
        {
            ValueExpression? newLeft = GetRecursivelyUpdatedCondition(binaryOperatorExpression.Left, visitorFunc);
            ValueExpression? newRight = GetRecursivelyUpdatedCondition(binaryOperatorExpression.Right, visitorFunc);
            if (newLeft is null)
            {
                return newRight;
            }
            else if (newRight is null)
            {
                return newLeft;
            }
            else if (newLeft != binaryOperatorExpression.Left || newRight != binaryOperatorExpression.Right)
            {
                return new BinaryOperatorExpression(binaryOperatorExpression.Operator, newLeft, newRight);
            }
        }
        return newExpression;
    }
}