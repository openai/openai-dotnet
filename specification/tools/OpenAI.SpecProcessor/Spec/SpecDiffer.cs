using YamlDotNet.RepresentationModel;

namespace OpenAI.SpecProcessor.Spec;

/// <summary> Computes structural differences between two feature specs. </summary>
public static class SpecDiffer
{
    // Keys that hold documentation content, not structural data.

    private static readonly HashSet<string> _descriptionKeys = new(StringComparer.Ordinal)
    {
        "description", "summary", "title", "example", "x-oaiMeta"
    };

    // Keys within an operation that hold structural data worth diffing.

    private static readonly HashSet<string> _structuralOperationKeys = new(StringComparer.Ordinal)
    {
        "operationId", "parameters", "requestBody", "responses", "tags", "security"
    };

    /// <summary>
    /// Computes the diff between an old and new feature spec.
    /// If oldSpec is null, all content is treated as "Added".
    /// </summary>
    /// <param name="featureName"> The name of the feature being compared. </param>
    /// <param name="oldSpec"> The previous version of the spec, or null for initial baseline. </param>
    /// <param name="newSpec"> The new version of the spec. </param>
    /// <param name="includeDescriptions"> When true, description and summary changes are included. </param>
    /// <returns> A <see cref="FeatureDiff"/> summarizing all detected changes. </returns>
    public static FeatureDiff Diff(string featureName, SpecDocument? oldSpec, SpecDocument newSpec, bool includeDescriptions = false)
    {
        var changes = new List<SpecChange>();

        int pathsAdded = 0, pathsRemoved = 0;
        int opsAdded = 0, opsRemoved = 0, opsChanged = 0;
        int schemasAdded = 0, schemasRemoved = 0, schemasChanged = 0;

        // Compare paths between old and new specs.

        var oldPaths = oldSpec?.PathKeys.ToHashSet(StringComparer.Ordinal) ?? [];
        var newPaths = newSpec.PathKeys.ToHashSet(StringComparer.Ordinal);

        foreach (var path in newPaths.Except(oldPaths).OrderBy(p => p, StringComparer.Ordinal))
        {
            changes.Add(new SpecChange { Type = ChangeType.Added, Category = "path", Path = path });
            pathsAdded++;

            // All operations under this path are new.

            if (newSpec.Paths?.Children.TryGetValue(new YamlScalarNode(path), out var pathNode) == true
                && pathNode is YamlMappingNode pathItem)
            {
                foreach (var (method, op) in SpecDocument.GetOperations(pathItem))
                {
                    var opId = GetOperationId(op) ?? method.ToUpper();
                    changes.Add(new SpecChange
                    {
                        Type = ChangeType.Added,
                        Category = "operation",
                        Path = $"{path}.{method}",
                        NewValue = opId
                    });
                    opsAdded++;
                }
            }
        }

        foreach (var path in oldPaths.Except(newPaths).OrderBy(p => p, StringComparer.Ordinal))
        {
            changes.Add(new SpecChange { Type = ChangeType.Removed, Category = "path", Path = path });
            pathsRemoved++;

            if (oldSpec?.Paths?.Children.TryGetValue(new YamlScalarNode(path), out var pathNode) == true
                && pathNode is YamlMappingNode pathItem)
            {
                foreach (var (method, op) in SpecDocument.GetOperations(pathItem))
                {
                    var opId = GetOperationId(op) ?? method.ToUpper();
                    changes.Add(new SpecChange
                    {
                        Type = ChangeType.Removed,
                        Category = "operation",
                        Path = $"{path}.{method}",
                        OldValue = opId
                    });
                    opsRemoved++;
                }
            }
        }

        // Compare operations on shared paths.

        foreach (var path in oldPaths.Intersect(newPaths).OrderBy(p => p, StringComparer.Ordinal))
        {
            var oldPathItem = GetPathItem(oldSpec!, path);
            var newPathItem = GetPathItem(newSpec, path);

            if (oldPathItem == null || newPathItem == null)
            {
                continue;
            }

            CompareOperations(path, oldPathItem, newPathItem, changes, includeDescriptions, ref opsAdded, ref opsRemoved, ref opsChanged);
        }

        // Compare schemas between old and new specs.

        var oldSchemas = oldSpec?.SchemaNames.ToHashSet(StringComparer.Ordinal) ?? [];
        var newSchemas = newSpec.SchemaNames.ToHashSet(StringComparer.Ordinal);

        foreach (var name in newSchemas.Except(oldSchemas).OrderBy(n => n, StringComparer.Ordinal))
        {
            var schema = GetSchema(newSpec, name);
            var info = schema != null ? ExtractSchemaInfo(name, schema) : null;

            changes.Add(new SpecChange
            {
                Type = ChangeType.Added,
                Category = "schema",
                Path = name,
                Detail = info?.Type,
                Schema = info
            });
            schemasAdded++;
        }

        foreach (var name in oldSchemas.Except(newSchemas).OrderBy(n => n, StringComparer.Ordinal))
        {
            var oldSchemaNode = GetSchema(oldSpec!, name);
            var oldInfo = oldSchemaNode != null ? ExtractSchemaInfo(name, oldSchemaNode) : null;

            changes.Add(new SpecChange
            {
                Type = ChangeType.Removed,
                Category = "schema",
                Path = name,
                Detail = GetSchemaType(oldSpec!, name),
                Schema = oldInfo
            });
            schemasRemoved++;
        }

        // Compare shared schemas for property-level changes.

        foreach (var name in oldSchemas.Intersect(newSchemas).OrderBy(n => n, StringComparer.Ordinal))
        {
            var oldSchema = GetSchema(oldSpec!, name);
            var newSchema = GetSchema(newSpec, name);

            if (oldSchema == null || newSchema == null)
            {
                continue;
            }

            var propChanges = CompareSchemaStructure(name, oldSchema, newSchema, includeDescriptions);

            if (propChanges.Count > 0)
            {
                var newInfo = ExtractSchemaInfo(name, newSchema);

                schemasChanged++;
                changes.Add(new SpecChange
                {
                    Type = ChangeType.Changed,
                    Category = "schema",
                    Path = name,
                    Detail = GetSchemaType(newSpec, name),
                    PropertyChanges = propChanges,
                    Schema = newInfo
                });
            }
        }

        // Detect schema renames: removed schemas that closely match an added schema by property set.

        DetectSchemaRenames(changes, oldSpec, newSpec);

        var schemasRenamed = changes.Count(c => c.Category == "schema-rename");

        // Detect possible duplicate schemas among the changes (added schemas that are very similar to each other).

        var duplicates = DetectDuplicateSchemas(changes, newSpec);

        // Detect structurally equivalent schemas within the new spec file.

        var equivalentSchemas = DetectEquivalentSchemas(newSpec);

        // Capture request/response schemas for added and changed operations.

        CaptureOperationSchemas(changes, newSpec);

        // Detect duplicate operations (same request/response schema refs).

        var duplicateOperations = DetectDuplicateOperations(newSpec);

        // Detect spec anomalies.

        var anomalies = DetectAnomalies(newSpec);

        return new FeatureDiff(
            featureName, changes,
            pathsAdded, pathsRemoved,
            opsAdded, opsRemoved, opsChanged,
            schemasAdded, schemasRemoved, schemasChanged,
            schemasRenamed, duplicates, equivalentSchemas,
            duplicateOperations, anomalies);
    }

    /// <summary> Extracts a SchemaInfo skeleton for a named schema from a spec document. </summary>
    public static SchemaInfo? GetSchemaInfo(SpecDocument spec, string schemaName)
    {
        var schema = GetSchema(spec, schemaName);
        return schema != null ? ExtractSchemaInfo(schemaName, schema) : null;
    }

