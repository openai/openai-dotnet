<#
.SYNOPSIS
    Moves inline object schemas into OpenAPI components.

.DESCRIPTION
    Finds every inline schema with the exact scalar type "object", creates one
    components.schemas entry per occurrence, and replaces each inline definition
    with a local $ref. Nested object schemas become separate components referenced
    by their containing component.

    Component names combine underscore-separated source-path tokens and a
    TypeOf-prefixed singleton string enum from properties.type when available.
    Structural composition/item tokens are omitted, and numeric suffixes are
    used only to disambiguate collisions. Structurally identical occurrences
    intentionally remain separate components.

.PARAMETER InputPath
    The OpenAPI JSON document to componentize. When omitted, reads
    specification/openapi/responses.openapi.json beneath the repository root.

.PARAMETER OutputPath
    The destination JSON file. When omitted for the canonical input, writes
    specification/openapi/responses.componentized.openapi.json. Other inputs use
    a sibling filename with '.componentized' inserted before the JSON suffix.

.EXAMPLE
    ./scripts/Componentize-OpenApiObjectSchemas.ps1

.EXAMPLE
    ./scripts/Componentize-OpenApiObjectSchemas.ps1 `
        ./artifacts/custom.openapi.json `
        -OutputPath ./artifacts/custom.componentized.openapi.json
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$InputPath,

    [Parameter()]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$helperPath = Join-Path $PSScriptRoot "_OpenApiDocument.Helpers.ps1"
if (-not (Test-Path -LiteralPath $helperPath -PathType Leaf)) {
    throw "The shared OpenAPI helper script was not found: $helperPath"
}
. $helperPath

$script:ComponentizerRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$script:DefaultComponentizerInputPath = Join-Path $script:ComponentizerRepositoryRoot "specification" "openapi" "responses.openapi.json"
$script:DefaultComponentizerOutputPath = Join-Path $script:ComponentizerRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"

function Test-IsInlineOpenApiObjectSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context
    )

    if (-not [string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -or
        -not (Test-JsonObjectValue -Value $Value)) {
        return $false
    }

    $type = Get-JsonPropertyValue -Object $Value -Name "type"
    return $type -is [string] -and
        [string]::Equals($type, "object", [System.StringComparison]::Ordinal)
}

function Get-OpenApiObjectSchemaTypeEnumValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema
    )

    $properties = Get-JsonPropertyValue -Object $Schema -Name "properties"
    if (-not (Test-JsonObjectValue -Value $properties)) {
        return $null
    }
    $typeSchema = Get-JsonPropertyValue -Object $properties -Name "type"
    if (-not (Test-JsonObjectValue -Value $typeSchema)) {
        return $null
    }
    $enum = Get-JsonPropertyValue -Object $typeSchema -Name "enum"
    if (-not (Test-JsonArrayValue -Value $enum) -or
        $enum.Count -ne 1 -or
        $enum[0] -isnot [string]) {
        return $null
    }

    return $enum[0]
}

