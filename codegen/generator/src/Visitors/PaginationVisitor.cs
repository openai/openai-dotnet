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
/// This visitor modifies GetRawPagesAsync methods to consider HasMore in addition to LastId when deciding whether to continue pagination.
/// It also replaces specific parameters with an options type for pagination methods.
/// </summary>
public class PaginationVisitor : ScmLibraryVisitor
{

    private static readonly string[] _chatParamsToReplace = ["after", "before", "limit", "order", "model", "metadata"];

    private static readonly Dictionary<string, (string ReturnType, string OptionsType, string[] ParamsToReplace)> _optionsReplacements = new()
    {
        {
            "GetChatCompletions",
            ("ChatCompletion", "ChatCompletionCollectionOptions", _chatParamsToReplace)
        },
        {
            "GetChatCompletionsAsync",
            ("ChatCompletion", "ChatCompletionCollectionOptions", _chatParamsToReplace)
        }
    };

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        // Check if the method is one of the pagination methods we want to modify.
        // If so, we will update its parameters to replace the specified parameters with the options type.
        if (method.Signature.ReturnType is not null &&
            method.Signature.ReturnType.Name.EndsWith("CollectionResult") &&
            _optionsReplacements.TryGetValue(method.Signature.Name, out var options) &&
            method.Signature.ReturnType.IsGenericType &&
            method.Signature.ReturnType.Arguments.Count == 1 &&
            method.Signature.ReturnType.Arguments[0].Name == options.ReturnType)
        {
            var optionsType = OpenAILibraryGenerator.Instance.OutputLibrary.TypeProviders.SingleOrDefault(t => t.Type.Name == options.ReturnType);
            if (optionsType is not null)
            {
                // replace the method parameters with names in the _paramsToReplace array with the optionsType
                var methodSignature = method.Signature;
                var newParameters = methodSignature.Parameters.ToList();
                int lastRemovedIndex = -1;
                for (int i = 0; i < newParameters.Count; i++)
                {
                    if (_chatParamsToReplace.Contains(newParameters[i].Name))
                    {
                        newParameters.RemoveAt(i);
                        lastRemovedIndex = i;
                        i--;
                    }
                }
                if (lastRemovedIndex >= 0)
                {
                    newParameters.Insert(
                        lastRemovedIndex,
                        new ParameterProvider("options", $"The pagination options", optionsType.Type, defaultValue: new KeywordExpression("default", null)));

                    var newSignature = new MethodSignature(
                    methodSignature.Name,
                    methodSignature.Description,
                    methodSignature.Modifiers,
                    methodSignature.ReturnType,
                    methodSignature.ReturnDescription,
                    newParameters,
                    methodSignature.Attributes,
                    methodSignature.GenericArguments,
                    methodSignature.GenericParameterConstraints,
                    methodSignature.ExplicitInterface,
                    methodSignature.NonDocumentComment);

                    method.Update(signature: newSignature);
                }
            }
        }

        // If the method is GetRawPagesAsync and is internal, we will modify the body statements to add a check for hasMore == false.
        // This is to ensure that pagination stops when hasMore is false, in addition to checking LastId.
        if (method.Signature.Name == "GetRawPagesAsync" && method.EnclosingType.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Internal))
        {
            var statements = method.BodyStatements?.ToList() ?? new List<MethodBodyStatement>();
            VisitExplodedMethodBodyStatements(
                statements!,
                statement =>
                {
                    if (statement is IfStatement)
                    {
                        return GetUpdatedIfStatement(
                        statement,
                        expression =>
                        {
                            // Check if this is a binary expression with "==" operator
                            if (expression is ScopedApi scopedApi && scopedApi.Original is BinaryOperatorExpression binaryExpr && binaryExpr.Operator == "==")
                            {
                                // Check if left side is "nextToken" and right side is "null"
                                if (binaryExpr.Left is VariableExpression leftVar &&
                                    leftVar.Declaration.RequestedName == "nextToken" &&
                                    binaryExpr.Right is KeywordExpression rightKeyword &&
                                    rightKeyword.Keyword == "null")
                                {
                                    // Create "hasMore == null" condition
                                    var hasMoreNullCheck = new BinaryOperatorExpression(
                                        "==",
                                        new MemberExpression(null, "hasMore"),
                                        new KeywordExpression("false", null));

                                    // Return "nextToken == null || hasMore == null"
                                    return new BinaryOperatorExpression("||", binaryExpr, hasMoreNullCheck);
                                }
                            }
                            return expression;
                        },
                        "Plugin customization: add hasMore == false check to pagination condition");
                    }
                    else if (statement is WhileStatement whileStatement)
                    {
                        var statementList = whileStatement.Body
                            .SelectMany(bodyStatement => bodyStatement)
                            .ToList();

                        for (int i = 0; i < statementList.Count; i++)
                        {
                            if (statementList[i] is ExpressionStatement expressionStatement &&
                                expressionStatement.Expression is AssignmentExpression assignmentExpression &&
                                assignmentExpression.Variable is VariableExpression variableExpression &&
                                variableExpression.Declaration.RequestedName == "nextToken" &&
                                assignmentExpression.Value is MemberExpression memberExpression &&
                                memberExpression.MemberName == "LastId")
                            {
                                // Create a new assignment for hasMore
                                var hasMoreAssignment = new AssignmentExpression(
                                    new DeclarationExpression(typeof(bool), "hasMore"),
                                    new MemberExpression(memberExpression.Inner, "HasMore"));

                                // Insert the new assignment before the existing one
                                statementList.Insert(i, new ExpressionStatement(hasMoreAssignment));
                                statementList.Insert(i, new SingleLineCommentStatement("Plugin customization: add hasMore assignment"));
                                var updatedWhileStatement = new WhileStatement(whileStatement.Condition);
                                foreach (MethodBodyStatement bodyStatement in statementList)
                                {
                                    updatedWhileStatement.Add(bodyStatement);
                                }
                                return updatedWhileStatement;
                            }
                        }
                    }
                    return statement;
                });

            method.Update(bodyStatements: statements);
            return method;
        }

        return base.VisitMethod(method);
    }
}