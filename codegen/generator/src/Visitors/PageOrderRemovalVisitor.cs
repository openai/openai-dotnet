using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

public class PageOrderRemovalVisitor : ScmLibraryVisitor
{
    private List<CSharpType> SuppressedTypes { get; } = [];
    private OpenAILibraryGenerator _parentPlugin;
    private readonly InputParameter _syntheticPageOrderInputParameter;

    public PageOrderRemovalVisitor(OpenAILibraryGenerator parentPlugin)
    {
        _parentPlugin = parentPlugin;
        _syntheticPageOrderInputParameter = CreateSyntheticPageOrderInputParameter();
    }

    protected override MethodProvider? VisitMethod(MethodProvider method)
    {
        foreach (ParameterProvider parameter in method.Signature?.Parameters ?? [])
        {
            if (SuppressedTypes.Any(suppressedType => suppressedType.Name == parameter.Type.Name))
            {
                // Pending full plugin integration of synthetic type, we'll just suppress the generation of any method
                // using one of the replaced types.
                return null;
            }
        }

        return method;
    }

    protected override TypeProvider? VisitType(TypeProvider type)
    {
        if (type.Name.EndsWith("RequestOrder")
            && type.IsEnum
            && type.EnumValues.Count == 2
            && type.EnumValues[0].Value.ToString() == "asc"
            && type.EnumValues[1].Value.ToString() == "desc")
        {
            SuppressedTypes.Add(type.Type);
            return null;
        }
        return type;
    }

    private TypeProvider? CreateOrderTypeProvider(TypeProvider originalProvider)
    {
        InputEnumType inputEnumType = new(
            name: "OpenAIPageOrder",
            @namespace: "",
            crossLanguageDefinitionId: "",
            access: null,
            deprecation: null,
            summary: "",
            doc: "",
            usage: InputModelTypeUsage.Input | InputModelTypeUsage.Json,
            valueType: InputPrimitiveType.String,
            values:
            [
                new InputEnumTypeValue(
                name: "Ascending",
                value: "asc",
                valueType: InputPrimitiveType.String,
                summary: "",
                doc: ""),
            new InputEnumTypeValue(
                name: "Descending",
                value: "desc",
                valueType: InputPrimitiveType.String,
                summary: "",
                doc: "")
            ],
            isExtensible: true);
        return _parentPlugin.TypeFactory.CreateEnum(inputEnumType);
    }

    private static InputParameter CreateSyntheticPageOrderInputParameter()
    {
        InputModelType inputModelType = new(
            name: "OpenAIPageOrder",
            @namespace: "",
            crossLanguageDefinitionId: "",
            access: null,
            deprecation: null,
            summary: "",
            doc: "",
            usage: InputModelTypeUsage.Input,
            properties: [],
            baseModel: null,
            derivedModels: [],
            discriminatorValue: null,
            discriminatorProperty: null,
            discriminatedSubtypes: new Dictionary<string, InputModelType>(),
            additionalProperties: null,
            modelAsStruct: true,
            serializationOptions: new());
        return new InputParameter(
            name: "order",
            nameInRequest: "order",
            summary: "",
            doc: "",
            inputModelType,
            InputRequestLocation.Query,
            defaultValue: null,
            InputParameterKind.Spread,
            isRequired: false,
            isApiVersion: false,
            isContentType: false,
            isEndpoint: false,
            skipUrlEncoding: false,
            explode: true,
            arraySerializationDelimiter: null,
            headerCollectionPrefix: null,
            serverUrlTemplate: null);
    }
}