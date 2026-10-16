using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// Adds the sentinel members used when serializing additional binary data properties.
/// </summary>
public class ModelSerializationExtensionsVisitor : ScmLibraryVisitor
{
    private const string SentinelValueFieldName = "_sentinelValue";
    private const string ModelSerializationExtensionsTypeName = "ModelSerializationExtensions";
    private const string IsSentinelValueMethodName = "IsSentinelValue";

    protected override TypeProvider VisitType(TypeProvider type)
    {
        if (type.Name != ModelSerializationExtensionsTypeName)
        {
            return type;
        }

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
        return type;
    }
}
