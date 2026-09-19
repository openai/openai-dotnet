<#
.SYNOPSIS
    Extracts one route from an OpenAPI JSON document.

.DESCRIPTION
    Reads an OpenAPI document, groups routes by their first path segment, and
    guides the user through selecting a feature area, route, and extraction
    mode. Exact mode keeps references and copies their component definitions.
    Inlined mode expands references except where a reference is needed to
    represent a recursive schema, and flattens all allOf compositions.

.PARAMETER SpecificationPath
    The OpenAPI JSON document to read. Relative paths are resolved from the
    repository root.

.PARAMETER Route
    The exact key under the OpenAPI paths object. When omitted, the script
    displays interactive feature-area and route menus.

.PARAMETER Mode
    The extraction mode: Exact or Inlined. When omitted, the script displays
    an interactive mode menu.

.PARAMETER OutputPath
    The destination JSON file. When omitted, the script writes beneath
    artifacts/openapi-extracts using a name derived from the route and mode.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1
    Interactively selects and extracts a route.

.EXAMPLE
    ./scripts/Extract-OpenApiRoute.ps1 -Route "/audio/speech" -Mode Exact
    Extracts /audio/speech without prompting for a route or mode.
#>

#Requires -Version 7.0

[CmdletBinding()]
param(
    [Parameter()]
    [string]$SpecificationPath,

    [Parameter()]
    [string]$Route,

    [Parameter()]
    [ValidateSet("Exact", "Inlined")]
    [string]$Mode,

    [Parameter()]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

# Returns an object's JSON properties without treating arrays or scalar values
# as objects. ConvertFrom-Json represents JSON objects as PSCustomObject values.
function Get-JsonProperties {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    if ($Value -is [System.Management.Automation.PSCustomObject]) {
        return @($Value.PSObject.Properties)
    }

    if ($Value -is [System.Collections.IDictionary]) {
        return @($Value.GetEnumerator() | ForEach-Object {
            [PSCustomObject]@{
                Name = [string]$_.Key
                Value = $_.Value
            }
        })
    }

    return @()
}

# Gets a named property without PowerShell's case-insensitive member lookup.
function Get-JsonPropertyValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Object,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    foreach ($property in (Get-JsonProperties -Value $Object)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            return ,$property.Value
        }
    }

    return $null
}

# Tests for a named property using JSON's case-sensitive name semantics.
function Test-JsonProperty {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Object,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    foreach ($property in (Get-JsonProperties -Value $Object)) {
        if ([string]::Equals($property.Name, $Name, [System.StringComparison]::Ordinal)) {
            return $true
        }
    }

    return $false
}

# Reads and performs the structural checks needed before route discovery.
function Read-OpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "OpenAPI specification was not found: $Path"
    }

    try {
        $document = Get-Content -LiteralPath $Path -Raw -Encoding utf8 | ConvertFrom-Json -Depth 100
    }
    catch {
        throw "Could not parse OpenAPI JSON at '$Path': $($_.Exception.Message)"
    }

    foreach ($requiredProperty in @("openapi", "info", "paths")) {
        if (-not (Test-JsonProperty -Object $document -Name $requiredProperty)) {
            throw "OpenAPI specification '$Path' is missing the required '$requiredProperty' property."
        }
    }

    $paths = Get-JsonPropertyValue -Object $document -Name "paths"
    if ((Get-JsonProperties -Value $paths).Count -eq 0) {
        throw "OpenAPI specification '$Path' does not contain any routes under 'paths'."
    }

    return $document
}

# Converts a route's first path segment into a stable key and readable label.
# Query suffixes affect the literal route but do not create a new feature area.
function Get-RouteFeatureArea {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Route
    )

    $firstSegment = (($Route.TrimStart("/") -split "/", 2)[0] -split "\?", 2)[0]
    if ([string]::IsNullOrWhiteSpace($firstSegment)) {
        throw "Route '$Route' does not have a feature-area path segment."
    }

    $words = ($firstSegment -replace "[_-]+", " ").ToLowerInvariant()
    $label = [System.Globalization.CultureInfo]::InvariantCulture.TextInfo.ToTitleCase($words)

    return [PSCustomObject]@{
        Key = $firstSegment.ToLowerInvariant()
        Label = $label
    }
}

# Builds one catalog entry per literal path key. Only the paths object is read,
# so top-level webhook definitions cannot accidentally enter the route menu.
function Get-OpenApiRouteCatalog {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $paths = Get-JsonPropertyValue -Object $Document -Name "paths"
    $catalog = foreach ($pathProperty in (Get-JsonProperties -Value $paths)) {
        $featureArea = Get-RouteFeatureArea -Route $pathProperty.Name
        [PSCustomObject]@{
            FeatureKey = $featureArea.Key
            FeatureArea = $featureArea.Label
            Route = $pathProperty.Name
            PathItem = $pathProperty.Value
        }
    }

    return @($catalog | Sort-Object -Property @(
        @{ Expression = "FeatureArea"; Ascending = $true },
        @{ Expression = "Route"; Ascending = $true }
    ))
}

# Displays a one-based menu and keeps prompting until the user enters a valid
# number. InputProvider makes the same behavior directly testable without stdin.
function Read-NumberedSelection {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,

        [Parameter(Mandatory = $true)]
        [object[]]$Options,

        [Parameter(Mandatory = $true)]
        [scriptblock]$GetLabel,

        [Parameter()]
        [scriptblock]$InputProvider = { param($Prompt) Read-Host -Prompt $Prompt }
    )

    if ($Options.Count -eq 0) {
        throw "The '$Title' menu does not contain any options."
    }

    Write-Host ""
    Write-Host $Title
    for ($index = 0; $index -lt $Options.Count; $index++) {
        Write-Host "$($index + 1). $(& $GetLabel $Options[$index])"
    }

    while ($true) {
        $rawSelection = & $InputProvider "Selection"
        $selection = 0
        if ([int]::TryParse($rawSelection, [ref]$selection) -and $selection -ge 1 -and $selection -le $Options.Count) {
            return $Options[$selection - 1]
        }

        Write-Warning "Enter a number from 1 to $($Options.Count)."
    }
}

# Selects an exact path key from parameters or the two interactive route menus.
function Select-OpenApiRoute {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Catalog,

        [Parameter()]
        [string]$RequestedRoute
    )

    if (-not [string]::IsNullOrWhiteSpace($RequestedRoute)) {
        $match = @($Catalog | Where-Object {
            [string]::Equals($_.Route, $RequestedRoute, [System.StringComparison]::Ordinal)
        })
        if ($match.Count -ne 1) {
            throw "Route '$RequestedRoute' was not found in the OpenAPI paths object."
        }

        return $match[0]
    }

    $featureAreas = @($Catalog | Group-Object -Property FeatureKey | ForEach-Object {
        [PSCustomObject]@{
            Key = $_.Name
            Label = $_.Group[0].FeatureArea
            Routes = @($_.Group | Sort-Object -Property Route)
        }
    } | Sort-Object -Property Label)

    $selectedFeature = Read-NumberedSelection `
        -Title "Select a feature area" `
        -Options $featureAreas `
        -GetLabel { param($Option) $Option.Label }

    return Read-NumberedSelection `
        -Title "Select a route" `
        -Options $selectedFeature.Routes `
        -GetLabel { param($Option) $Option.Route }
}

