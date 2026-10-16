<#
.SYNOPSIS
    Runs focused, dependency-free tests for the TypeSpec enum converter.
#>

#Requires -Version 7.0

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"

$scriptPath = Join-Path $PSScriptRoot ".." "Convert-TypeSpecEnumsToUnions.ps1"
. $scriptPath
$script:PassedTests = 0
$script:FailedTests = 0
$script:Failures = [System.Collections.Generic.List[string]]::new()

function Invoke-Test {
    param([string]$Name, [scriptblock]$Test)
    try {
        & $Test
        $script:PassedTests++
        Write-Host "[PASS] $Name"
    }
    catch {
        $script:FailedTests++
        $message = "[FAIL] $Name`: $($_.Exception.Message)"
        $script:Failures.Add($message)
        Write-Host $message -ForegroundColor Red
    }
}

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { throw $Message }
}

function Assert-Equal {
    param([AllowEmptyString()][string]$Expected, [AllowEmptyString()][string]$Actual, [string]$Message)
    if (-not [string]::Equals($Expected, $Actual, [System.StringComparison]::Ordinal)) {
        throw "$Message Expected '$Expected', got '$Actual'."
    }
}

function Assert-Throws {
    param([scriptblock]$Action, [string]$MessagePattern)
    try { & $Action }
    catch {
        if ($_.Exception.Message -notlike $MessagePattern) {
            throw "Expected error matching '$MessagePattern', got '$($_.Exception.Message)'."
        }
        return
    }
    throw "Expected an error matching '$MessagePattern', but no error was thrown."
}

function Assert-BytesEqual {
    param([AllowEmptyCollection()][byte[]]$Expected, [AllowEmptyCollection()][byte[]]$Actual, [string]$Message)
    if (-not [System.Linq.Enumerable]::SequenceEqual($Expected, $Actual)) { throw $Message }
}

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "typespec-enum-converter-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)
$utf8 = [System.Text.UTF8Encoding]::new($false)

