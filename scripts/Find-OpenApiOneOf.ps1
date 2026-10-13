<#
.SYNOPSIS
    Classifies oneOf properties in an OpenAPI JSON document.

.DESCRIPTION
    Prints the 1-based line number of every oneOf property under one of three
    headings. Discriminated contains non-empty arrays where every member has a
    type property equal to object and the containing object has a discriminator
    with a propertyName. Potentially Discriminated contains the same arrays
    without a usable discriminator. Other contains every remaining oneOf
    property and reports every branch type in source order. A reconciliation report
    explains discriminator properties that are not counted as discriminated
    oneOf instances and exact string values equal to discriminator.

.PARAMETER SpecificationPath
    The path to the OpenAPI JSON document to inspect.

.EXAMPLE
    ./scripts/Find-OpenApiOneOf.ps1 ./specification/openapi/responses/responses.openapi.json
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateNotNullOrEmpty()]
    [string]$SpecificationPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$resolvedPath = (Resolve-Path -LiteralPath $SpecificationPath -ErrorAction Stop).ProviderPath
if (-not [System.IO.File]::Exists($resolvedPath)) {
    throw "The specification path is not a file: $resolvedPath"
}

if ($null -eq ("OpenAI.Scripts.OpenApiOneOf.V4.OpenApiOneOfCategorizer" -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenAI.Scripts.OpenApiOneOf.V4
{
    public enum OpenApiOneOfCategory
    {
        Discriminated,
        PotentiallyDiscriminated,
        Other,
    }

    public enum OpenApiDiscriminatorCategory
    {
        CountedByDiscriminatedOneOf,
        PairedWithRejectedOneOf,
        PairedWithMalformedDiscriminator,
        NoSiblingOneOf,
    }

    public sealed class CategorizedOpenApiOneOfOccurrence
    {
        public CategorizedOpenApiOneOfOccurrence(
            int lineNumber,
            OpenApiOneOfCategory category,
            string discriminatorPropertyName,
            IReadOnlyList<string> branchTypes)
        {
            LineNumber = lineNumber;
            Category = category;
            DiscriminatorPropertyName = discriminatorPropertyName;
            BranchTypes = branchTypes;
        }

        public int LineNumber { get; }

        public OpenApiOneOfCategory Category { get; }

        public string DiscriminatorPropertyName { get; }

        public IReadOnlyList<string> BranchTypes { get; }
    }

    public sealed class OpenApiDiscriminatorOccurrence
    {
        public OpenApiDiscriminatorOccurrence(
            int lineNumber,
            OpenApiDiscriminatorCategory category,
            string propertyName,
            int siblingOneOfLineNumber,
            string detail)
        {
            LineNumber = lineNumber;
            Category = category;
            PropertyName = propertyName;
            SiblingOneOfLineNumber = siblingOneOfLineNumber;
            Detail = detail;
        }

        public int LineNumber { get; }

        public OpenApiDiscriminatorCategory Category { get; }

        public string PropertyName { get; }

        public int SiblingOneOfLineNumber { get; }

        public string Detail { get; }
    }

    public sealed class OpenApiDiscriminatorStringOccurrence
    {
        public OpenApiDiscriminatorStringOccurrence(int lineNumber, string context)
        {
            LineNumber = lineNumber;
            Context = context;
        }

        public int LineNumber { get; }

        public string Context { get; }
    }

    public sealed class OpenApiOneOfAnalysis
    {
        public OpenApiOneOfAnalysis(
            IReadOnlyList<CategorizedOpenApiOneOfOccurrence> oneOfOccurrences,
            IReadOnlyList<OpenApiDiscriminatorOccurrence> discriminatorOccurrences,
            IReadOnlyList<OpenApiDiscriminatorStringOccurrence> discriminatorStringOccurrences)
        {
            OneOfOccurrences = oneOfOccurrences;
            DiscriminatorOccurrences = discriminatorOccurrences;
            DiscriminatorStringOccurrences = discriminatorStringOccurrences;
        }

        public IReadOnlyList<CategorizedOpenApiOneOfOccurrence> OneOfOccurrences { get; }

        public IReadOnlyList<OpenApiDiscriminatorOccurrence> DiscriminatorOccurrences { get; }

        public IReadOnlyList<OpenApiDiscriminatorStringOccurrence> DiscriminatorStringOccurrences { get; }
    }

    internal sealed class OpenApiOneOfClassification
    {
        public OpenApiOneOfClassification(
            OpenApiOneOfCategory category,
            string discriminatorPropertyName,
            IReadOnlyList<string> branchTypes,
            string failureReason)
        {
            Category = category;
            DiscriminatorPropertyName = discriminatorPropertyName;
            BranchTypes = branchTypes;
            FailureReason = failureReason;
        }

        public OpenApiOneOfCategory Category { get; }

        public string DiscriminatorPropertyName { get; }

        public IReadOnlyList<string> BranchTypes { get; }

        public string FailureReason { get; }

        public int LineNumber { get; set; }
    }

    internal sealed class OpenApiDiscriminatorClassification
    {
        public OpenApiDiscriminatorClassification(
            OpenApiDiscriminatorCategory category,
            string propertyName,
            OpenApiOneOfClassification siblingOneOf,
            string detail)
        {
            Category = category;
            PropertyName = propertyName;
            SiblingOneOf = siblingOneOf;
            Detail = detail;
        }

        public OpenApiDiscriminatorCategory Category { get; }

        public string PropertyName { get; }

        public OpenApiOneOfClassification SiblingOneOf { get; }

        public string Detail { get; }

        public int LineNumber { get; set; }
    }

    internal sealed class OpenApiDiscriminatorStringClassification
    {
        public OpenApiDiscriminatorStringClassification(string context)
        {
            Context = context;
        }

        public string Context { get; }

        public int LineNumber { get; set; }
    }

    public static class OpenApiOneOfCategorizer
    {
        public static OpenApiOneOfAnalysis Analyze(string path)
        {
            byte[] fileContents = File.ReadAllBytes(path);
            int contentOffset = HasUtf8ByteOrderMark(fileContents) ? 3 : 0;
            var json = new ReadOnlyMemory<byte>(
                fileContents,
                contentOffset,
                fileContents.Length - contentOffset);

            var documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow,
                MaxDepth = 1024,
            };

            using (JsonDocument document = JsonDocument.Parse(json, documentOptions))
            {
                var oneOfClassifications = new List<OpenApiOneOfClassification>();
                var discriminatorClassifications = new List<OpenApiDiscriminatorClassification>();
                var discriminatorStringClassifications = new List<OpenApiDiscriminatorStringClassification>();
                CollectClassifications(
                    document.RootElement,
                    oneOfClassifications,
                    discriminatorClassifications,
                    discriminatorStringClassifications,
                    null);
                CollectLocations(
                    json.Span,
                    oneOfClassifications,
                    discriminatorClassifications,
                    discriminatorStringClassifications);
                return CreateAnalysis(
                    oneOfClassifications,
                    discriminatorClassifications,
                    discriminatorStringClassifications);
            }
        }

        public static IReadOnlyList<CategorizedOpenApiOneOfOccurrence> Find(string path)
        {
            return Analyze(path).OneOfOccurrences;
        }

        private static bool HasUtf8ByteOrderMark(byte[] contents)
        {
            return contents.Length >= 3
                && contents[0] == 0xEF
                && contents[1] == 0xBB
                && contents[2] == 0xBF;
        }

        private static void CollectClassifications(
            JsonElement element,
            List<OpenApiOneOfClassification> oneOfClassifications,
            List<OpenApiDiscriminatorClassification> discriminatorClassifications,
            List<OpenApiDiscriminatorStringClassification> discriminatorStringClassifications,
            string stringContext)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                if (element.ValueEquals("discriminator"))
                {
                    discriminatorStringClassifications.Add(
                        new OpenApiDiscriminatorStringClassification(
                            stringContext ?? "string value"));
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                JsonElement siblingOneOfValue;
                OpenApiOneOfClassification siblingOneOf = null;
                if (element.TryGetProperty("oneOf", out siblingOneOfValue))
                {
                    siblingOneOf = Classify(element, siblingOneOfValue);
                }

                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (property.NameEquals("oneOf"))
                    {
                        oneOfClassifications.Add(siblingOneOf ?? Classify(element, property.Value));
                    }
                    else if (property.NameEquals("discriminator"))
                    {
                        discriminatorClassifications.Add(
                            ClassifyDiscriminator(property.Value, siblingOneOf));
                    }

                    CollectClassifications(
                        property.Value,
                        oneOfClassifications,
                        discriminatorClassifications,
                        discriminatorStringClassifications,
                        GetStringContext(property.Name));
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in element.EnumerateArray())
                {
                    CollectClassifications(
                        item,
                        oneOfClassifications,
                        discriminatorClassifications,
                        discriminatorStringClassifications,
                        stringContext);
                }
            }
        }

        private static string GetStringContext(string propertyName)
        {
            if (string.Equals(propertyName, "enum", StringComparison.Ordinal))
            {
                return "enum value";
            }

            if (string.Equals(propertyName, "required", StringComparison.Ordinal))
            {
                return "required name";
            }

            if (string.Equals(propertyName, "propertyName", StringComparison.Ordinal))
            {
                return "propertyName value";
            }

            return "property '" + propertyName + "' value";
        }

        private static OpenApiOneOfClassification Classify(
            JsonElement containingObject,
            JsonElement value)
        {
            if (HasOnlyObjectSchemas(value))
            {
                string discriminatorPropertyName = GetDiscriminatorPropertyName(containingObject);
                if (discriminatorPropertyName != null)
                {
                    return new OpenApiOneOfClassification(
                        OpenApiOneOfCategory.Discriminated,
                        discriminatorPropertyName,
                        Array.Empty<string>(),
                        null);
                }

                return new OpenApiOneOfClassification(
                    OpenApiOneOfCategory.PotentiallyDiscriminated,
                    null,
                    Array.Empty<string>(),
                    null);
            }

            return new OpenApiOneOfClassification(
                OpenApiOneOfCategory.Other,
                null,
                GetBranchTypes(value),
                GetOneOfFailureReason(value));
        }

        private static bool HasOnlyObjectSchemas(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() == 0)
            {
                return false;
            }

            foreach (JsonElement item in value.EnumerateArray())
            {
                JsonElement type;
                if (item.ValueKind != JsonValueKind.Object
                    || !item.TryGetProperty("type", out type)
                    || type.ValueKind != JsonValueKind.String
                    || !type.ValueEquals("object"))
                {
                    return false;
                }
            }

            return true;
        }

        private static string GetDiscriminatorPropertyName(JsonElement containingObject)
        {
            JsonElement discriminator;
            if (!containingObject.TryGetProperty("discriminator", out discriminator))
            {
                return null;
            }

            string validationIssue;
            return GetDiscriminatorPropertyName(discriminator, out validationIssue);
        }

        private static string GetDiscriminatorPropertyName(
            JsonElement discriminator,
            out string validationIssue)
        {
            if (discriminator.ValueKind != JsonValueKind.Object)
            {
                validationIssue = "discriminator is "
                    + GetValueKindName(discriminator.ValueKind)
                    + ", expected object";
                return null;
            }

            JsonElement propertyName;
            if (!discriminator.TryGetProperty("propertyName", out propertyName))
            {
                validationIssue = "discriminator is missing propertyName";
                return null;
            }

            if (propertyName.ValueKind != JsonValueKind.String)
            {
                validationIssue = "propertyName is "
                    + GetValueKindName(propertyName.ValueKind)
                    + ", expected string";
                return null;
            }

            string value = propertyName.GetString();
            if (string.IsNullOrEmpty(value))
            {
                validationIssue = "propertyName is empty";
                return null;
            }

            validationIssue = null;
            return value;
        }

        private static OpenApiDiscriminatorClassification ClassifyDiscriminator(
            JsonElement discriminator,
            OpenApiOneOfClassification siblingOneOf)
        {
            string validationIssue;
            string propertyName = GetDiscriminatorPropertyName(
                discriminator,
                out validationIssue);

            if (siblingOneOf == null)
            {
                return new OpenApiDiscriminatorClassification(
                    OpenApiDiscriminatorCategory.NoSiblingOneOf,
                    propertyName,
                    null,
                    validationIssue);
            }

            if (validationIssue != null)
            {
                return new OpenApiDiscriminatorClassification(
                    OpenApiDiscriminatorCategory.PairedWithMalformedDiscriminator,
                    null,
                    siblingOneOf,
                    validationIssue);
            }

            if (siblingOneOf.Category == OpenApiOneOfCategory.Discriminated)
            {
                return new OpenApiDiscriminatorClassification(
                    OpenApiDiscriminatorCategory.CountedByDiscriminatedOneOf,
                    propertyName,
                    siblingOneOf,
                    null);
            }

            return new OpenApiDiscriminatorClassification(
                OpenApiDiscriminatorCategory.PairedWithRejectedOneOf,
                propertyName,
                siblingOneOf,
                siblingOneOf.FailureReason ?? "oneOf is not an all-object union");
        }

        private static string GetOneOfFailureReason(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array)
            {
                return "oneOf is " + GetValueKindName(value.ValueKind) + ", expected array";
            }

            if (value.GetArrayLength() == 0)
            {
                return "oneOf array is empty";
            }

            var reasons = new List<string>();
            int branchNumber = 0;
            foreach (JsonElement item in value.EnumerateArray())
            {
                branchNumber++;
                if (item.ValueKind != JsonValueKind.Object)
                {
                    reasons.Add(
                        "branch "
                        + branchNumber
                        + " is "
                        + GetValueKindName(item.ValueKind)
                        + ", expected object schema");
                    continue;
                }

                JsonElement type;
                if (!item.TryGetProperty("type", out type))
                {
                    JsonElement reference;
                    if (item.TryGetProperty("$recursiveRef", out reference))
                    {
                        reasons.Add(
                            "branch "
                            + branchNumber
                            + " uses $recursiveRef without explicit type: object");
                    }
                    else if (item.TryGetProperty("$ref", out reference))
                    {
                        reasons.Add(
                            "branch "
                            + branchNumber
                            + " uses $ref without explicit type: object");
                    }
                    else
                    {
                        reasons.Add(
                            "branch "
                            + branchNumber
                            + " has no explicit type: object");
                    }

                    continue;
                }

                if (type.ValueKind != JsonValueKind.String)
                {
                    reasons.Add(
                        "branch "
                        + branchNumber
                        + " type is "
                        + GetValueKindName(type.ValueKind)
                        + ", expected string object");
                    continue;
                }

                if (!type.ValueEquals("object"))
                {
                    reasons.Add(
                        "branch "
                        + branchNumber
                        + " has type "
                        + (type.GetString() ?? "(null)"));
                }
            }

            return reasons.Count == 0
                ? "oneOf is not an all-object union"
                : string.Join("; ", reasons);
        }

        private static string GetValueKindName(JsonValueKind valueKind)
        {
            return valueKind.ToString().ToLowerInvariant();
        }

        private static IReadOnlyList<string> GetBranchTypes(JsonElement value)
        {
            var types = new List<string>();

            if (value.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in value.EnumerateArray())
                {
                    JsonElement type;
                    if (item.ValueKind != JsonValueKind.Object
                        || !item.TryGetProperty("type", out type)
                        || type.ValueKind != JsonValueKind.String)
                    {
                        types.Add("(unknown)");
                        continue;
                    }

                    JsonElement enumValues;
                    if (type.ValueEquals("string")
                        && item.TryGetProperty("enum", out enumValues)
                        && enumValues.ValueKind == JsonValueKind.Array)
                    {
                        types.Add("enum");
                        continue;
                    }

                    string typeName = type.GetString();
                    types.Add(typeName ?? "(unknown)");
                }
            }

            return types;
        }

        private static void CollectLocations(
            ReadOnlySpan<byte> json,
            IReadOnlyList<OpenApiOneOfClassification> oneOfClassifications,
            IReadOnlyList<OpenApiDiscriminatorClassification> discriminatorClassifications,
            IReadOnlyList<OpenApiDiscriminatorStringClassification> discriminatorStringClassifications)
        {
            var readerOptions = new JsonReaderOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow,
                MaxDepth = 1024,
            };
            var reader = new Utf8JsonReader(json, readerOptions);
            int oneOfIndex = 0;
            int discriminatorIndex = 0;
            int discriminatorStringIndex = 0;
            int lineNumber = 1;
            int lineScanIndex = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName
                    && reader.ValueTextEquals("oneOf"))
                {
                    if (oneOfIndex >= oneOfClassifications.Count)
                    {
                        throw new InvalidDataException("Found more oneOf tokens than parsed properties.");
                    }

                    int tokenStartIndex = checked((int)reader.TokenStartIndex);
                    AdvanceLineNumber(json, tokenStartIndex, ref lineScanIndex, ref lineNumber);
                    oneOfClassifications[oneOfIndex++].LineNumber = lineNumber;
                }
                else if (reader.TokenType == JsonTokenType.PropertyName
                    && reader.ValueTextEquals("discriminator"))
                {
                    if (discriminatorIndex >= discriminatorClassifications.Count)
                    {
                        throw new InvalidDataException(
                            "Found more discriminator tokens than parsed properties.");
                    }

                    int tokenStartIndex = checked((int)reader.TokenStartIndex);
                    AdvanceLineNumber(json, tokenStartIndex, ref lineScanIndex, ref lineNumber);
                    discriminatorClassifications[discriminatorIndex++].LineNumber = lineNumber;
                }
                else if (reader.TokenType == JsonTokenType.String
                    && reader.ValueTextEquals("discriminator"))
                {
                    if (discriminatorStringIndex >= discriminatorStringClassifications.Count)
                    {
                        throw new InvalidDataException(
                            "Found more discriminator string tokens than parsed values.");
                    }

                    int tokenStartIndex = checked((int)reader.TokenStartIndex);
                    AdvanceLineNumber(json, tokenStartIndex, ref lineScanIndex, ref lineNumber);
                    discriminatorStringClassifications[discriminatorStringIndex++].LineNumber = lineNumber;
                }
            }

            if (oneOfIndex != oneOfClassifications.Count)
            {
                throw new InvalidDataException("Found fewer oneOf tokens than parsed properties.");
            }

            if (discriminatorIndex != discriminatorClassifications.Count)
            {
                throw new InvalidDataException(
                    "Found fewer discriminator tokens than parsed properties.");
            }

            if (discriminatorStringIndex != discriminatorStringClassifications.Count)
            {
                throw new InvalidDataException(
                    "Found fewer discriminator string tokens than parsed values.");
            }
        }

        private static OpenApiOneOfAnalysis CreateAnalysis(
            IReadOnlyList<OpenApiOneOfClassification> oneOfClassifications,
            IReadOnlyList<OpenApiDiscriminatorClassification> discriminatorClassifications,
            IReadOnlyList<OpenApiDiscriminatorStringClassification> discriminatorStringClassifications)
        {
            var oneOfOccurrences = new List<CategorizedOpenApiOneOfOccurrence>(
                oneOfClassifications.Count);
            foreach (OpenApiOneOfClassification classification in oneOfClassifications)
            {
                oneOfOccurrences.Add(new CategorizedOpenApiOneOfOccurrence(
                    classification.LineNumber,
                    classification.Category,
                    classification.DiscriminatorPropertyName,
                    classification.BranchTypes));
            }

            var discriminatorOccurrences = new List<OpenApiDiscriminatorOccurrence>(
                discriminatorClassifications.Count);
            foreach (OpenApiDiscriminatorClassification classification in discriminatorClassifications)
            {
                discriminatorOccurrences.Add(new OpenApiDiscriminatorOccurrence(
                    classification.LineNumber,
                    classification.Category,
                    classification.PropertyName,
                    classification.SiblingOneOf == null
                        ? 0
                        : classification.SiblingOneOf.LineNumber,
                    classification.Detail));
            }

            var discriminatorStringOccurrences = new List<OpenApiDiscriminatorStringOccurrence>(
                discriminatorStringClassifications.Count);
            foreach (OpenApiDiscriminatorStringClassification classification
                in discriminatorStringClassifications)
            {
                discriminatorStringOccurrences.Add(
                    new OpenApiDiscriminatorStringOccurrence(
                        classification.LineNumber,
                        classification.Context));
            }

            return new OpenApiOneOfAnalysis(
                oneOfOccurrences,
                discriminatorOccurrences,
                discriminatorStringOccurrences);
        }

        private static void AdvanceLineNumber(
            ReadOnlySpan<byte> json,
            int tokenStartIndex,
            ref int lineScanIndex,
            ref int lineNumber)
        {
            while (lineScanIndex < tokenStartIndex)
            {
                byte current = json[lineScanIndex++];
                if (current == (byte)'\r')
                {
                    if (lineScanIndex < tokenStartIndex && json[lineScanIndex] == (byte)'\n')
                    {
                        lineScanIndex++;
                    }

                    lineNumber++;
                }
                else if (current == (byte)'\n')
                {
                    lineNumber++;
                }
            }
        }
    }
}
'@
}