# Selects Exact or Inlined while preserving the menu wording from the command's
# public contract.
function Select-ExtractionMode {
    param(
        [Parameter()]
        [string]$RequestedMode
    )

    if (-not [string]::IsNullOrWhiteSpace($RequestedMode)) {
        if ([string]::Equals($RequestedMode, "Exact", [System.StringComparison]::OrdinalIgnoreCase)) {
            return "Exact"
        }
        if ([string]::Equals($RequestedMode, "Inlined", [System.StringComparison]::OrdinalIgnoreCase)) {
            return "Inlined"
        }
        throw "Extraction mode '$RequestedMode' is invalid. Choose Exact or Inlined."
    }

    return Read-NumberedSelection `
        -Title "Select an extraction mode" `
        -Options @("Exact", "Inlined") `
        -GetLabel { param($Option) $Option }
}

# Resolves paths supplied on the command line relative to the repository root.
function Resolve-RepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string]$RepositoryRoot
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $RepositoryRoot $Path))
}

# Distinguishes JSON objects from arrays and scalar values after deserialization.
function Test-JsonObjectValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    return $Value -is [System.Management.Automation.PSCustomObject] -or
        $Value -is [System.Collections.IDictionary]
}

# Distinguishes JSON arrays from strings, objects, and scalar values.
function Test-JsonArrayValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value
    )

    return $null -ne $Value -and
        $Value -is [System.Collections.IList] -and
        $Value -isnot [string] -and
        $Value -isnot [System.Collections.IDictionary]
}

# Produces an independent ordered copy so extraction never mutates the loaded
# source document. The depth guard turns unexpectedly pathological JSON into a
# clear failure instead of a PowerShell stack overflow.
function Copy-JsonValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "JSON nesting exceeds the supported depth of $MaximumDepth."
    }

    if ($null -eq $Value) {
        return $null
    }

    if (Test-JsonObjectValue -Value $Value) {
        $copy = [ordered]@{}
        foreach ($property in (Get-JsonProperties -Value $Value)) {
            $copy[$property.Name] = Copy-JsonValue `
                -Value $property.Value `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return $copy
    }

    if (Test-JsonArrayValue -Value $Value) {
        [object[]]$copy = @(
            foreach ($item in $Value) {
                Copy-JsonValue -Value $item -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
            }
        )
        return ,$copy
    }

    return $Value
}

# Uses ordinal comparison so output ordering is stable across machine cultures.
function Sort-OrdinalStrings {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [string[]]$Values
    )

    [string[]]$sortedValues = @($Values)
    [System.Array]::Sort($sortedValues, [System.StringComparer]::Ordinal)
    return ,$sortedValues
}

# Compares JSON values structurally. Object member order is insignificant,
# array order is significant, and scalar types and values must match exactly.
function Test-JsonValuesEqual {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Left,

        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Right,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "JSON comparison exceeds the supported depth of $MaximumDepth."
    }

    if ($null -eq $Left -or $null -eq $Right) {
        return $null -eq $Left -and $null -eq $Right
    }

    $leftIsObject = Test-JsonObjectValue -Value $Left
    $rightIsObject = Test-JsonObjectValue -Value $Right
    if ($leftIsObject -or $rightIsObject) {
        if (-not ($leftIsObject -and $rightIsObject)) {
            return $false
        }

        $leftProperties = @(Get-JsonProperties -Value $Left)
        $rightProperties = @(Get-JsonProperties -Value $Right)
        if ($leftProperties.Count -ne $rightProperties.Count) {
            return $false
        }

        foreach ($property in $leftProperties) {
            if (-not (Test-JsonProperty -Object $Right -Name $property.Name)) {
                return $false
            }
            if (-not (Test-JsonValuesEqual `
                -Left $property.Value `
                -Right (Get-JsonPropertyValue -Object $Right -Name $property.Name) `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth)) {
                return $false
            }
        }

        return $true
    }

    $leftIsArray = Test-JsonArrayValue -Value $Left
    $rightIsArray = Test-JsonArrayValue -Value $Right
    if ($leftIsArray -or $rightIsArray) {
        if (-not ($leftIsArray -and $rightIsArray) -or $Left.Count -ne $Right.Count) {
            return $false
        }

        for ($index = 0; $index -lt $Left.Count; $index++) {
            if (-not (Test-JsonValuesEqual `
                -Left $Left[$index] `
                -Right $Right[$index] `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth)) {
                return $false
            }
        }

        return $true
    }

    if ($Left.GetType() -ne $Right.GetType()) {
        return $false
    }
    if ($Left -is [string]) {
        return [string]::Equals($Left, $Right, [System.StringComparison]::Ordinal)
    }

    return [object]::Equals($Left, $Right)
}

# Intersects enum arrays using structural JSON equality while preserving the
# first array's order and removing duplicate values.
function Get-JsonEnumIntersection {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Left,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Right
    )

    $intersection = [System.Collections.Generic.List[object]]::new()
    foreach ($leftValue in $Left) {
        $existsInRight = $false
        foreach ($rightValue in $Right) {
            if (Test-JsonValuesEqual -Left $leftValue -Right $rightValue) {
                $existsInRight = $true
                break
            }
        }
        if (-not $existsInRight) {
            continue
        }

        $alreadyAdded = $false
        foreach ($existingValue in $intersection) {
            if (Test-JsonValuesEqual -Left $leftValue -Right $existingValue) {
                $alreadyAdded = $true
                break
            }
        }
        if (-not $alreadyAdded) {
            $intersection.Add($leftValue)
        }
    }

    return ,([object[]]$intersection)
}

# Combines expanded allOf definitions without weakening their constraints.
# Documentation annotations use later-definition precedence; every validating
# keyword must either be unique or structurally identical across definitions.
function Merge-AllOfSchemaDefinitions {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$Definitions,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [string[]]$DefinitionBreadcrumbs,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    if ($Definitions.Count -eq 0) {
        throw "The allOf array at '$Breadcrumb/allOf' must contain at least one object definition."
    }
    if ($Definitions.Count -ne $DefinitionBreadcrumbs.Count) {
        throw "Internal error: allOf definitions and breadcrumbs have different lengths at '$Breadcrumb'."
    }

    $annotationKeywords = @("title", "description", '$comment', "example", "examples")
    $keywordValues = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    $keywordSources = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    $keywordOrder = [System.Collections.Generic.List[string]]::new()
    $propertyValues = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )
    $propertySources = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    $requiredNames = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $sawProperties = $false
    $sawRequired = $false

    for ($definitionIndex = 0; $definitionIndex -lt $Definitions.Count; $definitionIndex++) {
        $definition = $Definitions[$definitionIndex]
        $definitionBreadcrumb = $DefinitionBreadcrumbs[$definitionIndex]
        if (-not (Test-JsonObjectValue -Value $definition)) {
            throw "The allOf branch at '$definitionBreadcrumb' must be an object definition."
        }

        foreach ($property in (Get-JsonProperties -Value $definition)) {
            if ([string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
                continue
            }
            if ([string]::Equals($property.Name, "allOf", [System.StringComparison]::Ordinal)) {
                throw "Nested allOf at '$definitionBreadcrumb/allOf' was not flattened before merging."
            }

            if ([string]::Equals($property.Name, "properties", [System.StringComparison]::Ordinal)) {
                if (-not (Test-JsonObjectValue -Value $property.Value)) {
                    throw "The properties keyword at '$definitionBreadcrumb/properties' must be an object."
                }
                $sawProperties = $true
                foreach ($schemaProperty in (Get-JsonProperties -Value $property.Value)) {
                    $schemaPropertyBreadcrumb = "$definitionBreadcrumb/properties/$(ConvertTo-JsonPointerToken -Token $schemaProperty.Name)"
                    if ($propertyValues.ContainsKey($schemaProperty.Name)) {
                        if (-not (Test-JsonValuesEqual `
                            -Left $propertyValues[$schemaProperty.Name] `
                            -Right $schemaProperty.Value)) {
                            $propertyBreadcrumb = "$Breadcrumb/properties/$(ConvertTo-JsonPointerToken -Token $schemaProperty.Name)"
                            $propertyValues[$schemaProperty.Name] = Merge-AllOfSchemaDefinitions `
                                -Definitions @(
                                    $propertyValues[$schemaProperty.Name],
                                    $schemaProperty.Value
                                ) `
                                -DefinitionBreadcrumbs @(
                                    $propertySources[$schemaProperty.Name],
                                    $schemaPropertyBreadcrumb
                                ) `
                                -Breadcrumb $propertyBreadcrumb
                            $propertySources[$schemaProperty.Name] = "$propertyBreadcrumb (merged)"
                        }
                        continue
                    }

                    $propertyValues[$schemaProperty.Name] = $schemaProperty.Value
                    $propertySources[$schemaProperty.Name] = $schemaPropertyBreadcrumb
                }
                continue
            }

            if ([string]::Equals($property.Name, "required", [System.StringComparison]::Ordinal)) {
                if (-not (Test-JsonArrayValue -Value $property.Value)) {
                    throw "The required keyword at '$definitionBreadcrumb/required' must be an array."
                }
                $sawRequired = $true
                foreach ($requiredName in $property.Value) {
                    if ($requiredName -isnot [string]) {
                        throw "Every required name at '$definitionBreadcrumb/required' must be a string."
                    }
                    $null = $requiredNames.Add($requiredName)
                }
                continue
            }

            if ($annotationKeywords -ccontains $property.Name) {
                if (-not $keywordValues.ContainsKey($property.Name)) {
                    $keywordOrder.Add($property.Name)
                }
                $keywordValues[$property.Name] = $property.Value
                $keywordSources[$property.Name] = "$definitionBreadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
                continue
            }

            if ($keywordValues.ContainsKey($property.Name)) {
                if (-not (Test-JsonValuesEqual `
                    -Left $keywordValues[$property.Name] `
                    -Right $property.Value)) {
                    $currentBreadcrumb = "$definitionBreadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
                    if ([string]::Equals($property.Name, "enum", [System.StringComparison]::Ordinal) -and
                        (Test-JsonArrayValue -Value $keywordValues[$property.Name]) -and
                        (Test-JsonArrayValue -Value $property.Value)) {
                        $intersection = Get-JsonEnumIntersection `
                            -Left @($keywordValues[$property.Name]) `
                            -Right @($property.Value)
                        if ($intersection.Count -eq 0) {
                            throw "Cannot flatten allOf at '$Breadcrumb': enum constraints at '$($keywordSources[$property.Name])' and '$currentBreadcrumb' have no common values."
                        }
                        $keywordValues[$property.Name] = $intersection
                        $keywordSources[$property.Name] = "$Breadcrumb/enum (merged)"
                        continue
                    }
                    throw "Cannot flatten allOf at '$Breadcrumb': keyword '$($property.Name)' has incompatible definitions at '$($keywordSources[$property.Name])' and '$currentBreadcrumb'."
                }
                continue
            }

            $keywordValues[$property.Name] = $property.Value
            $keywordSources[$property.Name] = "$definitionBreadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
            $keywordOrder.Add($property.Name)
        }
    }

    $mergedDefinition = [ordered]@{}
    foreach ($keywordName in $keywordOrder) {
        $mergedDefinition[$keywordName] = $keywordValues[$keywordName]
    }

    if ($sawProperties) {
        [string[]]$requiredPropertyNames = @($propertyValues.Keys | Where-Object {
            $requiredNames.Contains($_)
        })
        [string[]]$optionalPropertyNames = @($propertyValues.Keys | Where-Object {
            -not $requiredNames.Contains($_)
        })
        $orderedProperties = [ordered]@{}
        foreach ($propertyName in @(
            (Sort-OrdinalStrings -Values $requiredPropertyNames)
            (Sort-OrdinalStrings -Values $optionalPropertyNames)
        )) {
            $orderedProperties[$propertyName] = $propertyValues[$propertyName]
        }
        $mergedDefinition["properties"] = $orderedProperties
    }

    if ($sawRequired) {
        [string[]]$sortedRequiredNames = Sort-OrdinalStrings -Values @($requiredNames)
        $mergedDefinition["required"] = @($sortedRequiredNames)
    }

    return $mergedDefinition
}

