<#
.SYNOPSIS
    Refactors componentized discriminated oneOf schemas into inheritance.

.DESCRIPTION
    Replaces each eligible component that directly owns a discriminator and
    oneOf with a discriminator-only object base. Each mapped child retains its
    direct selector and payload constraints and receives one allOf reference to
    the base. A child shared by multiple discriminator parents is cloned once
    per parent so every derived component has exactly one base.

.PARAMETER InputPath
    The componentized OpenAPI JSON document.

.PARAMETER OutputPath
    The destination JSON file. When omitted, inserts
    '.discriminator-inherited' before the OpenAPI JSON suffix.

.EXAMPLE
    ./scripts/specification/pipeline/Refactor-OpenApiDiscriminatedInheritance.ps1 `
        ./specification/openapi/responses.componentized.openapi.json
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

$script:InheritanceRepositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".." ".." ".."))
$script:DefaultInheritanceInputPath = Join-Path $script:InheritanceRepositoryRoot "specification" "openapi" "responses.componentized.openapi.json"
$script:DefaultInheritanceOutputPath = Join-Path $script:InheritanceRepositoryRoot "specification" "openapi" "responses.componentized.discriminator-inherited.openapi.json"

function ConvertTo-OpenApiInheritanceNameToken {
    param(
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]$Value
    )

    $segments = [System.Text.RegularExpressions.Regex]::Matches(
        $Value,
        "[A-Z]+(?=[A-Z][a-z]|[0-9]|$)|[A-Z]?[a-z]+|[0-9]+"
    )
    $builder = [System.Text.StringBuilder]::new()
    foreach ($segmentMatch in $segments) {
        $segment = $segmentMatch.Value.ToLowerInvariant()
        if ($segment.Length -eq 0) {
            continue
        }
        if ([char]::IsDigit($segment[0]) -and $builder.Length -eq 0) {
            $null = $builder.Append("X")
        }
        $null = $builder.Append([char]::ToUpperInvariant($segment[0]))
        if ($segment.Length -gt 1) {
            $null = $builder.Append($segment.Substring(1))
        }
    }
    return $builder.ToString()
}

function Get-OpenApiInheritanceComponentPointer {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ComponentName
    )

    return "#/components/schemas/$(ConvertTo-JsonPointerToken -Token $ComponentName)"
}

function Get-OpenApiInheritanceComponentNameFromPointer {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Pointer
    )

    if (-not $Pointer.StartsWith("#", [System.StringComparison]::Ordinal)) {
        return $null
    }
    [string[]]$tokens = Get-JsonPointerTokens -Pointer (Get-CanonicalJsonPointer -Pointer $Pointer)
    if ($tokens.Count -lt 3 -or
        -not [string]::Equals($tokens[0], "components", [System.StringComparison]::Ordinal) -or
        -not [string]::Equals($tokens[1], "schemas", [System.StringComparison]::Ordinal)) {
        return $null
    }
    return $tokens[2]
}

function New-OpenApiInheritedChildSchema {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Schema,

        [Parameter(Mandatory = $true)]
        [string]$ParentName
    )

    if (Test-JsonProperty -Object $Schema -Name "allOf") {
        throw "Cannot add inheritance to a child schema that already declares allOf."
    }
    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Schema)) {
        $result[$property.Name] = Copy-JsonValue -Value $property.Value
    }
    $result["allOf"] = [object[]]@([ordered]@{
        '$ref' = Get-OpenApiInheritanceComponentPointer -ComponentName $ParentName
    })
    return $result
}

function Get-OpenApiUniqueInheritedComponentName {
    param(
        [Parameter(Mandatory = $true)]
        [string]$BaseName,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.HashSet[string]]$ReservedNames
    )

    if ($ReservedNames.Add($BaseName)) {
        return $BaseName
    }
    $suffix = 1
    while ($true) {
        $candidate = "${BaseName}_$suffix"
        if ($ReservedNames.Add($candidate)) {
            return $candidate
        }
        $suffix++
    }
}

function ConvertTo-OpenApiDiscriminatedInheritanceDocument {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Document
    )

    Assert-OpenApiReferencesResolve -Document $Document
    Assert-AdditionalOpenApiReferencesResolve -Document $Document
    $null = Assert-OpenApiDiscriminatorMappings -Document $Document

    $components = Get-JsonPropertyValue -Object $Document -Name "components"
    $schemas = if (Test-JsonObjectValue -Value $components) {
        Get-JsonPropertyValue -Object $components -Name "schemas"
    }
    else {
        $null
    }
    if (-not (Test-JsonObjectValue -Value $schemas)) {
        throw "The OpenAPI document must contain a components.schemas object."
    }

    [object[]]$schemaProperties = @(Get-JsonProperties -Value $schemas)
    $schemaByName = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $reservedNames = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($property in $schemaProperties) {
        $schemaByName[$property.Name] = $property.Value
        $null = $reservedNames.Add($property.Name)
    }

    $plans = [System.Collections.Generic.List[object]]::new()
    $planByParent = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $edgesByChild = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $branchReferenceBreadcrumbs = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)

    foreach ($schemaProperty in $schemaProperties) {
        $parentName = $schemaProperty.Name
        $schema = $schemaProperty.Value
        if (-not (Test-JsonObjectValue -Value $schema) -or
            -not (Test-JsonProperty -Object $schema -Name "discriminator") -or
            -not (Test-JsonProperty -Object $schema -Name "oneOf")) {
            continue
        }

        $parentPointer = Get-OpenApiInheritanceComponentPointer -ComponentName $parentName
        $allowedProperties = [System.Collections.Generic.HashSet[string]]::new(
            [string[]]@("type", "discriminator", "oneOf"),
            [System.StringComparer]::Ordinal
        )
        foreach ($property in (Get-JsonProperties -Value $schema)) {
            if (-not $allowedProperties.Contains($property.Name)) {
                throw "Discriminator parent '$parentName' has unsupported sibling '$($property.Name)'."
            }
        }
        if (Test-JsonProperty -Object $schema -Name "type") {
            $parentType = Get-JsonPropertyValue -Object $schema -Name "type"
            if ($parentType -isnot [string] -or
                -not [string]::Equals($parentType, "object", [System.StringComparison]::Ordinal)) {
                throw "Discriminator parent '$parentName' may declare only type 'object'."
            }
        }

        $discriminator = Get-JsonPropertyValue -Object $schema -Name "discriminator"
        $propertyName = if (Test-JsonObjectValue -Value $discriminator) {
            Get-JsonPropertyValue -Object $discriminator -Name "propertyName"
        }
        else {
            $null
        }
        if ($propertyName -isnot [string] -or [string]::IsNullOrWhiteSpace($propertyName)) {
            throw "Discriminator parent '$parentName' must have a nonempty propertyName."
        }
        $mapping = Get-JsonPropertyValue -Object $discriminator -Name "mapping"
        if (-not (Test-JsonObjectValue -Value $mapping) -or
            @(Get-JsonProperties -Value $mapping).Count -eq 0) {
            throw "Discriminator parent '$parentName' must have a nonempty exhaustive mapping."
        }
        $oneOf = Get-JsonPropertyValue -Object $schema -Name "oneOf"
        if (-not (Test-JsonArrayValue -Value $oneOf) -or $oneOf.Count -eq 0) {
            throw "Discriminator parent '$parentName' must have a nonempty oneOf array."
        }

        $mappingKeyByTarget = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
        foreach ($mappingProperty in (Get-JsonProperties -Value $mapping)) {
            if ($mappingProperty.Value -isnot [string]) {
                throw "Discriminator mapping '$parentName/$($mappingProperty.Name)' must be a string."
            }
            $target = Resolve-OpenApiDiscriminatorMappingTarget `
                -Document $Document `
                -Target $mappingProperty.Value `
                -Breadcrumb "$parentPointer/discriminator/mapping/$(ConvertTo-JsonPointerToken -Token $mappingProperty.Name)"
            if ($mappingKeyByTarget.ContainsKey($target.ComponentName)) {
                throw "Discriminator parent '$parentName' maps multiple values to child '$($target.ComponentName)'; inheritance requires one selector value per child."
            }
            $mappingKeyByTarget[$target.ComponentName] = $mappingProperty.Name
        }

        $edges = [System.Collections.Generic.List[object]]::new()
        $seenChildren = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        for ($index = 0; $index -lt $oneOf.Count; $index++) {
            $branch = $oneOf[$index]
            [object[]]$branchProperties = @()
            if (Test-JsonObjectValue -Value $branch) {
                $branchProperties = @(Get-JsonProperties -Value $branch)
            }
            if ($branchProperties.Count -ne 1 -or
                -not [string]::Equals($branchProperties[0].Name, '$ref', [System.StringComparison]::Ordinal) -or
                $branchProperties[0].Value -isnot [string]) {
                throw "Discriminator branch '$parentPointer/oneOf/$index' must contain only a local component `$ref."
            }
            $target = Resolve-OpenApiDiscriminatorMappingTarget `
                -Document $Document `
                -Target $branchProperties[0].Value `
                -Breadcrumb "$parentPointer/oneOf/$index/`$ref"
            if (-not $seenChildren.Add($target.ComponentName)) {
                throw "Discriminator parent '$parentName' references child '$($target.ComponentName)' more than once."
            }
            if (-not $mappingKeyByTarget.ContainsKey($target.ComponentName)) {
                throw "Discriminator parent '$parentName' has no mapping key for child '$($target.ComponentName)'."
            }
            $mappingKey = $mappingKeyByTarget[$target.ComponentName]
            $childSchema = $target.Schema
            if (-not (Test-JsonObjectValue -Value $childSchema)) {
                throw "Discriminator child '$($target.ComponentName)' must be a schema object."
            }
            if (Test-JsonProperty -Object $childSchema -Name "allOf") {
                throw "Discriminator child '$($target.ComponentName)' already declares allOf."
            }
            $required = Get-JsonPropertyValue -Object $childSchema -Name "required"
            if (-not (Test-JsonArrayValue -Value $required) -or
                -not (@($required | Where-Object {
                    $_ -is [string] -and [string]::Equals($_, $propertyName, [System.StringComparison]::Ordinal)
                }).Count -gt 0)) {
                throw "Discriminator child '$($target.ComponentName)' must directly require '$propertyName'."
            }
            $properties = Get-JsonPropertyValue -Object $childSchema -Name "properties"
            $selector = if (Test-JsonObjectValue -Value $properties) {
                Get-JsonPropertyValue -Object $properties -Name $propertyName
            }
            else {
                $null
            }
            $selectorType = if (Test-JsonObjectValue -Value $selector) {
                Get-JsonPropertyValue -Object $selector -Name "type"
            }
            else {
                $null
            }
            $selectorEnum = if (Test-JsonObjectValue -Value $selector) {
                Get-JsonPropertyValue -Object $selector -Name "enum"
            }
            else {
                $null
            }
            if ($selectorType -isnot [string] -or
                -not [string]::Equals($selectorType, "string", [System.StringComparison]::Ordinal) -or
                -not (Test-JsonArrayValue -Value $selectorEnum) -or
                $selectorEnum.Count -ne 1 -or
                $selectorEnum[0] -isnot [string] -or
                -not [string]::Equals($selectorEnum[0], $mappingKey, [System.StringComparison]::Ordinal)) {
                throw "Discriminator child '$($target.ComponentName)' must directly declare '$propertyName' as the singleton string enum '$mappingKey'."
            }

            $edge = [PSCustomObject]@{
                ParentName = $parentName
                ParentPointer = $parentPointer
                ChildName = $target.ComponentName
                MappingKey = $mappingKey
                PropertyName = $propertyName
                OutputChildName = $target.ComponentName
            }
            $edges.Add($edge)
            if (-not $edgesByChild.ContainsKey($target.ComponentName)) {
                $edgesByChild[$target.ComponentName] = [System.Collections.Generic.List[object]]::new()
            }
            $edgesByChild[$target.ComponentName].Add($edge)
            $null = $branchReferenceBreadcrumbs.Add("$parentPointer/oneOf/$index/`$ref")
        }
        if ($edges.Count -ne $mappingKeyByTarget.Count) {
            throw "Discriminator mapping for '$parentName' contains targets that are not oneOf branches."
        }
        $plan = [PSCustomObject]@{
            ParentName = $parentName
            ParentPointer = $parentPointer
            PropertyName = $propertyName
            Mapping = $mapping
            Edges = [object[]]$edges.ToArray()
            BaseSchema = $null
        }
        $plans.Add($plan)
        $planByParent[$parentName] = $plan
    }

    if ($plans.Count -eq 0) {
        return [PSCustomObject]@{
            Document = Copy-JsonValue -Value $Document
            RefactoredParentCount = 0
            InheritedChildCount = 0
            ClonedChildCount = 0
            RemovedSharedChildCount = 0
        }
    }

    foreach ($childName in $edgesByChild.Keys) {
        if ($planByParent.ContainsKey($childName)) {
            throw "Component '$childName' is both a discriminator parent and a discriminator child; nested inheritance is not supported."
        }
    }

    $externalReferencesByChild = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    foreach ($occurrence in @(Get-OpenApiReferenceOccurrences -Value $Document)) {
        if (-not [string]::Equals($occurrence.Kind, "Reference", [System.StringComparison]::Ordinal) -or
            $branchReferenceBreadcrumbs.Contains($occurrence.Breadcrumb)) {
            continue
        }
        $referencedComponent = Get-OpenApiInheritanceComponentNameFromPointer -Pointer $occurrence.Pointer
        if ($referencedComponent -is [string] -and $edgesByChild.ContainsKey($referencedComponent)) {
            $null = $externalReferencesByChild.Add($referencedComponent)
        }
    }

    $cloneSchemas = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $cloneNamesBySource = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $removeOriginalChildren = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $clonedChildCount = 0
    foreach ($childName in $edgesByChild.Keys) {
        $childEdges = $edgesByChild[$childName]
        if ($childEdges.Count -eq 1) {
            continue
        }
        $cloneNames = [System.Collections.Generic.List[string]]::new()
        foreach ($edge in $childEdges) {
            $propertyToken = ConvertTo-OpenApiInheritanceNameToken -Value $edge.PropertyName
            if ([string]::IsNullOrWhiteSpace($propertyToken)) {
                $propertyToken = "Discriminator"
            }
            $valueToken = ConvertTo-OpenApiInheritanceNameToken -Value $edge.MappingKey
            if ([string]::IsNullOrWhiteSpace($valueToken)) {
                $valueToken = "Value"
            }
            $baseName = "$($edge.ParentName)_$($propertyToken.Substring(0, 1).ToLowerInvariant())$($propertyToken.Substring(1))Of$valueToken"
            $cloneName = Get-OpenApiUniqueInheritedComponentName `
                -BaseName $baseName `
                -ReservedNames $reservedNames
            $edge.OutputChildName = $cloneName
            $cloneSchemas[$cloneName] = New-OpenApiInheritedChildSchema `
                -Schema $schemaByName[$childName] `
                -ParentName $edge.ParentName
            $cloneNames.Add($cloneName)
            $clonedChildCount++
        }
        $cloneNamesBySource[$childName] = $cloneNames
        if (-not $externalReferencesByChild.Contains($childName)) {
            $null = $removeOriginalChildren.Add($childName)
        }
    }

    foreach ($plan in $plans) {
        $mapping = [ordered]@{}
        foreach ($mappingProperty in (Get-JsonProperties -Value $plan.Mapping)) {
            $edge = @($plan.Edges | Where-Object {
                [string]::Equals($_.MappingKey, $mappingProperty.Name, [System.StringComparison]::Ordinal)
            })[0]
            $mapping[$mappingProperty.Name] = Get-OpenApiInheritanceComponentPointer `
                -ComponentName $edge.OutputChildName
        }
        $plan.BaseSchema = [ordered]@{
            type = "object"
            required = @($plan.PropertyName)
            properties = [ordered]@{
                $plan.PropertyName = [ordered]@{
                    type = "string"
                }
            }
            discriminator = [ordered]@{
                propertyName = $plan.PropertyName
                mapping = $mapping
            }
        }
    }

    $newSchemas = [ordered]@{}
    foreach ($schemaProperty in $schemaProperties) {
        $name = $schemaProperty.Name
        if ($planByParent.ContainsKey($name)) {
            $newSchemas[$name] = $planByParent[$name].BaseSchema
            continue
        }
        if ($edgesByChild.ContainsKey($name)) {
            $childEdges = $edgesByChild[$name]
            if ($childEdges.Count -eq 1) {
                $newSchemas[$name] = New-OpenApiInheritedChildSchema `
                    -Schema $schemaProperty.Value `
                    -ParentName $childEdges[0].ParentName
                continue
            }
            if (-not $removeOriginalChildren.Contains($name)) {
                $newSchemas[$name] = Copy-JsonValue -Value $schemaProperty.Value
            }
            foreach ($cloneName in $cloneNamesBySource[$name]) {
                $newSchemas[$cloneName] = $cloneSchemas[$cloneName]
            }
            continue
        }
        $newSchemas[$name] = Copy-JsonValue -Value $schemaProperty.Value
    }

    $newComponents = [ordered]@{ schemas = $newSchemas }
    foreach ($property in (Get-JsonProperties -Value $components)) {
        if (-not [string]::Equals($property.Name, "schemas", [System.StringComparison]::Ordinal)) {
            $newComponents[$property.Name] = Copy-JsonValue -Value $property.Value
        }
    }
    $result = [ordered]@{}
    foreach ($property in (Get-JsonProperties -Value $Document)) {
        if ([string]::Equals($property.Name, "components", [System.StringComparison]::Ordinal)) {
            $result[$property.Name] = $newComponents
        }
        else {
            $result[$property.Name] = Copy-JsonValue -Value $property.Value
        }
    }

    Assert-OpenApiReferencesResolve -Document $result
    Assert-AdditionalOpenApiReferencesResolve -Document $result
    $null = Assert-OpenApiDiscriminatorMappings -Document $result
    return [PSCustomObject]@{
        Document = $result
        RefactoredParentCount = $plans.Count
        InheritedChildCount = @($plans | ForEach-Object Edges).Count
        ClonedChildCount = $clonedChildCount
        RemovedSharedChildCount = $removeOriginalChildren.Count
    }
}

function Get-DefaultDiscriminatorInheritedOpenApiPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$InputPath
    )

    $fullPath = [System.IO.Path]::GetFullPath($InputPath)
    $directory = [System.IO.Path]::GetDirectoryName($fullPath)
    $fileName = [System.IO.Path]::GetFileName($fullPath)
    if ($fileName.EndsWith(".discriminator-inherited.openapi.json", [System.StringComparison]::OrdinalIgnoreCase) -or
        $fileName.EndsWith(".discriminator-inherited.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        return $fullPath
    }
    if ($fileName.EndsWith(".openapi.json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $stem = $fileName.Substring(0, $fileName.Length - ".openapi.json".Length)
        return Join-Path $directory "$stem.discriminator-inherited.openapi.json"
    }
    if ($fileName.EndsWith(".json", [System.StringComparison]::OrdinalIgnoreCase)) {
        $stem = $fileName.Substring(0, $fileName.Length - ".json".Length)
        return Join-Path $directory "$stem.discriminator-inherited.json"
    }
    return Join-Path $directory "$fileName.discriminator-inherited.json"
}

function Resolve-InheritanceRepositoryPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }
    return [System.IO.Path]::GetFullPath((Join-Path $script:InheritanceRepositoryRoot $Path))
}

function Invoke-OpenApiDiscriminatedInheritanceRefactor {
    param(
        [Parameter()]
        [string]$InputPath,

        [Parameter()]
        [string]$OutputPath
    )

    $usingCanonicalInput = [string]::IsNullOrWhiteSpace($InputPath)
    if ($usingCanonicalInput) {
        $InputPath = $script:DefaultInheritanceInputPath
    }
    else {
        $InputPath = Resolve-InheritanceRepositoryPath -Path $InputPath
    }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The componentized OpenAPI input path is not a file: $resolvedInputPath"
    }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $OutputPath = if ($usingCanonicalInput) {
            $script:DefaultInheritanceOutputPath
        }
        else {
            Get-DefaultDiscriminatorInheritedOpenApiPath -InputPath $resolvedInputPath
        }
    }
    else {
        $OutputPath = Resolve-InheritanceRepositoryPath -Path $OutputPath
    }

    $document = Read-OpenApiDocument -Path $resolvedInputPath
    $result = ConvertTo-OpenApiDiscriminatedInheritanceDocument -Document $document
    $writtenPath = Write-OpenApiJsonFile -Document $result.Document -Path $OutputPath

    Write-Host ""
    Write-Host "Input: $resolvedInputPath"
    Write-Host "Output: $writtenPath"
    Write-Host "Discriminator parents refactored: $($result.RefactoredParentCount)"
    Write-Host "Inherited children: $($result.InheritedChildCount)"
    Write-Host "Parent-specific clones: $($result.ClonedChildCount)"
    Write-Host "Unreferenced shared children removed: $($result.RemovedSharedChildCount)"
    return $result
}

if ($MyInvocation.InvocationName -ne ".") {
    $null = Invoke-OpenApiDiscriminatedInheritanceRefactor `
        -InputPath $InputPath `
        -OutputPath $OutputPath
}