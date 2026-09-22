<#
.SYNOPSIS
    Classifies anyOf properties in an OpenAPI JSON document.

.DESCRIPTION
    Prints the 1-based line number of every anyOf property under one of three
    headings. Nullable contains arrays with exactly two object members, where
    exactly one member has a type property equal to null. Enum contains arrays
    with exactly two string schemas, where exactly one has an enum. Other
    contains every remaining anyOf property.

.PARAMETER SpecificationPath
    The path to the OpenAPI JSON document to inspect.

.EXAMPLE
    ./scripts/Find-OpenApiAnyOf.ps1 ./specification/openapi/responses.openapi.json
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

if ($null -eq ("OpenAI.Scripts.OpenApiAnyOfCategorizer" -as [type])) {
    Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenAI.Scripts
{
    public enum OpenApiAnyOfCategory
    {
        Nullable,
        Enum,
        Other,
    }

    public sealed class CategorizedOpenApiAnyOfOccurrence
    {
        public CategorizedOpenApiAnyOfOccurrence(int lineNumber, OpenApiAnyOfCategory category)
        {
            LineNumber = lineNumber;
            Category = category;
        }

        public int LineNumber { get; }

        public OpenApiAnyOfCategory Category { get; }
    }

    public static class OpenApiAnyOfCategorizer
    {
        public static IReadOnlyList<CategorizedOpenApiAnyOfOccurrence> Find(string path)
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
                var classifications = new List<OpenApiAnyOfCategory>();
                CollectClassifications(document.RootElement, classifications);
                return CollectOccurrences(json.Span, classifications);
            }
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
            List<OpenApiAnyOfCategory> classifications)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in element.EnumerateObject())
                {
                    if (property.NameEquals("anyOf"))
                    {
                        classifications.Add(Classify(property.Value));
                    }

                    CollectClassifications(property.Value, classifications);
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in element.EnumerateArray())
                {
                    CollectClassifications(item, classifications);
                }
            }
        }

        private static OpenApiAnyOfCategory Classify(JsonElement value)
        {
            if (IsNullable(value))
            {
                return OpenApiAnyOfCategory.Nullable;
            }

            if (IsEnum(value))
            {
                return OpenApiAnyOfCategory.Enum;
            }

            return OpenApiAnyOfCategory.Other;
        }

        private static bool IsNullable(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 2)
            {
                return false;
            }

            int nullTypeCount = 0;
            foreach (JsonElement item in value.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    return false;
                }

                JsonElement type;
                if (item.TryGetProperty("type", out type)
                    && type.ValueKind == JsonValueKind.String
                    && type.ValueEquals("null"))
                {
                    nullTypeCount++;
                }
            }

            return nullTypeCount == 1;
        }

        private static bool IsEnum(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Array || value.GetArrayLength() != 2)
            {
                return false;
            }

            int enumCount = 0;
            foreach (JsonElement item in value.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    return false;
                }

                JsonElement type;
                if (!item.TryGetProperty("type", out type)
                    || type.ValueKind != JsonValueKind.String
                    || !type.ValueEquals("string"))
                {
                    return false;
                }

                JsonElement enumValues;
                if (item.TryGetProperty("enum", out enumValues))
                {
                    if (enumValues.ValueKind != JsonValueKind.Array)
                    {
                        return false;
                    }

                    enumCount++;
                }
            }

            return enumCount == 1;
        }

        private static IReadOnlyList<CategorizedOpenApiAnyOfOccurrence> CollectOccurrences(
            ReadOnlySpan<byte> json,
            IReadOnlyList<OpenApiAnyOfCategory> classifications)
        {
            var readerOptions = new JsonReaderOptions
            {
                AllowTrailingCommas = false,
                CommentHandling = JsonCommentHandling.Disallow,
                MaxDepth = 1024,
            };
            var reader = new Utf8JsonReader(json, readerOptions);
            var occurrences = new List<CategorizedOpenApiAnyOfOccurrence>(classifications.Count);
            int classificationIndex = 0;
            int lineNumber = 1;
            int lineScanIndex = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.PropertyName
                    || !reader.ValueTextEquals("anyOf"))
                {
                    continue;
                }

                if (classificationIndex >= classifications.Count)
                {
                    throw new InvalidDataException("Found more anyOf tokens than parsed properties.");
                }

                int tokenStartIndex = checked((int)reader.TokenStartIndex);
                AdvanceLineNumber(json, tokenStartIndex, ref lineScanIndex, ref lineNumber);
                occurrences.Add(new CategorizedOpenApiAnyOfOccurrence(
                    lineNumber,
                    classifications[classificationIndex]));
                classificationIndex++;
            }

            if (classificationIndex != classifications.Count)
            {
                throw new InvalidDataException("Found fewer anyOf tokens than parsed properties.");
            }

            return occurrences;
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

$occurrences = @([OpenAI.Scripts.OpenApiAnyOfCategorizer]::Find($resolvedPath))
$nullableLineNumbers = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiAnyOfCategory]::Nullable } |
        ForEach-Object { $_.LineNumber }
)
$enumLineNumbers = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiAnyOfCategory]::Enum } |
        ForEach-Object { $_.LineNumber }
)
$otherLineNumbers = @(
    $occurrences |
        Where-Object { $_.Category -eq [OpenAI.Scripts.OpenApiAnyOfCategory]::Other } |
        ForEach-Object { $_.LineNumber }
)

Write-Output "Nullable:"
$nullableLineNumbers | Write-Output
Write-Output ""
Write-Output "Enum:"
$enumLineNumbers | Write-Output
Write-Output ""
Write-Output "Other:"
$otherLineNumbers | Write-Output