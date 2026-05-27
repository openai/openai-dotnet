using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System;
using System.Collections.Generic;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Adds the sentinel field and helper method used by model serialization extensions.
/// </summary>
public class ModelSerializationSentinelVisitor : ScmLibraryVisitor
{
    private const string SentinelValueFieldName = "_sentinelValue";

    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (type.Name != SerializationVisitorHelpers.ModelSerializationExtensionsTypeName)
        {
            return type;
        }

        FieldProvider sentinelValueField = new(
            FieldModifiers.Private | FieldModifiers.Static | FieldModifiers.ReadOnly,
            typeof(BinaryData),
            SentinelValueFieldName,
            type,
            $"",
            BinaryDataSnippets.FromBytes(LiteralU8("\"__EMPTY__\"").Invoke("ToArray")));

        ParameterProvider valueParameter = new("value", $"", typeof(BinaryData));
        List<FieldProvider> fields = [.. type.Fields, sentinelValueField];
        List<MethodProvider> methods =
        [
            .. type.Methods,
            new MethodProvider(
                new MethodSignature(
                    SerializationVisitorHelpers.IsSentinelValueMethodName,
                    $"",
                    MethodSignatureModifiers.Internal | MethodSignatureModifiers.Static,
                    typeof(bool),
                    $"",
                    [valueParameter]),
                new MethodBodyStatement[]
                {
                    Declare("sentinelSpan", typeof(ReadOnlySpan<byte>), sentinelValueField.As<BinaryData>().ToMemory().Property("Span"), out var sentinelVariable),
                    Declare("valueSpan", typeof(ReadOnlySpan<byte>), valueParameter.As<BinaryData>().ToMemory().Property("Span"), out var valueVariable),
                    Return(sentinelVariable.Invoke("SequenceEqual", valueVariable)),
                },
                type),
        ];

        type.Update(fields: fields, methods: methods);
        return type;
    }
}
