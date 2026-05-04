using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.Snippets;
using Microsoft.TypeSpec.Generator.Statements;

namespace OpenAILibraryPlugin.Visitors
{
    /// <summary>
    /// A visitor to add the <see cref="ExperimentalAttribute"/> to types, properties, and methods that are not stable.
    /// </summary>
    public class ExperimentalAttributeVisitor : ScmLibraryVisitor
    {
        private const string _realtimeNamespace = "OpenAI.Realtime";
        private static readonly AttributeStatement _experimental001Attribute = new(typeof(ExperimentalAttribute), Snippet.Literal("OPENAI001"));
        private static readonly AttributeStatement _experimental002Attribute = new(typeof(ExperimentalAttribute), Snippet.Literal("OPENAI002"));
        private static readonly AttributeStatement _experimentalCUA001Attribute = new(typeof(ExperimentalAttribute), Snippet.Literal("OPENAICUA001"));

        // Stable sets loaded from the embedded ga-apis.yaml resource
        private static readonly HashSet<string> _stableClasses;
        private static readonly HashSet<string> _stableProperties;
        private static readonly HashSet<string> _stableMethods;

        static ExperimentalAttributeVisitor()
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ga-apis.yaml")
                ?? throw new InvalidOperationException("Embedded resource 'ga-apis.yaml' not found.");
            using StreamReader reader = new(stream);

            _stableClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _stableProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _stableMethods = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            HashSet<string>? current = null;
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith("#"))
                    continue;

                if (trimmed == "stableClasses:")
                    current = _stableClasses;
                else if (trimmed == "stableProperties:")
                    current = _stableProperties;
                else if (trimmed == "stableMethods:")
                    current = _stableMethods;
                else if (trimmed.StartsWith("- ") && current != null)
                    current.Add(trimmed.Substring(2).Trim());
            }
        }

        private static readonly HashSet<string> _OPENAICUA001AttributeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "ComputerCallAction",
            "ComputerCallActionKind",
            "ComputerCallActionMouseButton",
            "ComputerCallOutputResponseItem",
            "ComputerCallOutputStatus",
            "ComputerCallResponseItem",
            "ComputerCallSafetyCheck",
            "ComputerCallStatus",
            "ComputerCallOutput",
            "ComputerToolEnvironment",
        };

        protected override PropertyProvider? VisitProperty(PropertyProvider property)
        {
            // Skip properties that are already marked as experimental
            if (property.Attributes.Any(attr => attr.Type.Equals(typeof(ExperimentalAttribute))))
            {
                return base.VisitProperty(property);
            }

            // Skip properties that are not public or are in non-stable classes
            if ((!property.Modifiers.HasFlag(MethodSignatureModifiers.Public) &&
                    !property.Modifiers.HasFlag(MethodSignatureModifiers.Protected)) ||
                !_stableClasses.Contains($"{property.EnclosingType.Type.Namespace}.{property.EnclosingType.Name}"))
            {
                return base.VisitProperty(property);
            }

            if (!_stableProperties.Contains($"{property.EnclosingType.Name}.{property.Name}"))
            {
                property.Update(
                    attributes: [.. property.Attributes,
                        property.EnclosingType.Type.Namespace.StartsWith(_realtimeNamespace) ? _experimental002Attribute : _experimental001Attribute]);

                return property;
            }

            return base.VisitProperty(property);
        }

        protected override MethodProvider? VisitMethod(MethodProvider methodProvider)
        {
            // Skip methods that are not public or are in non-stable classes
            if ((!methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Public) &&
                    !methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Protected)) ||
                !_stableClasses.Contains($"{methodProvider.EnclosingType.Type.Namespace}.{methodProvider.EnclosingType.Name}"))
            {
                return base.VisitMethod(methodProvider);
            }

            string lookupName = methodProvider.Signature.Parameters.Count switch
            {
                0 => $"{methodProvider.Signature.Name}",
                1 => $"{methodProvider.Signature.Name}|{methodProvider.Signature.Parameters[0].Type.Name}",
                _ => $"{methodProvider.Signature.Name}|{string.Join("|", methodProvider.Signature.Parameters.Select(p => p.Type.Name))}"
            };

            // Generate a lookup name based on method signature
            string operatorPrefix = "operator ";
            bool isOperator = methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Operator);
            bool isImplicit = methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Implicit);
            bool isExplicit = methodProvider.Signature.Modifiers.HasFlag(MethodSignatureModifiers.Explicit);
            lookupName = $"{methodProvider.EnclosingType.Name}.{(isOperator ? operatorPrefix : "")}{(isImplicit ? $"implicit {methodProvider.EnclosingType.Name}" : "")}{lookupName}";

            if (!_stableMethods.Contains(lookupName))
            {
                methodProvider.Signature.Update(
                    attributes: [.. methodProvider.Signature.Attributes,
                        methodProvider.EnclosingType.Type.Namespace.StartsWith(_realtimeNamespace) || (methodProvider.Signature.ReturnType?.Namespace.StartsWith(_realtimeNamespace) ?? false) ?
                            _experimental002Attribute :
                            _experimental001Attribute]);

                return methodProvider;
            }

            return base.VisitMethod(methodProvider);
        }

        // Tracks which (Namespace.Name) pairs have already been decorated in the
        // current emit, so multiple TypeProviders that emit the same partial class
        // (e.g., a model and its companion serialization partial) don't produce
        // duplicate [Experimental] attributes.
        private readonly HashSet<string> _attributedTypes = new(StringComparer.Ordinal);

        protected override TypeProvider? VisitType(TypeProvider type)
        {
            // Decorate any public/protected generated type that isn't in the stable
            // set. The provider-kind allow-list previously used here (ClientProvider,
            // ModelProvider, ClientOptionsProvider, EnumProvider) silently skipped
            // other generated public types -- e.g., the ModelReaderWriterContext
            // partials such as OpenAIResponsesContext -- leaving them un-attributed.
            // Visibility plus the stable-list check is sufficient to gate this.
            if ((type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Public) ||
                    type.DeclarationModifiers.HasFlag(TypeSignatureModifiers.Protected)) &&
                !_stableClasses.Contains($"{type.Type.Namespace}.{type.Name}") &&
                !type.Attributes.Any(attr => attr.Type.Equals(typeof(ExperimentalAttribute))) &&
                _attributedTypes.Add($"{type.Type.Namespace}.{type.Name}"))
            {
                AttributeStatement experimentalAttribute = type.Type.Namespace switch
                {
                    _ when type.Type.Namespace.StartsWith(_realtimeNamespace) => _experimental002Attribute,
                    _ when _OPENAICUA001AttributeTypes.Contains(type.Name) => _experimentalCUA001Attribute,
                    _ => _experimental001Attribute
                };
                type.Update(
                    attributes: [.. type.Attributes,
                        experimentalAttribute]);

                return type;
            }

            return base.VisitType(type);
        }
    }
}