try {
    Invoke-Test "dot-sourcing exposes helpers without generating files" {
        Assert-True ($null -ne (Get-Command ConvertTo-TypeSpecExtensibleEnumText -ErrorAction SilentlyContinue)) "Conversion function was not defined."
        Assert-True ($null -ne (Get-Command Invoke-TypeSpecEnumConversion -ErrorAction SilentlyContinue)) "File conversion function was not defined."
        Assert-Equal "0" ([System.IO.Directory]::GetFiles($temporaryDirectory).Length) "Dot-sourcing wrote files."
    }

    Invoke-Test "requested enum becomes an open union in original order" {
        $source = "enum Detail {`n  auto,`n  high,`n  low,`n  original,`n}`n"
        $expected = "union Detail {`n  string,`n  auto: `"auto`",`n  high: `"high`",`n  low: `"low`",`n  original: `"original`",`n}`n"
        $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source
        Assert-Equal $expected $result.Text "Requested conversion differed."
        Assert-Equal "1" $result.ConvertedEnumCount "Enum count was incorrect."
    }

    Invoke-Test "multiple inline enums and existing unions preserve surrounding text" {
        $source = "namespace Test;`nenum A { one, two }`nunion Existing { string, int32 }`nenum B { yes, }`nmodel M { a: A; b: B; }"
        $expected = "namespace Test;`nunion A { string, one: `"one`", two: `"two`" }`nunion Existing { string, int32 }`nunion B { string, yes: `"yes`", }`nmodel M { a: A; b: B; }"
        $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source
        Assert-Equal $expected $result.Text "Surrounding text or existing union changed."
        Assert-Equal "2" $result.ConvertedEnumCount "Multiple enum count was incorrect."
    }

    Invoke-Test "explicit strings and their escaped wire values remain verbatim" {
        $source = 'enum Values { first: "wire-value", second: "escaped \"quote\" and \\slash", third: "\${literal}", }'
        $expected = $source.Replace("enum Values {", "union Values { string,")
        Assert-Equal $expected (ConvertTo-TypeSpecExtensibleEnumText -Text $source).Text "Explicit wire values changed."
    }

    Invoke-Test "comments, strings and decorators are retained without false enum matches" {
        $source = @'
// enum Ignored { fake, }
/* enum AlsoIgnored { fake, } */
@doc("Text with enum Fake { brace }, comma, and \"quotes\"")
enum /* name comment */ Values {
  // member comment
  @doc("Member } , enum Foo")
  first /* trailing comment */,
  second: "value with } , // /* enum NotADeclaration",
}
model M { value: Values; note: "enum Literal { x, }"; }
'@
        $newline = if ($source.Contains("`r`n")) { "`r`n" } else { "`n" }
        $expected = $source.Replace("enum /* name comment */", "union /* name comment */").
            Replace("Values {$newline", "Values {$newline  string,$newline").
            Replace("  first /*", '  first: "first" /*')
        Assert-Equal $expected (ConvertTo-TypeSpecExtensibleEnumText -Text $source).Text "Comments or decorators changed."
    }

    Invoke-Test "backticked identifiers use their decoded names as string values" {
        $source = 'enum `enum-name` { `wire-value`, `quote\"name`, `dollar\${name}`, string, }'
        $expected = 'union `enum-name` { string, `wire-value`: "wire-value", `quote\"name`: "quote\"name", `dollar\${name}`: "dollar\${name}", string: "string", }'
        Assert-Equal $expected (ConvertTo-TypeSpecExtensibleEnumText -Text $source).Text "Escaped identifiers were converted incorrectly."
    }

    Invoke-Test "qualified references and decorator names are not declarations" {
        $source = '@Test.enum("value") model M { value: Test.enum; }'
        $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source
        Assert-Equal $source $result.Text "Qualified enum-like reference changed."
        Assert-Equal "0" $result.ConvertedEnumCount "Qualified reference was counted."
    }

    Invoke-Test "literal keyword member names become valid union labels" {
        $source = 'enum Values { true, false: "no", null, }'
        $expected = 'union Values { string, `true`: "true", `false`: "no", `null`: "null", }'
        Assert-Equal $expected (ConvertTo-TypeSpecExtensibleEnumText -Text $source).Text "Literal keywords were not escaped."
    }

    Invoke-Test "enum-named properties, parameters and existing union labels remain unchanged" {
        $source = 'model M { `enum`?: string; } op test(`enum`: string): string; union U { `enum`: "value", string, }'
        $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source
        Assert-Equal $source $result.Text "Enum-named non-declarations changed."
        Assert-Equal "0" $result.ConvertedEnumCount "Enum-named label was counted."
    }

    Invoke-Test "empty enum becomes a union containing string" {
        Assert-Equal "union Empty { string, }" (ConvertTo-TypeSpecExtensibleEnumText -Text "enum Empty { }").Text "Empty enum failed."
    }

    Invoke-Test "CRLF and tab indentation remain unchanged" {
        $source = "enum Values {`r`n`talpha,`r`n`tbeta,`r`n}`r`n"
        $expected = "union Values {`r`n`tstring,`r`n`talpha: `"alpha`",`r`n`tbeta: `"beta`",`r`n}`r`n"
        Assert-Equal $expected (ConvertTo-TypeSpecExtensibleEnumText -Text $source).Text "Line endings or indentation changed."
    }

    Invoke-Test "numeric members, spreads, duplicates and interpolation fail clearly" {
        foreach ($case in @(
            @{ Source = "enum Values { one: 1 }"; Error = "*numeric values are not supported*" },
            @{ Source = "enum Values { one: -1 }"; Error = "*numeric values are not supported*" },
            @{ Source = "enum Values { ...Other }"; Error = "*spreads are not supported*" },
            @{ Source = "enum Values { one, one }"; Error = "*Duplicate member*" },
            @{ Source = 'enum Values { one, `one` }'; Error = "*Duplicate member*" },
            @{ Source = 'enum Values { one: "${expression}" }'; Error = "*Interpolated string values*" }
        )) {
            Assert-Throws { ConvertTo-TypeSpecExtensibleEnumText -Text $case.Source } $case.Error
        }
    }

    Invoke-Test "malformed enums, comments and strings fail clearly" {
        foreach ($case in @(
            @{ Source = "enum"; Error = "*Expected an enum name*" },
            @{ Source = "enum Values"; Error = "*Expected '{'*" },
            @{ Source = "enum Values { one,"; Error = "*Expected a member or closing*"},
            @{ Source = "enum Values { one two }"; Error = "*Expected a member separator*"},
            @{ Source = 'enum Values { one: "unterminated'; Error = "*Unterminated TypeSpec String*"},
            @{ Source = 'enum Values { one: "\q" }'; Error = "*Invalid TypeSpec escape*"},
            @{ Source = "/* unterminated"; Error = "*Unterminated TypeSpec comment*"}
        )) {
            Assert-Throws { ConvertTo-TypeSpecExtensibleEnumText -Text $case.Source } $case.Error
        }
    }

    Invoke-Test "file conversion honors explicit paths and does not modify input" {
        $sourcePath = Join-Path $temporaryDirectory "custom input.tsp"
        $destination = Join-Path $temporaryDirectory "nested" "custom output.tsp"
        [System.IO.File]::WriteAllText($sourcePath, "enum Values { one, }", $utf8)
        $before = [System.IO.File]::ReadAllBytes($sourcePath)
        $result = Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination
        Assert-Equal $destination $result.OutputPath "Explicit destination was ignored."
        Assert-Equal "union Values { string, one: `"one`", }" ([System.IO.File]::ReadAllText($destination)) "File output was incorrect."
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($sourcePath)) "Input file changed."
        Assert-True ([System.IO.Directory]::GetFiles((Split-Path $destination), "*.tmp").Length -eq 0) "Temporary output was not cleaned up."
        $firstOutput = [System.IO.File]::ReadAllBytes($destination)
        $null = Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination
        Assert-BytesEqual $firstOutput ([System.IO.File]::ReadAllBytes($destination)) "Repeated conversion changed output."
    }

    Invoke-Test "no-enum files remain byte-equivalent including BOM and empty files" {
        $sourcePath = Join-Path $temporaryDirectory "unchanged.tsp"
        $destination = Join-Path $temporaryDirectory "unchanged-output.tsp"
        foreach ($content in @(
            [byte[]]@(),
            $utf8.GetBytes("model M { value: string; }`r`n"),
            [byte[]](0xEF, 0xBB, 0xBF) + $utf8.GetBytes("// no enum`n")
        )) {
            [System.IO.File]::WriteAllBytes($sourcePath, $content)
            $result = Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination
            Assert-Equal "0" $result.ConvertedEnumCount "Non-enum file was counted."
            Assert-BytesEqual $content ([System.IO.File]::ReadAllBytes($destination)) "No-enum output changed bytes."
        }
    }

    Invoke-Test "converted UTF-8 input emits no BOM and preserves non-ASCII strings" {
        $sourcePath = Join-Path $temporaryDirectory "utf8.tsp"
        $destination = Join-Path $temporaryDirectory "utf8-output.tsp"
        $value = [string][char]0x00E9
        [System.IO.File]::WriteAllText($sourcePath, "enum Values { one: `"$value`" }", [System.Text.UTF8Encoding]::new($true))
        $null = Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination
        Assert-BytesEqual ($utf8.GetBytes("union Values { string, one: `"$value`" }")) ([System.IO.File]::ReadAllBytes($destination)) "Output encoding or string value changed."
    }

    Invoke-Test "conversion and path failures preserve existing destination" {
        $sourcePath = Join-Path $temporaryDirectory "unsafe.tsp"
        $destination = Join-Path $temporaryDirectory "preserved.tsp"
        [System.IO.File]::WriteAllText($destination, "preserved", $utf8)
        $before = [System.IO.File]::ReadAllBytes($destination)
        foreach ($source in @("enum Values { one: 1 }", "enum Good { one, } enum Bad { ...Other }")) {
            [System.IO.File]::WriteAllText($sourcePath, $source, $utf8)
            Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination } "*not supported*"
            Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($destination)) "Failure changed output."
        }
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $temporaryDirectory -OutputPath $destination } "*is not a file*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath (Join-Path $temporaryDirectory "missing.tsp") -OutputPath $destination } "*Cannot find path*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $sourcePath } "*different files*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath (Join-Path $temporaryDirectory "." "unsafe.tsp") } "*different files*"
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($destination)) "Path failure changed output."
    }

    Invoke-Test "invalid UTF-8 fails without publishing output" {
        $sourcePath = Join-Path $temporaryDirectory "invalid-utf8.tsp"
        $destination = Join-Path $temporaryDirectory "invalid-utf8-output.tsp"
        [System.IO.File]::WriteAllBytes($sourcePath, [byte[]](0xFF, 0xFE))
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath $destination } "*Unable to translate bytes*"
        Assert-True (-not [System.IO.File]::Exists($destination)) "Invalid UTF-8 produced output."
    }

    Invoke-Test "directory links cannot disguise an input overwrite" {
        $realDirectory = Join-Path $temporaryDirectory "real-directory"
        $linkDirectory = Join-Path $temporaryDirectory "linked-directory"
        $null = [System.IO.Directory]::CreateDirectory($realDirectory)
        $sourcePath = Join-Path $realDirectory "source.tsp"
        [System.IO.File]::WriteAllText($sourcePath, "enum Values { one, }", $utf8)
        if ($IsWindows) {
            $null = New-Item -ItemType Junction -Path $linkDirectory -Value $realDirectory
        }
        else { $null = [System.IO.Directory]::CreateSymbolicLink($linkDirectory, $realDirectory) }
        try {
            $before = [System.IO.File]::ReadAllBytes($sourcePath)
            Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath (Join-Path $linkDirectory "source.tsp") } "*different files*"
            Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($sourcePath)) "Linked path overwrote source."
        }
        finally { [System.IO.Directory]::Delete($linkDirectory) }
    }

    Invoke-Test "missing and blank paths have no defaults or fallback" {
        $sourcePath = Join-Path $temporaryDirectory "valid.tsp"
        $destination = Join-Path $temporaryDirectory "should-not-exist.tsp"
        [System.IO.File]::WriteAllText($sourcePath, "enum Values { one, }", $utf8)
        Assert-Throws { Invoke-TypeSpecEnumConversion -OutputPath $destination } "*InputPath is required*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath } "*OutputPath is required*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath " " -OutputPath $destination } "*InputPath is required*"
        Assert-Throws { Invoke-TypeSpecEnumConversion -InputPath $sourcePath -OutputPath " " } "*OutputPath is required*"
        Assert-True (-not [System.IO.File]::Exists($destination)) "Missing paths produced output."

        $pwshPath = (Get-Process -Id $PID).Path
        foreach ($case in @(
            @{ Arguments = @(); Error = "*InputPath is required*" },
            @{ Arguments = @("-OutputPath", $destination); Error = "*InputPath is required*" },
            @{ Arguments = @("-InputPath", $sourcePath); Error = "*OutputPath is required*" },
            @{ Arguments = @("-InputPath", " ", "-OutputPath", $destination); Error = "*InputPath is required*" },
            @{ Arguments = @("-InputPath", $sourcePath, "-OutputPath", " "); Error = "*OutputPath is required*" }
        )) {
            $arguments = $case.Arguments
            $output = & $pwshPath -NoLogo -NoProfile -NonInteractive -File $scriptPath @arguments 2>&1
            Assert-True ($LASTEXITCODE -ne 0) "CLI accepted missing or blank arguments."
            Assert-True (($output -join "`n") -like $case.Error) "CLI error was unclear: $output"
        }
        Assert-True (-not [System.IO.File]::Exists($destination)) "CLI inferred a destination."
    }

    Invoke-Test "CLI converts explicit paths successfully" {
        $sourcePath = Join-Path $temporaryDirectory "cli.tsp"
        $destination = Join-Path $temporaryDirectory "cli-output.tsp"
        [System.IO.File]::WriteAllText($sourcePath, "enum Values { one, }", $utf8)
        $pwshPath = (Get-Process -Id $PID).Path
        $output = & $pwshPath -NoLogo -NoProfile -NonInteractive -File $scriptPath -InputPath $sourcePath -OutputPath $destination 2>&1
        Assert-True ($LASTEXITCODE -eq 0) "CLI failed: $output"
        Assert-True (($output -join "`n") -like "*Converted 1 enum(s).*") "CLI summary was missing."
        Assert-Equal "union Values { string, one: `"one`", }" ([System.IO.File]::ReadAllText($destination)) "CLI output was incorrect."
    }

    Invoke-Test "decisions source matches the requested transformation exactly" {
        $sourcePath = Join-Path $PSScriptRoot ".." ".." ".." "specification" "openapi" "decisions" "typespec" "main.tsp"
        $source = [System.IO.File]::ReadAllText($sourcePath)
        $name = "createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject_typeOfInputImage_Detail_enum"
        $newline = if ($source.Contains("`r`n")) { "`r`n" } else { "`n" }
        $enumText = (@("enum $name {", "  auto,", "  high,", "  low,", "  original,", "}") -join $newline)
        $unionText = (@("union $name {", "  string,", '  auto: "auto",', '  high: "high",', '  low: "low",', '  original: "original",', "}") -join $newline)
        Assert-True ($source.Contains($enumText)) "Expected decisions enum was not found."
        $result = ConvertTo-TypeSpecExtensibleEnumText -Text $source
        Assert-Equal "1" $result.ConvertedEnumCount "Decisions enum count was incorrect."
        Assert-Equal ($source.Replace($enumText, $unionText)) $result.Text "Decisions output changed unrelated text."
    }
}
finally {
    if ([System.IO.Directory]::Exists($temporaryDirectory)) {
        [System.IO.Directory]::Delete($temporaryDirectory, $true)
    }
}

Write-Host ""
Write-Host "Passed: $script:PassedTests"
Write-Host "Failed: $script:FailedTests"
if ($script:FailedTests -gt 0) {
    foreach ($failure in $script:Failures) {
        Write-Host $failure -ForegroundColor Red
    }
    exit 1
}