# Removes x-oaiMeta recursively and applies the requested ordering only to JSON
# Schema property maps. Other object key order and every array order are kept.
function ConvertTo-NormalizedOpenApiValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "JSON normalization exceeds the supported depth of $MaximumDepth."
    }

    if ($null -eq $Value) {
        return $null
    }

    if (Test-JsonArrayValue -Value $Value) {
        [object[]]$normalizedItems = @(
            foreach ($item in $Value) {
                ConvertTo-NormalizedOpenApiValue `
                    -Value $item `
                    -Depth ($Depth + 1) `
                    -MaximumDepth $MaximumDepth
            }
        )
        return ,$normalizedItems
    }

    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $requiredNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $requiredValue = Get-JsonPropertyValue -Object $Value -Name "required"
    if (Test-JsonArrayValue -Value $requiredValue) {
        foreach ($requiredName in $requiredValue) {
            if ($requiredName -is [string]) {
                $null = $requiredNames.Add($requiredName)
            }
        }
    }

    $normalizedObject = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
            continue
        }

        if ([string]::Equals($property.Name, "properties", [System.StringComparison]::Ordinal) -and
            (Test-JsonObjectValue -Value $property.Value)) {
            $propertyEntries = @(Get-JsonProperties -Value $property.Value)
            [string[]]$requiredPropertyNames = @($propertyEntries | Where-Object {
                $requiredNames.Contains($_.Name)
            } | ForEach-Object Name)
            [string[]]$optionalPropertyNames = @($propertyEntries | Where-Object {
                -not $requiredNames.Contains($_.Name)
            } | ForEach-Object Name)

            $orderedProperties = [ordered]@{}
            foreach ($propertyName in @(
                (Sort-OrdinalStrings -Values $requiredPropertyNames)
                (Sort-OrdinalStrings -Values $optionalPropertyNames)
            )) {
                $propertyValue = Get-JsonPropertyValue -Object $property.Value -Name $propertyName
                $orderedProperties[$propertyName] = ConvertTo-NormalizedOpenApiValue `
                    -Value $propertyValue `
                    -Depth ($Depth + 1) `
                    -MaximumDepth $MaximumDepth
            }
            $normalizedObject[$property.Name] = $orderedProperties
            continue
        }

        $normalizedObject[$property.Name] = ConvertTo-NormalizedOpenApiValue `
            -Value $property.Value `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $normalizedObject
}

# Encodes one RFC 6901 token for use in a JSON Pointer.
function ConvertTo-JsonPointerToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Token
    )

    return $Token.Replace("~", "~0").Replace("/", "~1")
}

# Decodes one RFC 6901 token and rejects malformed tilde escapes.
function ConvertFrom-JsonPointerToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Token
    )

    if ($Token -match "~(?![01])") {
        throw "JSON Pointer token '$Token' contains an invalid escape sequence."
    }

    return $Token.Replace("~1", "/").Replace("~0", "~")
}

