<#
.SYNOPSIS
    Runs focused, dependency-free TypeSpec discriminator union transformation tests.
#>

#Requires -Version 7.0

[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest
$env:CLIENTMODEL_TEST_MODE = "Playback"
$env:CLIENTMODEL_DISABLE_AUTO_RECORDING = "true"
$scriptPath = Join-Path $PSScriptRoot ".." "Add-TypeSpecDiscriminatorUnions.ps1"
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

$temporaryDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "typespec-discriminator-tests-$([Guid]::NewGuid().ToString('N'))"
$null = [System.IO.Directory]::CreateDirectory($temporaryDirectory)
$utf8 = [System.Text.UTF8Encoding]::new($false)
$basicSource = "@discriminator(`"type`")`nmodel Content {`n  type: string;`n}`n`nmodel Image extends Content {`n  type: `"input_image`";`n}`n`nmodel Text extends Content {`n  type: `"input_text`";`n}`n"
$basicExpected = "@discriminator(`"type`")`nmodel Content {`n  type: Content_discriminator;`n}`n`nunion Content_discriminator {`n  string,`n  input_image: `"input_image`",`n  input_text: `"input_text`",`n}`n`nmodel Image extends Content {`n  type: Content_discriminator.input_image;`n}`n`nmodel Text extends Content {`n  type: Content_discriminator.input_text;`n}`n"

try {
    Invoke-Test "dot-sourcing exposes helpers without generating files" {
        Assert-True ($null -ne (Get-Command ConvertTo-TypeSpecDiscriminatorUnionText -ErrorAction SilentlyContinue)) "Text converter was not defined."
        Assert-True ($null -ne (Get-Command Invoke-TypeSpecDiscriminatorUnionGeneration -ErrorAction SilentlyContinue)) "File converter was not defined."
        Assert-Equal "0" ([System.IO.Directory]::GetFiles($temporaryDirectory).Length) "Dot-sourcing wrote files."
    }

    Invoke-Test "requested hierarchy is transformed exactly" {
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $basicSource
        Assert-Equal $basicExpected $result.Text "Output differed."
        Assert-Equal "1" $result.AddedUnionCount "Union count was incorrect."
        Assert-Equal "1" $result.BasePropertyCount "Base count was incorrect."
        Assert-Equal "2" $result.DerivedPropertyCount "Derived count was incorrect."
        Assert-Equal "2" $result.VariantCount "Variant count was incorrect."
    }

    Invoke-Test "source-order values are independent of base declaration order" {
        $source = 'model Z extends Base { kind: "z"; } model A extends Base { kind: "a"; } @discriminator("kind") model Base { kind: string; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "2" $result.DerivedPropertyCount "Forward descendants were missed."
        Assert-True ($result.Text.IndexOf('z: "z",', [System.StringComparison]::Ordinal) -lt $result.Text.IndexOf('a: "a",', [System.StringComparison]::Ordinal)) "Values were sorted instead of preserving declaration order."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.z;')) "Forward value was not rewritten."
    }

    Invoke-Test "indirect descendants and inherited literal properties are handled" {
        $source = '@discriminator("kind") model Base { kind: string; } model Middle extends Base {} model Leaf extends Middle { kind: "leaf"; } model Detail extends Leaf { note: string; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "1" $result.VariantCount "Inherited values produced extra variants."
        Assert-Equal "1" $result.DerivedPropertyCount "Indirect literal was not rewritten."
        Assert-True ($result.Text.Contains('model Middle extends Base {}')) "Intermediate inherited property was redeclared."
        Assert-True ($result.Text.Contains('model Detail extends Leaf { note: string; }')) "Inherited leaf was changed."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.leaf;')) "Indirect value reference was incorrect."
    }

    Invoke-Test "file-scoped and block namespaces resolve qualified inheritance" {
        $source = 'namespace Api; namespace Shapes { @TypeSpec.discriminator("kind") model Base { kind: string; } } namespace Other { model Leaf extends Api.Shapes.Base { kind: "leaf"; } }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "1" $result.VariantCount "Namespaced value was missed."
        Assert-True ($result.Text.Contains('kind: Api.Shapes.Base_discriminator.leaf;')) "Cross-namespace reference was not qualified."
    }

    Invoke-Test "same model names in separate namespaces have independent unions" {
        $source = 'namespace A { @discriminator("kind") model Base { kind: string; } model Leaf extends Base { kind: "a"; } } namespace B { @discriminator("type") model Base { type: string; } model Leaf extends Base { type: "b"; } }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "2" $result.AddedUnionCount "Independent bases were merged."
        Assert-Equal "2" $result.DerivedPropertyCount "Independent descendants were missed."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.a;')) "First namespace was incorrect."
        Assert-True ($result.Text.Contains('type: Base_discriminator.b;')) "Second namespace was incorrect."
    }

    Invoke-Test "shadowed generated references fail rather than bind to an unrelated type" {
        $source = '@discriminator("kind") model Base { kind: string; } namespace Other { union Base_discriminator { different: "different", } model Leaf extends Base { kind: "leaf"; } }'
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text $source } "*Cannot reference generated union*unambiguously*"
    }

    Invoke-Test "nested decorated bases use their own unions" {
        $source = '@discriminator("kind") model Base { kind: string; } @discriminator("kind") model Middle extends Base { kind: string; } model Leaf extends Middle { kind: "leaf"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "2" $result.AddedUnionCount "Nested union was missed."
        Assert-Equal "2" $result.VariantCount "Ancestor union did not include descendant values."
        Assert-Equal "1" $result.DerivedPropertyCount "Nested literal was rewritten more than once."
        Assert-True ($result.Text.Contains('kind: Middle_discriminator.leaf;')) "Literal did not use the nearest discriminator union."
        Assert-True ($result.Text.Contains('model Middle extends Base { kind: Middle_discriminator; }')) "Nested base property was incorrect."
    }

    Invoke-Test "nested bases with different properties transform both discriminators" {
        $source = '@discriminator("kind") model Base { kind: string; } @discriminator("type") model Branch extends Base { kind: "branch"; type: string; } model Leaf extends Branch { type: "leaf"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "2" $result.AddedUnionCount "Different nested discriminator was missed."
        Assert-Equal "2" $result.DerivedPropertyCount "Nested literal count was incorrect."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.branch; type: Branch_discriminator;')) "Branch properties were incorrect."
        Assert-True ($result.Text.Contains('type: Branch_discriminator.leaf;')) "Nested leaf property was incorrect."
    }

    Invoke-Test "non-identifier and reserved discriminator values are escaped without sanitization" {
        $source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "input-image"; } model B extends Base { kind: "input_image"; } model C extends Base { kind: "true"; } model D extends Base { kind: "quote\"slash\\tick`"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "4" $result.VariantCount "Escaping collapsed distinct wire values."
        Assert-True ($result.Text.Contains('`input-image`: "input-image",')) "Hyphenated label was not escaped."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.`input-image`;')) "Hyphenated member reference was incorrect."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.`true`;')) "Reserved label was not escaped."
        Assert-True ($result.Text.Contains('`quote\"slash\\tick\``: "quote\"slash\\tick`",')) "Escaped value was corrupted."
    }

    Invoke-Test "escaped model and property names remain resolvable" {
        $source = '@discriminator("wire-kind") model `base-name` { `wire-kind`: string; } model Leaf extends `base-name` { `wire-kind`: "value"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-True ($result.Text.Contains('`wire-kind`: `base-name_discriminator`.value;')) "Escaped union reference was incorrect."
        Assert-True ($result.Text.Contains('union `base-name_discriminator` {')) "Escaped union name was incorrect."
    }

    Invoke-Test "case-distinct values remain distinct" {
        $source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "a"; } model B extends Base { kind: "A"; }'
        Assert-Equal "2" (ConvertTo-TypeSpecDiscriminatorUnionText -Text $source).VariantCount "Case-distinct values were merged."
    }

    Invoke-Test "empty and escaped newline values retain their wire strings" {
        $source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: ""; } model B extends Base { kind: "new\nline"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "2" $result.VariantCount "Empty or escaped value was lost."
        Assert-True ($result.Text.Contains('``: "",')) "Empty variant label was incorrect."
        Assert-True ($result.Text.Contains('kind: Base_discriminator.``;')) "Empty variant reference was incorrect."
        Assert-True ($result.Text.Contains('`new\nline`: "new\nline",')) "Escaped newline wire value changed."
    }

    Invoke-Test "comments, other decorators, defaults and unrelated unions remain unchanged" {
        $source = @'
// @discriminator("ignored") model Fake { type: string; }
@doc("Text containing @discriminator and model Fake { }")
@discriminator("kind")
@summary("Base")
model Base { kind: /* before */ string /* after */; literal: "unchanged"; }
union Existing { string, known: "known", }
model A extends Base { @doc("kind") kind: /* value */ "a" /* tail */ = "a"; note: "other"; }
'@
        $newline = if ($source.Contains("`r`n")) { "`r`n" } else { "`n" }
        $union = (@("union Base_discriminator {", "  string,", '  a: "a",', "}") -join $newline)
        $expected = $source.Replace('string /* after */', 'Base_discriminator /* after */').
            Replace('literal: "unchanged"; }', 'literal: "unchanged"; }' + "$newline$newline" + $union).
            Replace('"a" /* tail */', 'Base_discriminator.a /* tail */')
        Assert-Equal $expected (ConvertTo-TypeSpecDiscriminatorUnionText -Text $source).Text "Unrelated text changed."
    }

    Invoke-Test "CRLF and namespace indentation are retained" {
        $source = "namespace N {`r`n  @discriminator(`"kind`")`r`n  model Base { kind: string; }`r`n  model A extends Base { kind: `"a`"; }`r`n}`r`n"
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-True ($result.Text.Contains("`r`n  union Base_discriminator {`r`n    string,`r`n    a: `"a`",`r`n  }")) "Inserted indentation or CRLF was incorrect."
        Assert-True (-not $result.Text.Replace("`r`n", "").Contains("`n")) "Bare LF was introduced."
    }

    Invoke-Test "base with no descendants receives a string-only union" {
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base { kind: string; }'
        Assert-Equal "1" $result.AddedUnionCount "Empty hierarchy was skipped."
        Assert-Equal "0" $result.VariantCount "Empty hierarchy acquired values."
        Assert-Equal "0" $result.DerivedPropertyCount "Empty hierarchy acquired overrides."
    }

    Invoke-Test "unsupported discriminator properties and hierarchy forms fail clearly" {
        foreach ($case in @(
            @{ Source = '@discriminator("kind") model Base {}'; Error = "*explicitly declared*" },
            @{ Source = '@discriminator("kind") model Base { kind?: string; }'; Error = "*required plain string*" },
            @{ Source = '@discriminator("kind") model Base { kind: int32; }'; Error = "*required plain string*" },
            @{ Source = '@discriminator("kind") model Base { kind: string | "a"; }'; Error = "*required plain string*" },
            @{ Source = '@discriminator("kind") model Base<T> { kind: string; }'; Error = "*Unsupported templates*" },
            @{ Source = '@discriminator("kind") model Base { ...Other; kind: string; }'; Error = "*Unsupported model spreads*" },
            @{ Source = '@discriminator("kind") model Base is Other;'; Error = "*explicitly declared*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: 1; }'; Error = "*required string literal*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind?: "a"; }'; Error = "*required string literal*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base {}'; Error = "*Missing string-literal*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A<T> extends Base { kind: "a"; }'; Error = "*Unsupported templates*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base<string> { kind: "a"; }'; Error = "*Unsupported templates*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: Existing.a; }'; Error = "*required string literal*" },
            @{ Source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "${value}"; }'; Error = "*Interpolated discriminator*" }
        )) {
            Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text $case.Source } $case.Error
        }
    }

    Invoke-Test "cycles and unresolved relevant ancestry fail" {
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base extends A { kind: string; } model A extends Base {}' } "*Cyclic model inheritance*"
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base extends Imported { kind: string; }' } "*Cannot resolve relevant base*"
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text 'model Ancestor<T> { kind: string; } @discriminator("kind") model Base extends Ancestor { kind: string; }' } "*Unsupported templates in ancestor*"
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text 'model Ancestor { kind: "ancestor"; } @discriminator("kind") model Base extends Ancestor { kind: string; }' } "*Contradictory discriminator override*"
    }

    Invoke-Test "unrelated imported references and templates remain untouched" {
        $source = 'model External extends Imported {} model Box<T> { value: T; } @discriminator("kind") model Base { kind: string; } model A extends Base { kind: "a"; }'
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-True ($result.Text.StartsWith('model External extends Imported {} model Box<T> { value: T; }')) "Unrelated declarations were modified."
    }

    Invoke-Test "duplicate alternatives and contradictory overrides fail" {
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "a"; } model B extends Base { kind: "a"; }' } "*Ambiguous duplicate discriminator*"
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "a"; } model B extends A { kind: "b"; }' } "*Contradictory discriminator override*"
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "a"; } model B extends A { kind: "a"; }'
        Assert-Equal "1" $result.VariantCount "Same-branch reuse produced duplicate variants."
        Assert-Equal "2" $result.DerivedPropertyCount "Same-branch explicit reuse was not rewritten."
    }

    Invoke-Test "generated names cannot collide with existing declarations" {
        foreach ($declaration in @("union Base_discriminator { string, }", "model Base_discriminator {}", "alias Base_discriminator = string;", "namespace Base_discriminator {}", "namespace Base_discriminator.Nested {}")) {
            Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text ('@discriminator("kind") model Base { kind: string; } ' + $declaration) } "*conflicts with an existing declaration*"
        }
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text '@discriminator("kind") model Base { kind: string; } model Base {}' } "*Ambiguous TypeSpec declaration*"
    }

    Invoke-Test "invalid decorators and malformed TypeSpec fail" {
        foreach ($case in @(
            @{ Source = '@@discriminator(Base, "kind"); model Base { kind: string; }'; Error = "*Augment discriminator*" },
            @{ Source = '@discriminator(name) model Base { kind: string; }'; Error = "*one literal string*" },
            @{ Source = '@discriminator("") model Base { kind: string; }'; Error = "*cannot be empty*" },
            @{ Source = '@discriminator("kind") union Base { string, }'; Error = "*must target a model*" },
            @{ Source = '@discriminator("kind")'; Error = "*no model declaration*" },
            @{ Source = '@discriminator("kind") model Base { kind: string;'; Error = "*Unterminated TypeSpec delimiter*" },
            @{ Source = '@discriminator("kind") model Base { kind: ; }'; Error = "*Missing type*" },
            @{ Source = '/* unterminated'; Error = "*Unterminated TypeSpec comment*" }
        )) {
            Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text $case.Source } $case.Error
        }
    }

    Invoke-Test "repeated conversion from the source is deterministic and transformed input is rejected" {
        $first = ConvertTo-TypeSpecDiscriminatorUnionText -Text $basicSource
        Assert-Equal $first.Text (ConvertTo-TypeSpecDiscriminatorUnionText -Text $basicSource).Text "Repeated conversion changed output."
        Assert-Throws { ConvertTo-TypeSpecDiscriminatorUnionText -Text $first.Text } "*required plain string*"
    }

    Invoke-Test "explicit file paths create atomic output without modifying source" {
        $sourcePath = Join-Path $temporaryDirectory "custom input.tsp"
        $destination = Join-Path $temporaryDirectory "nested" "custom output.tsp"
        [System.IO.File]::WriteAllText($sourcePath, $basicSource, $utf8)
        $before = [System.IO.File]::ReadAllBytes($sourcePath)
        $result = Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination
        Assert-Equal $destination $result.OutputPath "Explicit destination was not honored."
        Assert-Equal $basicExpected ([System.IO.File]::ReadAllText($destination)) "File output differed."
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($sourcePath)) "Input changed."
        $null = Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination
        Assert-Equal $basicExpected ([System.IO.File]::ReadAllText($destination)) "Atomic replacement changed output."
        Assert-True ([System.IO.Directory]::GetFiles((Split-Path $destination), "*.tmp").Length -eq 0) "Temporary files were not cleaned up."
    }

    Invoke-Test "no-discriminator files remain byte-equivalent, including empty files and BOM" {
        $sourcePath = Join-Path $temporaryDirectory "unchanged.tsp"
        $destination = Join-Path $temporaryDirectory "unchanged-output.tsp"
        foreach ($bytes in @([byte[]]@(), $utf8.GetBytes('union Existing { string, }'), [byte[]](0xEF, 0xBB, 0xBF) + $utf8.GetBytes("// no discriminator`r`n"))) {
            [System.IO.File]::WriteAllBytes($sourcePath, $bytes)
            $result = Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination
            Assert-Equal "0" $result.AddedUnionCount "Unrelated file was counted."
            Assert-BytesEqual $bytes ([System.IO.File]::ReadAllBytes($destination)) "Unrelated file bytes changed."
        }
    }

    Invoke-Test "UTF-8 values are preserved and transformed output has no BOM" {
        $sourcePath = Join-Path $temporaryDirectory "utf8.tsp"
        $destination = Join-Path $temporaryDirectory "utf8-output.tsp"
        $value = [string][char]0x00E9
        $source = '@discriminator("kind") model Base { kind: string; } model A extends Base { kind: "' + $value + '"; }'
        [System.IO.File]::WriteAllText($sourcePath, $source, [System.Text.UTF8Encoding]::new($true))
        $null = Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination
        $bytes = [System.IO.File]::ReadAllBytes($destination)
        Assert-True ($bytes[0] -ne 0xEF) "Output retained a BOM."
        Assert-True ([System.IO.File]::ReadAllText($destination).Contains('`' + $value + '`: "' + $value + '",')) "Unicode wire value changed."
    }

    Invoke-Test "failed conversion preserves an existing destination and rejects unsafe paths" {
        $sourcePath = Join-Path $temporaryDirectory "unsafe.tsp"
        $destination = Join-Path $temporaryDirectory "preserved.tsp"
        [System.IO.File]::WriteAllText($destination, "preserved", $utf8)
        $before = [System.IO.File]::ReadAllBytes($destination)
        [System.IO.File]::WriteAllText($sourcePath, $basicSource + '@discriminator("other") model Bad { other: int32; }', $utf8)
        Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination } "*required plain string*"
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($destination)) "Failure published partial output."
        Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $sourcePath } "*different files*"
        Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $temporaryDirectory -OutputPath $destination } "*is not a file*"
        Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath (Join-Path $temporaryDirectory "missing.tsp") -OutputPath $destination } "*Cannot find path*"
        [System.IO.File]::WriteAllBytes($sourcePath, [byte[]](0xFF, 0xFE))
        Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath $destination } "*Unable to translate bytes*"
        Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($destination)) "Invalid UTF-8 changed output."
    }

    Invoke-Test "CLI has no path defaults, prompts or fallback" {
        $sourcePath = Join-Path $temporaryDirectory "cli-input.tsp"
        $destination = Join-Path $temporaryDirectory "cli-output.tsp"
        [System.IO.File]::WriteAllText($sourcePath, $basicSource, $utf8)
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
            Assert-True ($LASTEXITCODE -ne 0) "CLI accepted missing paths."
            Assert-True (($output -join "`n") -like $case.Error) "CLI error was unclear: $output"
        }
        Assert-True (-not [System.IO.File]::Exists($destination)) "Missing paths produced output."
        $output = & $pwshPath -NoLogo -NoProfile -NonInteractive -File $scriptPath -InputPath $sourcePath -OutputPath $destination 2>&1
        Assert-True ($LASTEXITCODE -eq 0) "CLI failed: $output"
        Assert-Equal $basicExpected ([System.IO.File]::ReadAllText($destination)) "CLI output was incorrect."
    }

    Invoke-Test "directory links cannot disguise an input overwrite" {
        $realDirectory = Join-Path $temporaryDirectory "real-directory"
        $linkDirectory = Join-Path $temporaryDirectory "linked-directory"
        $null = [System.IO.Directory]::CreateDirectory($realDirectory)
        $sourcePath = Join-Path $realDirectory "source.tsp"
        [System.IO.File]::WriteAllText($sourcePath, $basicSource, $utf8)
        if ($IsWindows) {
            $null = New-Item -ItemType Junction -Path $linkDirectory -Value $realDirectory
        }
        else { $null = [System.IO.Directory]::CreateSymbolicLink($linkDirectory, $realDirectory) }
        try {
            $before = [System.IO.File]::ReadAllBytes($sourcePath)
            Assert-Throws { Invoke-TypeSpecDiscriminatorUnionGeneration -InputPath $sourcePath -OutputPath (Join-Path $linkDirectory "source.tsp") } "*different files*"
            Assert-BytesEqual $before ([System.IO.File]::ReadAllBytes($sourcePath)) "Linked path overwrote source."
        }
        finally { [System.IO.Directory]::Delete($linkDirectory) }
    }

    Invoke-Test "decisions output contains exactly the four expected unions and fourteen replacements" {
        $sourcePath = Join-Path $PSScriptRoot ".." ".." ".." "specification" "openapi" "decisions" "typespec" "main.extensible-enums.tsp"
        $source = [System.IO.File]::ReadAllText($sourcePath)
        $result = ConvertTo-TypeSpecDiscriminatorUnionText -Text $source
        Assert-Equal "4" $result.AddedUnionCount "Decisions union count was incorrect."
        Assert-Equal "4" $result.BasePropertyCount "Decisions base count was incorrect."
        Assert-Equal "10" $result.DerivedPropertyCount "Decisions derived count was incorrect."
        Assert-Equal "10" $result.VariantCount "Decisions variant count was incorrect."
        $newline = if ($source.Contains("`r`n")) { "`r`n" } else { "`n" }
        $hierarchies = @(
            @{ Name = "createDecision_request_object_Input_union_array_item_discriminatedObject"; Values = @("message") },
            @{ Name = "createDecision_request_object_Input_union_array_item_discriminatedObject_typeOfMessage_Content_union_array_item_discriminatedObject"; Values = @("input_image", "input_text") },
            @{ Name = "createDecision_request_object_Questions_array_item_discriminatedObject"; Values = @("choice", "predicate", "score") },
            @{ Name = "createDecision_response_200_object_Answers_array_item_discriminatedObject"; Values = @("choice", "predicate", "refusal", "score") }
        )
        $expected = $source
        foreach ($hierarchy in $hierarchies) {
            $name = $hierarchy.Name
            $unionName = "${name}_discriminator"
            $originalBase = "model $name {$newline  type: string;$newline}"
            $lines = @("union $unionName {", "  string,") + @($hierarchy.Values | ForEach-Object { "  ${_}: `"$_`"," }) + "}"
            $expected = $expected.Replace($originalBase, "model $name {$newline  type: $unionName;$newline}$newline$newline" + ($lines -join $newline))
            foreach ($value in $hierarchy.Values) {
                $originalProperty = "extends $name {$newline  type: `"$value`";"
                $expected = $expected.Replace($originalProperty, "extends $name {$newline  type: $unionName.$value;")
            }
        }
        Assert-Equal $expected $result.Text "Decisions output changed unrelated text or missed a reference."
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
    foreach ($failure in $script:Failures) { Write-Host $failure -ForegroundColor Red }
    exit 1
}
