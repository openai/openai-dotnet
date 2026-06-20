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
    private readonly record struct OptionsReplacementInfo(string ReturnType, string OptionsType, IReadOnlySet<string> ParamsToReplace);

    private static readonly string[] _paginationParamsToReplace = ["after", "afterId", "before", "limit", "pageSizeLimit", "order", "model", "metadata", "filter", "name"];
    private static readonly string[] _containerFilePaginationParamsToReplace = [.. _paginationParamsToReplace, "containerId"];
    private static readonly Dictionary<string, string> _paramReplacementMap = new()
    {
        { "after", "AfterId" },
        { "afterId", "AfterId" },
        { "before", "BeforeId" },
        { "limit", "PageSizeLimit" },
        { "pageSizeLimit", "PageSizeLimit" },
        { "order", "Order" },
        { "model", "Model" },
        { "metadata", "Metadata" },
        { "filter", "Filter" },
        { "name", "Name" },
        { "containerId", "ContainerId" }
    };
    private static readonly HashSet<string> _normalizedPaginationParamsToReplace = CreateNormalizedNameSet(_paginationParamsToReplace);
    private static readonly HashSet<string> _normalizedContainerFilePaginationParamsToReplace = CreateNormalizedNameSet(_containerFilePaginationParamsToReplace);
    private static readonly Dictionary<string, string> _normalizedParamReplacementMap = CreateNormalizedReplacementMap(_paramReplacementMap);
    private static readonly Dictionary<string, OptionsReplacementInfo> _optionsReplacements = new()
    {
        {
            "GetChatCompletions",
            new OptionsReplacementInfo("ChatCompletion", "ChatCompletionCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetChatCompletionsAsync",
            new OptionsReplacementInfo("ChatCompletion", "ChatCompletionCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetChatCompletionMessages",
            new OptionsReplacementInfo("ChatCompletionMessageListDatum", "ChatCompletionMessageCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetChatCompletionMessagesAsync",
            new OptionsReplacementInfo("ChatCompletionMessageListDatum", "ChatCompletionMessageCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetBatches",
            new OptionsReplacementInfo("BatchJob", "BatchCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetBatchesAsync",
            new OptionsReplacementInfo("BatchJob", "BatchCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStores",
            new OptionsReplacementInfo("VectorStore", "VectorStoreCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStoresAsync",
            new OptionsReplacementInfo("VectorStore", "VectorStoreCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStoreFiles",
            new OptionsReplacementInfo("VectorStoreFile", "VectorStoreFileCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStoreFilesAsync",
            new OptionsReplacementInfo("VectorStoreFile", "VectorStoreFileCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStoreFilesInBatch",
            new OptionsReplacementInfo("VectorStoreFile", "VectorStoreFileCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetVectorStoreFilesInBatchAsync",
            new OptionsReplacementInfo("VectorStoreFile", "VectorStoreFileCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetContainers",
            new OptionsReplacementInfo("ContainerResource", "ContainerCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetContainersAsync",
            new OptionsReplacementInfo("ContainerResource", "ContainerCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetContainerFiles",
            new OptionsReplacementInfo("ContainerFileResource", "ContainerFileCollectionOptions", _normalizedContainerFilePaginationParamsToReplace)
        },
        {
            "GetContainerFilesAsync",
            new OptionsReplacementInfo("ContainerFileResource", "ContainerFileCollectionOptions", _normalizedContainerFilePaginationParamsToReplace)
        },
        {
            "GetResponseInputItems",
            new OptionsReplacementInfo("ResponseItem", "ResponseItemCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetResponseInputItemsAsync",
            new OptionsReplacementInfo("ResponseItem", "ResponseItemCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetAssistants",
            new OptionsReplacementInfo("Assistant", "AssistantCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetAssistantsAsync",
            new OptionsReplacementInfo("Assistant", "AssistantCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetMessages",
            new OptionsReplacementInfo("ThreadMessage", "MessageCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetMessagesAsync",
            new OptionsReplacementInfo("ThreadMessage", "MessageCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetRuns",
            new OptionsReplacementInfo("ThreadRun", "RunCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetRunsAsync",
            new OptionsReplacementInfo("ThreadRun", "RunCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetRunSteps",
            new OptionsReplacementInfo("RunStep", "RunStepCollectionOptions", _normalizedPaginationParamsToReplace)
        },
        {
            "GetRunStepsAsync",
            new OptionsReplacementInfo("RunStep", "RunStepCollectionOptions", _normalizedPaginationParamsToReplace)
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
            _optionsReplacements.TryGetValue(method.Signature.Name, out OptionsReplacementInfo options) &&
            method.Signature.ReturnType.IsGenericType &&
            method.Signature.ReturnType.Arguments.Count == 1 &&
            method.Signature.ReturnType.Arguments[0].Name == options.ReturnType)
        {
            TypeProvider? optionsType = OpenAILibraryGenerator.Instance.OutputLibrary.TypeProviders.SingleOrDefault(t => t.Type.Name == options.OptionsType);
            if (optionsType is not null)
            {
                // replace the method parameters with names in the _paramsToReplace array with the optionsType
                MethodSignature methodSignature = method.Signature;
                List<ParameterProvider> newParameters = methodSignature.Parameters.ToList();
                List<ParameterProvider> replacedParameters = new List<ParameterProvider>();
                int lastRemovedIndex = -1;
                for (int i = 0; i < newParameters.Count; i++)
                {
                    if (TryGetReplacedParameterName(newParameters[i], options.ParamsToReplace, out _))
                    {
                        replacedParameters.Add(newParameters[i]);
                        newParameters.RemoveAt(i);
                        lastRemovedIndex = i;
                        i--;
                    }
                }
                if (lastRemovedIndex >= 0)
                {
                    bool optionsParameterIsOptional = replacedParameters.All(parameter => parameter.DefaultValue is not null);
                    newParameters.Insert(
                        lastRemovedIndex,
                        new ParameterProvider(
                            "options",
                            $"The pagination options",
                            optionsType.Type,
                            defaultValue: optionsParameterIsOptional ? Snippet.Default : null,
                            validation: optionsParameterIsOptional ? null : ParameterValidationType.AssertNotNull));

                    MethodSignature newSignature = new MethodSignature(
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

                    ParameterProvider optionsParam = newParameters[lastRemovedIndex];

                    // Update the method body statements to replace the old parameters with the new options parameter.
                    List<MethodBodyStatement> statements = method.BodyStatements?
                        .Where(statement => !replacedParameters.Any(parameter => IsValidationStatementForParameter(statement, parameter)))
                        .ToList() ?? new List<MethodBodyStatement>();

                    int insertIndex = 0;
                    if (!optionsParameterIsOptional)
                    {
                        statements.Insert(insertIndex++, ArgumentSnippets.ValidateParameter(optionsParam));
                    }

                    foreach (ParameterProvider replacedParameter in replacedParameters.Where(parameter => parameter.Validation != ParameterValidationType.None))
                    {
                        if (TryGetReplacementPropertyName(replacedParameter, out string replacementPropertyName))
                        {
                            statements.Insert(insertIndex++, CreatePropertyValidationStatement(optionsParam, replacementPropertyName, replacedParameter.Validation));
                        }
                    }

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
                                    List<ValueExpression> newParameters = new List<ValueExpression>();
                                    foreach (ValueExpression param in newInstance.Parameters)
                                    {
                                        if (param is VariableExpression variableExpression
                                            && TryGetReplacementPropertyName(variableExpression.Declaration.RequestedName, options.ParamsToReplace, out string replacementPropertyName))
                                        {
                                            bool useNullConditional = ShouldUseNullConditional(optionsParameterIsOptional, replacedParameters, variableExpression.Declaration.RequestedName);
                                            newParameters.Add(GetOptionsPropertyValue(optionsParam, replacementPropertyName, useNullConditional));
                                        }
                                        else if (param is InvokeMethodExpression invokeMethod && invokeMethod.MethodName == "ToString" &&
                                                 invokeMethod.InstanceReference is NullConditionalExpression nullConditional &&
                                                 nullConditional.Inner is VariableExpression invokeVariableExpression &&
                                                 TryGetReplacementPropertyName(invokeVariableExpression.Declaration.RequestedName, options.ParamsToReplace, out string invokeReplacementPropertyName))
                                        {
                                            bool useNullConditional = ShouldUseNullConditional(optionsParameterIsOptional, replacedParameters, invokeVariableExpression.Declaration.RequestedName);
                                            ValueExpression propertyValue = GetOptionsPropertyValue(optionsParam, invokeReplacementPropertyName, useNullConditional);
                                            newParameters.Add(useNullConditional
                                                ? propertyValue.NullConditional().Invoke("ToString", Array.Empty<ValueExpression>())
                                                : propertyValue.Invoke("ToString", Array.Empty<ValueExpression>()));
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

    private static bool ShouldUseNullConditional(bool optionsParameterIsOptional, IReadOnlyList<ParameterProvider> replacedParameters, string parameterName)
        => optionsParameterIsOptional || replacedParameters.Any(parameter =>
            ParameterMatchesName(parameter, parameterName)
            && parameter.DefaultValue is not null);

    private static ValueExpression GetOptionsPropertyValue(ParameterProvider optionsParam, string replacement, bool useNullConditional)
        => useNullConditional
            ? optionsParam.NullConditional().Property(replacement)
            : optionsParam.Property(replacement);

    private static bool IsValidationStatementForParameter(MethodBodyStatement statement, ParameterProvider parameter)
        => parameter.Validation switch
        {
            ParameterValidationType.AssertNotNull => MatchesValidationStatement(statement, parameter, "Argument.AssertNotNull"),
            ParameterValidationType.AssertNotNullOrEmpty => MatchesValidationStatement(statement, parameter, "Argument.AssertNotNullOrEmpty"),
            _ => false,
        };

    private static bool MatchesValidationStatement(MethodBodyStatement statement, ParameterProvider parameter, string validationMethod)
    {
        string normalizedStatement = NormalizeParameterName(statement.ToDisplayString());
        return GetNormalizedParameterNames(parameter).Any(normalizedParameterName =>
            normalizedStatement.Contains(
                $"{validationMethod}({normalizedParameterName}, nameof({normalizedParameterName}))",
                StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryGetReplacementPropertyName(ParameterProvider parameter, out string replacement)
    {
        foreach (string normalizedParameterName in GetNormalizedParameterNames(parameter))
        {
            if (_normalizedParamReplacementMap.TryGetValue(normalizedParameterName, out string? mappedReplacement))
            {
                replacement = mappedReplacement;
                return true;
            }
        }

        replacement = string.Empty;
        return false;
    }

    private static bool TryGetReplacementPropertyName(string parameterName, IReadOnlySet<string> paramsToReplace, out string replacement)
    {
        string normalizedParameterName = NormalizeParameterName(parameterName);
        if (paramsToReplace.Contains(normalizedParameterName) &&
            _normalizedParamReplacementMap.TryGetValue(normalizedParameterName, out string? mappedReplacement))
        {
            replacement = mappedReplacement;
            return true;
        }

        replacement = string.Empty;
        return false;
    }

    private static bool TryGetReplacedParameterName(ParameterProvider parameter, IReadOnlySet<string> paramsToReplace, out string parameterName)
    {
        foreach (string normalizedParameterName in GetNormalizedParameterNames(parameter))
        {
            if (paramsToReplace.Contains(normalizedParameterName))
            {
                parameterName = normalizedParameterName;
                return true;
            }
        }

        parameterName = string.Empty;
        return false;
    }

    private static bool ParameterMatchesName(ParameterProvider parameter, string parameterName)
    {
        string normalizedParameterName = NormalizeParameterName(parameterName);
        return GetNormalizedParameterNames(parameter).Contains(normalizedParameterName);
    }

    private static IReadOnlySet<string> GetNormalizedParameterNames(ParameterProvider parameter)
    {
        HashSet<string> parameterNames = new(StringComparer.OrdinalIgnoreCase)
        {
            NormalizeParameterName(parameter.Name)
        };

        if (parameter.InputParameter?.Name is string inputName)
        {
            parameterNames.Add(NormalizeParameterName(inputName));
        }

        if (parameter.InputParameter?.OriginalName is string originalName)
        {
            parameterNames.Add(NormalizeParameterName(originalName));
        }

        if (parameter.InputParameter?.SerializedName is string serializedName)
        {
            parameterNames.Add(NormalizeParameterName(serializedName));
        }

        return parameterNames;
    }

    private static HashSet<string> CreateNormalizedNameSet(IEnumerable<string> parameterNames)
        => new HashSet<string>(parameterNames.Select(NormalizeParameterName), StringComparer.OrdinalIgnoreCase);

    private static Dictionary<string, string> CreateNormalizedReplacementMap(IEnumerable<KeyValuePair<string, string>> replacements)
        => replacements.ToDictionary(pair => NormalizeParameterName(pair.Key), pair => pair.Value, StringComparer.OrdinalIgnoreCase);

    private static string NormalizeParameterName(string parameterName)
        => parameterName
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);

    private static MethodBodyStatement CreatePropertyValidationStatement(
        ParameterProvider optionsParam,
        string replacement,
        ParameterValidationType validation)
    {
        ValueExpression propertyExpression = optionsParam.Property(replacement);
        LiteralExpression propertyName = new LiteralExpression($"options.{replacement}");

        return validation switch
        {
            ParameterValidationType.AssertNotNull => ArgumentSnippets.AssertNotNull(propertyExpression, propertyName),
            ParameterValidationType.AssertNotNullOrEmpty => ArgumentSnippets.AssertNotNullOrEmpty(propertyExpression, propertyName),
            _ => MethodBodyStatement.Empty,
        };
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
            List<MethodBodyStatement> statements = method.BodyStatements?.ToList() ?? new List<MethodBodyStatement>();
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
                            // Check if this is a string.IsNullOrEmpty(nextToken) call
                            if (expression is InvokeMethodExpression invokeExpr && invokeExpr.MethodName == "IsNullOrEmpty")
                            {
                                // Check if the argument is "nextToken"
                                if (invokeExpr.Arguments.Count == 1 &&
                                    invokeExpr.Arguments[0] is VariableExpression argVar &&
                                    argVar.Declaration.RequestedName == "nextToken"
                                    && hasMoreVariable != null)
                                {
                                    // Create "!hasMore" condition. Note the hasMoreVariable gets assigned earlier in the method statements
                                    // in the WhileStatement handler below.
                                    ValueExpression hasMoreNullCheck = Snippet.Not(hasMoreVariable);

                                    // Return "string.IsNullOrEmpty(nextToken) || !hasMore"
                                    return BoolSnippets.Or(invokeExpr.As<bool>(), hasMoreNullCheck);
                                }
                            }
                            return expression;
                        },
                        "Plugin customization: add hasMore == false check to pagination condition");
                    }
                    else if (statement is WhileStatement whileStatement)
                    {
                        List<MethodBodyStatement> statementList = whileStatement.Body
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
                                AssignmentExpression hasMoreAssignment = new AssignmentExpression(
                                    Snippet.Declare("hasMore", typeof(bool), out hasMoreVariable),
                                    new MemberExpression(memberExpression.Inner, "HasMore"));

                                // Insert the new assignment before the existing one
                                statementList.Insert(i, hasMoreAssignment.Terminate());
                                statementList.Insert(i, new SingleLineCommentStatement("Plugin customization: add hasMore assignment"));
                                WhileStatement updatedWhileStatement = new WhileStatement(whileStatement.Condition);
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