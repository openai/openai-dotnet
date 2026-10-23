<#
.SYNOPSIS
    Copies TypeSpec, introducing extensible unions for model discriminators.

.DESCRIPTION
    For each @discriminator("property") model, replaces its required plain string
    property with a BaseName_discriminator union. The union contains string and
    the known literal values in source declaration order. Descendant literals
    become Union.variant references, including through indirect inheritance.

    Only local model declarations are resolved; imports are not traversed.
    Relevant aliases, templates, spreads, optional properties, non-literal
    values, augment decorators, and unresolved inheritance are rejected.
    Discriminator strings must be single-line, non-interpolated literals.
    Generated-name collisions and already-transformed bases are errors.
    Unrelated text is preserved. Input is never overwritten; output is atomic.
    Both paths are required, with no defaults or inferred filenames.

.PARAMETER InputPath
    The UTF-8 TypeSpec file to transform, typically the enum converter's output.

.PARAMETER OutputPath
    The explicit destination TypeSpec file, distinct from the input.

.EXAMPLE
    .\scripts\typespec\Add-TypeSpecDiscriminatorUnions.ps1 `
        -InputPath .\specification\openapi\decisions\typespec\main.extensible-enums.tsp `
        -OutputPath .\specification\openapi\decisions\typespec\main.discriminator-unions.tsp
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
. (Join-Path $PSScriptRoot "_TypeSpecDocument.Helpers.ps1")

function Get-TypeSpecClosingTokenIndex {
    param([object[]]$Tokens, [int]$Start, [switch]$IncludeAngles)

    $pairs = @{ "(" = ")"; "{" = "}"; "[" = "]" }
    if ($IncludeAngles) { $pairs["<"] = ">" }
    $stack = [System.Collections.Generic.Stack[string]]::new()
    if (-not $pairs.ContainsKey($Tokens[$Start].Text)) {
        throw "Expected an opening delimiter at offset $($Tokens[$Start].Start)."
    }
    for ($index = $Start; $index -lt $Tokens.Length; $index++) {
        $text = $Tokens[$index].Text
        if ($pairs.ContainsKey($text)) { $stack.Push($pairs[$text]) }
        elseif ($text -in $pairs.Values) {
            if ($stack.Count -eq 0 -or $stack.Pop() -cne $text) {
                throw "Mismatched TypeSpec delimiter at offset $($Tokens[$index].Start)."
            }
            if ($stack.Count -eq 0) { return $index }
        }
    }
    throw "Unterminated TypeSpec delimiter at offset $($Tokens[$Start].Start)."
}

function Get-TypeSpecLocalName {
    param([object[]]$Tokens, [int]$Start)

    $parts = [System.Collections.Generic.List[string]]::new()
    $index = $Start
    while ($true) {
        if ($Tokens[$index].Kind -ne "Identifier") {
            throw "Expected a TypeSpec identifier at offset $($Tokens[$index].Start)."
        }
        $parts.Add((Get-TypeSpecEnumIdentifierValue -Identifier $Tokens[$index].Text))
        $index++
        if ($Tokens[$index].Text -ne ".") { break }
        $index++
    }
    return [PSCustomObject]@{ Parts = $parts.ToArray(); Next = $index }
}

function Get-TypeSpecDiscriminatorDecorator {
    param([object[]]$Tokens, [int]$Start)

    $index = $Start + 1
    $augment = $Tokens[$index].Text -eq "@"
    if ($augment) { $index++ }
    $name = Get-TypeSpecLocalName -Tokens $Tokens -Start $index
    $index = $name.Next
    $standard = ($name.Parts -join ".") -cin "discriminator", "TypeSpec.discriminator"
    if ($augment -and $standard) { throw "Augment discriminator decorators are not supported." }
    $propertyName = $null
    if ($Tokens[$index].Text -eq "(") {
        $close = Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $index
        if ($standard) {
            $argumentEnd = $close
            if ($Tokens[$close - 1].Text -eq ",") { $argumentEnd-- }
            if ($argumentEnd -ne $index + 2 -or $Tokens[$index + 1].Kind -ne "String") {
                throw "The discriminator decorator requires one literal string property name."
            }
            $propertyName = ConvertFrom-TypeSpecStringLiteral -Literal $Tokens[$index + 1].Text
            if ([string]::IsNullOrEmpty($propertyName)) { throw "The discriminator property name cannot be empty." }
        }
        $index = $close + 1
    }
    elseif ($standard) { throw "The discriminator decorator requires one literal string property name." }
    return [PSCustomObject]@{ Next = $index; IsDiscriminator = $standard; PropertyName = $propertyName }
}

