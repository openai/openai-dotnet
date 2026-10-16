<#
.SYNOPSIS
    Copies a TypeSpec file, converting string enums into extensible string unions.

.DESCRIPTION
    Requires explicit input and output paths. Preserves text outside enum
    declarations and inserts a plain string variant before the named variants.
    Identifier members use their names as wire values; explicitly assigned
    string values are retained. Numeric values and enum spreads are rejected.
    Only the specified file is transformed; imports are not traversed.
    The input is never overwritten, and output is published atomically.

.PARAMETER InputPath
    The UTF-8 TypeSpec source file. Required; there is no default.

.PARAMETER OutputPath
    The destination TypeSpec file. Required; there is no default or inferred name.

.EXAMPLE
    .\scripts\typespec\Convert-TypeSpecEnumsToUnions.ps1 `
        -InputPath .\specification\openapi\decisions\typespec\main.tsp `
        -OutputPath .\specification\openapi\decisions\typespec\main.extensible-enums.tsp
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

function ConvertTo-TypeSpecExtensibleEnumText {
    param([AllowEmptyString()][string]$Text)

    $tokens = Get-TypeSpecConversionTokens -Text $Text
    $edits = [System.Collections.Generic.List[object]]::new()
    $enumCount = 0
    for ($index = 0; $index -lt $tokens.Length - 1; $index++) {
        if ($tokens[$index].Text -cne "enum") { continue }
        # Qualified references and decorator names are not declarations.
        if ($index -gt 0 -and $tokens[$index - 1].Text -in ".", "@") { continue }
        if ($tokens[$index + 1].Kind -ne "Identifier") {
            throw "Expected an enum name at offset $($tokens[$index].Start)."
        }
        $enumName = $tokens[$index + 1].Text
        $openIndex = $index + 2
        if ($tokens[$openIndex].Text -ne "{") {
            throw "Expected '{' after enum '$enumName'."
        }
        $edits.Add([PSCustomObject]@{
            Start = $tokens[$index].Start
            End = $tokens[$index].End
            Text = "union"
        })
        $open = $tokens[$openIndex]
        $newline = if ($Text.Contains("`r`n")) { "`r`n" } elseif ($Text.Contains("`n")) { "`n" } else { "`r" }
        $afterOpen = $Text.Substring($open.End)
        $indentMatch = [System.Text.RegularExpressions.Regex]::Match($afterOpen, '\A[ \t]*\r?\n([ \t]*)')
        if ($indentMatch.Success) {
            $indent = $indentMatch.Groups[1].Value
            if ($tokens[$openIndex + 1].Text -eq "}") { $indent += "  " }
            $stringVariant = "$newline${indent}string,"
        }
        else {
            $stringVariant = " string,"
        }
        $edits.Add([PSCustomObject]@{ Start = $open.End; End = $open.End; Text = $stringVariant })

        $memberIndex = $openIndex + 1
        $names = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        while ($tokens[$memberIndex].Text -ne "}") {
            while ($tokens[$memberIndex].Text -eq "@") {
                $memberIndex++
                if ($tokens[$memberIndex].Kind -ne "Identifier") {
                    throw "Invalid member decorator in enum '$enumName'."
                }
                $memberIndex++
                while ($tokens[$memberIndex].Text -eq ".") {
                    $memberIndex++
                    if ($tokens[$memberIndex].Kind -ne "Identifier") {
                        throw "Invalid member decorator in enum '$enumName'."
                    }
                    $memberIndex++
                }
                if ($tokens[$memberIndex].Text -eq "(") {
                    $depth = 1
                    $memberIndex++
                    while ($depth -gt 0) {
                        if ($tokens[$memberIndex].Kind -eq "End") {
                            throw "Unterminated member decorator in enum '$enumName'."
                        }
                        if ($tokens[$memberIndex].Text -eq "(") { $depth++ }
                        if ($tokens[$memberIndex].Text -eq ")") { $depth-- }
                        $memberIndex++
                    }
                }
            }
            $member = $tokens[$memberIndex]
            if ($member.Text -eq ".") { throw "Enum spreads are not supported in enum '$enumName'." }
            if ($member.Kind -ne "Identifier") {
                throw "Expected a member or closing '}' in enum '$enumName' at offset $($member.Start)."
            }
            $memberName = Get-TypeSpecEnumIdentifierValue -Identifier $member.Text
            if (-not $names.Add($memberName)) {
                throw "Duplicate member '$memberName' in enum '$enumName'."
            }
            if ($member.Text -cin "true", "false", "null") {
                $edits.Add([PSCustomObject]@{
                    Start = $member.Start
                    End = $member.End
                    Text = '`' + $member.Text + '`'
                })
            }
            $memberIndex++
            if ($tokens[$memberIndex].Text -eq ":") {
                $memberIndex++
                $value = $tokens[$memberIndex]
                if ($value.Kind -ne "String") {
                    throw "Only string-valued members are supported in enum '$enumName' (member '$memberName'); numeric values are not supported."
                }
                if ([System.Text.RegularExpressions.Regex]::IsMatch($value.Text, '(?<!\\)(?:\\\\)*\$\{')) {
                    throw "Interpolated string values are not supported in enum '$enumName' (member '$memberName')."
                }
                $memberIndex++
            }
            else {
                $edits.Add([PSCustomObject]@{
                    Start = $member.End
                    End = $member.End
                    Text = ": $(ConvertTo-TypeSpecStringLiteral -Value $memberName)"
                })
            }
            if ($tokens[$memberIndex].Text -in ",", ";") {
                $memberIndex++
            }
            elseif ($tokens[$memberIndex].Text -ne "}") {
                throw "Expected a member separator or closing '}' in enum '$enumName' at offset $($tokens[$memberIndex].Start)."
            }
        }
        $index = $memberIndex
        $enumCount++
    }

    return [PSCustomObject]@{
        Text = Invoke-TypeSpecTextEdits -Text $Text -Edits $edits.ToArray()
        ConvertedEnumCount = $enumCount
    }
}

function Invoke-TypeSpecEnumConversion {
    param([string]$InputPath, [string]$OutputPath)

    $paths = Get-TypeSpecTransformationPaths -InputPath $InputPath -OutputPath $OutputPath
    $source = Read-TypeSpecUtf8File -Path $paths.InputPath
    $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source.Text
    [byte[]]$outputBytes = $source.Bytes
    if ($result.ConvertedEnumCount -gt 0) {
        $outputBytes = [System.Text.UTF8Encoding]::new($false, $true).GetBytes($result.Text)
    }
    Write-TypeSpecFileAtomically -Path $paths.OutputPath -Bytes $outputBytes
    return [PSCustomObject]@{ OutputPath = $paths.OutputPath; ConvertedEnumCount = $result.ConvertedEnumCount }
}

if ($MyInvocation.InvocationName -ne ".") {
    $result = Invoke-TypeSpecEnumConversion -InputPath $InputPath -OutputPath $OutputPath
    Write-Host "Converted $($result.ConvertedEnumCount) enum(s). Output: $($result.OutputPath)"
}