$analysis = [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiOneOfCategorizer]::Analyze($resolvedPath)
$occurrences = @($analysis.OneOfOccurrences)
$discriminatorOccurrences = @($analysis.DiscriminatorOccurrences)
$discriminatorStringOccurrences = @($analysis.DiscriminatorStringOccurrences)
$discriminated = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiOneOfCategory]::Discriminated }
)
$potentiallyDiscriminated = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiOneOfCategory]::PotentiallyDiscriminated }
)
$other = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiOneOfCategory]::Other }
)

Write-Output "Discriminated ($($discriminated.Count)):"
$discriminated |
    ForEach-Object { "$($_.LineNumber): $($_.DiscriminatorPropertyName)" } |
    Write-Output
Write-Output ""
Write-Output "Potentially Discriminated ($($potentiallyDiscriminated.Count)):"
$potentiallyDiscriminated |
    ForEach-Object { $_.LineNumber } |
    Write-Output
Write-Output ""
Write-Output "Other ($($other.Count)):"
$other |
    ForEach-Object {
        $typeNames = @($_.BranchTypes)
        $typeSummary = if ($typeNames.Count -eq 0) { "(unknown)" } else { $typeNames -join ", " }
        "$($_.LineNumber): $typeSummary"
    } |
    Write-Output