function Get-TypeSpecModelProperties {
    param([object[]]$Tokens, [int]$Open, [int]$Close)

    $properties = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    $unsupported = [System.Collections.Generic.List[string]]::new()
    $index = $Open + 1
    while ($index -lt $Close) {
        if ($Tokens[$index].Text -in ";", ",") { $index++; continue }
        if ($Tokens[$index].Text -eq "@") {
            $decorator = Get-TypeSpecDiscriminatorDecorator -Tokens $Tokens -Start $index
            if ($decorator.IsDiscriminator) { throw "A discriminator decorator must target a model, not a property." }
            $index = $decorator.Next
            continue
        }
        if ($Tokens[$index].Text -eq "#") {
            $index += 2
            while ($Tokens[$index].Kind -eq "String") { $index++ }
            continue
        }
        if ($Tokens[$index].Text -eq ".") {
            $unsupported.Add("model spreads")
        }
        $start = $index
        while ($index -lt $Close -and $Tokens[$index].Text -notin ";", ",") {
            if ($Tokens[$index].Text -in "{", "(", "[", "<") {
                $index = (Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $index -IncludeAngles) + 1
            }
            else { $index++ }
        }
        $end = $index
        if ($Tokens[$start].Text -eq ".") { continue }
        if ($Tokens[$start].Kind -notin "Identifier", "String") {
            throw "Malformed TypeSpec model property at offset $($Tokens[$start].Start)."
        }
        $name = if ($Tokens[$start].Kind -eq "String") {
            ConvertFrom-TypeSpecStringLiteral -Literal $Tokens[$start].Text
        } else {
            Get-TypeSpecEnumIdentifierValue -Identifier $Tokens[$start].Text
        }
        $typeStart = $start + 1
        $optional = $Tokens[$typeStart].Text -eq "?"
        if ($optional) { $typeStart++ }
        if ($Tokens[$typeStart].Text -ne ":") {
            throw "Expected ':' after model property '$name'."
        }
        $typeStart++
        $typeEnd = $typeStart
        while ($typeEnd -lt $end -and $Tokens[$typeEnd].Text -ne "=") {
            if ($Tokens[$typeEnd].Text -in "{", "(", "[", "<") {
                $typeEnd = (Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $typeEnd -IncludeAngles) + 1
            }
            else { $typeEnd++ }
        }
        if ($typeStart -ge $typeEnd) { throw "Missing type for model property '$name'." }
        $property = [PSCustomObject]@{
            Name = $name
            Optional = $optional
            Tokens = @($Tokens[$typeStart..($typeEnd - 1)])
            Start = $Tokens[$typeStart].Start
            End = $Tokens[$typeEnd - 1].End
        }
        if (-not $properties.TryAdd($name, $property)) { throw "Duplicate model property '$name'." }
    }
    return [PSCustomObject]@{ Properties = $properties; Unsupported = $unsupported.ToArray() }
}

