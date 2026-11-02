#Requires -Version 7.0

# Private implementation shared by the OpenAPI extraction and transformation
# scripts. Dot-sourcing this file defines functions but performs no file I/O.

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$script:OpenApiHttpMethodNames = @(
    "get"
    "put"
    "post"
    "delete"
    "options"
    "head"
    "patch"
    "trace"
)

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
    if (@(Get-JsonProperties -Value $paths).Count -eq 0) {
        throw "OpenAPI specification '$Path' does not contain any routes under 'paths'."
    }

    return $document
}

# Builds one catalog entry per HTTP operation that defines an operationId. Only
# paths are read, so top-level webhook operations cannot enter the menu.
function Get-OpenApiOperationCatalog {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $paths = Get-JsonPropertyValue -Object $Document -Name "paths"
    $catalog = [System.Collections.Generic.List[object]]::new()
    $operationIds = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    foreach ($pathProperty in (Get-JsonProperties -Value $paths)) {
        foreach ($operationEntry in @(Get-OpenApiOperations -PathItem $pathProperty.Value)) {
            if (-not (Test-JsonObjectValue -Value $operationEntry.Operation)) {
                throw "HTTP operation '$($operationEntry.Method.ToUpperInvariant()) $($pathProperty.Name)' must be an object."
            }

            if (-not (Test-JsonProperty -Object $operationEntry.Operation -Name "operationId")) {
                continue
            }
            $operationId = Get-JsonPropertyValue -Object $operationEntry.Operation -Name "operationId"
            if ($operationId -isnot [string] -or [string]::IsNullOrWhiteSpace($operationId)) {
                throw "HTTP operation '$($operationEntry.Method.ToUpperInvariant()) $($pathProperty.Name)' has an invalid operationId."
            }
            if (-not $operationIds.Add($operationId)) {
                throw "OpenAPI operationId '$operationId' is not unique."
            }

            $catalog.Add([PSCustomObject]@{
                OperationId = $operationId
                Method = $operationEntry.Method
                Route = $pathProperty.Name
                PathItem = $pathProperty.Value
                Operation = $operationEntry.Operation
            })
        }
    }

    if ($catalog.Count -eq 0) {
        throw "The OpenAPI paths object does not contain any operations with an operationId."
    }

    return @($catalog | Sort-Object -Property OperationId, Route, Method)
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

# Selects an operation by exact operationId or from one interactive menu.
function Select-OpenApiOperation {
    param(
        [Parameter(Mandatory = $true)]
        [object[]]$Catalog,

        [Parameter()]
        [string]$RequestedOperationId
    )

    if (-not [string]::IsNullOrWhiteSpace($RequestedOperationId)) {
        $match = @($Catalog | Where-Object {
            [string]::Equals(
                $_.OperationId,
                $RequestedOperationId,
                [System.StringComparison]::Ordinal
            )
        })
        if ($match.Count -ne 1) {
            throw "OperationId '$RequestedOperationId' was not found in the OpenAPI paths object."
        }

        return $match[0]
    }

    return Read-NumberedSelection `
        -Title "Select an operationId" `
        -Options $Catalog `
        -GetLabel {
            param($Option)
            "$($Option.OperationId) [$($Option.Method.ToUpperInvariant()) $($Option.Route)]"
        }
}

# Converts the internal mode value to the label shown in menus and summaries.
function Get-OpenApiExtractionModeLabel {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode
    )

    switch ($Mode) {
        "RequestBody" { return "Request Body" }
        "Responses" { return "Responses" }
        "Both" { return "Both" }
    }
}

# Uses an explicit mode when supplied or displays a second one-based menu after
# operation selection. Canonical return values keep downstream switches stable
# even though ValidateSet accepts case-insensitive command-line input.
function Select-OpenApiExtractionMode {
    param(
        [Parameter()]
        [AllowNull()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$RequestedMode,

        [Parameter()]
        [scriptblock]$InputProvider = { param($Prompt) Read-Host -Prompt $Prompt }
    )

    if (-not [string]::IsNullOrWhiteSpace($RequestedMode)) {
        switch ($RequestedMode) {
            "RequestBody" { return "RequestBody" }
            "Responses" { return "Responses" }
            "Both" { return "Both" }
        }
    }

    $selection = Read-NumberedSelection `
        -Title "Select extraction content" `
        -Options @("RequestBody", "Responses", "Both") `
        -GetLabel { param($Option) Get-OpenApiExtractionModeLabel -Mode $Option } `
        -InputProvider $InputProvider
    return $selection
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


# Selects how a child value should be normalized. Explicit context prevents
# ordinary OpenAPI objects that happen to use schema-like names from being
# reordered as schemas.
function Get-OpenApiSchemaChildContext {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$ParentContext,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$PropertyValue
    )

    if ([string]::Equals($ParentContext, "Literal", [System.StringComparison]::Ordinal)) {
        return "Literal"
    }

    if ([string]::Equals($ParentContext, "SchemaMap", [System.StringComparison]::Ordinal)) {
        return "Schema"
    }

    if ([string]::Equals($ParentContext, "Components", [System.StringComparison]::Ordinal)) {
        if ([string]::Equals($PropertyName, "schemas", [System.StringComparison]::Ordinal)) {
            return "SchemaMap"
        }
        return "OpenApi"
    }

    if ([string]::Equals($ParentContext, "Schema", [System.StringComparison]::Ordinal)) {
        if (@("properties", "patternProperties", "dependentSchemas", '$defs', "definitions") -ccontains $PropertyName) {
            return "SchemaMap"
        }
        if (@("allOf", "anyOf", "oneOf", "prefixItems") -ccontains $PropertyName) {
            return "SchemaArray"
        }
        if (@(
            "items",
            "additionalItems",
            "additionalProperties",
            "unevaluatedItems",
            "unevaluatedProperties",
            "contains",
            "propertyNames",
            "not",
            "if",
            "then",
            "else",
            "contentSchema"
        ) -ccontains $PropertyName) {
            if (Test-JsonArrayValue -Value $PropertyValue) {
                return "SchemaArray"
            }
            return "Schema"
        }

        return "Literal"
    }

    if ([string]::Equals($PropertyName, "components", [System.StringComparison]::Ordinal)) {
        return "Components"
    }
    if ([string]::Equals($PropertyName, "schema", [System.StringComparison]::Ordinal)) {
        return "Schema"
    }

    return "OpenApi"
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

# Returns the finite string values accepted directly by a discriminator property
# schema. Complex constraints are intentionally not inferred: a branch must expose
# its routing values through const, enum, or their intersection.
function Get-OpenApiFiniteStringSchemaValues {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    if (-not (Test-JsonObjectValue -Value $Schema)) {
        throw "Discriminator property schema at '$Breadcrumb' must be an object."
    }

    if (Test-JsonProperty -Object $Schema -Name "type") {
        $type = Get-JsonPropertyValue -Object $Schema -Name "type"
        $allowsString = $type -is [string] -and
            [string]::Equals($type, "string", [System.StringComparison]::Ordinal)
        if (Test-JsonArrayValue -Value $type) {
            $allowsString = @($type | Where-Object {
                $_ -is [string] -and
                [string]::Equals($_, "string", [System.StringComparison]::Ordinal)
            }).Count -gt 0
        }
        if (-not $allowsString) {
            throw "Discriminator property schema at '$Breadcrumb' does not allow string values."
        }
    }

    $hasConst = Test-JsonProperty -Object $Schema -Name "const"
    $constValue = if ($hasConst) {
        Get-JsonPropertyValue -Object $Schema -Name "const"
    }
    else {
        $null
    }
    if ($hasConst -and $constValue -isnot [string]) {
        throw "Discriminator const at '$Breadcrumb/const' must be a string."
    }

    $hasEnum = Test-JsonProperty -Object $Schema -Name "enum"
    $enum = if ($hasEnum) {
        Get-JsonPropertyValue -Object $Schema -Name "enum"
    }
    else {
        $null
    }
    $enumValues = [System.Collections.Generic.List[string]]::new()
    $seenValues = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    if ($hasEnum) {
        if (-not (Test-JsonArrayValue -Value $enum) -or $enum.Count -eq 0) {
            throw "Discriminator enum at '$Breadcrumb/enum' must be a nonempty array."
        }
        for ($index = 0; $index -lt $enum.Count; $index++) {
            $value = $enum[$index]
            if ($value -isnot [string]) {
                throw "Discriminator enum value at '$Breadcrumb/enum/$index' must be a string."
            }
            if (-not $seenValues.Add($value)) {
                throw "Discriminator enum at '$Breadcrumb/enum' contains duplicate value '$value'."
            }
            $enumValues.Add($value)
        }
    }

    if (-not $hasConst -and -not $hasEnum) {
        throw "Discriminator property schema at '$Breadcrumb' must define a string const or nonempty string enum."
    }
    if ($hasConst) {
        if ($hasEnum -and -not $seenValues.Contains($constValue)) {
            throw "Discriminator const '$constValue' at '$Breadcrumb/const' is excluded by its enum."
        }
        return [string[]]@($constValue)
    }
    return [string[]]$enumValues.ToArray()
}

# Resolves the direct discriminator property contract on one alternative schema.
function Get-OpenApiDiscriminatorPropertyValues {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    if (-not (Test-JsonObjectValue -Value $Schema)) {
        throw "Discriminator alternative at '$Breadcrumb' must resolve to a Schema Object."
    }
    $required = Get-JsonPropertyValue -Object $Schema -Name "required"
    if (-not (Test-JsonArrayValue -Value $required) -or
        -not (@($required | Where-Object {
            $_ -is [string] -and
            [string]::Equals($_, $PropertyName, [System.StringComparison]::Ordinal)
        }).Count -gt 0)) {
        throw "Discriminator property '$PropertyName' must be required directly by alternative '$Breadcrumb'."
    }

    $properties = Get-JsonPropertyValue -Object $Schema -Name "properties"
    if (-not (Test-JsonObjectValue -Value $properties) -or
        -not (Test-JsonProperty -Object $properties -Name $PropertyName)) {
        throw "Discriminator alternative '$Breadcrumb' has no direct property '$PropertyName'."
    }
    $propertyToken = ConvertTo-JsonPointerToken -Token $PropertyName
    return [string[]]@(Get-OpenApiFiniteStringSchemaValues `
        -Schema (Get-JsonPropertyValue -Object $properties -Name $PropertyName) `
        -Breadcrumb "$Breadcrumb/properties/$propertyToken")
}

# Scope-sensitive schema keywords and extensions cannot be discarded from a
# nested group without potentially changing its meaning.
function Test-ContainsUnsafeOpenApiDiscriminatorUnionSibling {
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
        throw "Discriminator union sibling inspection exceeds the supported depth of $MaximumDepth."
    }
    if ($null -eq $Value) {
        return $false
    }
    if (Test-JsonArrayValue -Value $Value) {
        foreach ($item in $Value) {
            if (Test-ContainsUnsafeOpenApiDiscriminatorUnionSibling `
                -Value $item `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth) {
                return $true
            }
        }
        return $false
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $false
    }

    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if (@('$id', '$anchor', '$dynamicAnchor', '$recursiveAnchor') -ccontains $property.Name -or
            $property.Name.StartsWith("x-", [System.StringComparison]::OrdinalIgnoreCase)) {
            return $true
        }
        if (Test-ContainsUnsafeOpenApiDiscriminatorUnionSibling `
            -Value $property.Value `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth) {
            return $true
        }
    }
    return $false
}

# Mapping is intentionally excluded: branch pointers move during flattening and
# the final componentizer reconstructs the canonical exhaustive mapping.
function Test-OpenApiDiscriminatorMetadataCompatible {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentSchema,

        [Parameter(Mandatory = $true)]
        [object]$GroupSchema,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter()]
        [switch]$IgnoreParentDiscriminator
    )

    $parentDiscriminator = Get-JsonPropertyValue -Object $ParentSchema -Name "discriminator"
    $groupDiscriminator = Get-JsonPropertyValue -Object $GroupSchema -Name "discriminator"
    if (-not (Test-JsonObjectValue -Value $groupDiscriminator)) {
        return $false
    }
    $groupPropertyName = Get-JsonPropertyValue -Object $groupDiscriminator -Name "propertyName"
    if ($groupPropertyName -isnot [string] -or
        -not [string]::Equals($groupPropertyName, $PropertyName, [System.StringComparison]::Ordinal)) {
        return $false
    }

    $expected = [ordered]@{ propertyName = $PropertyName }
    if (-not $IgnoreParentDiscriminator -and
        (Test-JsonObjectValue -Value $parentDiscriminator)) {
        $expected = [ordered]@{}
        foreach ($property in (Get-JsonProperties -Value $parentDiscriminator)) {
            if (-not [string]::Equals($property.Name, "mapping", [System.StringComparison]::Ordinal)) {
                $expected[$property.Name] = $property.Value
            }
        }
    }
    $actual = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $groupDiscriminator)) {
        if (-not [string]::Equals($property.Name, "mapping", [System.StringComparison]::Ordinal)) {
            $actual[$property.Name] = $property.Value
        }
    }
    return Test-JsonValuesEqual -Left $expected -Right $actual
}

function Test-OpenApiDiscriminatorUnionSiblingsMatch {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ParentSchema,

        [Parameter(Mandatory = $true)]
        [object]$GroupSchema,

        [Parameter()]
        [ValidateSet("oneOf", "anyOf")]
        [string]$ParentCompositionName = "oneOf",

        [Parameter()]
        [ValidateSet("oneOf", "anyOf")]
        [string]$GroupCompositionName = "oneOf"
    )

    $parentProperties = @(
        Get-JsonProperties -Value $ParentSchema |
            Where-Object { @("type", "discriminator", $ParentCompositionName) -cnotcontains $_.Name }
    )
    $groupProperties = @(
        Get-JsonProperties -Value $GroupSchema |
            Where-Object { @("type", "discriminator", $GroupCompositionName) -cnotcontains $_.Name }
    )
    if ($parentProperties.Count -ne $groupProperties.Count) {
        return $false
    }
    foreach ($property in $parentProperties) {
        if (-not (Test-JsonProperty -Object $GroupSchema -Name $property.Name) -or
            -not (Test-JsonValuesEqual `
                -Left $property.Value `
                -Right (Get-JsonPropertyValue -Object $GroupSchema -Name $property.Name))) {
            return $false
        }
        $sibling = [ordered]@{}
        $sibling[$property.Name] = $property.Value
        if (Test-ContainsUnsafeOpenApiDiscriminatorUnionSibling -Value $sibling) {
            return $false
        }
    }
    return $true
}

function Get-OpenApiCommonNestedDiscriminatorValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Plan,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Common nested-group discriminator value analysis exceeds the supported depth of $MaximumDepth."
    }
    $commonValue = $null
    foreach ($branchPlan in $Plan.BranchPlans) {
        if ($null -ne $branchPlan.NestedPlan) {
            $value = Get-OpenApiCommonNestedDiscriminatorValue `
                -Plan $branchPlan.NestedPlan `
                -PropertyName $PropertyName `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        else {
            try {
                [string[]]$acceptedValues = @(Get-OpenApiDiscriminatorPropertyValues `
                    -Schema $branchPlan.Branch `
                    -PropertyName $PropertyName `
                    -Breadcrumb $branchPlan.Breadcrumb)
            }
            catch {
                return $null
            }
            if ($acceptedValues.Count -ne 1) {
                return $null
            }
            $value = $acceptedValues[0]
        }
        if ($null -eq $value -or
            ($null -ne $branchPlan.NestedPlan -and
                -not (Test-OpenApiNestedDiscriminatorPropertyCompatible `
                    -Schema $branchPlan.Branch `
                    -PropertyName $PropertyName `
                    -Value $value `
                    -Breadcrumb $branchPlan.Breadcrumb)) -or
            ($null -ne $commonValue -and
                -not [string]::Equals($commonValue, $value, [System.StringComparison]::Ordinal))) {
            return $null
        }
        $commonValue = $value
    }
    return $commonValue
}

function Test-OpenApiNestedDiscriminatorPropertyCompatible {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    if (Test-JsonProperty -Object $Schema -Name "required") {
        $required = Get-JsonPropertyValue -Object $Schema -Name "required"
        if (-not (Test-JsonArrayValue -Value $required) -or
            @($required | Where-Object { $_ -isnot [string] }).Count -gt 0) {
            return $false
        }
    }
    if (-not (Test-JsonProperty -Object $Schema -Name "properties")) {
        return $true
    }
    $properties = Get-JsonPropertyValue -Object $Schema -Name "properties"
    if (-not (Test-JsonObjectValue -Value $properties)) {
        return $false
    }
    if (-not (Test-JsonProperty -Object $properties -Name $PropertyName)) {
        return $true
    }
    $discriminatorProperty = Get-JsonPropertyValue -Object $properties -Name $PropertyName
    if (-not (Test-JsonObjectValue -Value $discriminatorProperty)) {
        return $false
    }
    if (Test-JsonProperty -Object $discriminatorProperty -Name "type") {
        $discriminatorPropertyType = Get-JsonPropertyValue -Object $discriminatorProperty -Name "type"
        if ($discriminatorPropertyType -isnot [string] -or
            -not [string]::Equals($discriminatorPropertyType, "string", [System.StringComparison]::Ordinal)) {
            return $false
        }
    }
    if ((Test-JsonProperty -Object $discriminatorProperty -Name "const") -or
        (Test-JsonProperty -Object $discriminatorProperty -Name "enum")) {
        try {
            [string[]]$acceptedValues = @(Get-OpenApiFiniteStringSchemaValues `
                -Schema $discriminatorProperty `
                -Breadcrumb "$Breadcrumb/properties/$(ConvertTo-JsonPointerToken -Token $PropertyName)")
            return $acceptedValues -ccontains $Value
        }
        catch {
            return $false
        }
    }
    return $true
}

function ConvertTo-NormalizedOpenApiDiscriminatorUnionSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Plan,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Nested-group discriminator property normalization exceeds the supported depth of $MaximumDepth."
    }
    $result = Copy-JsonValue -Value $Plan.Schema
    [object[]]$branches = [object[]]::new($Plan.BranchPlans.Count)
    for ($index = 0; $index -lt $branches.Count; $index++) {
        $branchPlan = $Plan.BranchPlans[$index]
        $branch = if ($null -ne $branchPlan.NestedPlan) {
            ConvertTo-NormalizedOpenApiDiscriminatorUnionSchema `
                -Plan $branchPlan.NestedPlan `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        else {
            Copy-JsonValue -Value $branchPlan.Branch
        }
        if ([string]::Equals($branchPlan.Kind, "NestedDiscriminatorGroup", [System.StringComparison]::Ordinal)) {
            if (-not (Test-JsonProperty -Object $branch -Name "type")) {
                $branch["type"] = "object"
            }
            if (-not (Test-JsonProperty -Object $branch -Name "properties")) {
                $branch["properties"] = [ordered]@{}
            }
            $properties = Get-JsonPropertyValue -Object $branch -Name "properties"
            $discriminatorProperty = if (Test-JsonProperty -Object $properties -Name $Plan.PropertyName) {
                Copy-JsonValue -Value (Get-JsonPropertyValue -Object $properties -Name $Plan.PropertyName)
            }
            else {
                [ordered]@{}
            }
            if (-not (Test-JsonProperty -Object $discriminatorProperty -Name "type")) {
                $discriminatorProperty["type"] = "string"
            }
            $discriminatorProperty["enum"] = [object[]]@($branchPlan.Values[0])
            $properties[$Plan.PropertyName] = $discriminatorProperty
            [object[]]$required = if (Test-JsonProperty -Object $branch -Name "required") {
                @(Get-JsonPropertyValue -Object $branch -Name "required")
            }
            else {
                @()
            }
            if ($required -cnotcontains $Plan.PropertyName) {
                $required = [object[]]@($required + $Plan.PropertyName)
            }
            $branch["required"] = $required
        }
        $branches[$index] = $branch
    }
    $result[$Plan.CompositionName] = $branches
    return $result
}

<#
A discriminator union plan is a PSCustomObject describing the result of schema
analysis, not a pipeline stage or an output document. Building a plan does not
modify the input. Schema and Branch reference the original schema objects;
conversion functions copy those objects before applying changes.

Each union plan contains:
    Schema          The Schema Object owning the composition.
    PropertyName    The discriminator property at this level, such as type or role.
    CompositionName The analyzed composition keyword: oneOf or anyOf.
    Branches        The original immediate alternatives, in encounter order.
    BranchPlans     One analysis result per immediate alternative.
    Values          All accepted discriminator values at this level.

Each branch plan contains Kind, Branch, Breadcrumb (its JSON Pointer), Values
(its discriminator values at the parent's level), and NestedPlan. NestedPlan is
null for a Leaf; otherwise it describes the nested oneOf using that group's own
discriminator property.

Kind describes a branch relative to its immediate parent, not its depth:
    Leaf                     A concrete object alternative with finite, required
                                                     discriminator values.
    FlattenableGroup         A nested group using the SAME discriminator property
                                                     as its parent. Matching structural constraints make
                                                     it a transparent grouping whose alternatives can be
                                                     promoted into the parent by the flatten stage.
    NestedDiscriminatorGroup A nested group using a DIFFERENT discriminator
                                                     property. Every concrete member must already require
                                                     the same single outer discriminator value. The group
                                                     counts once at the outer level and remains intact so
                                                     its own discriminator can distinguish its members.

For example, type chooses Message or FunctionCall, and role chooses UserMessage
or AssistantMessage inside Message. An abbreviated outer plan is:
    PropertyName = "type"
    Values = @("message", "function_call")
    BranchPlans = @(
        @{ Kind = "NestedDiscriminatorGroup"; Values = @("message"); NestedPlan = @{
                 PropertyName = "role"; Values = @("user", "assistant")
             } },
        @{ Kind = "Leaf"; Values = @("function_call"); NestedPlan = $null }
    )
The role group's two message members do not create duplicate outer values:
"message" occurs once in the outer plan and user/assistant belong to NestedPlan.
By contrast, a type group containing input_text and input_image inside another
type group can be a FlattenableGroup: both values contribute to the parent's
values.

For a NestedDiscriminatorGroup, analysis proves that all concrete members
already require the same outer discriminator value. Normalization declares that
constraint directly on the subgroup. Component naming, exhaustive mappings, and
immediate-parent inheritance then preserve both discriminator levels; the group
is not flattened into alternatives sharing a duplicate outer value.

A null result means the composition does not satisfy these conservative rules.
That includes malformed discriminator metadata or compositions, non-object
alternatives, optional/missing discriminator properties, non-finite or
conflicting const/enum constraints, and overlapping values across immediate
logical branches. A NestedDiscriminatorGroup must also have a valid inner
discriminator and one common required outer value compatible with its existing
constraints. A same-property group must meet the structural/metadata equivalence
and scope-safety checks for flattening. Unsupported analysis does not necessarily
mean the original JSON Schema is invalid. Stage 10 removes unsupported oneOf
discriminator metadata and may infer a valid type discriminator instead; it does
not remove the oneOf alternatives. Component mapping validation and inheritance
have additional fail-fast requirements documented in docs/OpenApiPipelineDesign.md.
Recursion supports more than two levels, subject to MaximumDepth and the same
required, finite, unambiguous discriminator checks at each level.
#>
function Get-OpenApiInlineDiscriminatorUnionPlan {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$PropertyName,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter()]
        [ValidateSet("oneOf", "anyOf")]
        [string]$CompositionName = "oneOf",

        [Parameter()]
        [switch]$RequireDiscriminator,

        [Parameter()]
        [switch]$IgnoreSchemaDiscriminatorMetadata,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Nested discriminator union analysis exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
    }
    if (-not (Test-JsonObjectValue -Value $Schema)) {
        return $null
    }
    if ($RequireDiscriminator) {
        $discriminator = Get-JsonPropertyValue -Object $Schema -Name "discriminator"
        if (-not (Test-JsonObjectValue -Value $discriminator)) {
            return $null
        }
        $actualPropertyName = Get-JsonPropertyValue -Object $discriminator -Name "propertyName"
        if ($actualPropertyName -isnot [string] -or
            -not [string]::Equals($actualPropertyName, $PropertyName, [System.StringComparison]::Ordinal)) {
            return $null
        }
    }

    $composition = Get-JsonPropertyValue -Object $Schema -Name $CompositionName
    if (-not (Test-JsonArrayValue -Value $composition) -or $composition.Count -eq 0) {
        return $null
    }

    $values = [System.Collections.Generic.List[string]]::new()
    $seenValues = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $branchPlans = [System.Collections.Generic.List[object]]::new()
    for ($index = 0; $index -lt $composition.Count; $index++) {
        $branch = $composition[$index]
        $branchBreadcrumb = "$Breadcrumb/$CompositionName/$index"
        if (-not (Test-JsonObjectValue -Value $branch)) {
            return $null
        }

        $isNestedGroup = Test-JsonProperty -Object $branch -Name "oneOf"
        if ($isNestedGroup) {
            $hasBranchType = Test-JsonProperty -Object $branch -Name "type"
            $branchType = Get-JsonPropertyValue -Object $branch -Name "type"
            # Composition-only groups may omit type because their recursively
            # validated leaves are explicit objects. Any present type remains
            # strict and must be the exact scalar string "object".
            if (($hasBranchType -and
                    ($branchType -isnot [string] -or
                        -not [string]::Equals($branchType, "object", [System.StringComparison]::Ordinal))) -or
                (Test-JsonProperty -Object $branch -Name '$ref')) {
                return $null
            }
            $branchDiscriminator = Get-JsonPropertyValue -Object $branch -Name "discriminator"
            $nestedPropertyName = if (Test-JsonObjectValue -Value $branchDiscriminator) {
                Get-JsonPropertyValue -Object $branchDiscriminator -Name "propertyName"
            }
            else {
                $null
            }
            if ($nestedPropertyName -isnot [string] -or [string]::IsNullOrWhiteSpace($nestedPropertyName)) {
                return $null
            }
            $nestedPlan = Get-OpenApiInlineDiscriminatorUnionPlan `
                -Schema $branch `
                -PropertyName $nestedPropertyName `
                -Breadcrumb $branchBreadcrumb `
                -RequireDiscriminator `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
            if ($null -eq $nestedPlan) {
                return $null
            }
            $kind = "FlattenableGroup"
            if ([string]::Equals($nestedPropertyName, $PropertyName, [System.StringComparison]::Ordinal)) {
                if (-not (Test-OpenApiDiscriminatorMetadataCompatible `
                        -ParentSchema $Schema `
                        -GroupSchema $branch `
                        -PropertyName $PropertyName `
                        -IgnoreParentDiscriminator:$IgnoreSchemaDiscriminatorMetadata) -or
                    -not (Test-OpenApiDiscriminatorUnionSiblingsMatch `
                        -ParentSchema $Schema `
                        -GroupSchema $branch `
                        -ParentCompositionName $CompositionName `
                        -GroupCompositionName "oneOf")) {
                    return $null
                }
                [string[]]$branchValues = [string[]]$nestedPlan.Values
            }
            else {
                $commonValue = Get-OpenApiCommonNestedDiscriminatorValue `
                    -Plan $nestedPlan `
                    -PropertyName $PropertyName `
                    -Depth ($Depth + 1) `
                    -MaximumDepth $MaximumDepth
                if ($null -eq $commonValue -or
                    -not (Test-OpenApiNestedDiscriminatorPropertyCompatible `
                        -Schema $branch `
                        -PropertyName $PropertyName `
                        -Value $commonValue `
                        -Breadcrumb $branchBreadcrumb)) {
                    return $null
                }
                $kind = "NestedDiscriminatorGroup"
                [string[]]$branchValues = @($commonValue)
            }
            $branchPlans.Add([PSCustomObject]@{
                Kind = $kind
                Branch = $branch
                Breadcrumb = $branchBreadcrumb
                Values = $branchValues
                NestedPlan = $nestedPlan
            })
        }
        else {
            $branchType = Get-JsonPropertyValue -Object $branch -Name "type"
            if ($branchType -isnot [string] -or
                -not [string]::Equals($branchType, "object", [System.StringComparison]::Ordinal)) {
                return $null
            }
            try {
                [string[]]$branchValues = @(Get-OpenApiDiscriminatorPropertyValues `
                    -Schema $branch `
                    -PropertyName $PropertyName `
                    -Breadcrumb $branchBreadcrumb)
            }
            catch {
                return $null
            }
            $branchPlans.Add([PSCustomObject]@{
                Kind = "Leaf"
                Branch = $branch
                Breadcrumb = $branchBreadcrumb
                Values = $branchValues
                NestedPlan = $null
            })
        }

        foreach ($value in $branchValues) {
            if (-not $seenValues.Add($value)) {
                return $null
            }
            $values.Add($value)
        }
    }

    return [PSCustomObject]@{
        Schema = $Schema
        PropertyName = $PropertyName
        CompositionName = $CompositionName
        Branches = $composition
        BranchPlans = [object[]]$branchPlans.ToArray()
        Values = [string[]]$values.ToArray()
    }
}

# Normalizes a discriminator mapping target to a local component-schema root.
# OpenAPI permits either a schema name or a reference string in the mapping.
function Resolve-OpenApiDiscriminatorMappingTarget {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [string]$Target,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    $pointer = if ($Target.StartsWith("#", [System.StringComparison]::Ordinal)) {
        Get-CanonicalJsonPointer -Pointer $Target
    }
    else {
        "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $Target)"
    }
    [string[]]$tokens = Get-JsonPointerTokens -Pointer $pointer
    if ($tokens.Count -ne 3 -or
        -not [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal) -or
        -not [string]::Equals($tokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        throw "Discriminator mapping target '$Target' at '$Breadcrumb' must identify a local component schema root."
    }

    $schema = Resolve-LocalJsonPointer `
        -Document $Document `
        -Pointer $pointer `
        -Context "discriminator mapping target at $Breadcrumb"
    return [PSCustomObject]@{
        Pointer = $pointer
        ComponentName = $tokens[2]
        Schema = $schema
    }
}

# Builds and validates the canonical mapping for one discriminator-bearing schema.
function Get-OpenApiDiscriminatorMappingPlan {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb
    )

    $discriminator = Get-JsonPropertyValue -Object $Schema -Name "discriminator"
    if (-not (Test-JsonObjectValue -Value $discriminator)) {
        throw "Discriminator at '$Breadcrumb/discriminator' must be an object."
    }
    $propertyName = Get-JsonPropertyValue -Object $discriminator -Name "propertyName"
    if ($propertyName -isnot [string] -or [string]::IsNullOrWhiteSpace($propertyName)) {
        throw "Discriminator propertyName at '$Breadcrumb/discriminator/propertyName' must be a nonempty string."
    }

    $compositionNames = @(
        foreach ($name in @("oneOf", "anyOf", "allOf")) {
            if (Test-JsonProperty -Object $Schema -Name $name) {
                $name
            }
        }
    )
    if ($compositionNames.Count -gt 1) {
        throw "Discriminator at '$Breadcrumb/discriminator' may have at most one direct oneOf, anyOf, or allOf sibling."
    }
    $compositionName = if ($compositionNames.Count -eq 1) {
        $compositionNames[0]
    }
    else {
        "mapping"
    }
    $mapping = Get-JsonPropertyValue -Object $discriminator -Name "mapping"

    if ([string]::Equals($compositionName, "allOf", [System.StringComparison]::Ordinal) -or
        [string]::Equals($compositionName, "mapping", [System.StringComparison]::Ordinal)) {
        if (-not (Test-JsonObjectValue -Value $mapping) -or @(Get-JsonProperties -Value $mapping).Count -eq 0) {
            throw "$compositionName discriminator at '$Breadcrumb/discriminator' requires a nonempty explicit mapping."
        }
        $canonicalMapping = [ordered]@{}
        $valuesByTarget = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
        foreach ($mappingProperty in (Get-JsonProperties -Value $mapping)) {
            if ($mappingProperty.Value -isnot [string]) {
                throw "Discriminator mapping '$($mappingProperty.Name)' at '$Breadcrumb/discriminator/mapping' must be a string."
            }
            $mappingBreadcrumb = "$Breadcrumb/discriminator/mapping/$(ConvertTo-JsonPointerToken -Token $mappingProperty.Name)"
            $target = Resolve-OpenApiDiscriminatorMappingTarget `
                -Document $Document `
                -Target $mappingProperty.Value `
                -Breadcrumb $mappingBreadcrumb
            if (-not $valuesByTarget.ContainsKey($target.Pointer)) {
                $valuesByTarget[$target.Pointer] = [string[]]@(Get-OpenApiDiscriminatorPropertyValues `
                    -Schema $target.Schema `
                    -PropertyName $propertyName `
                    -Breadcrumb $target.Pointer)
            }
            if ($valuesByTarget[$target.Pointer] -cnotcontains $mappingProperty.Name) {
                throw "Discriminator mapping key '$($mappingProperty.Name)' at '$mappingBreadcrumb' is not accepted by '$($target.Pointer)'."
            }
            $canonicalMapping[$mappingProperty.Name] = $target.Pointer
        }
        foreach ($targetPointer in $valuesByTarget.Keys) {
            foreach ($value in [string[]]$valuesByTarget[$targetPointer]) {
                if (-not $canonicalMapping.Contains($value) -or
                    -not [string]::Equals($canonicalMapping[$value], $targetPointer, [System.StringComparison]::Ordinal)) {
                    throw "$compositionName discriminator mapping at '$Breadcrumb/discriminator/mapping' does not map accepted value '$value' to '$targetPointer'."
                }
            }
        }
        return [PSCustomObject]@{
            CompositionName = $compositionName
            Mapping = $canonicalMapping
            ExistingMapping = $mapping
        }
    }

    $composition = Get-JsonPropertyValue -Object $Schema -Name $compositionName
    if (-not (Test-JsonArrayValue -Value $composition) -or $composition.Count -eq 0) {
        throw "Discriminator composition '$Breadcrumb/$compositionName' must be a nonempty array."
    }
    $expectedMapping = [ordered]@{}
    for ($index = 0; $index -lt $composition.Count; $index++) {
        $branch = $composition[$index]
        $branchBreadcrumb = "$Breadcrumb/$compositionName/$index"
        if (-not (Test-JsonObjectValue -Value $branch)) {
            throw "Discriminator alternative at '$branchBreadcrumb' must be a component reference object."
        }
        $branchProperties = @(Get-JsonProperties -Value $branch)
        if ($branchProperties.Count -ne 1 -or
            -not [string]::Equals($branchProperties[0].Name, '$ref', [System.StringComparison]::Ordinal) -or
            $branchProperties[0].Value -isnot [string]) {
            throw "Discriminator alternative at '$branchBreadcrumb' must contain only a string `$ref to a component schema root."
        }
        $target = Resolve-OpenApiDiscriminatorMappingTarget `
            -Document $Document `
            -Target $branchProperties[0].Value `
            -Breadcrumb "$branchBreadcrumb/`$ref"
        [string[]]$acceptedValues = @(Get-OpenApiDiscriminatorPropertyValues `
            -Schema $target.Schema `
            -PropertyName $propertyName `
            -Breadcrumb $target.Pointer)
        foreach ($value in $acceptedValues) {
            if ($expectedMapping.Contains($value)) {
                throw "Discriminator value '$value' at '$branchBreadcrumb' is also accepted by another $compositionName alternative."
            }
            $expectedMapping[$value] = $target.Pointer
        }
    }

    if ($null -ne $mapping) {
        if (-not (Test-JsonObjectValue -Value $mapping)) {
            throw "Discriminator mapping at '$Breadcrumb/discriminator/mapping' must be an object."
        }
        foreach ($mappingProperty in (Get-JsonProperties -Value $mapping)) {
            if (-not $expectedMapping.Contains($mappingProperty.Name)) {
                throw "Discriminator mapping key '$($mappingProperty.Name)' at '$Breadcrumb/discriminator/mapping' is not accepted by any $compositionName alternative."
            }
            if ($mappingProperty.Value -isnot [string]) {
                throw "Discriminator mapping '$($mappingProperty.Name)' at '$Breadcrumb/discriminator/mapping' must be a string."
            }
            $mappingBreadcrumb = "$Breadcrumb/discriminator/mapping/$(ConvertTo-JsonPointerToken -Token $mappingProperty.Name)"
            $target = Resolve-OpenApiDiscriminatorMappingTarget `
                -Document $Document `
                -Target $mappingProperty.Value `
                -Breadcrumb $mappingBreadcrumb
            if (-not [string]::Equals($target.Pointer, $expectedMapping[$mappingProperty.Name], [System.StringComparison]::Ordinal)) {
                throw "Discriminator mapping key '$($mappingProperty.Name)' at '$mappingBreadcrumb' targets '$($target.Pointer)' instead of '$($expectedMapping[$mappingProperty.Name])'."
            }
        }
    }

    return [PSCustomObject]@{
        CompositionName = $compositionName
        Mapping = $expectedMapping
        ExistingMapping = $mapping
    }
}

# Copies a document while materializing complete canonical discriminator mappings.
function ConvertTo-OpenApiDiscriminatorMappedValue {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context,

        [Parameter(Mandatory = $true)]
        [string]$Breadcrumb,

        [Parameter(Mandatory = $true)]
        [ref]$DiscriminatorCount,

        [Parameter(Mandatory = $true)]
        [ref]$ChangedMappingCount,

        [Parameter(Mandatory = $true)]
        [ref]$MappingEntryCount,

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "Discriminator mapping materialization exceeds the supported depth of $MaximumDepth at '$Breadcrumb'."
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
            $items[$index] = ConvertTo-OpenApiDiscriminatorMappedValue `
                -Value $Value[$index] `
                -Document $Document `
                -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" `
                -DiscriminatorCount $DiscriminatorCount `
                -ChangedMappingCount $ChangedMappingCount `
                -MappingEntryCount $MappingEntryCount `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $plan = $null
    if ([string]::Equals($Context, "Schema", [System.StringComparison]::Ordinal) -and
        (Test-JsonProperty -Object $Value -Name "discriminator")) {
        $plan = Get-OpenApiDiscriminatorMappingPlan `
            -Document $Document `
            -Schema $Value `
            -Breadcrumb $Breadcrumb
        $DiscriminatorCount.Value++
        $MappingEntryCount.Value += @(Get-JsonProperties -Value $plan.Mapping).Count
        if (-not (Test-JsonValuesEqual -Left $plan.ExistingMapping -Right $plan.Mapping)) {
            $ChangedMappingCount.Value++
        }
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ($null -ne $plan -and
            [string]::Equals($property.Name, "discriminator", [System.StringComparison]::Ordinal)) {
            $discriminator = [ordered]@{}
            $insertedMapping = $false
            foreach ($discriminatorProperty in (Get-JsonProperties -Value $property.Value)) {
                if ([string]::Equals($discriminatorProperty.Name, "mapping", [System.StringComparison]::Ordinal)) {
                    continue
                }
                $discriminator[$discriminatorProperty.Name] = Copy-JsonValue -Value $discriminatorProperty.Value
                if ([string]::Equals($discriminatorProperty.Name, "propertyName", [System.StringComparison]::Ordinal)) {
                    $discriminator["mapping"] = Copy-JsonValue -Value $plan.Mapping
                    $insertedMapping = $true
                }
            }
            if (-not $insertedMapping) {
                $discriminator["mapping"] = Copy-JsonValue -Value $plan.Mapping
            }
            $result[$property.Name] = $discriminator
            continue
        }

        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $Context `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $result[$property.Name] = ConvertTo-OpenApiDiscriminatorMappedValue `
            -Value $property.Value `
            -Document $Document `
            -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -DiscriminatorCount $DiscriminatorCount `
            -ChangedMappingCount $ChangedMappingCount `
            -MappingEntryCount $MappingEntryCount `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

function ConvertTo-OpenApiDiscriminatorMappedDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $discriminatorCount = 0
    $changedMappingCount = 0
    $mappingEntryCount = 0
    $converted = ConvertTo-OpenApiDiscriminatorMappedValue `
        -Value $Document `
        -Document $Document `
        -Context OpenApi `
        -Breadcrumb "#" `
        -DiscriminatorCount ([ref]$discriminatorCount) `
        -ChangedMappingCount ([ref]$changedMappingCount) `
        -MappingEntryCount ([ref]$mappingEntryCount)
    return [PSCustomObject]@{
        Document = $converted
        DiscriminatorCount = $discriminatorCount
        ChangedMappingCount = $changedMappingCount
        MappingEntryCount = $mappingEntryCount
    }
}

function Assert-OpenApiDiscriminatorMappings {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    $validation = ConvertTo-OpenApiDiscriminatorMappedDocument -Document $Document
    if (-not (Test-JsonValuesEqual -Left $Document -Right $validation.Document)) {
        throw "OpenAPI document contains incomplete or noncanonical discriminator mappings."
    }
    return $validation
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

# Combines expanded schema definitions without weakening their constraints.
# Documentation annotations use later-definition precedence; every validating
# keyword must either be unique or structurally identical across definitions.
function Merge-OpenApiSchemaDefinitions {
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
        throw "The schema definition list at '$Breadcrumb' must contain at least one object definition."
    }
    if ($Definitions.Count -ne $DefinitionBreadcrumbs.Count) {
        throw "Internal error: schema definitions and breadcrumbs have different lengths at '$Breadcrumb'."
    }

    $annotationKeywords = @("title", "description", '$comment')
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
    $propertyOrder = [System.Collections.Generic.List[string]]::new()
    $propertySources = [System.Collections.Generic.Dictionary[string, string]]::new(
        [System.StringComparer]::Ordinal
    )
    $requiredNames = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )
    $requiredOrder = [System.Collections.Generic.List[string]]::new()
    $sawProperties = $false
    $sawRequired = $false

    for ($definitionIndex = 0; $definitionIndex -lt $Definitions.Count; $definitionIndex++) {
        $definition = $Definitions[$definitionIndex]
        $definitionBreadcrumb = $DefinitionBreadcrumbs[$definitionIndex]
        if (-not (Test-JsonObjectValue -Value $definition)) {
            throw "The schema definition at '$definitionBreadcrumb' must be an object definition."
        }

        foreach ($property in (Get-JsonProperties -Value $definition)) {
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
                            $propertyValues[$schemaProperty.Name] = Merge-OpenApiSchemaDefinitions `
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
                    $propertyOrder.Add($schemaProperty.Name)
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
                    if ($requiredNames.Add($requiredName)) {
                        $requiredOrder.Add($requiredName)
                    }
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
                            throw "Cannot merge schema definitions at '$Breadcrumb': enum constraints at '$($keywordSources[$property.Name])' and '$currentBreadcrumb' have no common values."
                        }
                        $keywordValues[$property.Name] = $intersection
                        $keywordSources[$property.Name] = "$Breadcrumb/enum (merged)"
                        continue
                    }
                    throw "Cannot merge schema definitions at '$Breadcrumb': keyword '$($property.Name)' has incompatible definitions at '$($keywordSources[$property.Name])' and '$currentBreadcrumb'."
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
        $orderedProperties = [ordered]@{}
        foreach ($propertyName in $propertyOrder) {
            $orderedProperties[$propertyName] = $propertyValues[$propertyName]
        }
        $mergedDefinition["properties"] = $orderedProperties
    }

    if ($sawRequired) {
        $mergedDefinition["required"] = @($requiredOrder)
    }

    return $mergedDefinition
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

# Returns the component entry containing a component pointer. Nested cycle
# pointers still retain the complete owning fallback component.
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext = "OpenApi",

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
        $itemContext = $SchemaContext
        if ([string]::Equals($SchemaContext, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Get-OpenApiReferenceOccurrences `
                -Value $Value[$index] `
                -Breadcrumb "$Breadcrumb/$index" `
                -SchemaContext $itemContext `
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
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $SchemaContext `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        Get-OpenApiReferenceOccurrences `
            -Value $property.Value `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -SchemaContext $childContext `
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

# Records a source-to-destination JSON Pointer prefix relocation. A source
# prefix may be registered repeatedly only when its destination is identical.
function Add-OpenApiPointerRelocation {
    param(
        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, string]]$Relocations,

        [Parameter(Mandatory = $true)]
        [string]$SourcePrefix,

        [Parameter(Mandatory = $true)]
        [string]$DestinationPrefix
    )

    if ($Relocations.ContainsKey($SourcePrefix)) {
        if (-not [string]::Equals(
            $Relocations[$SourcePrefix],
            $DestinationPrefix,
            [System.StringComparison]::Ordinal
        )) {
            throw "JSON Pointer prefix '$SourcePrefix' has conflicting relocation destinations."
        }
        return
    }

    $Relocations[$SourcePrefix] = $DestinationPrefix
}

# Existing callers follow chained moves. SinglePass is for transformations that
# already map original locations to final paths, including moves below themselves.
function Get-RelocatedOpenApiPointer {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, string]]$Relocations,

        [Parameter()]
        [switch]$SinglePass
    )

    if ([string]::Equals($Pointer, "#", [System.StringComparison]::Ordinal) -or
        -not $Pointer.StartsWith("#/", [System.StringComparison]::Ordinal)) {
        return $Pointer
    }

    $relocatedPointer = $Pointer
    $seenPointers = [System.Collections.Generic.HashSet[string]]::new(
        [System.StringComparer]::Ordinal
    )

    while ($seenPointers.Add($relocatedPointer)) {
        $matchingPrefix = $null
        foreach ($sourcePrefix in $Relocations.Keys) {
            if (($relocatedPointer.Length -eq $sourcePrefix.Length -or
                ($relocatedPointer.Length -gt $sourcePrefix.Length -and
                    $relocatedPointer[$sourcePrefix.Length] -eq '/')) -and
                $relocatedPointer.StartsWith($sourcePrefix, [System.StringComparison]::Ordinal) -and
                ($null -eq $matchingPrefix -or $sourcePrefix.Length -gt $matchingPrefix.Length)) {
                $matchingPrefix = $sourcePrefix
            }
        }

        if ($null -eq $matchingPrefix) {
            return $relocatedPointer
        }

        $relocatedPointer = $Relocations[$matchingPrefix] +
            $relocatedPointer.Substring($matchingPrefix.Length)
        if ($SinglePass) {
            return $relocatedPointer
        }
    }

    throw "JSON Pointer relocation contains a cycle for '$Pointer'."
}

# Rewrites reference-valued keywords and discriminator mappings after schema
# paths move.
function ConvertTo-RelocatedOpenApiPointers {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.Dictionary[string, string]]$Relocations,

        [Parameter()]
        [switch]$SinglePass,

        [Parameter()]
        [switch]$SchemaAware,

        [Parameter()]
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext = "OpenApi",

        [Parameter()]
        [ValidateSet("Default", "Discriminator", "Mapping")]
        [string]$Context = "Default",

        [Parameter()]
        [int]$Depth = 0,

        [Parameter()]
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) {
        throw "JSON Pointer relocation exceeds the supported depth of $MaximumDepth."
    }
    if ($null -eq $Value) {
        return $null
    }
    if ($SchemaAware -and $SchemaContext -ceq "Literal" -and $Context -ceq "Default") {
        return Copy-JsonValue -Value $Value
    }
    if (Test-JsonArrayValue -Value $Value) {
        $itemSchemaContext = if ($SchemaContext -ceq "SchemaArray") { "Schema" } else { $SchemaContext }
        [object[]]$items = [object[]]::new($Value.Count)
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $items[$index] = ConvertTo-RelocatedOpenApiPointers `
                -Value $Value[$index] `
                -Relocations $Relocations `
                -SinglePass:$SinglePass `
                -SchemaAware:$SchemaAware `
                -SchemaContext $itemSchemaContext `
                -Context Default `
                -Depth ($Depth + 1) `
                -MaximumDepth $MaximumDepth
        }
        return ,$items
    }
    if (-not (Test-JsonObjectValue -Value $Value)) {
        return $Value
    }

    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        if ([string]::Equals($Context, "Mapping", [System.StringComparison]::Ordinal) -and
            $property.Value -is [string]) {
            $result[$property.Name] = Get-RelocatedOpenApiPointer `
                -Pointer $property.Value `
                -Relocations $Relocations `
                -SinglePass:$SinglePass
            continue
        }

        if (@('$ref', '$dynamicRef', '$recursiveRef') -ccontains $property.Name -and
            $property.Value -is [string] -and
            (-not $SchemaAware -or $SchemaContext -cne "SchemaMap")) {
            $result[$property.Name] = Get-RelocatedOpenApiPointer `
                -Pointer $property.Value `
                -Relocations $Relocations `
                -SinglePass:$SinglePass
            continue
        }

        $childContext = "Default"
        if ([string]::Equals($Context, "Default", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "discriminator", [System.StringComparison]::Ordinal) -and
            (-not $SchemaAware -or $SchemaContext -ceq "Schema")) {
            $childContext = "Discriminator"
        }
        elseif ([string]::Equals($Context, "Discriminator", [System.StringComparison]::Ordinal) -and
            [string]::Equals($property.Name, "mapping", [System.StringComparison]::Ordinal)) {
            $childContext = "Mapping"
        }

        $childSchemaContext = if ($SchemaAware) {
            Get-OpenApiSchemaChildContext -ParentContext $SchemaContext -PropertyName $property.Name -PropertyValue $property.Value
        }
        else {
            $SchemaContext
        }
        $result[$property.Name] = ConvertTo-RelocatedOpenApiPointers `
            -Value $property.Value `
            -Relocations $Relocations `
            -SinglePass:$SinglePass `
            -SchemaAware:$SchemaAware `
            -SchemaContext $childSchemaContext `
            -Context $childContext `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }
    return $result
}

function Assert-NoOpenApiSchemaTypeArrays {
    param(
        [Parameter(Mandatory = $true)]
        [AllowNull()]
        [object]$Value,

        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$Context = "OpenApi",

        [string]$Breadcrumb = "#",
        [int]$Depth = 0,
        [int]$MaximumDepth = 200
    )

    if ($Depth -gt $MaximumDepth) { throw "Schema type-array inspection exceeds depth $MaximumDepth at '$Breadcrumb'." }
    if ($null -eq $Value -or $Context -ceq "Literal") { return }
    if (Test-JsonArrayValue -Value $Value) {
        $itemContext = if ($Context -ceq "SchemaArray") { "Schema" } else { $Context }
        for ($index = 0; $index -lt $Value.Count; $index++) {
            Assert-NoOpenApiSchemaTypeArrays -Value $Value[$index] -Context $itemContext `
                -Breadcrumb "$Breadcrumb/$index" -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
        }
        return
    }
    if (-not (Test-JsonObjectValue -Value $Value)) { return }
    if ($Context -ceq "SchemaArray") { $Context = "Schema" }
    if ($Context -ceq "Schema" -and (Test-JsonArrayValue -Value (Get-JsonPropertyValue -Object $Value -Name "type"))) {
        throw "Schema type array remains at '$Breadcrumb/type'. Run Normalize-OpenApiTypeArrays.ps1 before componentization."
    }
    foreach ($property in (Get-JsonProperties -Value $Value)) {
        $childContext = Get-OpenApiSchemaChildContext -ParentContext $Context -PropertyName $property.Name -PropertyValue $property.Value
        Assert-NoOpenApiSchemaTypeArrays -Value $property.Value -Context $childContext `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" -Depth ($Depth + 1) -MaximumDepth $MaximumDepth
    }
}

# Validates dynamic and recursive references not covered by the shared
# OpenAPI $ref/discriminator validator.
function Assert-AdditionalOpenApiReferencesResolve {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    function Visit-ReferenceValue {
        param([AllowNull()][object]$Value, [string]$Breadcrumb)

        if ($null -eq $Value) {
            return
        }
        if (Test-JsonArrayValue -Value $Value) {
            for ($index = 0; $index -lt $Value.Count; $index++) {
                Visit-ReferenceValue -Value $Value[$index] -Breadcrumb "$Breadcrumb/$index"
            }
            return
        }
        if (-not (Test-JsonObjectValue -Value $Value)) {
            return
        }

        foreach ($property in (Get-JsonProperties -Value $Value)) {
            $propertyBreadcrumb = "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)"
            if (@('$dynamicRef', '$recursiveRef') -ccontains $property.Name -and
                $property.Value -is [string] -and
                -not [string]::Equals($property.Value, "#", [System.StringComparison]::Ordinal)) {
                $null = Resolve-LocalJsonPointer `
                    -Document $Document `
                    -Pointer $property.Value `
                    -Context "$($property.Name) at $propertyBreadcrumb"
            }
            Visit-ReferenceValue -Value $property.Value -Breadcrumb $propertyBreadcrumb
        }
    }

    Visit-ReferenceValue -Value $Document -Breadcrumb "#"
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
            -SchemaContext Literal `
            -Depth ($Depth + 1) `
            -MaximumDepth $MaximumDepth
    }

    return $expandedDiscriminator
}

# Returns every sibling of a $ref property.
function Get-ReferenceSiblingProperties {
    param(
        [Parameter(Mandatory = $true)]
        [object]$ReferenceObject
    )

    [object[]]$siblings = @(Get-JsonProperties -Value $ReferenceObject | Where-Object {
        -not [string]::Equals($_.Name, '$ref', [System.StringComparison]::Ordinal)
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $null = $CycleReferences.Add($CanonicalPointer)

    $cycleObject = [ordered]@{ '$ref' = $CanonicalPointer }
    foreach ($siblingProperty in $SiblingProperties) {
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $SchemaContext `
            -PropertyName $siblingProperty.Name `
            -PropertyValue $siblingProperty.Value
        $cycleObject[$siblingProperty.Name] = Expand-OpenApiReferences `
            -Value $siblingProperty.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $siblingProperty.Name)" `
            -SchemaContext $childContext `
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext,

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
        -SchemaContext $SchemaContext `
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
        -SchemaContext $SchemaContext `
        -Depth ($Depth + 1) `
        -MaximumDepth $MaximumDepth

    return Merge-OpenApiSchemaDefinitions `
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext,

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
        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $SchemaContext `
            -PropertyName $siblingProperty.Name `
            -PropertyValue $siblingProperty.Value
        $mergedTarget[$siblingProperty.Name] = Expand-OpenApiReferences `
            -Value $siblingProperty.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $siblingProperty.Name)" `
            -SchemaContext $childContext `
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext,

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
    $siblingProperties = Get-ReferenceSiblingProperties `
        -ReferenceObject $ReferenceObject

    if ($ActiveReferences.Contains($canonicalPointer)) {
        return Expand-CycleReferenceObject `
            -CanonicalPointer $canonicalPointer `
            -SiblingProperties $siblingProperties `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -SchemaContext $SchemaContext `
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
                -SchemaContext $SchemaContext `
                -Depth $Depth `
                -MaximumDepth $MaximumDepth
        }

        $expandedTarget = Expand-OpenApiReferences `
            -Value $target `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb $Breadcrumb `
            -SchemaContext $SchemaContext `
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
        -SchemaContext $SchemaContext `
        -Depth $Depth `
        -MaximumDepth $MaximumDepth
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext,

        [Parameter(Mandatory = $true)]
        [int]$Depth,

        [Parameter(Mandatory = $true)]
        [int]$MaximumDepth
    )

    $expandedObject = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Value)) {
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

        $childContext = Get-OpenApiSchemaChildContext `
            -ParentContext $SchemaContext `
            -PropertyName $property.Name `
            -PropertyValue $property.Value
        $expandedObject[$property.Name] = Expand-OpenApiReferences `
            -Value $property.Value `
            -Document $Document `
            -ActiveReferences $ActiveReferences `
            -CycleReferences $CycleReferences `
            -Breadcrumb "$Breadcrumb/$(ConvertTo-JsonPointerToken -Token $property.Name)" `
            -SchemaContext $childContext `
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
        [ValidateSet("OpenApi", "Components", "SchemaMap", "SchemaArray", "Schema", "Literal")]
        [string]$SchemaContext = "OpenApi",

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
        $itemContext = $SchemaContext
        if ([string]::Equals($SchemaContext, "SchemaArray", [System.StringComparison]::Ordinal)) {
            $itemContext = "Schema"
        }
        [object[]]$expandedItems = @(
            for ($index = 0; $index -lt $Value.Count; $index++) {
                Expand-OpenApiReferences `
                    -Value $Value[$index] `
                    -Document $Document `
                    -ActiveReferences $ActiveReferences `
                    -CycleReferences $CycleReferences `
                    -Breadcrumb "$Breadcrumb/$index" `
                    -SchemaContext $itemContext `
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
            -SchemaContext $SchemaContext `
            -Depth $Depth `
            -MaximumDepth $MaximumDepth
    }

    return Expand-OpenApiObjectProperties `
        -Value $Value `
        -Document $Document `
        -ActiveReferences $ActiveReferences `
        -CycleReferences $CycleReferences `
        -Breadcrumb $Breadcrumb `
        -SchemaContext $SchemaContext `
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
            -SchemaContext $(if ([string]::Equals($componentRoot.Section, "schemas", [System.StringComparison]::Ordinal)) { "Schema" } else { "OpenApi" }) `
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

# Returns only HTTP operation properties from a path item. Path-level metadata
# such as parameters and servers remains part of the extracted path item but is
# not mistaken for an operation while gathering tags and security requirements.
function Get-OpenApiOperations {
    param(
        [Parameter(Mandatory = $true)]
        [object]$PathItem
    )

    foreach ($property in (Get-JsonProperties -Value $PathItem)) {
        if ($script:OpenApiHttpMethodNames -ccontains $property.Name) {
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

# Projects the selected operation during extraction so excluded request or
# response content never reaches omission, inlining, or later stages.
function New-SelectedOpenApiOperation {
    param(
        [Parameter(Mandatory = $true)]
        [object]$OperationEntry,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "Both"
    )

    $operation = $OperationEntry.Operation
    if ([string]::Equals($Mode, "RequestBody", [System.StringComparison]::OrdinalIgnoreCase) -and
        -not (Test-JsonProperty -Object $operation -Name "requestBody")) {
        throw "OperationId '$($OperationEntry.OperationId)' does not define a requestBody required by RequestBody extraction mode."
    }
    if ([string]::Equals($Mode, "Responses", [System.StringComparison]::OrdinalIgnoreCase) -and
        -not (Test-JsonProperty -Object $operation -Name "responses")) {
        throw "OperationId '$($OperationEntry.OperationId)' does not define responses required by Responses extraction mode."
    }

    $selectedOperation = [ordered]@{}
    $replacedResponses = $false
    foreach ($property in (Get-JsonProperties -Value $operation)) {
        if ([string]::Equals($Mode, "Responses", [System.StringComparison]::OrdinalIgnoreCase) -and
            [string]::Equals($property.Name, "requestBody", [System.StringComparison]::Ordinal)) {
            continue
        }
        if ([string]::Equals($Mode, "RequestBody", [System.StringComparison]::OrdinalIgnoreCase) -and
            [string]::Equals($property.Name, "responses", [System.StringComparison]::Ordinal)) {
            $selectedOperation[$property.Name] = [ordered]@{
                default = [ordered]@{
                    description = "Responses omitted by RequestBody extraction mode."
                }
            }
            $replacedResponses = $true
            continue
        }
        $selectedOperation[$property.Name] = $property.Value
    }

    # Invalid source operations may omit responses. The placeholder still makes
    # a request-focused extraction conformant without traversing response data.
    if ([string]::Equals($Mode, "RequestBody", [System.StringComparison]::OrdinalIgnoreCase) -and
        -not $replacedResponses) {
        $selectedOperation["responses"] = [ordered]@{
            default = [ordered]@{
                description = "Responses omitted by RequestBody extraction mode."
            }
        }
    }

    return $selectedOperation
}

# Retains path-level metadata and only the selected, mode-projected HTTP operation.
function New-SelectedOpenApiPathItem {
    param(
        [Parameter(Mandatory = $true)]
        [object]$OperationEntry,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "Both"
    )

    $selectedPathItem = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $OperationEntry.PathItem)) {
        if ($script:OpenApiHttpMethodNames -ccontains $property.Name) {
            if (-not [string]::Equals(
                    $property.Name,
                    $OperationEntry.Method,
                    [System.StringComparison]::Ordinal
                )) {
                continue
            }
            $selectedPathItem[$property.Name] = New-SelectedOpenApiOperation `
                -OperationEntry $OperationEntry `
                -Mode $Mode
            continue
        }
        $selectedPathItem[$property.Name] = $property.Value
    }
    return $selectedPathItem
}

# Creates a focused OpenAPI document for one selected operation without
# interpreting metadata or resolving references.
function New-ExtractedOpenApiDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document,

        [Parameter(Mandatory = $true)]
        [object]$OperationEntry,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "Both"
    )

    $selectedPathItem = New-SelectedOpenApiPathItem `
        -OperationEntry $OperationEntry `
        -Mode $Mode

    $usedTagNames = Get-UsedTagNames -PathItem $selectedPathItem
    $selectedTags = Get-SelectedTagDefinitions `
        -Document $Document `
        -UsedTagNames $usedTagNames
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
    $selectedPaths[$OperationEntry.Route] = Copy-JsonValue -Value $selectedPathItem
    $extractedDocument["paths"] = $selectedPaths
    $sourceComponents = Get-JsonPropertyValue -Object $Document -Name "components"
    if (Test-JsonObjectValue -Value $sourceComponents) {
        $extractedDocument["components"] = Copy-JsonValue -Value $sourceComponents
    }

    return [PSCustomObject]@{
        Document = $extractedDocument
    }
}

# Converts an operationId to a portable, readable filename stem.
function ConvertTo-OperationFileStem {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OperationId
    )

    $stem = $OperationId -replace "[^A-Za-z0-9._-]+", "-"
    $stem = ($stem -replace "-+", "-").Trim("-", ".")
    if ([string]::IsNullOrWhiteSpace($stem)) {
        throw "OperationId '$OperationId' does not produce a valid filename."
    }
    return $stem
}

# Computes a short stable suffix only when two operationIds sanitize to the
# same stem, keeping ordinary output names uncluttered.
function Get-DefaultOutputPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$OperationId,

        [Parameter(Mandatory = $true)]
        [object[]]$Catalog,

        [Parameter(Mandatory = $true)]
        [string]$RepositoryRoot,

        [Parameter()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode = "Both"
    )

    $stem = ConvertTo-OperationFileStem -OperationId $OperationId
    $collisions = @($Catalog | Where-Object {
        [string]::Equals(
            (ConvertTo-OperationFileStem -OperationId $_.OperationId),
            $stem,
            [System.StringComparison]::OrdinalIgnoreCase
        )
    })
    if ($collisions.Count -gt 1) {
        $operationIdBytes = [System.Text.Encoding]::UTF8.GetBytes($OperationId)
        $hashBytes = [System.Security.Cryptography.SHA256]::HashData($operationIdBytes)
        $hash = [System.Convert]::ToHexString($hashBytes).Substring(0, 8).ToLowerInvariant()
        $stem = "$stem-$hash"
    }

    $modeSuffix = switch ($Mode) {
        "RequestBody" { ".request-body" }
        "Responses" { ".responses" }
        default { "" }
    }
    $outputDirectory = Join-Path $RepositoryRoot "artifacts" "openapi-extracts"
    return Join-Path $outputDirectory "$stem$modeSuffix.extracted.openapi.json"
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
        $json = $json.Replace("`r`n", "`n").Replace("`r", "`n")
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

function Invoke-OpenApiOperationExtractor {
    param(
        [Parameter()]
        [string]$SpecificationPath,

        [Parameter()]
        [string]$OperationId,

        [Parameter()]
        [AllowNull()]
        [ValidateSet("RequestBody", "Responses", "Both")]
        [string]$Mode,

        [Parameter()]
        [string]$OutputPath
    )

    $repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".." ".."))
    if ([string]::IsNullOrWhiteSpace($SpecificationPath)) {
        $SpecificationPath = Join-Path $repositoryRoot "specification" "openapi" "openapi.json"
    }
    else {
        $SpecificationPath = Resolve-RepositoryPath -Path $SpecificationPath -RepositoryRoot $repositoryRoot
    }

    $document = Read-OpenApiDocument -Path $SpecificationPath
    $catalog = Get-OpenApiOperationCatalog -Document $document
    $selectedOperation = Select-OpenApiOperation `
        -Catalog $catalog `
        -RequestedOperationId $OperationId
    $selectedMode = Select-OpenApiExtractionMode -RequestedMode $Mode
    $extraction = New-ExtractedOpenApiDocument `
        -Document $document `
        -OperationEntry $selectedOperation `
        -Mode $selectedMode

    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = Get-DefaultOutputPath `
            -OperationId $selectedOperation.OperationId `
            -Catalog $catalog `
            -RepositoryRoot $repositoryRoot `
            -Mode $selectedMode
    }
    else {
        $OutputPath = Resolve-RepositoryPath -Path $OutputPath -RepositoryRoot $repositoryRoot
    }

    $writtenPath = Write-OpenApiJsonFile `
        -Document $extraction.Document `
        -Path $OutputPath

    Write-Host ""
    Write-Host "OperationId: $($selectedOperation.OperationId)"
    Write-Host "Operation: $($selectedOperation.Method.ToUpperInvariant()) $($selectedOperation.Route)"
    Write-Host "Content: $(Get-OpenApiExtractionModeLabel -Mode $selectedMode)"
    Write-Host "Output: $writtenPath"
}