# Parses the URI-fragment form of a local JSON Pointer into decoded tokens.
function Get-JsonPointerTokens {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer
    )

    if (-not $Pointer.StartsWith("#", [System.StringComparison]::Ordinal)) {
        throw "Reference '$Pointer' is external. Only local JSON Pointers are supported."
    }

    if ([string]::Equals($Pointer, "#", [System.StringComparison]::Ordinal)) {
        return ,([string[]]@())
    }

    $fragment = [System.Uri]::UnescapeDataString($Pointer.Substring(1))
    if (-not $fragment.StartsWith("/", [System.StringComparison]::Ordinal)) {
        throw "Reference '$Pointer' is not a valid local JSON Pointer."
    }

    [string[]]$tokens = @(
        foreach ($token in $fragment.Substring(1).Split([char]"/", [System.StringSplitOptions]::None)) {
            ConvertFrom-JsonPointerToken -Token $token
        }
    )
    return ,$tokens
}

# Re-encodes a pointer into one canonical spelling for stack and set membership.
function Get-CanonicalJsonPointer {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer
    )

    $tokens = Get-JsonPointerTokens -Pointer $Pointer
    if ($tokens.Count -eq 0) {
        return "#"
    }

    return "#/" + (($tokens | ForEach-Object { ConvertTo-JsonPointerToken -Token $_ }) -join "/")
}

# Resolves a local JSON Pointer with case-sensitive object lookup and explicit
# diagnostics for missing members and invalid array indexes.
function Resolve-LocalJsonPointer {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter()]
        [string]$Context = "reference"
    )

    $tokens = Get-JsonPointerTokens -Pointer $Pointer
    $current = $Document
    $resolvedTokens = [System.Collections.Generic.List[string]]::new()

    foreach ($token in $tokens) {
        if (Test-JsonObjectValue -Value $current) {
            if (-not (Test-JsonProperty -Object $current -Name $token)) {
                $location = "#/" + (($resolvedTokens | ForEach-Object {
                    ConvertTo-JsonPointerToken -Token $_
                }) -join "/")
                throw "Could not resolve $Context '$Pointer': object at '$location' has no '$token' property."
            }
            $current = Get-JsonPropertyValue -Object $current -Name $token
        }
        elseif (Test-JsonArrayValue -Value $current) {
            $index = 0
            if (-not [int]::TryParse(
                $token,
                [System.Globalization.NumberStyles]::None,
                [System.Globalization.CultureInfo]::InvariantCulture,
                [ref]$index
            ) -or $index -lt 0 -or $index -ge $current.Count) {
                throw "Could not resolve $Context '$Pointer': '$token' is not a valid array index."
            }
            $current = $current[$index]
        }
        else {
            throw "Could not resolve $Context '$Pointer': '$token' attempts to traverse a scalar value."
        }

        $resolvedTokens.Add($token)
    }

    return $current
}

# Returns the component entry containing a component pointer. Nested pointers
# still cause the complete owning component entry to be copied in Exact mode.
function Get-ComponentReferenceRoot {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer
    )

    $tokens = Get-JsonPointerTokens -Pointer $Pointer
    if ($tokens.Count -lt 3 -or
        -not [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal)) {
        throw "Reference '$Pointer' does not target an OpenAPI component."
    }

    $rootPointer = "#/components/{0}/{1}" -f @(
        (ConvertTo-JsonPointerToken -Token $tokens[1]),
        (ConvertTo-JsonPointerToken -Token $tokens[2])
    )

    return [PSCustomObject]@{
        Section = $tokens[1]
        Name = $tokens[2]
        Pointer = $rootPointer
    }
}

# Finds $ref properties and discriminator mapping pointers at any depth. Mapping
# strings are references semantically even though OpenAPI does not spell them as
# $ref objects.
function Get-OpenApiReferenceOccurrences {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [string]$Breadcrumb = "#",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Reference discovery exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }

    if ($null -eq $Value) {
        return
    }

    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Get-OpenApiReferenceOccurrences `
                -Value $Value[$index] `
                -Breadcrumb "$Breadcrumb/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }

    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    if (Test-JsonProperty -Object $Value -Name '$ref') {
        $pointer = Get-JsonPropertyValue -Object $Value -Name '$ref'
        if ($pointer -isnot [string]) {
            throw "The `$ref value at '$Breadcrumb' must be a string."
        }
        [PSCustomObject]@{
            Kind = "Reference"
            Pointer = $pointer
            Breadcrumb = "$Breadcrumb/`$ref"
        }
    }

    $discriminator = Get-JsonPropertyValue -Object $Value -Name "discriminator"
    if (Test-JsonObjectValue -Value $discriminator) {
        $mapping = Get-JsonPropertyValue -Object $discriminator -Name "mapping"
        if (Test-JsonObjectValue -Value $mapping) {
            foreach ($mappingProperty in (Get-JsonProperties -Value $mapping)) {
                if ($mappingProperty.Value -isnot [string]) {
                    throw "Discriminator mapping '$($mappingProperty.Name)' at '$Breadcrumb' must be a string."
                }
                [PSCustomObject]@{
                    Kind = "DiscriminatorMapping"
                    Pointer = $mappingProperty.Value
                    Breadcrumb = "$Breadcrumb/discriminator/mapping/$(ConvertTo-JsonPointerToken -Token $mappingProperty.Name)"
                }
            }
        }
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
            continue
        }

        Get-OpenApiReferenceOccurrences `
            -Value $property.Value `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Ensures every $ref and discriminator mapping pointer retained in an output
# document resolves locally. $recursiveRef is intentionally scoped separately.
function Assert-OpenApiReferencesResolve {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Document)) {
        $null = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $occurrence.Pointer `
            -Context "$($occurrence.Kind) at $($occurrence.Breadcrumb)"
    }
}

# Computes the complete set of component entries needed by a selected path item.
# A queue avoids recursive graph walking, and a visited set makes component cycles
# finite while still validating every reference at the point it is discovered.
function Get-TransitiveComponentReferences {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Value
    )

    $pending = [System.Collections.Generic.Queue[object]]::new()
    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Value)) {
        $pending.Enqueue($occurrence)
    }

    $componentRoots = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    while ($pending.Count -gt 0) {
        $occurrence = $pending.Dequeue()
        $canonicalPointer = Get-CanonicalJsonPointer -Pointer $occurrence.Pointer
        $null = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $canonicalPointer `
            -Context "$($occurrence.Kind) at $($occurrence.Breadcrumb)"

        $componentRoot = Get-ComponentReferenceRoot -Pointer $canonicalPointer
        if (-not $componentRoots.Add($componentRoot.Pointer)) {
            continue
        }

        $componentValue = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $componentRoot.Pointer `
            -Context "component dependency"
        foreach ($nestedOccurrence in @(Get-OpenApiReferenceOccurrences -Value $componentValue)) {
            $pending.Enqueue($nestedOccurrence)
        }
    }

    return ,$componentRoots
}

# Finds direct component-reference branches used by a discriminator. Mapping
# values can then point at the branch's new local location after inlining.
function Get-DiscriminatorBranchTargets {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Object,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    $targets = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )

    foreach ($compositionName in @("oneOf", "anyOf")) {
        $branches = Get-JsonPropertyValue -Object $Object -Name $compositionName
        if (-not (Test-JsonArrayValue -Value $branches)) {
            continue
        }

        for ($index = 0; $index -lt $branches.Count; $index++) {
            $branch = $branches[$index]
            if (-not (Test-JsonObjectValue -Value $branch) -or
                -not (Test-JsonProperty -Object $branch -Name '$ref')) {
                continue
            }

            $branchReference = Get-JsonPropertyValue -Object $branch -Name '$ref'
            if ($branchReference -isnot [string]) {
                throw "The `$ref value in '$Breadcrumb/$compositionName/$index' must be a string."
            }

            $canonicalPointer = Get-CanonicalJsonPointer -Pointer $branchReference
            if (-not $targets.ContainsKey($canonicalPointer)) {
                $targets[$canonicalPointer] = [System.Collections.Generic.List[string]]::new()
            }
            $targets[$canonicalPointer].Add(
                "$Breadcrumb/$compositionName/$index"
            )
        }
    }

    return $targets
}