    private static void CompareOperations(
        string path,
        YamlMappingNode oldPathItem,
        YamlMappingNode newPathItem,
        List<SpecChange> changes,
        bool includeDescriptions,
        ref int opsAdded,
        ref int opsRemoved,
        ref int opsChanged)
    {
        var oldOps = SpecDocument.GetOperations(oldPathItem).ToDictionary(o => o.Method, o => o.Operation);
        var newOps = SpecDocument.GetOperations(newPathItem).ToDictionary(o => o.Method, o => o.Operation);

        foreach (var method in newOps.Keys.Except(oldOps.Keys))
        {
            var opId = GetOperationId(newOps[method]) ?? method.ToUpper();
            changes.Add(new SpecChange
            {
                Type = ChangeType.Added,
                Category = "operation",
                Path = $"{path}.{method}",
                NewValue = opId
            });
            opsAdded++;
        }

        foreach (var method in oldOps.Keys.Except(newOps.Keys))
        {
            var opId = GetOperationId(oldOps[method]) ?? method.ToUpper();
            changes.Add(new SpecChange
            {
                Type = ChangeType.Removed,
                Category = "operation",
                Path = $"{path}.{method}",
                OldValue = opId
            });
            opsRemoved++;
        }

        foreach (var method in oldOps.Keys.Intersect(newOps.Keys))
        {
            var propChanges = CompareOperationStructure($"{path}.{method}", oldOps[method], newOps[method], includeDescriptions);

            if (propChanges.Count > 0)
            {
                opsChanged++;
                var opId = GetOperationId(newOps[method]) ?? method.ToUpper();
                changes.Add(new SpecChange
                {
                    Type = ChangeType.Changed,
                    Category = "operation",
                    Path = $"{path}.{method}",
                    NewValue = opId,
                    PropertyChanges = propChanges
                });
            }
        }
    }

