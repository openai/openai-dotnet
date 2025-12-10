using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor provides the customizations needed for protocol models.
/// </summary>
public class ProtocolModelVisitor : ScmLibraryVisitor
{
    // Classes that are protocol models.
    private static readonly HashSet<string> _protocolModels = new(StringComparer.OrdinalIgnoreCase)
    {
        "OpenAI.Responses.CreateResponseOptions",
        "OpenAI.Responses.ResponseDeletionResult",
        "OpenAI.Responses.ResponseItemCollectionPage",
        "OpenAI.Responses.ResponseResult",

        "OpenAI.Responses.StreamingResponseUpdate",
        "OpenAI.Responses.StreamingResponseCodeInterpreterCallCodeDeltaUpdate",
        "OpenAI.Responses.StreamingResponseCodeInterpreterCallCodeDoneUpdate",
        "OpenAI.Responses.StreamingResponseCodeInterpreterCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseCodeInterpreterCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseCodeInterpreterCallInterpretingUpdate",
        "OpenAI.Responses.StreamingResponseCompletedUpdate",
        "OpenAI.Responses.StreamingResponseContentPartAddedUpdate",
        "OpenAI.Responses.StreamingResponseContentPartDoneUpdate",
        "OpenAI.Responses.StreamingResponseCreatedUpdate",
        "OpenAI.Responses.StreamingResponseErrorUpdate",
        "OpenAI.Responses.StreamingResponseFailedUpdate",
        "OpenAI.Responses.StreamingResponseFileSearchCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseFileSearchCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseFileSearchCallSearchingUpdate",
        "OpenAI.Responses.StreamingResponseFunctionCallArgumentsDeltaUpdate",
        "OpenAI.Responses.StreamingResponseFunctionCallArgumentsDoneUpdate",
        "OpenAI.Responses.StreamingResponseImageGenerationCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseImageGenerationCallGeneratingUpdate",
        "OpenAI.Responses.StreamingResponseImageGenerationCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseImageGenerationCallPartialImageUpdate",
        "OpenAI.Responses.StreamingResponseIncompleteUpdate",
        "OpenAI.Responses.StreamingResponseInProgressUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallArgumentsDeltaUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallArgumentsDoneUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallFailedUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsCompletedUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsFailedUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsInProgressUpdate",
        "OpenAI.Responses.StreamingResponseOutputItemAddedUpdate",
        "OpenAI.Responses.StreamingResponseOutputItemDoneUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallFailedUpdate",
        "OpenAI.Responses.StreamingResponseMcpCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsCompletedUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsFailedUpdate",
        "OpenAI.Responses.StreamingResponseMcpListToolsInProgressUpdate",
        "OpenAI.Responses.StreamingResponseOutputItemAddedUpdate",
        "OpenAI.Responses.StreamingResponseOutputItemDoneUpdate",
        "OpenAI.Responses.StreamingResponseOutputTextDeltaUpdate",
        "OpenAI.Responses.StreamingResponseOutputTextDoneUpdate",
        "OpenAI.Responses.StreamingResponseQueuedUpdate",
        "OpenAI.Responses.StreamingResponseReasoningSummaryPartAddedUpdate",
        "OpenAI.Responses.StreamingResponseReasoningSummaryPartDoneUpdate",
        "OpenAI.Responses.StreamingResponseReasoningSummaryTextDeltaUpdate",
        "OpenAI.Responses.StreamingResponseReasoningSummaryTextDoneUpdate",
        "OpenAI.Responses.StreamingResponseReasoningTextDeltaUpdate",
        "OpenAI.Responses.StreamingResponseReasoningTextDoneUpdate",
        "OpenAI.Responses.StreamingResponseRefusalDeltaUpdate",
        "OpenAI.Responses.StreamingResponseRefusalDoneUpdate",
        "OpenAI.Responses.StreamingResponseTextAnnotationAddedUpdate",
        "OpenAI.Responses.StreamingResponseWebSearchCallCompletedUpdate",
        "OpenAI.Responses.StreamingResponseWebSearchCallInProgressUpdate",
        "OpenAI.Responses.StreamingResponseWebSearchCallSearchingUpdate",
    };

    // All the properties of protocol models should have setters, except for collection properties.
    protected override PropertyProvider? PreVisitProperty(InputProperty property, PropertyProvider? propertyProvider)
    {
        if (propertyProvider is not null
            && !propertyProvider.Type.IsCollection
            && _protocolModels.Contains($"{propertyProvider.EnclosingType.Type.Namespace}.{propertyProvider.EnclosingType.Name}"))
        {
            propertyProvider.Update(body: new AutoPropertyBody(HasSetter: true));
        }

        return propertyProvider;
    }

    // All protocol models should have a public parameterless constructor.
    protected override TypeProvider? VisitType(TypeProvider typeProvider)
    {
        if (typeProvider is ModelProvider modelProvider
           && !modelProvider.Type.IsValueType
           && !modelProvider.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Static))
        {
            List<ConstructorProvider> allGeneratedConstructors =
            [
              .. modelProvider.Constructors,
              .. modelProvider.SerializationProviders.SelectMany(mrwProvider => mrwProvider.Constructors),
            ];

            foreach (ConstructorProvider constructorProvider in allGeneratedConstructors)
            {
                if (constructorProvider is not null
                    && constructorProvider.Signature.Parameters.Count == 0 // Check that this is a default constructor
                    && modelProvider.DerivedModels.Count == 0 // The default constructor should be visible in the derived models, not the base model
                    && _protocolModels.Contains($"{constructorProvider.EnclosingType.Type.Namespace}.{constructorProvider.EnclosingType.Name}"))
                {
                    constructorProvider.Signature.Update(modifiers: MethodSignatureModifiers.Public);
                }
            }
        }

        return typeProvider;
    }

    // All protocol models should not have an implicit conversion to BinaryContent.
    // By contrast, this conversion should be surpressed for convenience models.
    protected override MethodProvider? VisitMethod(MethodProvider methodProvider)
    {
        if (methodProvider is not null
            && methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Implicit)
            && methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Operator)
            && methodProvider.Signature.Name == "BinaryContent"
            && methodProvider.Signature.Parameters.Count == 1
            && !_protocolModels.Contains($"{methodProvider.EnclosingType.Type.Namespace}.{methodProvider.EnclosingType.Name}"))
        {
            return null;
        }

        return methodProvider;
    }
}