# Rewrites component-valued discriminator mappings to the corresponding local
# inlined branch. Ambiguous or unmatched mappings would be misleading, so they
# fail extraction rather than leaving a dangling pointer.
function Expand-OpenApiDiscriminator {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Discriminator,

        [Parameter(Mandatory = $true)]
        [object]$ContainingObject,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $branchTargets = Get-DiscriminatorBranchTargets `
        -Object $ContainingObject `
        -Breadcrumb $Breadcrumb
    $expandedDiscriminator = [ordered]@{}

    foreach ($property in (Get-JsonProperties -Value $Discriminator)) {
        if ([string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
            continue
        }

        if ([string]::Equals($property.Name, "mapping", [System.StringComparison]::Ordinal) -and
            (Test-JsonObjectValue -Value $property.Value)) {
            $expandedMapping = [ordered]@{}
            foreach ($mappingProperty in (Get-JsonProperties -Value $property.Value)) {
                if ($mappingProperty.Value -isnot [string]) {
                    throw "Discriminator mapping '$($mappingProperty.Name)' at '$Breadcrumb' must be a string."
                }

                if ($mappingProperty.Value.StartsWith("#/components/", [System.StringComparison]::Ordinal)) {
                    $canonicalPointer = Get-CanonicalJsonPointer -Pointer $mappingProperty.Value
                    if (-not $branchTargets.ContainsKey($canonicalPointer) -or
                        $branchTargets[$canonicalPointer].Count -ne 1) {
                        throw "Discriminator mapping '$($mappingProperty.Name)' at '$Breadcrumb' does not identify exactly one inlined branch for '$canonicalPointer'."
                    }
                    $expandedMapping[$mappingProperty.Name] = $branchTargets[$canonicalPointer][0]
                }
                else {
                    $expandedMapping[$mappingProperty.Name] = $mappingProperty.Value
                }
            }
            $expandedDiscriminator[$property.Name] = $expandedMapping
            continue
        }

        $expandedDiscriminator[$property.Name] = Expand-OpenApiReferences `
            -Value $property.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/discriminator/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $expandedDiscriminator
}

# Returns every meaningful sibling of a $ref property. x-oaiMeta is discarded at
# this boundary so it cannot influence traversal or dependency collection.
function Get-ReferenceSiblingProperties {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ReferenceObject
    )

    [object[]]$siblings = @(Get-JsonProperties -Value $ReferenceObject | Where-Object {
        -not [string]::Equals($_.Name, '$ref', [System.StringComparison]::Ordinal) -and
        -not [string]::Equals($_.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)
    })
    return ,$siblings
}

# Keeps a reference only when its target is already active, then independently
# expands any sibling keywords attached to that recursive occurrence.
function Expand-CycleReferenceObject {
    param(
        [Parameter(Mandatory = $true)]
        [string]$CanonicalPointer,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$SiblingProperties,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $null = $CycleReferences.Add($CanonicalPointer)

    $cycleObject = [ordered]@{ '$ref' = $CanonicalPointer }
    foreach ($siblingProperty in $SiblingProperties) {
        $cycleObject[$siblingProperty.Name] = Expand-OpenApiReferences `
            -Value $siblingProperty.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $siblingProperty.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $cycleObject
}

# Combines a schema reference target with its siblings without introducing
# allOf. The merger rejects incompatible validating constraints.
function Expand-SchemaReferenceWithSiblings {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Target,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$SiblingProperties,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $expandedTarget = Expand-OpenApiReferences `
        -Value $Target `
        -Document $Document `
        -ActiveReferences $ActiveReferences `
        -CycleReferences $CycleReferences `
        -Breadcrumb $Breadcrumb `
        -Depth ($Depth + 1) `
        -MaximumDepth $MaximumDepth

    $siblingObject = [ordered]@{}
    foreach ($siblingProperty in $SiblingProperties) {
        $siblingObject[$siblingProperty.Name] = $siblingProperty.Value
    }

    $expandedSiblings = Expand-OpenApiReferences `
        -Value $siblingObject `
        -Document $Document `
        -ActiveReferences $ActiveReferences `
        -CycleReferences $CycleReferences `
        -Breadcrumb $Breadcrumb `
        -Depth ($Depth + 1) `
        -MaximumDepth $MaximumDepth

    return Merge-AllOfSchemaDefinitions `
        -Definitions @($expandedTarget, $expandedSiblings) `
        -DefinitionBreadcrumbs @(
            "$Breadcrumb (referenced target)",
            "$Breadcrumb (reference siblings)"
        ) `
        -Breadcrumb $Breadcrumb
}

# Applies Reference Object siblings after a non-schema component has been
# expanded. OpenAPI allows summary and description siblings to override values.
function Merge-ExpandedReferenceObject {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ExpandedTarget,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [object[]]$SiblingProperties,

        [Parameter(Mandatory = $true)]
        [string]$CanonicalPointer,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    if (-not (Test-JsonObjectValue -Value $ExpandedTarget)) {
        throw "Reference '$CanonicalPointer' at '$Breadcrumb' has siblings but does not resolve to an object."
    }

    $mergedTarget = [ordered]@{}
    foreach ($targetProperty in (Get-JsonProperties -Value $ExpandedTarget)) {
        $mergedTarget[$targetProperty.Name] = $targetProperty.Value
    }

    foreach ($siblingProperty in $SiblingProperties) {
        $mergedTarget[$siblingProperty.Name] = Expand-OpenApiReferences `
            -Value $siblingProperty.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $siblingProperty.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $mergedTarget
}

# Expands one object containing $ref, delegating its three distinct outcomes:
# cycle retention, schema conjunction, or Reference Object override merging.
function Expand-ComponentReferenceObject {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ReferenceObject,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $reference = Get-JsonPropertyValue -Object $ReferenceObject -Name '$ref'
    if ($reference -isnot [string]) {
        throw "The `$ref value at '$Breadcrumb' must be a string."
    }

    $canonicalPointer = Get-CanonicalJsonPointer -Pointer $reference
    $componentRoot = Get-ComponentReferenceRoot -Pointer $canonicalPointer
    $target = Resolve-LocalJsonPointer `
        -Document $Document `
        -Pointer $canonicalPointer `
        -Context "reference at $Breadcrumb"
    $siblingProperties = Get-ReferenceSiblingProperties -ReferenceObject $ReferenceObject

    if ($ActiveReferences.Contains($canonicalPointer)) {
        return Expand-CycleReferenceObject `
            -CanonicalPointer $canonicalPointer `
            -SiblingProperties $siblingProperties `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth $Depth `
            -MaximumDepth $MaximumDepth
    }

    $null = $ActiveReferences.Add($canonicalPointer)
    try {
        if ([string]::Equals($componentRoot.Section, "schemas", [System.StringComparison]::Ordinal) -and
            $siblingProperties.Count -gt 0) {
            return Expand-SchemaReferenceWithSiblings `
                -Target $target `
                -SiblingProperties $siblingProperties `
                -Document $Document `
                -ActiveReferences $ActiveReferences `
                -CycleReferences $CycleReferences `
                -Breadcrumb $Breadcrumb `
                -Depth $Depth `
                -MaximumDepth $MaximumDepth
        }

        $expandedTarget = Expand-OpenApiReferences `
            -Value $target `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    finally {
        $null = $ActiveReferences.Remove($canonicalPointer)
    }

    if ($siblingProperties.Count -eq 0) {
        return $expandedTarget
    }

    return Merge-ExpandedReferenceObject `
        -ExpandedTarget $expandedTarget `
        -SiblingProperties $siblingProperties `
        -CanonicalPointer $canonicalPointer `
        -Document $Document `
        -ActiveReferences $ActiveReferences `
        -CycleReferences $CycleReferences `
        -Breadcrumb $Breadcrumb `
        -Depth $Depth `
        -MaximumDepth $MaximumDepth
}

# Expands every allOf branch at its final output location, recursively flattening
# nested compositions before merging the branches and the containing siblings.
function Expand-AllOfSchemaObject {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $allOfBranches = Get-JsonPropertyValue -Object $Value -Name "allOf"
    if (-not (Test-JsonArrayValue -Value $allOfBranches)) {
        throw "The allOf keyword at '$Breadcrumb/allOf' must be an array."
    }
    if ($allOfBranches.Count -eq 0) {
        throw "The allOf array at '$Breadcrumb/allOf' must contain at least one object definition."
    }

    $expandedDefinitions = [System.Collections.Generic.List[object]]::new()
    $definitionBreadcrumbs = [System.Collections.Generic.List[string]]::new()
    for ($index = 0; $index -lt $allOfBranches.Count; $index++) {
        $branchBreadcrumb = "$Breadcrumb/allOf/$index"
        if (-not (Test-JsonObjectValue -Value $allOfBranches[$index])) {
            throw "The allOf branch at '$branchBreadcrumb' must be an object definition."
        }

        # Branch contents move to the containing schema, so rewritten pointers
        # must use the containing breadcrumb rather than the former allOf index.
        $expandedDefinitions.Add((Expand-OpenApiReferences `
            -Value $allOfBranches[$index] `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth))
        $definitionBreadcrumbs.Add($branchBreadcrumb)
    }

    $containingDefinition = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, "allOf", [System.StringComparison]::Ordinal) -or
            [string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
            continue
        }
        $containingDefinition[$property.Name] = $property.Value
    }

    if ($containingDefinition.Count -gt 0) {
        $expandedDefinitions.Add((Expand-OpenApiObjectProperties `
            -Value $containingDefinition `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth))
        $definitionBreadcrumbs.Add($Breadcrumb)
    }

    return Merge-AllOfSchemaDefinitions `
        -Definitions @($expandedDefinitions) `
        -DefinitionBreadcrumbs @($definitionBreadcrumbs) `
        -Breadcrumb $Breadcrumb
}

# Expands an ordinary JSON object and handles discriminator mapping rewrites at
# the object level where both the mapping and its composition branches are known.
function Expand-OpenApiObjectProperties {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    if (Test-JsonProperty -Object $Value -Name "allOf") {
        return Expand-AllOfSchemaObject `
            -Value $Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth $Depth `
            -MaximumDepth $MaximumDepth
    }

    $expandedObject = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($property.Name, "x-oaiMeta", [System.StringComparison]::Ordinal)) {
            continue
        }

        if ([string]::Equals($property.Name, "discriminator", [System.StringComparison]::Ordinal) -and
            (Test-JsonObjectValue -Value $property.Value)) {
            $expandedObject[$property.Name] = Expand-OpenApiDiscriminator `
                -Discriminator $property.Value `
                -ContainingObject $Value `
                -Document $Document `
                -ActiveReferences $ActiveReferences `
                -CycleReferences $CycleReferences `
                -Breadcrumb $Breadcrumb `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            continue
        }

        $expandedObject[$property.Name] = Expand-OpenApiReferences `
            -Value $property.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $expandedObject
}

# Recursively dispatches scalar, array, reference-object, and ordinary-object
# values. The active set contains only the current reference path.
function Expand-OpenApiReferences {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ActiveReferences,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Reference expansion exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return $null
    }
    if (Test-JsonArrayValue -Value $Value) {
        [object[]]$expandedItems = @(
            for ($index = 0; $index -lt $Value.Count; $index++) {
                Expand-OpenApiReferences `
                    -Value $Value[$index] `
                    -Document $Document `
                    -ActiveReferences $ActiveReferences `
                    -CycleReferences $CycleReferences `
                    -Breadcrumb "$Breadcrumb/$index" `
                    -Depth ($Depth + 1) `
                    -MaximumDepth $MaximumDepth
            }
        )
        return ,$expandedItems
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }
    if (Test-JsonProperty -Object $Value -Name '$ref') {
        return Expand-ComponentReferenceObject `
            -ReferenceObject $Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -Depth $Depth `
            -MaximumDepth $MaximumDepth
    }

    return Expand-OpenApiObjectProperties `
        -Value $Value `
        -Document $Document `
        -ActiveReferences $ActiveReferences `
        -CycleReferences $CycleReferences `
        -Breadcrumb $Breadcrumb `
        -Depth $Depth `
        -MaximumDepth $MaximumDepth
}

# Materializes only the component entries needed to resolve references retained
# as recursion back-edges in Inlined mode.
function New-InlinedFallbackComponents {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    $pending = [System.Collections.Generic.Queue[string]]::new()
    $queued = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $fallbacks = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )

    foreach ($pointer in $CycleReferences) {
        if ($queued.Add($pointer)) {
            $pending.Enqueue($pointer)
        }
    }

    while ($pending.Count -gt 0) {
        $cyclePointer = $pending.Dequeue()
        $componentRoot = Get-ComponentReferenceRoot -Pointer $cyclePointer
        if ($fallbacks.ContainsKey($componentRoot.Pointer)) {
            continue
        }

        $sourceComponent = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $componentRoot.Pointer `
            -Context "cycle fallback"
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new(
            [System.StringComparer]::Ordinal
        )
        $null = $activeReferences.Add($componentRoot.Pointer)
        $null = $activeReferences.Add($cyclePointer)

        $fallbacks[$componentRoot.Pointer] = Expand-OpenApiReferences `
            -Value $sourceComponent `
            -Document $Document `
            -ActiveReferences $activeReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $componentRoot.Pointer `
            -MaximumDepth $MaximumDepth

        foreach ($discoveredPointer in $CycleReferences) {
            if ($queued.Add($discoveredPointer)) {
                $pending.Enqueue($discoveredPointer)
            }
        }
    }

    return $fallbacks
}