function Add-InlineOpenApiObjectSchemaCandidates {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.List[object]]$Candidates,

        [Parameter()]
        [switch]$ComponentsSchemaMap,

        [Parameter()]
        [switch]$SuppressCurrentCandidate,

        [Parameter()]
        [int]$CandidateDepth = 0,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Object schema discovery exceeds the supported depth of $MaximumDepth at '$Pointer'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Add-InlineOpenApiObjectSchemaCandidates `
                -Value $Value[$index] `
                -Context $itemContext `
                -Pointer "$Pointer/$index" `
                -Candidates $Candidates `
                -CandidateDepth $CandidateDepth `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    $isCandidate = -not $SuppressCurrentCandidate -and
        (Test-IsInlineOpenApiObjectSchema -Value $Value -Context $Context)
    $childCandidateDepth = $CandidateDepth
    if ($isCandidate) {
        $Candidates.Add([PSCustomObject]@{
            Pointer = $Pointer
            Schema = $Value
            CandidateDepth = $CandidateDepth
            TypeEnumValue = Get-OpenApiObjectSchemaTypeEnumValue -Schema $Value
        })
        $childCandidateDepth++
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childPointer = "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $childIsComponentsSchemaMap = [string]::Equals($Context, "Components", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)

        if ([string]::Equals($Context, "SchemaMap", [System.StringComparison]::Ordinal) -and
            $ComponentsSchemaMap) {
            Add-InlineOpenApiObjectSchemaCandidates `
                -Value $property.Value `
                -Context $childContext `
                -Pointer $childPointer `
                -Candidates $Candidates `
                -SuppressCurrentCandidate `
                -CandidateDepth $childCandidateDepth `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            continue
        }

        Add-InlineOpenApiObjectSchemaCandidates `
            -Value $property.Value `
            -Context $childContext `
            -Pointer $childPointer `
            -Candidates $Candidates `
            -ComponentsSchemaMap:$childIsComponentsSchemaMap `
            -CandidateDepth $childCandidateDepth `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

function Get-InlineOpenApiObjectSchemaCandidates {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $candidates = [System.Collections.Generic.List[object]]::new()
    Add-InlineOpenApiObjectSchemaCandidates `
        -Value $Document `
        -Context OpenApi `
        -Pointer "#" `
        -Candidates $candidates
    return [object[]]$candidates.ToArray()
}

function ConvertTo-OpenApiComponentNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $segments = [System.Text.RegularExpressions.Regex]::Matches($Value, "[A-Za-z0-9]+")
    $builder = [System.Text.StringBuilder]::new()
    foreach ($segmentMatch in $segments) {
        $segment = $segmentMatch.Value
        if ($segment.Length -eq 0) {
            continue
        }
        if ([char]::IsDigit($segment[0])) {
            $null = $builder.Append("X")
        }
        $null = $builder.Append([char]::ToUpperInvariant($segment[0]))
        if ($segment.Length -gt 1) {
            $null = $builder.Append($segment.Substring(1))
        }
    }
    return $builder.ToString()
}

function Get-OpenApiOperationComponentNamePrefix {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [string[]]$Tokens
    )

    if ($Tokens.Count -lt 3 -or
        -not [string]::Equals($Tokens[0], "paths", [System.StringComparison]::Ordinal)) {
        return $null
    }

    $paths = Get-JsonPropertyValue -Object $Document -Name "paths"
    $pathItem = Get-JsonPropertyValue -Object $paths -Name $Tokens[1]
    if (-not (Test-JsonObjectValue -Value $pathItem)) {
        return $null
    }
    $operation = Get-JsonPropertyValue -Object $pathItem -Name $Tokens[2]
    if (-not (Test-JsonObjectValue -Value $operation)) {
        return $null
    }
    $operationId = Get-JsonPropertyValue -Object $operation -Name "operationId"
    if ($operationId -is [string] -and -not [string]::IsNullOrWhiteSpace($operationId)) {
        return ConvertTo-OpenApiComponentNameToken -Value $operationId
    }

    return (ConvertTo-OpenApiComponentNameToken -Value $Tokens[2]) +
        (ConvertTo-OpenApiComponentNameToken -Value $Tokens[1])
}

function Get-OpenApiObjectSchemaComponentBaseName {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Candidate
    )

    [string[]]$tokens = Get-JsonPointerTokens -Pointer $Candidate.Pointer
    $parts = [System.Collections.Generic.List[string]]::new()

    function Add-NamePart {
        param([AllowEmptyString()][string]$Part)

        if ([string]::IsNullOrWhiteSpace($Part)) {
            return
        }
        if ($parts.Count -eq 0 -or
            -not [string]::Equals($parts[$parts.Count - 1], $Part, [System.StringComparison]::Ordinal)) {
            $parts.Add($Part)
        }
    }

    $startIndex = 0
    $operationPrefix = Get-OpenApiOperationComponentNamePrefix -Document $Document -Tokens $tokens
    if (-not [string]::IsNullOrWhiteSpace($operationPrefix)) {
        Add-NamePart -Part $operationPrefix
        if ($tokens -ccontains "requestBody") {
            Add-NamePart -Part "Request"
        }
        elseif ($tokens -ccontains "responses") {
            Add-NamePart -Part "Response"
            $responsesIndex = [Array]::IndexOf($tokens, "responses")
            if ($responsesIndex -ge 0 -and $responsesIndex + 1 -lt $tokens.Count) {
                Add-NamePart -Part (ConvertTo-OpenApiComponentNameToken -Value $tokens[$responsesIndex + 1])
            }
        }
        $schemaIndex = [Array]::IndexOf($tokens, "schema")
        $startIndex = if ($schemaIndex -ge 0) { $schemaIndex + 1 } else { 3 }
    }
    elseif ($tokens.Count -ge 3 -and
        [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal) -and
        [string]::Equals($tokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        Add-NamePart -Part (ConvertTo-OpenApiComponentNameToken -Value $tokens[2])
        $startIndex = 3
    }

    for ($index = $startIndex; $index -lt $tokens.Count; $index++) {
        $token = $tokens[$index]
        if (@(
            "paths",
            "requestBody",
            "content",
            "schema",
            "properties",
            "components",
            "schemas",
            "items",
            "additionalProperties",
            "oneOf",
            "anyOf",
            "allOf",
            "prefixItems"
        ) -ccontains $token -or
            $script:OpenApiHttpMethodNames -ccontains $token -or
            $token -match "^[0-9]+$") {
            continue
        }
        if ($token -match "^[^/]+/[^/]+$") {
            continue
        }

        $nameToken = ConvertTo-OpenApiComponentNameToken -Value $token
        Add-NamePart -Part $nameToken
    }

    if ($Candidate.TypeEnumValue -is [string]) {
        $enumToken = ConvertTo-OpenApiComponentNameToken -Value $Candidate.TypeEnumValue
        if (-not [string]::IsNullOrWhiteSpace($enumToken)) {
            Add-NamePart -Part "TypeOf$enumToken"
        }
    }

    $baseName = $parts -join "_"
    if ([string]::IsNullOrWhiteSpace($baseName)) {
        return "Object"
    }
    return $baseName
}

