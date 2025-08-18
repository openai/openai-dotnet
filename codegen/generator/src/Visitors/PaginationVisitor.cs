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
/// This visitor modifies GetRawPagesAsync and GetRawPages methods to consider HasMore in addition to LastId when deciding whether to continue pagination.
/// It also replaces specific parameters with an options type for pagination methods.
/// </summary>
public class PaginationVisitor : ScmLibraryVisitor
{

    private static readonly string[] _paginationParamsToReplace = ["after", "before", "limit", "order", "model", "metadata", "filter"];
    private static readonly Dictionary<string, string> _paramReplacementMap = new()
    {
        { "after", "AfterId" },
        { "before", "BeforeId" },
        { "limit", "PageSizeLimit" },
        { "order", "Order" },
        { "model", "Model" },
        { "metadata", "Metadata" },
        { "filter", "Filter" }
    };
    private static readonly Dictionary<string, (string ReturnType, string OptionsType, string[] ParamsToReplace)> _optionsReplacements = new()
    {
        {
            "GetChatCompletions",
            ("ChatCompletion", "ChatCompletionCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetChatCompletionsAsync",
            ("ChatCompletion", "ChatCompletionCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetChatCompletionMessages",
            ("ChatCompletionMessageListDatum", "ChatCompletionCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetChatCompletionMessagesAsync",
            ("ChatCompletionMessageListDatum", "ChatCompletionMessageCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetVectorStores",
            ("VectorStore", "VectorStoreCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetVectorStoresAsync",
            ("VectorStore", "VectorStoreCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetFileAssociations",
            ("VectorStoreFileAssociation", "VectorStoreFileAssociationCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetFileAssociationsAsync",
            ("VectorStoreFileAssociation", "VectorStoreFileAssociationCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetFileAssociationsInBatch",
            ("VectorStoreFileAssociation", "VectorStoreFileAssociationCollectionOptions", _paginationParamsToReplace)
        },
        {
            "GetFileAssociationsInBatchAsync",
            ("VectorStoreFileAssociation", "VectorStoreFileAssociationCollectionOptions", _paginationParamsToReplace)
        }
    };

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        // Try to handle pagination methods with options replacement
        if (TryHandlePaginationMethodWithOptions(method))
        {
            return method;
        }

        // Try to handle GetRawPagesAsync methods for hasMore checks
        if (TryHandleGetRawPagesAsyncMethod(method))
        {
            return method;
        }

        return base.VisitMethod(method);
    }

    /// <summary>
    /// Handles pagination methods that need their parameters replaced with an options type.
    /// </summary>
    /// <param name="method">The method to potentially handle. Will be modified in place if handling is successful.</param>
    /// <returns>True if the method was handled, false otherwise.</returns>
    private bool TryHandlePaginationMethodWithOptions(MethodProvider method)
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
            var optionsType = OpenAILibraryGenerator.Instance.OutputLibrary.TypeProviders.SingleOrDefault(t => t.Type.Name == options.OptionsType);
            if (optionsType is not null)
            {
                // replace the method parameters with names in the _paramsToReplace array with the optionsType
                var methodSignature = method.Signature;
                var newParameters = methodSignature.Parameters.ToList();
                int lastRemovedIndex = -1;
                for (int i = 0; i < newParameters.Count; i++)
                {
                    if (_paginationParamsToReplace.Contains(newParameters[i].Name))
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
                        new ParameterProvider("options", $"The pagination options", optionsType.Type, defaultValue: Snippet.Default));

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

                    var optionsParam = newParameters[lastRemovedIndex];

                    // Update the method body statements to replace the old parameters with the new options parameter.
                    var statements = method.BodyStatements?.ToList() ?? new List<MethodBodyStatement>();
                    VisitExplodedMethodBodyStatements(statements!,
                        statement =>
                        {
                            // Check if the statement is a return statement
                            if (statement is ExpressionStatement exp && exp.Expression is KeywordExpression keyword && keyword.Keyword == "return")
                            {
                                // If it is, we will replace the parameters with the options parameter.
                                if (keyword.Expression is NewInstanceExpression newInstance &&
                                    newInstance.Parameters.Count > 0)
                                {
                                    // Create the new parameters with the options parameter.
                                    var newParameters = new List<ValueExpression>();
                                    foreach (var param in newInstance.Parameters)
                                    {
                                        if (param is VariableExpression varExpr && options.ParamsToReplace.Contains(varExpr.Declaration.RequestedName))
                                        {
                                            // Replace the parameter with the options parameter.
                                            if (_paramReplacementMap.TryGetValue(varExpr.Declaration.RequestedName, out var replacement))
                                            {
                                                newParameters.Add(optionsParam.NullConditional().Property(replacement));
                                            }
                                        }
                                        else if (param is InvokeMethodExpression invokeMethod && invokeMethod.MethodName == "ToString" &&
                                                 invokeMethod.InstanceReference is NullConditionalExpression nullConditional &&
                                                 nullConditional.Inner is VariableExpression varExpr2 &&
                                                 options.ParamsToReplace.Contains(varExpr2.Declaration.RequestedName))
                                        {
                                            // Replace the parameter with the options parameter.
                                            if (_paramReplacementMap.TryGetValue(varExpr2.Declaration.RequestedName, out var replacement))
                                            {
                                                newParameters.Add(optionsParam.NullConditional().Property(replacement).NullConditional().Invoke("ToString", Array.Empty<ValueExpression>()));
                                            }
                                        }
                                        else
                                        {
                                            // Keep the original parameter.
                                            newParameters.Add(param);
                                        }
                                    }
                                    // Create a new ExpressionStatement with the same children as the original, but with the new parameters.
                                    return Snippet.Return(Snippet.New.Instance(newInstance.Type!, newParameters));
                                }
                            }
                            return statement;
                        });

                    method.Update(signature: newSignature, bodyStatements: statements);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Handles GetRawPagesAsync methods to add hasMore == false checks for pagination.
    /// </summary>
    /// <param name="method">The method to potentially handle. Will be modified in place if handling is successful.</param>
    /// <returns>True if the method was handled, false otherwise.</returns>
    private bool TryHandleGetRawPagesAsyncMethod(MethodProvider method)
    {
        // If the method is GetRawPagesAsync or GetRawPages and is internal, we will modify the body statements to add a check for hasMore == false.
        // This is to ensure that pagination stops when hasMore is false, in addition to checking LastId.
        if ((method.Signature.Name == "GetRawPagesAsync" || method.Signature.Name == "GetRawPages") && method.EnclosingType.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Internal))
        {
            VariableExpression? hasMoreVariable = null;
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
                                    // Create "!hasMore" condition. Note the hasMoreVariable gets assigned earlier in the method statements
                                    // in the WhileStatement handler below.
                                    var hasMoreNullCheck = Snippet.Not(hasMoreVariable);

                                    // Return "nextToken == null || !hasMore"
                                    return BoolSnippets.Or(binaryExpr.As<bool>(), hasMoreNullCheck);
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

                        // Check for the assignment of nextToken and add hasMore assignment
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
                                    Snippet.Declare("hasMore", typeof(bool), out hasMoreVariable),
                                    new MemberExpression(memberExpression.Inner, "HasMore"));

                                // Insert the new assignment before the existing one
                                statementList.Insert(i, hasMoreAssignment.Terminate());
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
            return true;
        }

        return false;
    }
}