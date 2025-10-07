using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin;

public class OpenAILibraryVisitor : ScmLibraryVisitor
{
    private const string RawDataPropertyName = "SerializedAdditionalRawData";
    private const string AdditionalPropertiesFieldName = "_additionalBinaryDataProperties";
    private const string SentinelValueFieldName = "_sentinelValue";
    private const string ModelSerializationExtensionsTypeName = "ModelSerializationExtensions";
    private const string IsSentinelValueMethodName = "IsSentinelValue";
    private const string JsonModelWriteCoreMethodName = "JsonModelWriteCore";

    // This dictionary defines properties within types that should have their plain serialization calls wrapped with
    // a conditional that includes an appropriate "Optional" check, e.g.:
    //   - Optional.IsCollectionDefined(Messages) ... writer.WritePropertyName("messages"u8)
    //   - Optional.IsDefined(Model) ... writer.WritePropertyName("model"u8)
    private static WritePropertyNameAdditionalReplacementInfo _readonlyStatusReplacementInfo = new("Status", "status", isCollection: false); 
    private static readonly Dictionary<string, List<WritePropertyNameAdditionalReplacementInfo>> TypeNameToWritePropertyNameAdditionalConditionMap = new()
    {
        ["ChatCompletionOptions"] =
            [
                new("Messages", "messages", isCollection: true),
                new("Model", "model", isCollection: false)
            ],
        ["ResponseItem"] =
            [
                new("Id", "id", isCollection: false),
            ],
        ["ComputerCallResponseItem"] = [_readonlyStatusReplacementInfo],
        ["ComputerCallOutputResponseItem"] = [_readonlyStatusReplacementInfo],
        ["FileSearchCallResponseItem"] = [_readonlyStatusReplacementInfo],
        ["FunctionCallResponseItem"] = [_readonlyStatusReplacementInfo],
        ["FunctionCallOutputResponseItem"] = [_readonlyStatusReplacementInfo],
        ["MessageResponseItem"] = [_readonlyStatusReplacementInfo],
        ["ReasoningResponseItem"] = [_readonlyStatusReplacementInfo],
        ["WebSearchCallResponseItem"] = [_readonlyStatusReplacementInfo],
    };
    private static readonly SingleLineCommentStatement OptionalDefinedCheckComment =
        new("Plugin customization: apply Optional.Is*Defined() check based on type name dictionary lookup");