# Enforces the defining promise of Inlined mode after document assembly.
function Assert-InlinedReferencePolicy {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$CycleReferences
    )

    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Document)) {
        if (-not [string]::Equals($occurrence.Kind, "Reference", [System.StringComparison]::Ordinal)) {
            continue
        }

        $canonicalPointer = Get-CanonicalJsonPointer -Pointer $occurrence.Pointer
        if (-not $CycleReferences.Contains($canonicalPointer)) {
            throw "Inlined output retained non-cyclic reference '$canonicalPointer' at '$($occurrence.Breadcrumb)'."
        }
    }
}

# Rejects any allOf keyword that survives final Inlined document assembly.
function Assert-NoAllOfKeywords {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter()]
        [string]$Breadcrumb = "#",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "allOf validation exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if ($null -eq $Value) {
        return
    }
    if (Test-JsonArrayValue -Value $Value) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-NoAllOfKeywords `
                -Value $Value[$index] `
                -Breadcrumb "$Breadcrumb/$index" `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $propertyBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
        if ([string]::Equals($property.Name, "allOf", [System.StringComparison]::Ordinal)) {
            throw "Inlined output retained allOf at '$propertyBreadcrumb'."
        }
        Assert-NoAllOfKeywords `
            -Value $property.Value `
            -Breadcrumb $propertyBreadcrumb `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
}