function Read-TypeSpecDiscriminatorScope {
    param(
        [object[]]$Tokens, [int]$Start, [int]$End,
        [AllowEmptyCollection()][string[]]$Namespace,
        [System.Collections.Generic.List[object]]$Models,
        [System.Collections.Generic.Dictionary[string, string]]$Symbols
    )

    $scope = $Namespace
    $index = $Start
    $discriminator = $null
    while ($index -lt $End) {
        $token = $Tokens[$index]
        if ($token.Text -eq "@") {
            $decorator = Get-TypeSpecDiscriminatorDecorator -Tokens $Tokens -Start $index
            if ($decorator.IsDiscriminator) {
                if ($null -ne $discriminator) { throw "Multiple discriminator decorators on one declaration are not supported." }
                $discriminator = $decorator.PropertyName
            }
            $index = $decorator.Next
            continue
        }
        if ($token.Text -eq "namespace") {
            if ($null -ne $discriminator) { throw "A discriminator decorator must target a model." }
            $name = Get-TypeSpecLocalName -Tokens $Tokens -Start ($index + 1)
            $nestedScope = @($scope) + @($name.Parts)
            for ($part = 0; $part -lt $nestedScope.Count; $part++) {
                $key = $nestedScope[0..$part] -join [char]0x1F
                if ($Symbols.ContainsKey($key) -and $Symbols[$key] -ne "namespace") {
                    throw "Conflicting TypeSpec declaration '$($nestedScope[0..$part] -join '.')'."
                }
                $Symbols[$key] = "namespace"
            }
            $index = $name.Next
            if ($Tokens[$index].Text -eq "{") {
                $close = Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $index
                Read-TypeSpecDiscriminatorScope -Tokens $Tokens -Start ($index + 1) -End $close `
                    -Namespace $nestedScope -Models $Models -Symbols $Symbols
                $index = $close + 1
            }
            elseif ($Tokens[$index].Text -eq ";") {
                $scope = $nestedScope
                $index++
            }
            else { throw "Expected a namespace body or ';' at offset $($Tokens[$index].Start)." }
            continue
        }
        if ($token.Text -cin "model", "union", "enum", "scalar", "alias", "interface", "op", "const") {
            $kind = $token.Text
            if ($null -ne $discriminator -and $kind -ne "model") {
                throw "A discriminator decorator must target a model."
            }
            if ($Tokens[$index + 1].Kind -ne "Identifier") {
                throw "Expected a name after '$kind' at offset $($token.Start)."
            }
            $name = Get-TypeSpecEnumIdentifierValue -Identifier $Tokens[$index + 1].Text
            $key = (@($scope) + $name) -join [char]0x1F
            if (-not $Symbols.TryAdd($key, $kind)) { throw "Ambiguous TypeSpec declaration '$name'." }
            $headerStart = $index + 2
            $index = $headerStart
            while ($index -lt $End -and $Tokens[$index].Text -notin "{", ";") {
                if ($Tokens[$index].Text -in "<", "(", "[") {
                    $index = (Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $index -IncludeAngles) + 1
                }
                else { $index++ }
            }
            if ($index -ge $End) { throw "Unterminated TypeSpec $kind '$name'." }
            $open = $index
            $close = if ($Tokens[$open].Text -eq "{") {
                Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $open
            } else { $open }
            if ($kind -eq "model") {
                $unsupported = [System.Collections.Generic.List[string]]::new()
                $parentParts = @()
                if ($headerStart -lt $open) {
                    $header = @($Tokens[$headerStart..($open - 1)])
                    if ($header.Text -contains "<") { $unsupported.Add("templates") }
                    if ($header.Text -contains "is") { $unsupported.Add("model aliases") }
                    $extends = -1
                    for ($headerIndex = $headerStart; $headerIndex -lt $open; $headerIndex++) {
                        if ($Tokens[$headerIndex].Text -eq "<") {
                            $headerIndex = Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $headerIndex -IncludeAngles
                            continue
                        }
                        if ($Tokens[$headerIndex].Text -ceq "extends") { $extends = $headerIndex }
                    }
                    if ($extends -ge 0) {
                        $parent = Get-TypeSpecLocalName -Tokens $Tokens -Start ($extends + 1)
                        $parentParts = $parent.Parts
                        if ($parent.Next -ne $open) { $unsupported.Add("non-simple inheritance") }
                    }
                    elseif ($unsupported.Count -eq 0) { $unsupported.Add("non-simple model declarations") }
                }
                $body = if ($close -ne $open) {
                    Get-TypeSpecModelProperties -Tokens $Tokens -Open $open -Close $close
                } else {
                    $unsupported.Add("models without explicit bodies")
                    [PSCustomObject]@{
                        Properties = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
                        Unsupported = @()
                    }
                }
                foreach ($reason in $body.Unsupported) { $unsupported.Add($reason) }
                $Models.Add([PSCustomObject]@{
                    Name = $name
                    Namespace = @($scope)
                    Key = $key
                    DisplayName = (@($scope) + $name) -join "."
                    Discriminator = $discriminator
                    Properties = $body.Properties
                    ParentParts = @($parentParts)
                    Parent = $null
                    Unsupported = $unsupported.ToArray()
                    Start = $token.Start
                    End = $Tokens[$close].End
                    UnionName = $name + "_discriminator"
                })
            }
            $discriminator = $null
            $index = $close + 1
            continue
        }
        if ($token.Text -in "{", "(", "[") {
            $index = (Get-TypeSpecClosingTokenIndex -Tokens $Tokens -Start $index) + 1
        }
        else { $index++ }
    }
    if ($null -ne $discriminator) { throw "A discriminator decorator has no model declaration." }
}

function Test-TypeSpecModelDescendant {
    param([object]$Model, [object]$Base)

    $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
    $current = $Model
    $matches = $false
    while ($null -ne $current) {
        if (-not $seen.Add($current.Key)) { throw "Cyclic model inheritance involving '$($current.DisplayName)'." }
        if ($current.Key -ceq $Base.Key) { $matches = $true }
        $current = $current.Parent
    }
    return $matches
}

function Get-TypeSpecInheritedDiscriminatorValue {
    param([object]$Model, [string]$PropertyName)

    $current = $Model
    while ($null -ne $current) {
        if ($current.Properties.ContainsKey($PropertyName)) {
            $property = $current.Properties[$PropertyName]
            if ($property.Tokens.Count -eq 1 -and $property.Tokens[0].Kind -eq "String") {
                return ConvertFrom-TypeSpecStringLiteral -Literal $property.Tokens[0].Text
            }
            return $null
        }
        $current = $current.Parent
    }
    return $null
}

function Resolve-TypeSpecLocalSymbol {
    param(
        [string[]]$Parts, [AllowEmptyCollection()][string[]]$Namespace,
        [System.Collections.Generic.Dictionary[string, string]]$Symbols
    )

    for ($length = $Namespace.Count; $length -ge 0; $length--) {
        $prefix = @()
        if ($length -gt 0) { $prefix = @($Namespace[0..($length - 1)]) }
        $first = ($prefix + $Parts[0]) -join [char]0x1F
        if ($Symbols.ContainsKey($first)) {
            $key = ($prefix + @($Parts)) -join [char]0x1F
            if ($Symbols.ContainsKey($key)) { return $key }
            return $null
        }
    }
    return $null
}

function Get-TypeSpecDiscriminatorUnionReference {
    param([object]$Base, [object]$Model, [System.Collections.Generic.Dictionary[string, string]]$Symbols)

    $expected = (@($Base.Namespace) + $Base.UnionName) -join [char]0x1F
    if (($Base.Namespace -join [char]0x1F) -ceq ($Model.Namespace -join [char]0x1F)) {
        return ConvertTo-TypeSpecIdentifier -Value $Base.UnionName
    }
    for ($start = 0; $start -le $Base.Namespace.Count; $start++) {
        $parts = @()
        if ($start -lt $Base.Namespace.Count) { $parts = @($Base.Namespace[$start..($Base.Namespace.Count - 1)]) }
        $parts += $Base.UnionName
        $resolved = Resolve-TypeSpecLocalSymbol -Parts $parts -Namespace $Model.Namespace -Symbols $Symbols
        if ($resolved -ceq $expected) {
            return ($parts | ForEach-Object { ConvertTo-TypeSpecIdentifier -Value $_ }) -join "."
        }
    }
    throw "Cannot reference generated union '$($Base.UnionName)' unambiguously from '$($Model.DisplayName)'."
}

function ConvertTo-TypeSpecDiscriminatorUnionText {
    param([AllowEmptyString()][string]$Text)

    $tokens = Get-TypeSpecConversionTokens -Text $Text
    $models = [System.Collections.Generic.List[object]]::new()
    $symbols = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::Ordinal)
    Read-TypeSpecDiscriminatorScope -Tokens $tokens -Start 0 -End ($tokens.Length - 1) `
        -Namespace @() -Models $models -Symbols $symbols
    $bases = @($models | Where-Object { $null -ne $_.Discriminator })
    if ($bases.Count -eq 0) {
        return [PSCustomObject]@{ Text = $Text; AddedUnionCount = 0; BasePropertyCount = 0; DerivedPropertyCount = 0; VariantCount = 0 }
    }
    $byName = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
    foreach ($model in $models) { $byName.Add($model.Key, $model) }
    foreach ($model in $models) {
        if ($model.ParentParts.Count -eq 0) { continue }
        $key = Resolve-TypeSpecLocalSymbol -Parts $model.ParentParts -Namespace $model.Namespace -Symbols $symbols
        if ($null -ne $key -and $byName.ContainsKey($key)) {
            $model.Parent = $byName[$key]
        }
    }
    foreach ($base in $bases) {
        if (-not $base.Properties.ContainsKey($base.Discriminator)) {
            throw "Discriminator '$($base.Discriminator)' must be explicitly declared on '$($base.DisplayName)'."
        }
        $property = $base.Properties[$base.Discriminator]
        if ($property.Optional -or $property.Tokens.Count -ne 1 -or $property.Tokens[0].Text -cne "string") {
            throw "Discriminator '$($base.Discriminator)' on '$($base.DisplayName)' must be a required plain string property."
        }
        $unionKey = (@($base.Namespace) + $base.UnionName) -join [char]0x1F
        if ($symbols.ContainsKey($unionKey)) { throw "Generated union name '$($base.UnionName)' conflicts with an existing declaration." }
        $symbols.Add($unionKey, "union")
    }
    $edits = [System.Collections.Generic.List[object]]::new()
    $propertyEdits = [System.Collections.Generic.Dictionary[int, object]]::new()
    $variantCount = 0
    $derivedCount = 0
    $newline = if ($Text.Contains("`r`n")) { "`r`n" } elseif ($Text.Contains("`n")) { "`n" } else { "`r`n" }
    foreach ($base in $bases) {
        $ancestors = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        $ancestor = $base
        while ($null -ne $ancestor) {
            if (-not $ancestors.Add($ancestor.Key)) { throw "Cyclic model inheritance involving '$($base.DisplayName)'." }
            if ($ancestor.ParentParts.Count -gt 0 -and $null -eq $ancestor.Parent) {
                throw "Cannot resolve relevant base '$($ancestor.ParentParts -join '.')' locally for '$($base.DisplayName)'."
            }
            if ($ancestor.Key -cne $base.Key -and $ancestor.Unsupported.Count -gt 0) {
                throw "Unsupported $($ancestor.Unsupported -join ', ') in ancestor '$($ancestor.DisplayName)'."
            }
            $ancestor = $ancestor.Parent
        }
        $baseProperty = $base.Properties[$base.Discriminator]
        if ($null -ne (Get-TypeSpecInheritedDiscriminatorValue -Model $base.Parent -PropertyName $base.Discriminator)) {
            throw "Contradictory discriminator override on '$($base.DisplayName)'."
        }
        $values = [System.Collections.Generic.List[object]]::new()
        $owners = [System.Collections.Generic.Dictionary[string, object]]::new([System.StringComparer]::Ordinal)
        foreach ($model in $models) {
            # Inspect only this hierarchy; unrelated external references remain untouched.
            $current = $model
            $seen = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
            $relevant = $false
            while ($null -ne $current -and $seen.Add($current.Key)) {
                if ($current.Key -ceq $base.Key) { $relevant = $true; break }
                $current = $current.Parent
            }
            if (-not $relevant) { continue }
            $null = Test-TypeSpecModelDescendant -Model $model -Base $base
            if ($model.Unsupported.Count -gt 0) {
                throw "Unsupported $($model.Unsupported -join ', ') in discriminator model '$($model.DisplayName)'."
            }
            if ($model.Key -ceq $base.Key) { continue }
            if (-not $model.Properties.ContainsKey($base.Discriminator)) {
                $inherited = Get-TypeSpecInheritedDiscriminatorValue -Model $model.Parent -PropertyName $base.Discriminator
                $hasChildren = @($models | Where-Object { $null -ne $_.Parent -and $_.Parent.Key -ceq $model.Key }).Count -gt 0
                if ($null -eq $inherited -and -not $hasChildren) {
                    throw "Missing string-literal discriminator '$($base.Discriminator)' on leaf '$($model.DisplayName)'."
                }
                continue
            }
            $property = $model.Properties[$base.Discriminator]
            if ($model.Discriminator -ceq $base.Discriminator -and $property.Tokens.Count -eq 1 -and
                $property.Tokens[0].Text -ceq "string" -and -not $property.Optional) {
                continue
            }
            if ($property.Optional -or $property.Tokens.Count -ne 1 -or $property.Tokens[0].Kind -ne "String") {
                throw "Discriminator '$($base.Discriminator)' on '$($model.DisplayName)' must be a required string literal."
            }
            $value = ConvertFrom-TypeSpecStringLiteral -Literal $property.Tokens[0].Text
            $inherited = Get-TypeSpecInheritedDiscriminatorValue -Model $model.Parent -PropertyName $base.Discriminator
            if ($null -ne $inherited -and $inherited -cne $value) {
                throw "Contradictory discriminator override on '$($model.DisplayName)'."
            }
            if ($owners.ContainsKey($value)) {
                $owner = $owners[$value]
                if (-not (Test-TypeSpecModelDescendant -Model $model -Base $owner) -and
                    -not (Test-TypeSpecModelDescendant -Model $owner -Base $model)) {
                    throw "Ambiguous duplicate discriminator value '$value' in '$($base.DisplayName)'."
                }
            }
            else {
                $owners.Add($value, $model)
                $values.Add([PSCustomObject]@{ Value = $value; Name = ConvertTo-TypeSpecIdentifier -Value $value })
            }
            $nearest = $model.Parent
            while ($null -ne $nearest -and $nearest.Discriminator -cne $base.Discriminator) { $nearest = $nearest.Parent }
            if ($null -ne $nearest -and $nearest.Key -ceq $base.Key) {
                $reference = Get-TypeSpecDiscriminatorUnionReference -Base $base -Model $model -Symbols $symbols
                $propertyEdits.Add($property.Start, [PSCustomObject]@{
                    Start = $property.Start
                    End = $property.End
                    Text = "$reference.$(ConvertTo-TypeSpecIdentifier -Value $value)"
                })
                $derivedCount++
            }
        }
        $baseReference = Get-TypeSpecDiscriminatorUnionReference -Base $base -Model $base -Symbols $symbols
        $propertyEdits.Add($baseProperty.Start, [PSCustomObject]@{ Start = $baseProperty.Start; End = $baseProperty.End; Text = $baseReference })
        $prefix = $Text.Substring(0, $base.Start)
        $indent = [System.Text.RegularExpressions.Regex]::Match($prefix, '(?:\A|[\r\n])([ \t]*)[^\r\n]*\z').Groups[1].Value
        $lines = [System.Collections.Generic.List[string]]::new()
        $lines.Add("${indent}union $(ConvertTo-TypeSpecIdentifier -Value $base.UnionName) {")
        $lines.Add("${indent}  string,")
        foreach ($value in $values) { $lines.Add("${indent}  $($value.Name): $(ConvertTo-TypeSpecStringLiteral -Value $value.Value),") }
        $lines.Add("${indent}}")
        $edits.Add([PSCustomObject]@{ Start = $base.End; End = $base.End; Text = "$newline$newline" + ($lines -join $newline) })
        $variantCount += $values.Count
    }
    foreach ($edit in $propertyEdits.Values) { $edits.Add($edit) }
    return [PSCustomObject]@{
        Text = Invoke-TypeSpecTextEdits -Text $Text -Edits $edits.ToArray()
        AddedUnionCount = $bases.Count
        BasePropertyCount = $bases.Count
        DerivedPropertyCount = $derivedCount
        VariantCount = $variantCount
    }
}

function Invoke-TypeSpecDiscriminatorUnionGeneration {
    param([string]$InputPath, [string]$OutputPath)

    $paths = Get-TypeSpecTransformationPaths -InputPath $InputPath -OutputPath $OutputPath
    $source = Read-TypeSpecUtf8File -Path $paths.InputPath
    $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source.Text
    [byte[]]$bytes = $source.Bytes
    if ($result.AddedUnionCount -gt 0) { $bytes = [System.Text.UTF8Encoding]::new($false, $true).GetBytes($result.Text) }
    Write-TypeSpecFileAtomically -Path $paths.OutputPath -Bytes $bytes
    return [PSCustomObject]@{
        OutputPath = $paths.OutputPath
        AddedUnionCount = $result.AddedUnionCount
        BasePropertyCount = $result.BasePropertyCount
        DerivedPropertyCount = $result.DerivedPropertyCount
        VariantCount = $result.VariantCount
    }
}

if ($MyInvocation.InvocationName -ne ".") {
    $result = Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $InputPath -OutputPath $OutputPath
    Write-Host "Added $($result.AddedUnionCount) discriminator union(s); updated $($result.BasePropertyCount) base and $($result.DerivedPropertyCount) derived properties. Output: $($result.OutputPath)"
}
