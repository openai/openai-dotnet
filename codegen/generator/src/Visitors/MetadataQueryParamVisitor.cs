using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using static OpenAILibraryPlugin.Visitors.VisitorHelpers;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor fixes up usage of the metadata query parameter into the proper format.
/// </summary>
public class MetadataQueryParamVisitor : ScmLibraryVisitor
{

    private static readonly string[] _chatParamsToReplace = ["after", "before", "limit", "order", "model", "metadata"];

    /// <summary>
    /// Visits Create*Request methods to modify how metadata query parameters are handled.
    /// It replaces the following statements:
    /// <code>
    /// List<object> list = new List<object>();
	/// foreach (var @param in metadata)
	/// {
    ///     uri.AppendQuery($"metadata[{@param.Key}]", @param.Value, true);
    ///     list.Add(@param.Key);
    ///     list.Add(@param.Value);
    /// }
    /// uri.AppendQueryDelimited("metadata", list, ",", null, true);
    /// </code>
    /// with:
    /// <code>
    /// foreach (var @param in metadata)
    /// {
    ///    uri.AppendQuery($"metadata[{@param.Key}]", @param.Value, true);
    /// }
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        // Check if the method is one of the Create*Request methods and has a signature that takes a metadata parameter like IDictionary<string, string> metadata
        if (method.Signature.Name.StartsWith("Create") && method.Signature.Name.EndsWith("Request") &&
            method.Signature.Parameters.Any(p => p.Type.IsDictionary && p.Name == "metadata"))
        {
            ValueExpression? uri = null;
            var statements = method.BodyStatements?.ToList() ?? new List<MethodBodyStatement>();
            VisitExplodedMethodBodyStatements(
                statements!,
                statement =>
                {
                    // Check if the statement is an assignment to a variable named "uri"
                    // Capture it if so
                    if (statement is ExpressionStatement expressionStatement &&
                       expressionStatement.Expression is AssignmentExpression assignmentExpression &&
                       assignmentExpression.Variable is DeclarationExpression declarationExpression &&
                       declarationExpression.Variable is VariableExpression variableExpression &&
                       variableExpression.Declaration.RequestedName == "uri")
                    {
                        uri = variableExpression;
                    }
                    // Try to remove the unnecessary list declaration
                    if (statement is ExpressionStatement expressionStatement2 &&
                       expressionStatement2.Expression is AssignmentExpression assignmentExpression2 &&
                       assignmentExpression2.Variable is DeclarationExpression declarationExpression2 &&
                       declarationExpression2.Variable is VariableExpression variableExpression2 &&
                       variableExpression2.Declaration.RequestedName == "list" &&
                       variableExpression2.Type.IsCollection && variableExpression2.Type.IsGenericType)
                    {
                        // Remove the list declaration
                        return new SingleLineCommentStatement("Plugin customization: remove unnecessary list declaration");
                    }

                    if (uri is not null &&
                        statement is ForEachStatement foreachStatement &&
                        foreachStatement.Enumerable is DictionaryExpression dictionaryExpression &&
                        dictionaryExpression.Original is VariableExpression variable &&
                        variable.Declaration.RequestedName == "metadata")
                    {
                        var formatString = new FormattableStringExpression("metadata[{0}]", [foreachStatement.ItemVariable.Property("Key")]);
                        var appendQueryStatement = uri.Invoke("AppendQuery", [formatString, foreachStatement.ItemVariable.Property("Value"), Snippet.True]);
                        foreachStatement.Body.Clear();
                        foreachStatement.Body.Add(new SingleLineCommentStatement("Plugin customization: Properly handle metadata query parameters"));
                        foreachStatement.Body.Add(appendQueryStatement.Terminate());
                    }

                    // Remove the call to AppendQueryDelimited for metadata
                    if (statement is ExpressionStatement expressionStatement3 &&
                        expressionStatement3.Expression is InvokeMethodExpression invokeMethodExpression &&
                        invokeMethodExpression.MethodName == "AppendQueryDelimited" &&
                        invokeMethodExpression.Arguments.Count == 4 &&
                        invokeMethodExpression.Arguments[0].ToDisplayString() == "\"metadata\"")
                    {
                        return new SingleLineCommentStatement("Plugin customization: remove unnecessary AppendQueryDelimited for metadata");
                    }
                    return statement;
                });

            // Rebuild the method body with the modified statements
            method.Update(bodyStatements: statements);
        }

        return base.VisitMethod(method);
    }
}