function Set-OpenApiObjectSchemaComponentNames {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Candidates
    )

    $usedNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $components = Get-JsonPropertyValue -Object $Document -Name "components"
    if (Test-JsonObjectValue -Value $components) {
        $schemas = Get-JsonPropertyValue -Object $components -Name "schemas"
        if (Test-JsonObjectValue -Value $schemas) {
            foreach ($property in (Get-JsonProperties -Value $schemas)) {
                $null = $usedNames.Add($property.Name)
            }
        }
    }

    foreach ($candidate in $Candidates) {
        $baseName = Get-OpenApiObjectSchemaComponentBaseName `
            -Document $Document `
            -Candidate $candidate
        $name = $baseName
        if (-not $usedNames.Add($name)) {
            $suffix = 2
            do {
                $name = "${baseName}_$suffix"
                $suffix++
            } while (-not $usedNames.Add($name))
        }

        $candidate | Add-Member -NotePropertyName Name -NotePropertyValue $name
        $candidate | Add-Member `
            -NotePropertyName DestinationPointer `
            -NotePropertyValue "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $name)"
    }
}

function ConvertTo-ComponentizedOpenApiValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, object]]$CandidateByPointer,

        [Parameter()]
        [AllowNull()]
        [string]$ComponentRootPointer,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Object schema componentization exceeds the supported depth of $MaximumDepth at '$Pointer'."
    }
    if ($null -eq $Value) {
        return $null
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = $Context
        if ([string]::Equals($Context, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        [object[]]$items = [object[]]::new($Value.Count)
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-ComponentizedOpenApiValue `
                -Value $Value[$index] `
                -Context $itemContext `
                -Pointer "$Pointer/$index" `
                -CandidateByPointer $CandidateByPointer `
                -ComponentRootPointer $ComponentRootPointer `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        $CandidateByPointer.ContainsKey($Pointer) -and
        -not [string]::Equals($Pointer, $ComponentRootPointer, [System.StringComparison]::Ordinal)) {
        return [ordered]@{
            '$ref' = $CandidateByPointer[$Pointer].DestinationPointer
        }
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $result[$property.Name] = ConvertTo-ComponentizedOpenApiValue `
            -Value $property.Value `
            -Context $childContext `
            -Pointer "$Pointer/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -CandidateByPointer $CandidateByPointer `
            -ComponentRootPointer $ComponentRootPointer `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

function Assert-GeneratedOpenApiComponentsAreReferenced {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Candidates
    )

    $referencedPointers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Document)) {
        if ([string]::Equals($occurrence.Kind, "Reference", [System.StringComparison]::Ordinal)) {
            $null = $referencedPointers.Add((Get-CanonicalJsonPointer -Pointer $occurrence.Pointer))
        }
    }

    foreach ($candidate in $Candidates) {
        if (-not $referencedPointers.Contains($candidate.DestinationPointer)) {
            throw "Generated component '$($candidate.Name)' is not referenced."
        }
    }
}

function ConvertTo-ComponentizedOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    [object[]]$candidates = @(Get-InlineOpenApiObjectSchemaCandidates -Document $Document)
    if ($candidates.Count -eq 0) {
        $copy = Copy-JsonValue -Value $Document
        Assert-OpenApiReferencesResolve -Document $copy
        Assert-AdditionalOpenApiReferencesResolve -Document $copy
        return [PSCustomObject]@{
            Document = $copy
            DiscoveredObjectSchemaCount = 0
            GeneratedComponentCount = 0
            RelocationCount = 0
            Candidates = [object[]]@()
        }
    }

    Set-OpenApiObjectSchemaComponentNames -Document $Document -Candidates $candidates
    $candidateByPointer = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    $relocations = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($candidate in $candidates) {
        $candidateByPointer[$candidate.Pointer] = $candidate
        Add-OpenApiPointerRelocation `
            -Relocations $relocations `
            -SourcePrefix $candidate.Pointer `
            -DestinationPrefix $candidate.DestinationPointer
    }

    # Relocate source-authored pointers before inserting generated refs so the
    # latter cannot be rewritten through a source-prefix relocation.
    $relocatedSource = ConvertTo-RelocatedOpenApiPointers `
        -Value $Document `
        -Relocations $relocations
    $componentized = ConvertTo-ComponentizedOpenApiValue `
        -Value $relocatedSource `
        -Context OpenApi `
        -Pointer "#" `
        -CandidateByPointer $candidateByPointer

    $sourceComponents = Get-JsonPropertyValue -Object $componentized -Name "components"
    $sourceSchemas = if (Test-JsonObjectValue -Value $sourceComponents) {
        Get-JsonPropertyValue -Object $sourceComponents -Name "schemas"
    }
    else {
        $null
    }
    $schemas = [ordered]@{}
    if (Test-JsonObjectValue -Value $sourceSchemas) {
        foreach ($property in (Get-JsonProperties -Value $sourceSchemas)) {
            $schemas[$property.Name] = $property.Value
        }
    }

    foreach ($candidate in $candidates) {
        $sourceSchema = Resolve-LocalJsonPointer `
            -Document $relocatedSource `
            -Pointer $candidate.Pointer `
            -Context "component source"
        $schemas[$candidate.Name] = ConvertTo-ComponentizedOpenApiValue `
            -Value $sourceSchema `
            -Context Schema `
            -Pointer $candidate.Pointer `
            -CandidateByPointer $candidateByPointer `
            -ComponentRootPointer $candidate.Pointer
    }

    $newComponents = [ordered]@{ schemas = $schemas }
    if (Test-JsonObjectValue -Value $sourceComponents) {
        foreach ($property in (Get-JsonProperties -Value $sourceComponents)) {
            if ([string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)) {
                continue
            }
            $newComponents[$property.Name] = $property.Value
        }
    }

    $result = [ordered]@{}
    $sawComponents = $false
    foreach ($property in (Get-JsonProperties -Value $componentized)) {
        if ([string]::Equals($property.Name, "components", [System.StringComparison]::Ordinal)) {
            $result[$property.Name] = $newComponents
            $sawComponents = $true
        }
        else {
            $result[$property.Name] = $property.Value
        }
    }
    if (-not $sawComponents) {
        $result["components"] = $newComponents
    }

    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result
    [object[]]$remainingCandidates = @(Get-InlineOpenApiObjectSchemaCandidates -Document $result)
    if ($remainingCandidates.Count -ne 0) {
        throw "Componentized output retained $($remainingCandidates.Count) inline object schema candidates."
    }
    Assert-GeneratedOpenApiComponentsAreReferenced `
        -Document $result `
        -Candidates $candidates

    return [PSCustomObject]@{
        Document = $result
        DiscoveredObjectSchemaCount = $candidates.Count
        GeneratedComponentCount = $candidates.Count
        RelocationCount = $relocations.Count
        Candidates = $candidates
    }
}

function Get-DefaultComponentizedOpenApiPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullInputPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullInputPath)
    $fileName = [System.IO.Path]::GetFileName($fullInputPath)
    if ($fileName.EndsWith(".componentized.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".componentized.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullInputPath
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$baseName.componentized.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $baseName = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$baseName.componentized.json"
    }
    return Join-Path $directory "$fileName.componentized.json"
}