$countedDiscriminators = @(
    $discriminatorOccurrences |
        Where-Object {
            $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiDiscriminatorCategory]::CountedByDiscriminatedOneOf
        }
)
$rejectedDiscriminators = @(
    $discriminatorOccurrences |
        Where-Object {
            $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiDiscriminatorCategory]::PairedWithRejectedOneOf
        }
)
$malformedDiscriminators = @(
    $discriminatorOccurrences |
        Where-Object {
            $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiDiscriminatorCategory]::PairedWithMalformedDiscriminator
        }
)
$unpairedDiscriminators = @(
    $discriminatorOccurrences |
        Where-Object {
            $_.Category -eq [OpenAI.Scripts.OpenApiOneOf.V4.OpenApiDiscriminatorCategory]::NoSiblingOneOf
        }
)

Write-Output ""
Write-Output "Discriminator Reconciliation:"
Write-Output "Structural discriminator properties: $($discriminatorOccurrences.Count)"
Write-Output "Counted by Discriminated oneOf: $($countedDiscriminators.Count)"
Write-Output "Paired with rejected oneOf: $($rejectedDiscriminators.Count)"
Write-Output "Paired with malformed discriminator: $($malformedDiscriminators.Count)"
Write-Output "Without sibling oneOf: $($unpairedDiscriminators.Count)"
Write-Output "Exact discriminator string values: $($discriminatorStringOccurrences.Count)"
Write-Output "Exact quoted discriminator occurrences: $(
    $discriminatorOccurrences.Count + $discriminatorStringOccurrences.Count
)"
Write-Output ""
Write-Output "Paired with rejected oneOf ($($rejectedDiscriminators.Count)):"
$rejectedDiscriminators |
    ForEach-Object {
        "$($_.LineNumber) -> oneOf $($_.SiblingOneOfLineNumber): $($_.PropertyName); $($_.Detail)"
    } |
    Write-Output
Write-Output ""
Write-Output "Paired with malformed discriminator ($($malformedDiscriminators.Count)):"
$malformedDiscriminators |
    ForEach-Object {
        "$($_.LineNumber) -> oneOf $($_.SiblingOneOfLineNumber): $($_.Detail)"
    } |
    Write-Output
Write-Output ""
Write-Output "Without sibling oneOf ($($unpairedDiscriminators.Count)):"
$unpairedDiscriminators |
    ForEach-Object {
        $summary = if ([string]::IsNullOrEmpty($_.PropertyName)) {
            $_.Detail
        }
        elseif ([string]::IsNullOrEmpty($_.Detail)) {
            $_.PropertyName
        }
        else {
            "$($_.PropertyName); $($_.Detail)"
        }
        "$($_.LineNumber): $summary"
    } |
    Write-Output
Write-Output ""
Write-Output "Exact discriminator string values ($($discriminatorStringOccurrences.Count)):"
$discriminatorStringOccurrences |
    ForEach-Object {
        "$($_.LineNumber): $($_.Context) (not a discriminator keyword)"
    } |
    Write-Output