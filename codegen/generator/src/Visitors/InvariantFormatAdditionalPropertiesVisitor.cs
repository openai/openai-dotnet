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
/// A visitor that removes all "options.Format != "W"" condition checks from JsonModelWriteCore and emitted type
/// deserialization methods, which causes unknown properties to always be written to the additional properties
/// collection.
/// </summary>
public class InvariantFormatAdditionalPropertiesVisitor : ScmLibraryVisitor
{
    private const string Comment = "Plugin customization: remove options.Format != \"W\" check";

    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        if (method.Signature.Name == "JsonModelWriteCore"
            || method.Signature.Name.StartsWith("Deserialize"))
        {
            List<MethodBodyStatement> statements = method.BodyStatements?.ToList() ?? [];
            VisitExplodedMethodBodyStatements(
                statements!,
                statement => GetUpdatedIfStatement(
                    statement, expression =>
                    {
                        if (GetIsOptionsFormatNotEqualToWExpression(expression))
                        {
                            return null;
                        }
                        return expression;
                    },
                    Comment));
            method.Update(bodyStatements: statements);
        }
        return method;
    }

    private static bool GetIsOptionsFormatNotEqualToWExpression(
        ValueExpression expression)
    {
        BinaryOperatorExpression? binaryOperatorExpression
            = expression as BinaryOperatorExpression
                ?? (expression as ScopedApi<bool>)?.Original as BinaryOperatorExpression;

        return binaryOperatorExpression?.Left is MemberExpression leftMemberExpression
            && leftMemberExpression.Inner?.ToDisplayString() == "options"
            && leftMemberExpression.MemberName == "Format"
            && binaryOperatorExpression.Operator == "!="
            && binaryOperatorExpression.Right is ScopedApi<string> rightStringExpression
            && rightStringExpression.Original is LiteralExpression rightLiteralExpression
            && rightLiteralExpression.Literal is string rightStringLiteral
            && rightStringLiteral == "W";
    }
}