function Resolve-ComponentizerRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:ComponentizerRepositoryRoot $Path))
}

function Invoke-OpenApiObjectSchemaComponentizer {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$OutputPath
    )

    $usingCanonicalInput = [string]::IsNullOrWhiteSpace($InputPath)
    if ($usingCanonicalInput) {
        $InputPath = $script:DefaultComponentizerInputPath
    }
    else {
        $InputPath = Resolve-ComponentizerRepositoryPath -Path $InputPath
    }

    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The OpenAPI input path is not a file: $resolvedInputPath"
    }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = if ($usingCanonicalInput) {
            $script:DefaultComponentizerOutputPath
        }
        else {
            Get-DefaultComponentizedOpenApiPath -InputPath $resolvedInputPath
        }
    }
    else {
        $OutputPath = Resolve-ComponentizerRepositoryPath -Path $OutputPath
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-ComponentizedOpenApiDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Inline object schemas discovered: $($result.DiscoveredObjectSchemaCount)"
    Write-Host "Schema components generated: $($result.GeneratedComponentCount)"
    Write-Host "JSON Pointer prefixes relocated: $($result.RelocationCount)"

    return $result
}

# Dot-sourcing exposes conversion functions to tests without running the CLI.
if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiObjectSchemaComponentizer `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}