# Returns only HTTP operation properties from a path item. Path-level metadata
# such as parameters and servers remains part of the extracted path item but is
# not mistaken for an operation while gathering tags and security requirements.
function Get-OpenApiOperations {
    param(
        [Parameter(Mandatory = $true)]
        [object]$PathItem
    )

    [string[]]$httpMethods = @(
        "get", "put", "post", "delete", "options", "head", "patch", "trace"
    )
    foreach ($property in (Get-JsonProperties -Value $PathItem)) {
        if ($httpMethods -ccontains $property.Name) {
            [PSCustomObject]@{
                Method = $property.Name
                Operation = $property.Value
            }
        }
    }
}

# Collects operation tag names with ordinal, case-sensitive identity.
function Get-UsedTagNames {
    param(
        [Parameter(Mandatory = $true)]
        [object]$PathItem
    )

    $tagNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($operationEntry in @(Get-OpenApiOperations -PathItem $PathItem)) {
        $tags = Get-JsonPropertyValue -Object $operationEntry.Operation -Name "tags"
        if (-not (Test-JsonArrayValue -Value $tags)) {
            continue
        }

        foreach ($tag in $tags) {
            if ($tag -is [string]) {
                $null = $tagNames.Add($tag)
            }
        }
    }

    return ,$tagNames
}

# Reads scheme names from global and operation-level Security Requirement
# Objects. Copying the global requirement preserves behavior for operations that
# do not provide an override.
function Get-UsedSecuritySchemeNames {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$PathItem
    )

    $schemeNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $securityRequirements = [System.Collections.Generic.List[object]]::new()

    $globalSecurity = Get-JsonPropertyValue -Object $Document -Name "security"
    if (Test-JsonArrayValue -Value $globalSecurity) {
        foreach ($requirement in $globalSecurity) {
            $securityRequirements.Add($requirement)
        }
    }

    foreach ($operationEntry in @(Get-OpenApiOperations -PathItem $PathItem)) {
        if (-not (Test-JsonProperty -Object $operationEntry.Operation -Name "security")) {
            continue
        }

        $operationSecurity = Get-JsonPropertyValue -Object $operationEntry.Operation -Name "security"
        if (Test-JsonArrayValue -Value $operationSecurity) {
            foreach ($requirement in $operationSecurity) {
                $securityRequirements.Add($requirement)
            }
        }
    }

    foreach ($requirement in $securityRequirements) {
        if (-not (Test-JsonObjectValue -Value $requirement)) {
            throw "An OpenAPI security requirement must be an object."
        }
        foreach ($property in (Get-JsonProperties -Value $requirement)) {
            $null = $schemeNames.Add($property.Name)
        }
    }

    return ,$schemeNames
}

# Copies selected component entries in their original section and entry order.
# Overrides hold the transformed cycle fallbacks produced by Inlined mode.
function New-SelectedComponentSections {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ComponentPointers,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.Dictionary[string, object]]$Overrides
    )

    $selectedComponents = [ordered]@{}
    if ($ComponentPointers.Count -eq 0) {
        return $selectedComponents
    }

    $sourceComponents = Get-JsonPropertyValue -Object $Document -Name "components"
    if (-not (Test-JsonObjectValue -Value $sourceComponents)) {
        throw "The selected route requires components, but the OpenAPI document has no components object."
    }

    $emittedPointers = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($sectionProperty in (Get-JsonProperties -Value $sourceComponents)) {
        if (-not (Test-JsonObjectValue -Value $sectionProperty.Value)) {
            continue
        }

        $selectedEntries = [ordered]@{}
        foreach ($entryProperty in (Get-JsonProperties -Value $sectionProperty.Value)) {
            $rootPointer = "#/components/{0}/{1}" -f @(
                (ConvertTo-JsonPointerToken -Token $sectionProperty.Name),
                (ConvertTo-JsonPointerToken -Token $entryProperty.Name)
            )
            if (-not $ComponentPointers.Contains($rootPointer)) {
                continue
            }

            if ($Overrides.ContainsKey($rootPointer)) {
                $selectedEntries[$entryProperty.Name] = $Overrides[$rootPointer]
            }
            else {
                $selectedEntries[$entryProperty.Name] = Copy-JsonValue -Value $entryProperty.Value
            }
            $null = $emittedPointers.Add($rootPointer)
        }

        if ($selectedEntries.Count -gt 0) {
            $selectedComponents[$sectionProperty.Name] = $selectedEntries
        }
    }

    foreach ($pointer in $ComponentPointers) {
        if (-not $emittedPointers.Contains($pointer)) {
            throw "Required component '$pointer' was not found in the source document."
        }
    }

    return $selectedComponents
}

# Copies only tag definitions used by an operation in the selected path item.
function Get-SelectedTagDefinitions {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$UsedTagNames
    )

    $sourceTags = Get-JsonPropertyValue -Object $Document -Name "tags"
    if (-not (Test-JsonArrayValue -Value $sourceTags) -or $UsedTagNames.Count -eq 0) {
        return ,([object[]]@())
    }

    [object[]]$selectedTags = @(
        foreach ($tagDefinition in $sourceTags) {
            if (-not (Test-JsonObjectValue -Value $tagDefinition)) {
                continue
            }
            $name = Get-JsonPropertyValue -Object $tagDefinition -Name "name"
            if ($name -is [string] -and $UsedTagNames.Contains($name)) {
                Copy-JsonValue -Value $tagDefinition
            }
        }
    )
    return ,$selectedTags
}

