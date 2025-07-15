using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Expressions;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.TypeSpec.Generator.Snippets.Snippet;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// This visitor performs several in-place modifications of model-based type constructors:
///   1. All generated constructors ensure that collections are initialized (via null coalescence) in their bodies
///   2. All generated default constructors chain initialization to the generated serialization constructor
/// </summary>
public class ConstructorFixupVisitor : ScmLibraryVisitor
{
    private static readonly MethodBodyStatement CommentStatement
        = new SingleLineCommentStatement("Plugin customization: ensure initialization of collections");

    protected override TypeProvider? PostVisitType(TypeProvider type)
    {
        if (type is not ModelProvider modelProvider
            || modelProvider.Type.IsValueType
            || modelProvider.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Static))
        {
            return type;
        }

        List<ConstructorProvider> allGeneratedConstructors =
            [
              .. modelProvider.Constructors,
              .. modelProvider.SerializationProviders.SelectMany(mrwProvider => mrwProvider.Constructors),
            ];
        List<ConstructorProvider> allConstructorsIncludingCustom =
            [
                .. allGeneratedConstructors,
                .. (modelProvider?.CustomCodeView?.Constructors ?? [])
            ];
        ConstructorProvider? serializationConstructor = allConstructorsIncludingCustom.MaxBy(ctr => ctr.Signature.Parameters.Count);
        ConstructorProvider? generatedDefaultConstructor = allGeneratedConstructors.FirstOrDefault(ctr => ctr.Signature.Parameters.Count == 0);

        bool adjustmentPerformed = false;

        foreach (ConstructorProvider constructor in allGeneratedConstructors)
        {
            adjustmentPerformed |= TryUpdateConstructorForCollectionInitialization(constructor);
        }

        // If any generated constructors were updated to perform additional initialization, OR if the type uses a discriminator, adjust the
        // default constructor to chain to the serialization constructor that will produce an independently valid instance state.
        if (generatedDefaultConstructor is not null
            && (adjustmentPerformed || modelProvider?.DiscriminatorValueExpression is not null))
        {
            CSharpType? discriminatorType = modelProvider?.DiscriminatorValueExpression is MemberExpression enclosingDiscriminatorValueExpression
                && enclosingDiscriminatorValueExpression.Inner is TypeReferenceExpression enclosingDiscriminatorTypeReferenceExpression
                    ? enclosingDiscriminatorTypeReferenceExpression.Type
                    : null;

            List<ValueExpression> initializationExpressions = [];
            foreach (ParameterProvider parameter in serializationConstructor?.Signature.Parameters ?? [])
            {
                initializationExpressions.Add(
                    parameter.Type == discriminatorType && modelProvider?.DiscriminatorValueExpression is not null
                        ? modelProvider.DiscriminatorValueExpression!
                        : parameter.Type.IsValueType
                            ? Default
                            : Null);
            }

            ConstructorSignature updatedSignature = new(
                generatedDefaultConstructor.Signature.Type,
                generatedDefaultConstructor.Signature.Description,
                generatedDefaultConstructor.Signature.Modifiers,
                generatedDefaultConstructor.Signature.Parameters,
                generatedDefaultConstructor.Signature.Attributes,
                new(false, initializationExpressions));
            generatedDefaultConstructor.Update(signature: updatedSignature, bodyStatements: new MethodBodyStatements([]));
        }

        return type;
    }

    private static bool TryUpdateConstructorForCollectionInitialization(ConstructorProvider constructor)
    {
        IEnumerable<ParameterProvider> eligibleParameters
            = constructor?.Signature.Parameters
                .Where(parameter => parameter?.Type?.IsValueType == false
                        && parameter?.Name != "additionalBinaryDataProperties"
                        && (parameter?.Type?.IsList == true
                            || parameter?.Type?.IsCollection == true
                            || parameter?.Type?.IsDictionary == true
                            || parameter?.Type?.BaseType?.Name?.Contains("Collection") == true)) ?? [];

        if (eligibleParameters.Any() != true)
        {
            return false;
        }

        List<MethodBodyStatement> bodyStatements
            = constructor?.BodyStatements?.ToList() ?? [];

        if (TryUpdateStatementsForParameters(bodyStatements, eligibleParameters))
        {
            bodyStatements.Insert(0, CommentStatement);
            constructor?.Update(bodyStatements: bodyStatements);
            return true;
        }

        return false;
    }

    private static bool TryUpdateStatementsForParameters(
        List<MethodBodyStatement> statements,
        IEnumerable<ParameterProvider> parameters)
    {
        bool changed = false;

        for (int i = 0; i < statements.Count; i++)
        {
            changed |= TryUpdateStatementForParameters(
                statements[i],
                parameters,
                out MethodBodyStatement handledStatement);
            statements[i] = handledStatement;
        }

        return changed;
    }

    private static bool TryUpdateStatementForParameters(
        MethodBodyStatement originalStatement,
        IEnumerable<ParameterProvider> parameters,
        out MethodBodyStatement handledStatement)
    {
        if (originalStatement is ExpressionStatement expressionStatement
            && expressionStatement.Expression is AssignmentExpression assignmentExpression)
        {
            foreach (ParameterProvider parameter in parameters)
            {
                if (TryUpdateStatementForParameter(
                    assignmentExpression,
                    parameter,
                    out MethodBodyStatement? updatedStatement)
                        && updatedStatement is not null)
                {
                    handledStatement = updatedStatement;
                    return true;
                }
            }
        }
        handledStatement = originalStatement;
        return false;
    }

    private static bool TryUpdateStatementForParameter(
        AssignmentExpression assignmentExpression,
        ParameterProvider parameter,
        out MethodBodyStatement? handledStatement)
    {
        if (assignmentExpression.Value.ToDisplayString() == parameter.Name)
        {
            ValueExpression nullFallbackExpression = parameter.Type.IsList || parameter.Type.IsDictionary
                ? New.Instance(parameter.Type.PropertyInitializationType)
                : New.Instance(parameter.Type);
            ValueExpression coalescedValueExpression = assignmentExpression.Value
                .NullCoalesce(nullFallbackExpression);
            handledStatement = assignmentExpression.Variable
                .Assign(coalescedValueExpression)
                .Terminate();
            return true;
        }
        handledStatement = null;
        return false;
    }
}