    // Compares structural elements of two operations, ignoring description/summary by default.
    private static List<PropertyChange> CompareOperationStructure(
        string basePath,
        YamlMappingNode oldOp,
        YamlMappingNode newOp,
        bool includeDescriptions)
    {
        var propChanges = new List<PropertyChange>();
        var allKeys = oldOp.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!)
            .Union(newOp.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!))
            .ToHashSet(StringComparer.Ordinal);

        foreach (var key in allKeys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (!includeDescriptions && _descriptionKeys.Contains(key))
            {
                continue;
            }

            if (!_structuralOperationKeys.Contains(key) && !_descriptionKeys.Contains(key))
            {
                continue;
            }

            var hasOld = oldOp.Children.TryGetValue(new YamlScalarNode(key), out var oldChild);
            var hasNew = newOp.Children.TryGetValue(new YamlScalarNode(key), out var newChild);

            if (hasNew && !hasOld)
            {
                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Added,
                    Name = key,
                    NewType = DescribeNodeType(newChild!)
                });
            }
            else if (hasOld && !hasNew)
            {
                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Removed,
                    Name = key,
                    OldType = DescribeNodeType(oldChild!)
                });
            }
            else if (hasOld && hasNew && !NodesEqual(oldChild!, newChild!))
            {
                if (key == "parameters")
                {
                    propChanges.AddRange(CompareParameters(oldChild!, newChild!));
                }
                else if (key == "requestBody")
                {
                    propChanges.AddRange(CompareRequestBody(oldChild!, newChild!));
                }
                else if (key == "responses")
                {
                    propChanges.AddRange(CompareResponses(oldChild!, newChild!));
                }
                else
                {
                    propChanges.Add(new PropertyChange
                    {
                        Type = ChangeType.Changed,
                        Name = key,
                        OldType = DescribeNodeType(oldChild!),
                        NewType = DescribeNodeType(newChild!)
                    });
                }
            }
        }

        return propChanges;
    }

    // Compares two parameter arrays, identifying added/removed/changed parameters by name.
    private static List<PropertyChange> CompareParameters(YamlNode oldNode, YamlNode newNode)
    {
        var changes = new List<PropertyChange>();

        var oldParams = ExtractParameterMap(oldNode);
        var newParams = ExtractParameterMap(newNode);

        foreach (var name in newParams.Keys.Except(oldParams.Keys))
        {
            var param = newParams[name];
            changes.Add(new PropertyChange
            {
                Type = ChangeType.Added,
                Name = $"parameter:{name}",
                NewType = param.type,
                Detail = param.location != null ? $"in {param.location}" : null
            });
        }

        foreach (var name in oldParams.Keys.Except(newParams.Keys))
        {
            changes.Add(new PropertyChange
            {
                Type = ChangeType.Removed,
                Name = $"parameter:{name}",
                OldType = oldParams[name].type
            });
        }

        foreach (var name in oldParams.Keys.Intersect(newParams.Keys))
        {
            var oldP = oldParams[name];
            var newP = newParams[name];

            if (oldP.type != newP.type || oldP.required != newP.required || oldP.location != newP.location)
            {
                var details = new List<string>();

                if (oldP.type != newP.type)
                {
                    details.Add($"type: {oldP.type} → {newP.type}");
                }

                if (oldP.required != newP.required)
                {
                    details.Add($"required: {oldP.required} → {newP.required}");
                }

                if (oldP.location != newP.location)
                {
                    details.Add($"in: {oldP.location} → {newP.location}");
                }

                changes.Add(new PropertyChange
                {
                    Type = ChangeType.Changed,
                    Name = $"parameter:{name}",
                    OldType = oldP.type,
                    NewType = newP.type,
                    Detail = string.Join(", ", details)
                });
            }
        }

        return changes;
    }

    // Extracts a parameter name → (type, required, in) map from a parameters sequence.
    private static Dictionary<string, (string? type, bool required, string? location)> ExtractParameterMap(YamlNode node)
    {
        var map = new Dictionary<string, (string? type, bool required, string? location)>(StringComparer.Ordinal);

        if (node is not YamlSequenceNode seq)
        {
            return map;
        }

        foreach (var item in seq.Children.OfType<YamlMappingNode>())
        {
            var name = GetScalar(item, "name");

            if (name == null)
            {
                continue;
            }

            var type = GetSchemaTypeFromParam(item);
            var required = GetScalar(item, "required") == "true";
            var location = GetScalar(item, "in");

            map[name] = (type, required, location);
        }

        return map;
    }

    // Compares two requestBody nodes structurally.
    private static List<PropertyChange> CompareRequestBody(YamlNode oldNode, YamlNode newNode)
    {
        var changes = new List<PropertyChange>();

        if (oldNode is YamlMappingNode oldBody && newNode is YamlMappingNode newBody)
        {
            var oldRequired = GetScalar(oldBody, "required");
            var newRequired = GetScalar(newBody, "required");

            if (oldRequired != newRequired)
            {
                changes.Add(new PropertyChange
                {
                    Type = ChangeType.Changed,
                    Name = "requestBody.required",
                    OldType = oldRequired ?? "false",
                    NewType = newRequired ?? "false"
                });
            }

            // Compare the content type schemas.

            var oldContent = GetMapping(oldBody, "content");
            var newContent = GetMapping(newBody, "content");

            if (oldContent != null && newContent != null)
            {
                var oldTypes = oldContent.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet();
                var newTypes = newContent.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet();

                foreach (var ct in newTypes.Except(oldTypes))
                {
                    changes.Add(new PropertyChange
                    {
                        Type = ChangeType.Added,
                        Name = $"requestBody.content[{ct}]",
                        NewType = "content-type"
                    });
                }

                foreach (var ct in oldTypes.Except(newTypes))
                {
                    changes.Add(new PropertyChange
                    {
                        Type = ChangeType.Removed,
                        Name = $"requestBody.content[{ct}]",
                        OldType = "content-type"
                    });
                }

                // For shared content types, compare the schema ref.

                foreach (var ct in oldTypes.Intersect(newTypes))
                {
                    var oldRef = GetDeepRef(oldContent, ct);
                    var newRef = GetDeepRef(newContent, ct);

                    if (oldRef != newRef)
                    {
                        changes.Add(new PropertyChange
                        {
                            Type = ChangeType.Changed,
                            Name = $"requestBody.content[{ct}].schema",
                            OldType = oldRef ?? "(inline)",
                            NewType = newRef ?? "(inline)"
                        });
                    }
                }
            }
        }

        return changes;
    }

    // Compares response status codes and their schema refs.
    private static List<PropertyChange> CompareResponses(YamlNode oldNode, YamlNode newNode)
    {
        var changes = new List<PropertyChange>();

        if (oldNode is not YamlMappingNode oldResponses || newNode is not YamlMappingNode newResponses)
        {
            return changes;
        }

        var oldCodes = oldResponses.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet();
        var newCodes = newResponses.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet();

        foreach (var code in newCodes.Except(oldCodes))
        {
            changes.Add(new PropertyChange
            {
                Type = ChangeType.Added,
                Name = $"response:{code}",
                NewType = GetResponseSchemaRef(newResponses, code)
            });
        }

        foreach (var code in oldCodes.Except(newCodes))
        {
            changes.Add(new PropertyChange
            {
                Type = ChangeType.Removed,
                Name = $"response:{code}",
                OldType = GetResponseSchemaRef(oldResponses, code)
            });
        }

        foreach (var code in oldCodes.Intersect(newCodes))
        {
            var oldRef = GetResponseSchemaRef(oldResponses, code);
            var newRef = GetResponseSchemaRef(newResponses, code);

            if (oldRef != newRef)
            {
                changes.Add(new PropertyChange
                {
                    Type = ChangeType.Changed,
                    Name = $"response:{code}",
                    OldType = oldRef ?? "(no schema)",
                    NewType = newRef ?? "(no schema)"
                });
            }
        }

        return changes;
    }

    // Compares structural parts of two schemas: properties, required, type, enum, compositions.
    private static List<PropertyChange> CompareSchemaStructure(
        string schemaName,
        YamlMappingNode oldSchema,
        YamlMappingNode newSchema,
        bool includeDescriptions)
    {
        var propChanges = new List<PropertyChange>();

        // Compare type.

        var oldType = GetScalar(oldSchema, "type");
        var newType = GetScalar(newSchema, "type");

        if (oldType != newType)
        {
            propChanges.Add(new PropertyChange
            {
                Type = ChangeType.Changed,
                Name = "type",
                OldType = oldType ?? "(none)",
                NewType = newType ?? "(none)"
            });
        }

        // Compare enum values.

        var oldEnum = GetEnumValues(oldSchema);
        var newEnum = GetEnumValues(newSchema);

        if (oldEnum != null || newEnum != null)
        {
            var oldSet = oldEnum?.ToHashSet(StringComparer.Ordinal) ?? [];
            var newSet = newEnum?.ToHashSet(StringComparer.Ordinal) ?? [];

            foreach (var v in newSet.Except(oldSet))
            {
                propChanges.Add(new PropertyChange { Type = ChangeType.Added, Name = $"enum:{v}", NewType = "enum value" });
            }

            foreach (var v in oldSet.Except(newSet))
            {
                propChanges.Add(new PropertyChange { Type = ChangeType.Removed, Name = $"enum:{v}", OldType = "enum value" });
            }
        }

        // Compare properties.

        var oldProps = GetMapping(oldSchema, "properties");
        var newProps = GetMapping(newSchema, "properties");

        if (oldProps != null || newProps != null)
        {
            var oldRequired = GetRequiredSet(oldSchema);
            var newRequired = GetRequiredSet(newSchema);

            var oldKeys = oldProps?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet(StringComparer.Ordinal) ?? [];
            var newKeys = newProps?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet(StringComparer.Ordinal) ?? [];

            foreach (var key in newKeys.Except(oldKeys).OrderBy(k => k, StringComparer.Ordinal))
            {
                var propNode = newProps!.Children[new YamlScalarNode(key)];
                var propType = DescribePropertyType(propNode);
                var isReq = newRequired.Contains(key);
                var detailParts = new List<string>();

                if (isReq)
                {
                    detailParts.Add("required");
                }

                if (IsNullable(propNode))
                {
                    detailParts.Add("nullable");
                }

                var defaultVal = GetDefaultValue(propNode);

                if (defaultVal != null)
                {
                    detailParts.Add($"default: {defaultVal}");
                }

                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Added,
                    Name = key,
                    NewType = propType,
                    Detail = detailParts.Count > 0 ? string.Join(", ", detailParts) : null
                });
            }

            foreach (var key in oldKeys.Except(newKeys).OrderBy(k => k, StringComparer.Ordinal))
            {
                var propType = DescribePropertyType(oldProps!.Children[new YamlScalarNode(key)]);
                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Removed,
                    Name = key,
                    OldType = propType
                });
            }

            // Compare shared properties for type changes.

            foreach (var key in oldKeys.Intersect(newKeys).OrderBy(k => k, StringComparer.Ordinal))
            {
                var oldPropNode = oldProps!.Children[new YamlScalarNode(key)];
                var newPropNode = newProps!.Children[new YamlScalarNode(key)];

                var oldPropType = DescribePropertyType(oldPropNode);
                var newPropType = DescribePropertyType(newPropNode);
                var wasRequired = oldRequired.Contains(key);
                var isRequired = newRequired.Contains(key);

                var details = new List<string>();

                if (oldPropType != newPropType)
                {
                    details.Add($"type: {oldPropType} → {newPropType}");
                }

                if (wasRequired != isRequired)
                {
                    details.Add(isRequired ? "now required" : "now optional");
                }

                // Check nullable changes.

                var wasNullable = IsNullable(oldPropNode);
                var isNullable = IsNullable(newPropNode);

                if (wasNullable != isNullable)
                {
                    details.Add(isNullable ? "now nullable" : "no longer nullable");
                }

                // Check default value changes.

                var oldDefault = GetDefaultValue(oldPropNode);
                var newDefault = GetDefaultValue(newPropNode);

                if (oldDefault != newDefault)
                {
                    details.Add($"default: {oldDefault ?? "(none)"} → {newDefault ?? "(none)"}");
                }

                // Check constraint changes.

                if (oldPropNode is YamlMappingNode oldPropMap && newPropNode is YamlMappingNode newPropMap)
                {
                    foreach (var constraintKey in _constraintKeys)
                    {
                        var oldVal = GetScalar(oldPropMap, constraintKey);
                        var newVal = GetScalar(newPropMap, constraintKey);

                        if (oldVal != newVal)
                        {
                            details.Add($"{constraintKey}: {oldVal ?? "(none)"} → {newVal ?? "(none)"}");
                        }
                    }
                }

                // Check if the property subtree changed structurally (excluding descriptions).

                if (details.Count == 0 && !NodesEqualStructural(oldPropNode, newPropNode, includeDescriptions))
                {
                    details.Add(DescribeStructuralDiff(oldPropNode, newPropNode));
                }

                if (details.Count > 0)
                {
                    propChanges.Add(new PropertyChange
                    {
                        Type = ChangeType.Changed,
                        Name = key,
                        OldType = oldPropType,
                        NewType = newPropType,
                        Detail = string.Join(", ", details)
                    });
                }
            }
        }

        // Compare composition types (anyOf, oneOf, allOf).
        // Detect keyword-only changes (e.g., anyOf → oneOf with same variants) and collapse them.

        string? oldCompKeyword = null, newCompKeyword = null;
        List<string> oldCompRefs = [], newCompRefs = [];

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var oldRefs = GetCompositionRefs(oldSchema, compKey);
            var newRefs = GetCompositionRefs(newSchema, compKey);

            if (oldRefs.Count > 0)
            {
                oldCompKeyword = compKey;
                oldCompRefs = oldRefs;
            }

            if (newRefs.Count > 0)
            {
                newCompKeyword = compKey;
                newCompRefs = newRefs;
            }
        }

        if (oldCompKeyword != null || newCompKeyword != null)
        {
            var addedRefs = newCompRefs.Except(oldCompRefs).ToList();
            var removedRefs = oldCompRefs.Except(newCompRefs).ToList();
            var keywordChanged = oldCompKeyword != newCompKeyword && oldCompKeyword != null && newCompKeyword != null;

            // If only the keyword changed (anyOf → oneOf) with no variant changes, collapse to a single line.
            if (keywordChanged && addedRefs.Count == 0 && removedRefs.Count == 0)
            {
                var unchangedCount = oldCompRefs.Count;
                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Changed,
                    Name = "composition",
                    OldType = oldCompKeyword,
                    NewType = newCompKeyword,
                    Detail = $"{oldCompKeyword} → {newCompKeyword} ({unchangedCount} variants unchanged)"
                });
            }
            else
            {
                // Emit keyword change if it happened alongside variant changes.
                if (keywordChanged)
                {
                    propChanges.Add(new PropertyChange
                    {
                        Type = ChangeType.Changed,
                        Name = "composition",
                        OldType = oldCompKeyword,
                        NewType = newCompKeyword,
                        Detail = $"{oldCompKeyword} → {newCompKeyword}"
                    });
                }

                var effectiveKey = newCompKeyword ?? oldCompKeyword ?? "oneOf";

                foreach (var r in addedRefs)
                {
                    propChanges.Add(new PropertyChange { Type = ChangeType.Added, Name = $"{effectiveKey}:{r}", NewType = "type ref" });
                }

                foreach (var r in removedRefs)
                {
                    propChanges.Add(new PropertyChange { Type = ChangeType.Removed, Name = $"{effectiveKey}:{r}", OldType = "type ref" });
                }
            }

            // Report the discriminator only when it actually changed. Emitting it whenever one is
            // present marks every discriminated schema as changed on every run, which buries real
            // changes in recurring noise.

            var oldDiscriminatorNode = GetMapping(oldSchema, "discriminator");
            var newDiscriminatorNode = GetMapping(newSchema, "discriminator");
            var oldDiscriminator = oldDiscriminatorNode == null ? null : GetScalar(oldDiscriminatorNode, "propertyName");
            var newDiscriminator = newDiscriminatorNode == null ? null : GetScalar(newDiscriminatorNode, "propertyName");

            if (!string.Equals(oldDiscriminator, newDiscriminator, StringComparison.Ordinal))
            {
                propChanges.Add(new PropertyChange
                {
                    Type = ChangeType.Changed,
                    Name = "discriminator",
                    OldType = oldDiscriminator,
                    NewType = newDiscriminator,
                    Detail = (oldDiscriminator, newDiscriminator) switch
                    {
                        (null, not null) => $"added, propertyName: \"{newDiscriminator}\"",
                        (not null, null) => $"removed, was propertyName: \"{oldDiscriminator}\"",
                        _ => $"propertyName: \"{oldDiscriminator}\" → \"{newDiscriminator}\""
                    }
                });
            }
        }

        return propChanges;
    }

    // Produces a descriptive string for structural differences when type descriptions are identical.
    // This replaces the opaque "structure changed" fallback with specific details about what changed.
    private static string DescribeStructuralDiff(YamlNode oldNode, YamlNode newNode)
    {
        if (oldNode is not YamlMappingNode oldMap || newNode is not YamlMappingNode newMap)
        {
            return "structure changed";
        }

        var details = new List<string>();

        // Check nullable changes.
        var wasNullable = IsNullable(oldNode);
        var isNullable = IsNullable(newNode);

        if (wasNullable != isNullable)
        {
            details.Add(isNullable ? "now nullable" : "no longer nullable");
        }

        // Check default changes.
        var oldDefault = GetDefaultValue(oldNode);
        var newDefault = GetDefaultValue(newNode);

        if (oldDefault != newDefault)
        {
            details.Add($"default: {oldDefault ?? "(none)"} → {newDefault ?? "(none)"}");
        }

        // Check format changes.
        var oldFormat = GetScalar(oldMap, "format");
        var newFormat = GetScalar(newMap, "format");

        if (oldFormat != newFormat)
        {
            details.Add($"format: {oldFormat ?? "(none)"} → {newFormat ?? "(none)"}");
        }

        // Check constraint changes.
        foreach (var constraintKey in _constraintKeys)
        {
            var oldVal = GetScalar(oldMap, constraintKey);
            var newVal = GetScalar(newMap, constraintKey);

            if (oldVal != newVal)
            {
                details.Add($"{constraintKey}: {oldVal ?? "(none)"} → {newVal ?? "(none)"}");
            }
        }

        // Check enum value changes.
        var oldEnums = GetEnumValues(oldMap) ?? [];
        var newEnums = GetEnumValues(newMap) ?? [];

        if (oldEnums.Count > 0 || newEnums.Count > 0)
        {
            var addedEnums = newEnums.Except(oldEnums).ToList();
            var removedEnums = oldEnums.Except(newEnums).ToList();

            if (addedEnums.Count > 0)
            {
                details.Add($"enum added: {string.Join(", ", addedEnums)}");
            }

            if (removedEnums.Count > 0)
            {
                details.Add($"enum removed: {string.Join(", ", removedEnums)}");
            }
        }

        // Check additionalProperties changes.
        var oldAdditional = GetScalar(oldMap, "additionalProperties");
        var newAdditional = GetScalar(newMap, "additionalProperties");

        if (oldAdditional != newAdditional)
        {
            details.Add($"additionalProperties: {oldAdditional ?? "(none)"} → {newAdditional ?? "(none)"}");
        }

        // Check nested composition changes.
        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var oldRefs = GetCompositionRefs(oldMap, compKey);
            var newRefs = GetCompositionRefs(newMap, compKey);

            if (oldRefs.Count > 0 || newRefs.Count > 0)
            {
                var added = newRefs.Except(oldRefs).ToList();
                var removed = oldRefs.Except(newRefs).ToList();

                if (added.Count > 0)
                {
                    details.Add($"{compKey} added: {string.Join(", ", added)}");
                }

                if (removed.Count > 0)
                {
                    details.Add($"{compKey} removed: {string.Join(", ", removed)}");
                }
            }
        }

        // Check for keyword switch (e.g., anyOf ↔ oneOf) at the nested level.
        string? oldCompKey = null, newCompKey = null;

        foreach (var ck in new[] { "anyOf", "oneOf", "allOf" })
        {
            if (oldMap.Children.ContainsKey(new YamlScalarNode(ck)))
            {
                oldCompKey = ck;
            }

            if (newMap.Children.ContainsKey(new YamlScalarNode(ck)))
            {
                newCompKey = ck;
            }
        }

        if (oldCompKey != null && newCompKey != null && oldCompKey != newCompKey)
        {
            var oldRefs = GetCompositionRefs(oldMap, oldCompKey);
            var newRefs = GetCompositionRefs(newMap, newCompKey);

            if (oldRefs.SequenceEqual(newRefs) && !details.Any(d => d.Contains("added") || d.Contains("removed")))
            {
                details.Add($"{oldCompKey} → {newCompKey}");
            }
        }

        // Recurse into composition variant children to detect nested structural changes.
        // This catches cases like anyOf: [{anyOf: [...]}, null] → anyOf: [{oneOf: [...]}, null]
        // where the top-level keyword is the same but inner variants changed.

        var effectiveOldKey = oldCompKey ?? (new[] { "anyOf", "oneOf", "allOf" }).FirstOrDefault(k => oldMap.Children.ContainsKey(new YamlScalarNode(k)));
        var effectiveNewKey = newCompKey ?? (new[] { "anyOf", "oneOf", "allOf" }).FirstOrDefault(k => newMap.Children.ContainsKey(new YamlScalarNode(k)));

        if (effectiveOldKey != null && effectiveNewKey != null
            && oldMap.Children[new YamlScalarNode(effectiveOldKey)] is YamlSequenceNode oldVariants
            && newMap.Children[new YamlScalarNode(effectiveNewKey)] is YamlSequenceNode newVariants)
        {
            var count = Math.Min(oldVariants.Children.Count, newVariants.Children.Count);

            for (int i = 0; i < count; i++)
            {
                if (oldVariants.Children[i] is YamlMappingNode oldVariant
                    && newVariants.Children[i] is YamlMappingNode newVariant
                    && !NodesEqualStructural(oldVariant, newVariant, false))
                {
                    var variantDetail = DescribeStructuralDiff(oldVariant, newVariant);

                    if (variantDetail != "structure changed")
                    {
                        details.Add($"variant[{i}]: {variantDetail}");
                    }
                }
            }
        }

        // Check nested properties changes.
        var oldProps = GetMapping(oldMap, "properties");
        var newProps = GetMapping(newMap, "properties");

        if (oldProps != null || newProps != null)
        {
            var oldKeys = oldProps?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet(StringComparer.Ordinal) ?? [];
            var newKeys = newProps?.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet(StringComparer.Ordinal) ?? [];

            var addedProps = newKeys.Except(oldKeys).ToList();
            var removedProps = oldKeys.Except(newKeys).ToList();

            if (addedProps.Count > 0)
            {
                details.Add($"properties added: {string.Join(", ", addedProps)}");
            }

            if (removedProps.Count > 0)
            {
                details.Add($"properties removed: {string.Join(", ", removedProps)}");
            }

            // Recurse into shared properties that changed structurally.
            if (oldProps != null && newProps != null)
            {
                foreach (var key in oldKeys.Intersect(newKeys).OrderBy(k => k, StringComparer.Ordinal))
                {
                    var oldChild = oldProps.Children[new YamlScalarNode(key)];
                    var newChild = newProps.Children[new YamlScalarNode(key)];

                    if (!NodesEqualStructural(oldChild, newChild, false))
                    {
                        var childDetail = DescribeStructuralDiff(oldChild, newChild);
                        details.Add($"{key}: {childDetail}");
                    }
                }
            }
        }

        // Check nested items type change (for arrays).
        var oldItems = GetMapping(oldMap, "items");
        var newItems = GetMapping(newMap, "items");

        if (oldItems != null && newItems != null)
        {
            var oldItemType = DescribePropertyType(oldItems);
            var newItemType = DescribePropertyType(newItems);

            if (oldItemType != newItemType)
            {
                details.Add($"items: {oldItemType} → {newItemType}");
            }
        }

        // Check required array changes on nested schemas.
        var oldRequired = GetRequiredSet(oldMap);
        var newRequired = GetRequiredSet(newMap);

        if (!oldRequired.SetEquals(newRequired))
        {
            var nowReq = newRequired.Except(oldRequired).ToList();
            var nowOpt = oldRequired.Except(newRequired).ToList();

            if (nowReq.Count > 0)
            {
                details.Add($"now required: {string.Join(", ", nowReq)}");
            }

            if (nowOpt.Count > 0)
            {
                details.Add($"now optional: {string.Join(", ", nowOpt)}");
            }
        }

        return details.Count > 0 ? string.Join(", ", details) : "structure changed";
    }

    // Detects likely schema renames by matching removed schemas against added schemas using
    // property name similarity or common naming patterns (e.g., Foo → FooParam, FooResource).
    private static void DetectSchemaRenames(List<SpecChange> changes, SpecDocument? oldSpec, SpecDocument newSpec)
    {
        if (oldSpec == null)
        {
            return;
        }

        var removedSchemas = changes
            .Where(c => c.Category == "schema" && c.Type == ChangeType.Removed)
            .Select(c => c.Path)
            .ToList();

        var addedSchemas = changes
            .Where(c => c.Category == "schema" && c.Type == ChangeType.Added)
            .Select(c => c.Path)
            .ToList();

        if (removedSchemas.Count == 0 || addedSchemas.Count == 0)
        {
            return;
        }

        // Common suffixes that indicate a rename rather than a new schema.
        string[] renameSuffixes = ["Param", "Resource", "Request", "Response", "Body", "Item", "Object"];

        var matched = new HashSet<string>(StringComparer.Ordinal);
        var renames = new List<(string OldName, string NewName)>();

        foreach (var oldName in removedSchemas)
        {
            string? bestMatch = null;

            // First try suffix-based matching (Drag → DragParam).
            foreach (var suffix in renameSuffixes)
            {
                var candidate = oldName + suffix;

                if (addedSchemas.Contains(candidate) && !matched.Contains(candidate))
                {
                    bestMatch = candidate;
                    break;
                }

                // Also try removing a suffix (FooParam → Foo).
                if (oldName.EndsWith(suffix, StringComparison.Ordinal))
                {
                    candidate = oldName[..^suffix.Length];

                    if (addedSchemas.Contains(candidate) && !matched.Contains(candidate))
                    {
                        bestMatch = candidate;
                        break;
                    }
                }
            }

            // If no suffix match, try property-based similarity.
            if (bestMatch == null)
            {
                var oldSchema = GetSchema(oldSpec, oldName);
                if (oldSchema == null) continue;
                var oldProps = GetPropertyNames(oldSchema);
                if (oldProps.Count == 0) continue;

                double bestScore = 0;

                foreach (var newName in addedSchemas)
                {
                    if (matched.Contains(newName)) continue;

                    var newSchema = GetSchema(newSpec, newName);
                    if (newSchema == null) continue;
                    var newProps = GetPropertyNames(newSchema);
                    if (newProps.Count == 0) continue;

                    var intersection = oldProps.Intersect(newProps, StringComparer.Ordinal).Count();
                    var union = oldProps.Union(newProps, StringComparer.Ordinal).Count();
                    var similarity = (double)intersection / union;

                    if (similarity > bestScore && similarity >= 0.7)
                    {
                        bestScore = similarity;
                        bestMatch = newName;
                    }
                }
            }

            if (bestMatch != null)
            {
                matched.Add(bestMatch);
                renames.Add((oldName, bestMatch));
            }
        }

        // Add rename entries to the changes list.
        foreach (var (oldName, newName) in renames.OrderBy(r => r.OldName, StringComparer.Ordinal))
        {
            changes.Add(new SpecChange
            {
                Type = ChangeType.Changed,
                Category = "schema-rename",
                Path = newName,
                OldValue = oldName,
                NewValue = newName,
                Detail = $"{oldName} → {newName}"
            });
        }
    }

    private static HashSet<string> GetPropertyNames(YamlMappingNode schema)
    {
        var props = GetMapping(schema, "properties");

        if (props == null)
        {
            return [];
        }

        return props.Children.Keys.OfType<YamlScalarNode>().Select(k => k.Value!).ToHashSet(StringComparer.Ordinal);
    }

    /// <summary> Extracts a schema skeleton showing its structure for display. </summary>
    private static SchemaInfo ExtractSchemaInfo(string name, YamlMappingNode schema)
    {
        var type = GetScalar(schema, "type");
        var enumValues = GetEnumValues(schema);
        var required = GetRequiredSet(schema);

        List<SchemaProperty>? properties = null;
        var propsNode = GetMapping(schema, "properties");

        if (propsNode != null)
        {
            properties = [];

            foreach (var (key, value) in propsNode.Children)
            {
                if (key is not YamlScalarNode scalarKey || scalarKey.Value == null)
                {
                    continue;
                }

                var propName = scalarKey.Value;
                var propType = DescribePropertyType(value);
                string? itemType = null;
                string? refTarget = null;

                if (value is YamlMappingNode propMap)
                {
                    itemType = GetArrayItemType(propMap);
                    refTarget = GetRefTarget(propMap);
                }

                properties.Add(new SchemaProperty
                {
                    Name = propName,
                    Type = propType,
                    IsRequired = required.Contains(propName),
                    ItemType = itemType,
                    RefTarget = refTarget
                });
            }
        }

        // Extract composition refs.

        string? compositionType = null;
        List<string>? compositionRefs = null;

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var refs = GetCompositionRefs(schema, compKey);

            if (refs.Count > 0)
            {
                compositionType = compKey;
                compositionRefs = refs;
                break;
            }
        }

        return new SchemaInfo
        {
            Name = name,
            Type = type,
            EnumValues = enumValues,
            Properties = properties,
            Required = required.Count > 0 ? required.ToList() : null,
            CompositionRefs = compositionRefs,
            CompositionType = compositionType
        };
    }

    // ── Node comparison ──────────────────────────────────────────────────

    // Compares nodes structurally, optionally skipping description keys.
    private static bool NodesEqualStructural(YamlNode a, YamlNode b, bool includeDescriptions)
    {
        if (includeDescriptions)
        {
            return NodesEqual(a, b);
        }

        if (a.GetType() != b.GetType())
        {
            return false;
        }

        return (a, b) switch
        {
            (YamlScalarNode sa, YamlScalarNode sb) => sa.Value == sb.Value,
            (YamlMappingNode ma, YamlMappingNode mb) => MappingsEqualStructural(ma, mb),
            (YamlSequenceNode sa, YamlSequenceNode sb) => SequencesEqualStructural(sa, sb),
            _ => false
        };
    }

    // Compares mappings while skipping description and vendor extension keys.
    private static bool MappingsEqualStructural(YamlMappingNode a, YamlMappingNode b)
    {
        var aFiltered = a.Children
            .Where(kv => kv.Key is not YamlScalarNode sk || !IsNonStructuralKey(sk.Value ?? ""))
            .ToList();
        var bFiltered = b.Children
            .Where(kv => kv.Key is not YamlScalarNode sk || !IsNonStructuralKey(sk.Value ?? ""))
            .ToList();

        if (aFiltered.Count != bFiltered.Count)
        {
            return false;
        }

        foreach (var (key, value) in aFiltered)
        {
            if (!b.Children.TryGetValue(key, out var bValue))
            {
                return false;
            }

            if (!NodesEqualStructural(value, bValue, false))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsNonStructuralKey(string key) =>
        _descriptionKeys.Contains(key) || key.StartsWith("x-", StringComparison.Ordinal);

    private static bool NodesEqual(YamlNode a, YamlNode b)
    {
        if (a.GetType() != b.GetType())
        {
            return false;
        }

        return (a, b) switch
        {
            (YamlScalarNode sa, YamlScalarNode sb) => sa.Value == sb.Value,
            (YamlMappingNode ma, YamlMappingNode mb) => MappingsEqual(ma, mb),
            (YamlSequenceNode sa, YamlSequenceNode sb) => SequencesEqual(sa, sb),
            _ => false
        };
    }

    private static bool MappingsEqual(YamlMappingNode a, YamlMappingNode b)
    {
        if (a.Children.Count != b.Children.Count)
        {
            return false;
        }

        foreach (var (key, value) in a.Children)
        {
            if (!b.Children.TryGetValue(key, out var bValue))
            {
                return false;
            }

            if (!NodesEqual(value, bValue))
            {
                return false;
            }
        }

        return true;
    }

    private static bool SequencesEqual(YamlSequenceNode a, YamlSequenceNode b)
    {
        if (a.Children.Count != b.Children.Count)
        {
            return false;
        }

        return a.Children.Zip(b.Children).All(pair => NodesEqual(pair.First, pair.Second));
    }

    // Compares sequences while skipping description keys in child mappings.
    private static bool SequencesEqualStructural(YamlSequenceNode a, YamlSequenceNode b)
    {
        if (a.Children.Count != b.Children.Count)
        {
            return false;
        }

        return a.Children.Zip(b.Children).All(pair => NodesEqualStructural(pair.First, pair.Second, false));
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    // Describes a property node's type in a human-readable way.
    private static string DescribePropertyType(YamlNode node)
    {
        if (node is not YamlMappingNode map)
        {
            return node is YamlScalarNode s ? s.Value ?? "unknown" : "unknown";
        }

        // Check for $ref.

        var refVal = GetScalar(map, "$ref");

        if (refVal != null)
        {
            return ExtractRefName(refVal);
        }

        var type = GetScalar(map, "type");
        var format = GetScalar(map, "format");

        // Check for enum.

        if (GetEnumValues(map) != null)
        {
            return $"{type ?? "string"} enum";
        }

        // Check for array with items.

        if (type == "array")
        {
            var itemType = GetArrayItemType(map);
            return itemType != null ? $"array<{itemType}>" : "array";
        }

        // Check for composition types.

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var refs = GetCompositionRefs(map, compKey);

            if (refs.Count > 0)
            {
                return $"{compKey}({string.Join(" | ", refs)})";
            }
        }

        // Include format for numeric types (e.g., integer(int64), number(double)).

        if (format != null && type != null)
        {
            return $"{type}({format})";
        }

        return type ?? "unknown";
    }

    // Describes a node at the operation level (for non-property structural keys).
    private static string DescribeNodeType(YamlNode node)
    {
        return node switch
        {
            YamlScalarNode s => s.Value ?? "unknown",
            YamlMappingNode m => $"object ({m.Children.Count} keys)",
            YamlSequenceNode s => $"array ({s.Children.Count} items)",
            _ => "unknown"
        };
    }

    private static string? GetOperationId(YamlMappingNode operation)
    {
        return GetScalar(operation, "operationId");
    }

    private static YamlMappingNode? GetPathItem(SpecDocument spec, string path)
    {
        return spec.Paths?.Children.TryGetValue(new YamlScalarNode(path), out var node) == true
            && node is YamlMappingNode mapping ? mapping : null;
    }

    private static YamlMappingNode? GetSchema(SpecDocument spec, string name)
    {
        return spec.Schemas?.Children.TryGetValue(new YamlScalarNode(name), out var node) == true
            && node is YamlMappingNode mapping ? mapping : null;
    }

    private static string? GetSchemaType(SpecDocument spec, string name)
    {
        var schema = GetSchema(spec, name);
        return schema != null ? GetScalar(schema, "type") : null;
    }

    private static string? GetScalar(YamlMappingNode node, string key)
    {
        return node.Children.TryGetValue(new YamlScalarNode(key), out var v) && v is YamlScalarNode s ? s.Value : null;
    }

    private static YamlMappingNode? GetMapping(YamlMappingNode node, string key)
    {
        return node.Children.TryGetValue(new YamlScalarNode(key), out var v) && v is YamlMappingNode m ? m : null;
    }

    private static HashSet<string> GetRequiredSet(YamlMappingNode schema)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode("required"), out var reqNode) || reqNode is not YamlSequenceNode reqSeq)
        {
            return [];
        }

        return reqSeq.Children.OfType<YamlScalarNode>().Select(s => s.Value!).ToHashSet(StringComparer.Ordinal);
    }

    private static List<string>? GetEnumValues(YamlMappingNode schema)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode("enum"), out var enumNode) || enumNode is not YamlSequenceNode enumSeq)
        {
            return null;
        }

        return enumSeq.Children.OfType<YamlScalarNode>().Select(s => s.Value ?? "null").ToList();
    }

    // Checks if a property or schema is nullable via OAS 3.0 `nullable: true` or OAS 3.1 `type: [T, null]`.
    private static bool IsNullable(YamlNode node)
    {
        if (node is not YamlMappingNode map)
        {
            return false;
        }

        // OAS 3.0: nullable: true
        if (GetScalar(map, "nullable") == "true")
        {
            return true;
        }

        // OAS 3.1: type is an array containing "null"
        if (map.Children.TryGetValue(new YamlScalarNode("type"), out var typeNode) && typeNode is YamlSequenceNode typeSeq)
        {
            return typeSeq.Children.OfType<YamlScalarNode>().Any(s => s.Value == "null");
        }

        return false;
    }

    // Gets the default value from a property node, if any.
    private static string? GetDefaultValue(YamlNode node)
    {
        if (node is not YamlMappingNode map)
        {
            return null;
        }

        if (!map.Children.TryGetValue(new YamlScalarNode("default"), out var defaultNode))
        {
            return null;
        }

        return defaultNode switch
        {
            YamlScalarNode s => s.Value ?? "null",
            YamlSequenceNode => "[array]",
            YamlMappingNode => "{object}",
            _ => "unknown"
        };
    }

    // Constraint keys worth tracking for property changes.
    private static readonly string[] _constraintKeys =
        ["minimum", "maximum", "minLength", "maxLength", "minItems", "maxItems", "pattern", "exclusiveMinimum", "exclusiveMaximum"];

    private static List<string> GetCompositionRefs(YamlMappingNode schema, string compKey)
    {
        if (!schema.Children.TryGetValue(new YamlScalarNode(compKey), out var compNode) || compNode is not YamlSequenceNode compSeq)
        {
            return [];
        }

        var refs = new List<string>();

        foreach (var item in compSeq.Children)
        {
            if (item is YamlMappingNode itemMap)
            {
                var refVal = GetScalar(itemMap, "$ref");

                if (refVal != null)
                {
                    refs.Add(ExtractRefName(refVal));
                }
                else
                {
                    var itemType = GetScalar(itemMap, "type");
                    refs.Add(itemType ?? "inline");
                }
            }
        }

        return refs;
    }

    private static string? GetArrayItemType(YamlMappingNode schema)
    {
        var items = GetMapping(schema, "items");

        if (items == null)
        {
            return null;
        }

        var refVal = GetScalar(items, "$ref");

        if (refVal != null)
        {
            return ExtractRefName(refVal);
        }

        return GetScalar(items, "type");
    }

    private static string? GetRefTarget(YamlMappingNode schema)
    {
        var refVal = GetScalar(schema, "$ref");
        return refVal != null ? ExtractRefName(refVal) : null;
    }

    // Extracts the type name from a $ref value like "#/components/schemas/Foo" → "Foo".
    private static string ExtractRefName(string refPath)
    {
        var lastSlash = refPath.LastIndexOf('/');
        return lastSlash >= 0 ? refPath[(lastSlash + 1)..] : refPath;
    }

    private static string? GetSchemaTypeFromParam(YamlMappingNode param)
    {
        var schema = GetMapping(param, "schema");

        if (schema == null)
        {
            return null;
        }

        var refVal = GetScalar(schema, "$ref");

        if (refVal != null)
        {
            return ExtractRefName(refVal);
        }

        return GetScalar(schema, "type");
    }

    // Gets a $ref from a content type's schema (e.g., content -> application/json -> schema -> $ref).
    private static string? GetDeepRef(YamlMappingNode contentNode, string contentType)
    {
        if (!contentNode.Children.TryGetValue(new YamlScalarNode(contentType), out var ctNode) || ctNode is not YamlMappingNode ctMap)
        {
            return null;
        }

        var schema = GetMapping(ctMap, "schema");

        if (schema == null)
        {
            return null;
        }

        var refVal = GetScalar(schema, "$ref");
        return refVal != null ? ExtractRefName(refVal) : null;
    }

    private static string? GetResponseSchemaRef(YamlMappingNode responses, string code)
    {
        if (!responses.Children.TryGetValue(new YamlScalarNode(code), out var respNode) || respNode is not YamlMappingNode respMap)
        {
            return null;
        }

        var content = GetMapping(respMap, "content");

        if (content == null)
        {
            return null;
        }

        // Look in application/json first, then any content type.

        foreach (var ct in new[] { "application/json", "text/event-stream" })
        {
            var refVal = GetDeepRef(content, ct);

            if (refVal != null)
            {
                return refVal;
            }
        }

        // Try first available content type.

        var firstCt = content.Children.Keys.OfType<YamlScalarNode>().FirstOrDefault()?.Value;

        if (firstCt != null)
        {
            return GetDeepRef(content, firstCt);
        }

        return null;
    }

    // ── Duplicate and equivalence detection ──────────────────────────────

    // Detects pairs of added schemas that are structurally very similar (likely duplicates).
    private static List<SchemaDuplicate> DetectDuplicateSchemas(List<SpecChange> changes, SpecDocument newSpec)
    {
        var duplicates = new List<SchemaDuplicate>();
        var addedSchemas = changes
            .Where(c => c.Category == "schema" && c.Type == ChangeType.Added)
            .Select(c => c.Path)
            .ToList();

        if (addedSchemas.Count < 2)
        {
            return duplicates;
        }

        // Build property maps for all added schemas.

        var propertyMaps = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var name in addedSchemas)
        {
            var schema = GetSchema(newSpec, name);

            if (schema != null)
            {
                propertyMaps[name] = GetPropertyNames(schema);
            }
        }

        // Compare each pair using Jaccard similarity.

        var matched = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < addedSchemas.Count; i++)
        {
            if (!propertyMaps.TryGetValue(addedSchemas[i], out var propsA) || propsA.Count < 2)
            {
                continue;
            }

            for (int j = i + 1; j < addedSchemas.Count; j++)
            {
                if (!propertyMaps.TryGetValue(addedSchemas[j], out var propsB) || propsB.Count < 2)
                {
                    continue;
                }

                var intersection = propsA.Intersect(propsB, StringComparer.Ordinal).ToList();
                var union = propsA.Union(propsB, StringComparer.Ordinal).Count();
                var similarity = (double)intersection.Count / union;

                if (similarity >= 0.85)
                {
                    duplicates.Add(new SchemaDuplicate(
                        addedSchemas[i],
                        addedSchemas[j],
                        similarity,
                        intersection));
                }
            }
        }

        return duplicates;
    }

    // Detects groups of schemas within a single spec file that are structurally identical.
    private static List<SchemaEquivalenceGroup> DetectEquivalentSchemas(SpecDocument spec)
    {
        var groups = new List<SchemaEquivalenceGroup>();
        var schemas = spec.Schemas;

        if (schemas == null)
        {
            return groups;
        }

        var schemaNames = spec.SchemaNames.ToList();

        // Build a signature for each schema: sorted property names + types + required status.

        var signatures = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var name in schemaNames)
        {
            var schema = GetSchema(spec, name);

            if (schema == null)
            {
                continue;
            }

            signatures[name] = BuildSchemaSignature(schema);
        }

        // Group by identical signature.

        var signatureGroups = signatures
            .GroupBy(kv => kv.Value, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in signatureGroups)
        {
            // Skip trivially simple schemas (single-property objects, bare types, empty objects,
            // simple enums with fewer than 3 values).

            var sig = group.Key;

            if (string.IsNullOrEmpty(sig) || sig == "object{}" || sig == "string" || sig == "array")
            {
                continue;
            }

            // Count commas to estimate property count; skip if only 1 property.

            if (sig.StartsWith("object{", StringComparison.Ordinal))
            {
                var inner = sig["object{".Length..^1];
                var propCount = inner.Split(',').Length;

                if (propCount < 2)
                {
                    continue;
                }
            }

            // Skip simple enums with fewer than 3 values.

            if (sig.Contains("enum[", StringComparison.Ordinal))
            {
                var enumStart = sig.IndexOf('[');
                var enumEnd = sig.IndexOf(']');

                if (enumStart >= 0 && enumEnd > enumStart)
                {
                    var enumInner = sig[(enumStart + 1)..enumEnd];
                    var valueCount = enumInner.Split(',').Length;

                    if (valueCount < 3)
                    {
                        continue;
                    }
                }
            }

            groups.Add(new SchemaEquivalenceGroup(
                group.Select(kv => kv.Key).OrderBy(n => n, StringComparer.Ordinal).ToList(),
                group.Key));
        }

        return groups;
    }

    // Builds a deterministic signature string for a schema based on its structural shape.
    private static string BuildSchemaSignature(YamlMappingNode schema)
    {
        var type = GetScalar(schema, "type") ?? "";

        // Enum signature.

        var enumValues = GetEnumValues(schema);

        if (enumValues != null)
        {
            return $"{type} enum[{string.Join(",", enumValues.OrderBy(v => v, StringComparer.Ordinal))}]";
        }

        // Composition signature.

        foreach (var compKey in new[] { "anyOf", "oneOf", "allOf" })
        {
            var refs = GetCompositionRefs(schema, compKey);

            if (refs.Count > 0)
            {
                return $"{compKey}({string.Join(",", refs.OrderBy(r => r, StringComparer.Ordinal))})";
            }
        }

        // Object signature: property names, types, and required status.

        var propsNode = GetMapping(schema, "properties");

        if (propsNode != null)
        {
            var required = GetRequiredSet(schema);
            var propSigs = new List<string>();

            foreach (var (key, value) in propsNode.Children.OrderBy(kv => ((YamlScalarNode)kv.Key).Value, StringComparer.Ordinal))
            {
                var propName = ((YamlScalarNode)key).Value!;
                var propType = DescribePropertyType(value);
                var req = required.Contains(propName) ? "*" : "";
                propSigs.Add($"{propName}{req}:{propType}");
            }

            return $"{type}{{{string.Join(",", propSigs)}}}";
        }

        return type;
    }

    // Captures resolved request/response schemas for added and changed operations.
    private static void CaptureOperationSchemas(List<SpecChange> changes, SpecDocument spec)
    {
        for (int i = 0; i < changes.Count; i++)
        {
            var change = changes[i];

            if (change.Category != "operation" || (change.Type != ChangeType.Added && change.Type != ChangeType.Changed))
            {
                continue;
            }

            // Parse path and method from the change path (e.g., "/responses.post").

            var dotIndex = change.Path.LastIndexOf('.');

            if (dotIndex < 0)
            {
                continue;
            }

            var apiPath = change.Path[..dotIndex];
            var method = change.Path[(dotIndex + 1)..];

            var pathItem = spec.Paths?.Children.TryGetValue(new YamlScalarNode(apiPath), out var pathNode) == true
                && pathNode is YamlMappingNode pathMapping ? pathMapping : null;

            if (pathItem == null)
            {
                continue;
            }

            if (!pathItem.Children.TryGetValue(new YamlScalarNode(method), out var opNode) || opNode is not YamlMappingNode operation)
            {
                continue;
            }

            // Resolve request body schema.

            SchemaInfo? requestSchema = null;
            string? requestSchemaRef = null;
            var requestBody = GetMapping(operation, "requestBody");

            if (requestBody != null)
            {
                var content = GetMapping(requestBody, "content");

                if (content != null)
                {
                    (requestSchema, requestSchemaRef) = ResolveContentSchemaWithRef(content, spec);
                }
            }

            // Resolve primary response schema (200 or 201).

            SchemaInfo? responseSchema = null;
            string? responseSchemaRef = null;
            var responses = GetMapping(operation, "responses");

            if (responses != null)
            {
                foreach (var code in new[] { "200", "201" })
                {
                    if (responses.Children.TryGetValue(new YamlScalarNode(code), out var respNode)
                        && respNode is YamlMappingNode respMap)
                    {
                        var respContent = GetMapping(respMap, "content");

                        if (respContent != null)
                        {
                            (responseSchema, responseSchemaRef) = ResolveContentSchemaWithRef(respContent, spec);

                            if (responseSchema != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            if (requestSchema != null || responseSchema != null)
            {
                changes[i] = change with
                {
                    RequestSchema = requestSchema,
                    ResponseSchema = responseSchema,
                    RequestSchemaRef = requestSchemaRef,
                    ResponseSchemaRef = responseSchemaRef
                };
            }
        }
    }

    // Resolves the schema from a content node (application/json preferred).
    private static SchemaInfo? ResolveContentSchema(YamlMappingNode content, SpecDocument spec)
    {
        var (schema, _) = ResolveContentSchemaWithRef(content, spec);
        return schema;
    }

    // Resolves the schema and its $ref name from a content node.
    private static (SchemaInfo? Schema, string? RefName) ResolveContentSchemaWithRef(YamlMappingNode content, SpecDocument spec)
    {
        foreach (var ct in new[] { "application/json", "multipart/form-data" })
        {
            if (!content.Children.TryGetValue(new YamlScalarNode(ct), out var ctNode) || ctNode is not YamlMappingNode ctMap)
            {
                continue;
            }

            var schemaNode = GetMapping(ctMap, "schema");

            if (schemaNode == null)
            {
                continue;
            }

            // If it's a $ref, resolve to the actual schema in components/schemas.

            var refVal = GetScalar(schemaNode, "$ref");

            if (refVal != null)
            {
                var refName = ExtractRefName(refVal);
                var resolved = GetSchema(spec, refName);

                if (resolved != null)
                {
                    return (ExtractSchemaInfo(refName, resolved), refName);
                }
            }

            // Inline schema.

            return (ExtractSchemaInfo("(inline)", schemaNode), null);
        }

        return (null, null);
    }

    // ── Duplicate operations and anomaly detection ───────────────────────

    // Detects operations with identical request and/or response schema refs.
    private static List<OperationDuplicate> DetectDuplicateOperations(SpecDocument spec)
    {
        var duplicates = new List<OperationDuplicate>();
        var paths = spec.Paths;

        if (paths == null)
        {
            return duplicates;
        }

        // Build a map of operation → (requestRef, responseRef, operationId).

        var opSchemas = new List<(string Path, string? OpId, string? ReqRef, string? RespRef)>();

        foreach (var (pathKey, pathNode) in paths.Children)
        {
            if (pathKey is not YamlScalarNode pathScalar || pathNode is not YamlMappingNode pathItem)
            {
                continue;
            }

            var apiPath = pathScalar.Value!;

            foreach (var (method, operation) in SpecDocument.GetOperations(pathItem))
            {
                var opId = GetOperationId(operation);
                var reqRef = GetOperationRequestRef(operation);
                var respRef = GetOperationResponseRef(operation);

                if (reqRef != null || respRef != null)
                {
                    opSchemas.Add(($"{apiPath}.{method}", opId, reqRef, respRef));
                }
            }
        }

        // Compare each pair.

        for (int i = 0; i < opSchemas.Count; i++)
        {
            for (int j = i + 1; j < opSchemas.Count; j++)
            {
                var a = opSchemas[i];
                var b = opSchemas[j];

                // Both must share at least one schema ref.

                var sameReq = a.ReqRef != null && a.ReqRef == b.ReqRef;
                var sameResp = a.RespRef != null && a.RespRef == b.RespRef;

                if (sameReq || sameResp)
                {
                    duplicates.Add(new OperationDuplicate(
                        a.Path, a.OpId,
                        b.Path, b.OpId,
                        sameReq ? a.ReqRef : null,
                        sameResp ? a.RespRef : null));
                }
            }
        }

        return duplicates;
    }

    // Detects spec anomalies: type mismatches, input/output inconsistencies.
    private static List<SpecAnomaly> DetectAnomalies(SpecDocument spec)
    {
        var anomalies = new List<SpecAnomaly>();
        var paths = spec.Paths;

        if (paths == null)
        {
            return anomalies;
        }

        foreach (var (pathKey, pathNode) in paths.Children)
        {
            if (pathKey is not YamlScalarNode pathScalar || pathNode is not YamlMappingNode pathItem)
            {
                continue;
            }

            var apiPath = pathScalar.Value!;

            foreach (var (method, operation) in SpecDocument.GetOperations(pathItem))
            {
                var opId = GetOperationId(operation) ?? $"{apiPath}.{method}";

                // Check: request model parameter vs response model field.

                var reqModelType = GetRequestModelType(operation, spec);
                var respModelType = GetResponseModelType(operation, spec);

                if (reqModelType != null && respModelType != null && reqModelType != respModelType)
                {
                    anomalies.Add(new SpecAnomaly(
                        "warning",
                        "model-mismatch",
                        $"{apiPath}.{method}",
                        $"Request model type `{reqModelType}` differs from response model type `{respModelType}` in `{opId}`"));
                }

                // Check: input/output type consistency.
                // If the request body schema and response schema have the same name pattern
                // (e.g., CreateFoo/Foo), check they share core properties.

                var reqRef = GetOperationRequestRef(operation);
                var respRef = GetOperationResponseRef(operation);

                if (reqRef != null && respRef != null)
                {
                    var reqSchema = GetSchema(spec, reqRef);
                    var respSchema = GetSchema(spec, respRef);

                    if (reqSchema != null && respSchema != null)
                    {
                        var reqProps = GetPropertyNames(reqSchema);
                        var respProps = GetPropertyNames(respSchema);

                        // If they share a name pattern and have properties, check overlap.

                        if (reqProps.Count > 0 && respProps.Count > 0
                            && (respRef.Contains(reqRef, StringComparison.OrdinalIgnoreCase)
                                || reqRef.Contains(respRef, StringComparison.OrdinalIgnoreCase)
                                || reqRef.Replace("Create", "").Replace("Update", "") == respRef))
                        {
                            var reqOnly = reqProps.Except(respProps).ToList();

                            // Ignore standard output-only fields.

                            var respOnly = respProps.Except(reqProps)
                                .Where(p => p != "id" && p != "object" && p != "created_at" && p != "updated_at")
                                .ToList();

                            if (respOnly.Count > reqProps.Count / 2 && respOnly.Count > 2)
                            {
                                anomalies.Add(new SpecAnomaly(
                                    "warning",
                                    "input-output-divergence",
                                    $"{apiPath}.{method}",
                                    $"Request `{reqRef}` and response `{respRef}` in `{opId}` share only " +
                                    $"{reqProps.Intersect(respProps).Count()}/{reqProps.Count} input properties. " +
                                    $"Response has {respOnly.Count} additional non-standard fields: {string.Join(", ", respOnly.Take(5))}"));
                            }
                        }
                    }
                }
            }
        }

        return anomalies;
    }

    // Gets the $ref name from an operation's request body schema.
    private static string? GetOperationRequestRef(YamlMappingNode operation)
    {
        var body = GetMapping(operation, "requestBody");

        if (body == null)
        {
            return null;
        }

        var content = GetMapping(body, "content");

        if (content == null)
        {
            return null;
        }

        foreach (var ct in new[] { "application/json", "multipart/form-data" })
        {
            if (content.Children.TryGetValue(new YamlScalarNode(ct), out var ctNode) && ctNode is YamlMappingNode ctMap)
            {
                var schema = GetMapping(ctMap, "schema");

                if (schema != null)
                {
                    var refVal = GetScalar(schema, "$ref");

                    if (refVal != null)
                    {
                        return ExtractRefName(refVal);
                    }
                }
            }
        }

        return null;
    }

    // Gets the $ref name from an operation's primary response schema (200/201).
    private static string? GetOperationResponseRef(YamlMappingNode operation)
    {
        var responses = GetMapping(operation, "responses");

        if (responses == null)
        {
            return null;
        }

        foreach (var code in new[] { "200", "201" })
        {
            if (responses.Children.TryGetValue(new YamlScalarNode(code), out var respNode) && respNode is YamlMappingNode respMap)
            {
                var content = GetMapping(respMap, "content");

                if (content == null)
                {
                    continue;
                }

                foreach (var ct in new[] { "application/json", "text/event-stream" })
                {
                    if (content.Children.TryGetValue(new YamlScalarNode(ct), out var ctNode) && ctNode is YamlMappingNode ctMap)
                    {
                        var schema = GetMapping(ctMap, "schema");

                        if (schema != null)
                        {
                            var refVal = GetScalar(schema, "$ref");

                            if (refVal != null)
                            {
                                return ExtractRefName(refVal);
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    // Gets the model type from a request body schema's "model" property.
    private static string? GetRequestModelType(YamlMappingNode operation, SpecDocument spec)
    {
        var reqRef = GetOperationRequestRef(operation);

        if (reqRef == null)
        {
            return null;
        }

        var schema = GetSchema(spec, reqRef);

        if (schema == null)
        {
            return null;
        }

        var props = GetMapping(schema, "properties");

        if (props == null)
        {
            return null;
        }

        if (!props.Children.TryGetValue(new YamlScalarNode("model"), out var modelNode))
        {
            return null;
        }

        return DescribePropertyType(modelNode);
    }

    // Gets the model type from a response schema's "model" property.
    private static string? GetResponseModelType(YamlMappingNode operation, SpecDocument spec)
    {
        var respRef = GetOperationResponseRef(operation);

        if (respRef == null)
        {
            return null;
        }

        var schema = GetSchema(spec, respRef);

        if (schema == null)
        {
            return null;
        }

        var props = GetMapping(schema, "properties");

        if (props == null)
        {
            return null;
        }

        if (!props.Children.TryGetValue(new YamlScalarNode("model"), out var modelNode))
        {
            return null;
        }

        return DescribePropertyType(modelNode);
    }
}