# Adds component security schemes needed by copied security requirements.
function Add-SecuritySchemeComponents {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$SecuritySchemeNames,

        [Parameter(Mandatory = $true)]
        [AllowEmptyCollection()]
        [System.Collections.Generic.HashSet[string]]$ComponentPointers
    )

    foreach ($schemeName in $SecuritySchemeNames) {
        $pointer = "#/components/securitySchemes/$(ConvertTo-JsonPointerToken -Token $schemeName)"
        $null = Resolve-LocalJsonPointer `
            -Document $Document `
            -Pointer $pointer `
            -Context "security scheme"
        $null = $ComponentPointers.Add($pointer)
    }
}

# Creates a minimal valid OpenAPI document for one complete path item.
function New-ExtractedOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$RouteEntry,

        [Parameter(Mandatory = $true)]
        [ValidateSet("Exact", "Inlined")]
        [string]$Mode
    )

    $routePointer = "#/paths/$(ConvertTo-JsonPointerToken -Token $RouteEntry.Route)"
    $sourcePathItem = ConvertTo-NormalizedOpenApiValue -Value $RouteEntry.PathItem
    $cycleReferences = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $fallbacks = [System.Collections.Generic.Dictionary[string, object]]::new(
        [System.StringComparer]::Ordinal
    )

    if ([string]::Equals($Mode, "Exact", [System.StringComparison]::Ordinal)) {
        $selectedPathItem = $sourcePathItem
        $componentPointers = Get-TransitiveComponentReferences `
            -Document $Document `
            -Value $selectedPathItem
    }
    else {
        $activeReferences = [System.Collections.Generic.HashSet[string]]::new(
            [System.StringComparer]::Ordinal
        )
        $selectedPathItem = Expand-OpenApiReferences `
            -Value $sourcePathItem `
            -Document $Document `
            -ActiveReferences $activeReferences `
            -CycleReferences $cycleReferences `
            -Breadcrumb $routePointer
        $fallbacks = New-InlinedFallbackComponents `
            -Document $Document `
            -CycleReferences $cycleReferences
        $componentPointers = [System.Collections.Generic.HashSet[string]]::new(
            [System.StringComparer]::Ordinal
        )
        foreach ($pointer in $fallbacks.Keys) {
            $null = $componentPointers.Add($pointer)
        }
    }

    $usedSecuritySchemes = Get-UsedSecuritySchemeNames `
        -Document $Document `
        -PathItem $selectedPathItem
    Add-SecuritySchemeComponents `
        -Document $Document `
        -SecuritySchemeNames $usedSecuritySchemes `
        -ComponentPointers $componentPointers

    $usedTagNames = Get-UsedTagNames -PathItem $selectedPathItem
    $selectedTags = Get-SelectedTagDefinitions `
        -Document $Document `
        -UsedTagNames $usedTagNames
    $selectedComponents = New-SelectedComponentSections `
        -Document $Document `
        -ComponentPointers $componentPointers `
        -Overrides $fallbacks

    $extractedDocument = [ordered]@{
        openapi = Copy-JsonValue -Value (Get-JsonPropertyValue -Object $Document -Name "openapi")
        info = Copy-JsonValue -Value (Get-JsonPropertyValue -Object $Document -Name "info")
    }
    foreach ($optionalProperty in @("jsonSchemaDialect", "servers", "security")) {
        if (Test-JsonProperty -Object $Document -Name $optionalProperty) {
            $extractedDocument[$optionalProperty] = Copy-JsonValue `
                -Value (Get-JsonPropertyValue -Object $Document -Name $optionalProperty)
        }
    }
    if ($selectedTags.Count -gt 0) {
        $extractedDocument["tags"] = $selectedTags
    }

    $selectedPaths = [ordered]@{}
    $selectedPaths[$RouteEntry.Route] = $selectedPathItem
    $extractedDocument["paths"] = $selectedPaths
    if ($selectedComponents.Count -gt 0) {
        $extractedDocument["components"] = $selectedComponents
    }

    $normalizedDocument = ConvertTo-NormalizedOpenApiValue -Value $extractedDocument
    Assert-OpenApiReferencesResolve -Document $normalizedDocument
    if ([string]::Equals($Mode, "Inlined", [System.StringComparison]::Ordinal)) {
        Assert-InlinedReferencePolicy `
            -Document $normalizedDocument `
            -CycleReferences $cycleReferences
        Assert-NoAllOfKeywords -Value $normalizedDocument
    }

    return [PSCustomObject]@{
        Document = $normalizedDocument
        DependencyCount = $componentPointers.Count
        CycleCount = $cycleReferences.Count
    }
}

# Converts a literal route key to a portable, readable filename stem.
function ConvertTo-RouteFileStem {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Route
    )

    $stem = $Route.Trim("/") -replace "[^A-Za-z0-9._-]+", "-"
    $stem = ($stem -replace "-+", "-").Trim("-", ".")
    if ([string]::IsNullOrWhiteSpace($stem)) {
        return "root"
    }
    return $stem.ToLowerInvariant()
}

# Computes a short stable suffix only when two source routes sanitize to the
# same stem, keeping ordinary output names uncluttered.
function Get-DefaultOutputPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Route,

        [Parameter(Mandatory = $true)]
        [ValidateSet("Exact", "Inlined")]
        [string]$Mode,

        [Parameter(Mandatory = $true)]
        [object[]]$Catalog,

        [Parameter(Mandatory = $true)]
        [string]$RepositoryRoot
    )

    $stem = ConvertTo-RouteFileStem -Route $Route
    $collisions = @($Catalog | Where-Object {
        [string]::Equals(
            (ConvertTo-RouteFileStem -Route $_.Route),
            $stem,
            [System.StringComparison]::OrdinalIgnoreCase
        )
    })
    if ($collisions.Count -gt 1) {
        $routeBytes = [System.Text.Encoding]::UTF8.GetBytes($Route)
        $hashBytes = [System.Security.Cryptography.SHA256]::HashData($routeBytes)
        $hash = [System.Convert]::ToHexString($hashBytes).Substring(0, 8).ToLowerInvariant()
        $stem = "$stem-$hash"
    }

    $outputDirectory = Join-Path $RepositoryRoot "artifacts" "openapi-extracts"
    return Join-Path $outputDirectory "$stem.$($Mode.ToLowerInvariant()).openapi.json"
}

# Serializes with enough depth for the source schemas and atomically replaces
# the destination with UTF-8 JSON without a byte-order mark.
function Write-OpenApiJsonFile {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $directory = [System.IO.Path]::GetDirectoryName($fullPath)
    $null = [System.IO.Directory]::CreateDirectory($directory)
    $temporaryPath = Join-Path $directory ".$(Split-Path $fullPath -Leaf).$([System.Guid]::NewGuid().ToString('N')).tmp"

    try {
        $json = ConvertTo-Json -InputObject $Document -Depth 100
        $encoding = [System.Text.UTF8Encoding]::new($false)
        [System.IO.File]::WriteAllText($temporaryPath, "$json`n", $encoding)
        [System.IO.File]::Move($temporaryPath, $fullPath, $true)
    }
    finally {
        if ([System.IO.File]::Exists($temporaryPath)) {
            [System.IO.File]::Delete($temporaryPath)
        }
    }

    return $fullPath
}

function Invoke-OpenApiRouteExtractor {
    param(
        [Parameter()]
        [string]$SpecificationPath,

        [Parameter()]
        [string]$Route,

        [Parameter()]
        [string]$Mode,

        [Parameter()]
        [string]$OutputPath
    )

    $repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
    if ([string]::IsNullOrWhiteSpace($SpecificationPath)) {
        $SpecificationPath = Join-Path $repositoryRoot "specification" "openapi" "openapi.json"
    }
    else {
        $SpecificationPath = Resolve-RepositoryPath -Path $SpecificationPath -RepositoryRoot $repositoryRoot
    }

    $document = Read-OpenApiDocument -Path $SpecificationPath
    $catalog = Get-OpenApiRouteCatalog -Document $document
    $selectedRoute = Select-OpenApiRoute -Catalog $catalog -RequestedRoute $Route
    $selectedMode = Select-ExtractionMode -RequestedMode $Mode
    $extraction = New-ExtractedOpenApiDocument `
        -Document $document `
        -RouteEntry $selectedRoute `
        -Mode $selectedMode

    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = Get-DefaultOutputPath `
            -Route $selectedRoute.Route `
            -Mode $selectedMode `
            -Catalog $catalog `
            -RepositoryRoot $repositoryRoot
    }
    else {
        $OutputPath = Resolve-RepositoryPath -Path $OutputPath -RepositoryRoot $repositoryRoot
    }

    $writtenPath = Write-OpenApiJsonFile `
        -Document $extraction.Document `
        -Path $OutputPath

    Write-Host ""
    Write-Host "Route: $($selectedRoute.Route)"
    Write-Host "Mode: $selectedMode"
    Write-Host "Output: $writtenPath"
    Write-Host "Component dependencies: $($extraction.DependencyCount)"
    Write-Host "Cycle references retained: $($extraction.CycleCount)"
}

# Dot-sourcing exposes the helpers to tests without executing the command.
if ($MyInvocation.InvocationName -ne ".") {
    Invoke-OpenApiRouteExtractor `
        -SpecificationPath $SpecificationPath `
        -Route $Route `
        -Mode $Mode `
        -OutputPath $OutputPath
}