    protected override TypeProvider VisitType(TypeProvider type)
    {
        var additionalPropertiesField = type.Fields.FirstOrDefault(f => f.Name == AdditionalPropertiesFieldName);
        if (type is ModelProvider { BaseModelProvider: null } && additionalPropertiesField != null)
        {
            // Add an internal AdditionalProperties property to all base models
            var properties = new List<PropertyProvider>(type.Properties)
            {
                new PropertyProvider($"", MethodSignatureModifiers.Internal,
                    typeof(IDictionary<string, BinaryData>), RawDataPropertyName,
                    new ExpressionPropertyBody(
                        additionalPropertiesField,
                        type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly) ? null : additionalPropertiesField.Assign(Value)),
                    type)
            };
            
            type.Update(properties: properties);
        }
        else if (type.Name == ModelSerializationExtensionsTypeName)
        {
            // Add a static BinaryData field representing the sentinel value
            var sentinelValueField = new FieldProvider(
                FieldModifiers.Private | FieldModifiers.Static | FieldModifiers.ReadOnly,
                typeof(BinaryData),
                SentinelValueFieldName,
                type,
                $"",
                BinaryDataSnippets.FromBytes(LiteralU8("\"__EMPTY__\"").Invoke("ToArray")));
            var fields = new List<FieldProvider>(type.Fields)
            {
                sentinelValueField
            };

            // Add the IsSentinelValue method
            var valueParameter = new ParameterProvider("value", $"", typeof(BinaryData));
            var methods = new List<MethodProvider>(type.Methods)
            {
                new MethodProvider(
                    new MethodSignature(
                        IsSentinelValueMethodName,
                        $"",
                        MethodSignatureModifiers.Internal | MethodSignatureModifiers.Static,
                        typeof(bool),
                        $"",
                        [valueParameter]),
                    new[]
                    {
                        Declare("sentinelSpan", typeof(ReadOnlySpan<byte>), sentinelValueField.As<BinaryData>().ToMemory().Property("Span"), out var sentinelVariable),
                        Declare("valueSpan", typeof(ReadOnlySpan<byte>), valueParameter.As<BinaryData>().ToMemory().Property("Span"), out var valueVariable),
                        Return(sentinelVariable.Invoke("SequenceEqual", valueVariable))
                    },
                    type)
            };
            
            type.Update(fields: fields, methods: methods);
        }
        return type;
    }

    protected override FieldProvider VisitField(FieldProvider field)
    {
        // Make the backing additional properties field not be read only as long as the type is not readonly.
        if (field.Name == AdditionalPropertiesFieldName && !field.EnclosingType.DeclarationModifiers.HasFlag(TypeSignatureModifiers.ReadOnly))
        {
            field.Modifiers &= ~FieldModifiers.ReadOnly;
        }
        return field;
    }

    protected override MethodProvider VisitMethod(MethodProvider method)
    {
        // If there are no body statements, or the body statements are not MethodBodyStatements,
        // return the method as is return the method as is
        if (method.Signature.Name != JsonModelWriteCoreMethodName ||
            method.BodyStatements is not MethodBodyStatements statements)
        {
            return method;
        }

        var updatedStatements = new List<MethodBodyStatement>();
        var flattenedStatements = new List<MethodBodyStatement>();

        foreach (var stmt in statements)
        {
            if (stmt is SuppressionStatement { Inner: not null } suppressionStatement)
            {
                // TO-DO: remove once enumerable logic is updated to handle nested suppression statements
                flattenedStatements.Add(suppressionStatement.DisableStatement);
                flattenedStatements.AddRange(suppressionStatement.Inner);
                flattenedStatements.Add(suppressionStatement.RestoreStatement);
            }
            else
            {
                flattenedStatements.Add(stmt);
            }
        }

        List<WritePropertyNameAdditionalReplacementInfo> additionalConditionsForWritingType
            = TypeNameToWritePropertyNameAdditionalConditionMap.GetValueOrDefault(method.EnclosingType.Name) ?? [];

        for (int line = 0; line < flattenedStatements.Count; line++)
        {
            var statement = flattenedStatements[line];

            // Much of the customization centers around treatment of WritePropertyName
            string? writePropertyNameTarget = GetWritePropertyNameTargetFromStatement(statement);

            switch (statement)
            {
                // If we already have an if statement that contains property writing, we need to add the condition to the existing if statement.
                // For dynamic models, we can skip adding the SARD condition.
                case IfStatement ifStatement:
                    ProcessIfStatement(ifStatement, writePropertyNameTarget, additionalConditionsForWritingType, updatedStatements);
                    break;
                case IfElseStatement ifElseStatement when GetPatchContainsExpression(ifElseStatement.If.Condition) != null:
                    ProcessIfElseStatement(ifElseStatement, writePropertyNameTarget, additionalConditionsForWritingType, updatedStatements);
                    break;
                case var _ when writePropertyNameTarget is not null:
                    line = ProcessWritePropertyNameStatement(statement, writePropertyNameTarget, additionalConditionsForWritingType, flattenedStatements, line, updatedStatements);
                    break;
                default:
                    updatedStatements.Add(statement);
                    break;
            }
        }

        method.Update(bodyStatements: updatedStatements);
        return method;
    }

    private static void ProcessIfStatement(
        IfStatement ifStatement,
        string? writePropertyNameTarget,
        List<WritePropertyNameAdditionalReplacementInfo> additionalConditionsForWritingType,
        List<MethodBodyStatement> updatedStatements)
    {
        if (writePropertyNameTarget is not null)
        {
            ValueExpression? patchContainsCondition = GetPatchContainsExpression(ifStatement.Condition);

            if (patchContainsCondition is null)
            {
                ifStatement.Update(condition: ifStatement.Condition.As<bool>().And(GetContainsKeyCondition(writePropertyNameTarget)));
            }
            else if (additionalConditionsForWritingType.FirstOrDefault(additionalCondition => additionalCondition.JsonName == writePropertyNameTarget) is var matchingReplacementInfo && matchingReplacementInfo != null)
            {
                updatedStatements.Add(OptionalDefinedCheckComment);
                ifStatement.Update(condition: GetOptionalIsCollectionDefinedCondition(matchingReplacementInfo).And(ifStatement.Condition));
            }
        }
        // Handle writing AdditionalProperties
        else if (ifStatement.Body.First() is ForEachStatement foreachStatement)
        {
            foreachStatement.Body.Insert(
                0,
                new IfStatement(
                    Static(new ModelSerializationExtensionsDefinition().Type).Invoke(
                        IsSentinelValueMethodName,
                        foreachStatement.ItemVariable.Property("Value")))
                {
                    Continue
                });
        }

        updatedStatements.Add(ifStatement);
    }

    private static void ProcessIfElseStatement(
        IfElseStatement ifElseStatement,
        string? writePropertyNameTarget,
        List<WritePropertyNameAdditionalReplacementInfo> additionalConditionsForWritingType,
        List<MethodBodyStatement> updatedStatements)
    {
        if (ifElseStatement.Else is null)
        {
            updatedStatements.Add(ifElseStatement);
            return;
        }

        if (additionalConditionsForWritingType.FirstOrDefault(additionalCondition => additionalCondition.JsonName == writePropertyNameTarget) is var matchingReplacementInfo && matchingReplacementInfo != null)
        {
            var enclosingCondition = GetOptionalIsCollectionDefinedCondition(matchingReplacementInfo);
            var updatedCondition = new IfStatement(enclosingCondition) { ifElseStatement.Else };

            ifElseStatement.Update(elseStatement: new MethodBodyStatements([OptionalDefinedCheckComment, updatedCondition]));
        }

        updatedStatements.Add(ifElseStatement);
    }

    private static int ProcessWritePropertyNameStatement(
        MethodBodyStatement statement,
        string writePropertyNameTarget,
        List<WritePropertyNameAdditionalReplacementInfo> additionalConditionsForWritingType,
        List<MethodBodyStatement> flattenedStatements,
        int currentLine,
        List<MethodBodyStatement> updatedStatements)
    {
        var line = currentLine;
        ScopedApi<bool> enclosingIfCondition = GetContainsKeyCondition(writePropertyNameTarget);

        if (additionalConditionsForWritingType.FirstOrDefault(additionalCondition => additionalCondition.JsonName == writePropertyNameTarget) is var matchingReplacementInfo && matchingReplacementInfo != null)
        {
            updatedStatements.Add(OptionalDefinedCheckComment);
            enclosingIfCondition = GetOptionalIsCollectionDefinedCondition(matchingReplacementInfo).And(enclosingIfCondition);
        }

        var ifSt = new IfStatement(enclosingIfCondition) { statement };

        // If this is a plain expression statement, we need to add the next statement as well which
        // will either write the property value or start writing an array
        if (statement is ExpressionStatement)
        {
            ifSt.Add(flattenedStatements[++line]);
            // Include array writing in the if statement
            if (flattenedStatements[line + 1] is ForEachStatement)
            {
                // Foreach
                ifSt.Add(flattenedStatements[++line]);
                // End array
                ifSt.Add(flattenedStatements[++line]);
            }
        }

        updatedStatements.Add(ifSt);
        return line;
    }

    private static ScopedApi<bool> GetContainsKeyCondition(string propertyName)
    {
        return This.Property(AdditionalPropertiesFieldName)
            .NullConditional()
            .Invoke("ContainsKey", Literal(propertyName)).NotEqual(True);
    }

    private static string? GetWritePropertyNameTargetFromStatement(MethodBodyStatement? statement)
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
        if (statement is SuppressionStatement suppressionStatement)
        {
            return GetWritePropertyNameTargetFromStatement(suppressionStatement.Inner);
        }
        else if (statement is MethodBodyStatements compoundStatements)
        {
            foreach (MethodBodyStatement innerStatement in compoundStatements.Statements)
            {
                if (GetWritePropertyNameTargetFromStatement(innerStatement) is string innerTarget)
                {
                    return innerTarget;
                }
            }
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

    private static ScopedApi<bool> GetOptionalIsCollectionDefinedCondition(WritePropertyNameAdditionalReplacementInfo replacementInfo)
    {
        string methodName = replacementInfo.IsCollection ? "IsCollectionDefined" : "IsDefined";
        return new MemberExpression(null, "Optional")
            .Invoke(methodName, new MemberExpression(null, replacementInfo.PropertyName))
            .As<bool>();
    }

    public class WritePropertyNameAdditionalReplacementInfo(string propertyName, string jsonName, bool isCollection)
    {
        public string PropertyName { get; set; } = propertyName;
        public string JsonName { get; set; } = jsonName;
        public bool IsCollection { get; set; } = isCollection;
    }


    /// <summary>
    /// Recursively checks if the given expression or any of its sub-expressions is a call to Patch.Contains().
    /// Handles various wrapping scenarios including unary operators, binary operators, and nested expressions.
    /// </summary>
    private static ValueExpression? GetPatchContainsExpression(ValueExpression? expression)
    {
        if (expression is null)
        {
            return null;
        }

#pragma warning disable SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        return expression switch
        {
            // Case 1: Direct Patch.Contains() call
            ScopedApi<bool> { Original: InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } } => expression,

            // Case 2: !Patch.Contains() call
            ScopedApi<bool> { Original: UnaryOperatorExpression { Operator: "!", Operand: ScopedApi<bool> { Original: InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } } } } => expression,

            // Case 3 & 4: Binary operator expression (wrapped or unwrapped)
            ScopedApi<bool> { Original: BinaryOperatorExpression binaryExpr } =>
                GetPatchContainsExpression(binaryExpr.Left) ?? GetPatchContainsExpression(binaryExpr.Right),

            BinaryOperatorExpression binaryExpr =>
                GetPatchContainsExpression(binaryExpr.Left) ?? GetPatchContainsExpression(binaryExpr.Right),

            // Case 5: Direct UnaryOperatorExpression (not wrapped in ScopedApi)
            UnaryOperatorExpression { Operator: "!" } unaryExpr =>
                GetPatchContainsExpression(unaryExpr.Operand) != null ? expression : null,

            // Case 6: Direct InvokeMethodExpression (not wrapped in ScopedApi)
            InvokeMethodExpression { InstanceReference: ScopedApi<JsonPatch> } => expression,

            _ => null
        };

#pragma warning restore SCME0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}
