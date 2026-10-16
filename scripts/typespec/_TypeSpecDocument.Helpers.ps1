#Requires -Version 7.0

function Get-TypeSpecConversionTokens {
    param([AllowEmptyString()][string]$Text)

    $tokens = [System.Collections.Generic.List[object]]::new()
    $position = 0
    while ($position -lt $Text.Length) {
        $character = $Text[$position]
        if ([char]::IsWhiteSpace($character)) {
            $position++
            continue
        }
        if ($position + 1 -lt $Text.Length -and $Text.Substring($position, 2) -eq "//") {
            while ($position -lt $Text.Length -and $Text[$position] -notin "`r", "`n") {
                $position++
            }
            continue
        }
        if ($position + 1 -lt $Text.Length -and $Text.Substring($position, 2) -eq "/*") {
            $end = $Text.IndexOf("*/", $position + 2, [System.StringComparison]::Ordinal)
            if ($end -lt 0) { throw "Unterminated TypeSpec comment at offset $position." }
            $position = $end + 2
            continue
        }

        $start = $position
        $kind = "Punctuation"
        if ($character -eq '"' -or $character -eq '`') {
            $kind = if ($character -eq '"') { "String" } else { "Identifier" }
            $delimiter = [string]$character
            if ($character -eq '"' -and $position + 2 -lt $Text.Length -and
                $Text.Substring($position, 3) -eq '"""') {
                $delimiter = '"""'
            }
            $position += $delimiter.Length
            $closed = $false
            while ($position -lt $Text.Length) {
                if ($Text[$position] -eq '\') {
                    if ($position + 1 -ge $Text.Length -or $Text[$position + 1] -cnotin 'r', 'n', 't', '"', '\', '$', '@', '`') {
                        throw "Invalid TypeSpec escape at offset $position."
                    }
                    $position += 2
                    continue
                }
                if ($position + $delimiter.Length -le $Text.Length -and
                    $Text.Substring($position, $delimiter.Length) -eq $delimiter) {
                    $position += $delimiter.Length
                    $closed = $true
                    break
                }
                $position++
            }
            if (-not $closed) { throw "Unterminated TypeSpec $kind at offset $start." }
        }
        elseif ([System.Text.RegularExpressions.Regex]::IsMatch([string]$character, '[\p{L}_]')) {
            $kind = "Identifier"
            $position++
            while ($position -lt $Text.Length -and
                [System.Text.RegularExpressions.Regex]::IsMatch([string]$Text[$position], '[\p{L}\p{N}\p{M}_]')) {
                $position++
            }
        }
        else {
            $position++
        }
        $tokens.Add([PSCustomObject]@{
            Kind = $kind
            Text = $Text.Substring($start, $position - $start)
            Start = $start
            End = $position
        })
    }
    $tokens.Add([PSCustomObject]@{ Kind = "End"; Text = ""; Start = $Text.Length; End = $Text.Length })
    return ,$tokens.ToArray()
}

function ConvertFrom-TypeSpecEscapedText {
    param([AllowEmptyString()][string]$Text)

    return [System.Text.RegularExpressions.Regex]::Replace($Text, '\\(.)', {
        param($match)
        switch -CaseSensitive ($match.Groups[1].Value) {
            "r" { return "`r" }
            "n" { return "`n" }
            "t" { return "`t" }
            '"' { return '"' }
            '\' { return '\' }
            '$' { return '$' }
            '@' { return '@' }
            '`' { return '`' }
            default { throw "Invalid TypeSpec identifier escape '$($match.Value)'." }
        }
    })
}

function Get-TypeSpecEnumIdentifierValue {
    param([string]$Identifier)

    if (-not $Identifier.StartsWith('`', [System.StringComparison]::Ordinal)) {
        return $Identifier
    }
    return ConvertFrom-TypeSpecEscapedText -Text $Identifier.Substring(1, $Identifier.Length - 2)
}

function ConvertFrom-TypeSpecStringLiteral {
    param([string]$Literal)

    if ($Literal.Length -lt 2 -or -not $Literal.StartsWith('"') -or -not $Literal.EndsWith('"') -or
        $Literal.StartsWith('"""') -or $Literal.Contains("`n") -or $Literal.Contains("`r")) {
        throw "Expected a single-line TypeSpec string literal, got '$Literal'."
    }
    if ([System.Text.RegularExpressions.Regex]::IsMatch($Literal, '(?<!\\)(?:\\\\)*\$\{')) {
        throw "Interpolated discriminator strings are not supported."
    }
    return ConvertFrom-TypeSpecEscapedText -Text $Literal.Substring(1, $Literal.Length - 2)
}

function ConvertTo-TypeSpecStringLiteral {
    param([AllowEmptyString()][string]$Value)

    return '"' + $Value.Replace('\', '\\').Replace('"', '\"').
        Replace("`r", '\r').Replace("`n", '\n').Replace("`t", '\t').Replace('$', '\$') + '"'
}

function ConvertTo-TypeSpecIdentifier {
    param([AllowEmptyString()][string]$Value)

    $keywords = @("import", "model", "scalar", "namespace", "using", "op", "enum", "alias", "is",
        "interface", "union", "projection", "else", "if", "dec", "const", "init", "extern", "internal",
        "auto", "extends", "fn", "true", "false", "return", "void", "never", "unknown", "typeof",
        "valueof", "keyof", "null")
    if ($Value -cmatch '\A[A-Za-z_][A-Za-z0-9_]*\z' -and $Value -cnotin $keywords) { return $Value }
    $literal = ConvertTo-TypeSpecStringLiteral -Value $Value
    return '`' + $literal.Substring(1, $literal.Length - 2).Replace('`', '\`') + '`'
}

function Get-TypeSpecCanonicalPath {
    param([string]$Path, [int]$Depth = 0)

    if ($Depth -gt 32) { throw "Too many symbolic links while resolving TypeSpec path '$Path'." }
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    $root = [System.IO.Path]::GetPathRoot($fullPath)
    $parts = $fullPath.Substring($root.Length).Split(
        [char[]]@([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar),
        [System.StringSplitOptions]::RemoveEmptyEntries
    )
    $canonical = $root
    foreach ($part in $parts) {
        $canonical = [System.IO.Path]::Combine($canonical, $part)
        $entry = if ([System.IO.Directory]::Exists($canonical)) {
            [System.IO.DirectoryInfo]::new($canonical)
        } elseif ([System.IO.File]::Exists($canonical)) {
            [System.IO.FileInfo]::new($canonical)
        } else { $null }
        if ($null -ne $entry) {
            $target = $entry.ResolveLinkTarget($true)
            if ($null -ne $target) {
                $canonical = Get-TypeSpecCanonicalPath -Path $target.FullName -Depth ($Depth + 1)
            }
        }
    }
    return $canonical
}

function Get-TypeSpecTransformationPaths {
    param([string]$InputPath, [string]$OutputPath)

    if ([string]::IsNullOrWhiteSpace($InputPath)) { throw "InputPath is required." }
    if ([string]::IsNullOrWhiteSpace($OutputPath)) { throw "OutputPath is required." }
    $resolvedInputPath = (Resolve-Path -LiteralPath $InputPath -ErrorAction Stop).ProviderPath
    if (-not [System.IO.File]::Exists($resolvedInputPath)) {
        throw "The TypeSpec input path is not a file: $resolvedInputPath"
    }
    $resolvedOutputPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputPath)
    $comparison = if ($IsWindows) { [System.StringComparison]::OrdinalIgnoreCase } else { [System.StringComparison]::Ordinal }
    $canonicalInput = Get-TypeSpecCanonicalPath -Path $resolvedInputPath
    $canonicalOutput = Get-TypeSpecCanonicalPath -Path $resolvedOutputPath
    if ([string]::Equals($canonicalInput, $canonicalOutput, $comparison)) {
        throw "InputPath and OutputPath must refer to different files."
    }
    return [PSCustomObject]@{ InputPath = $resolvedInputPath; OutputPath = $resolvedOutputPath }
}

function Read-TypeSpecUtf8File {
    param([string]$Path)

    $bytes = [System.IO.File]::ReadAllBytes($Path)
    $encoding = [System.Text.UTF8Encoding]::new($false, $true)
    $offset = if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) { 3 } else { 0 }
    return [PSCustomObject]@{
        Bytes = $bytes
        Text = $encoding.GetString($bytes, $offset, $bytes.Length - $offset)
    }
}

function Write-TypeSpecFileAtomically {
    param([string]$Path, [AllowEmptyCollection()][byte[]]$Bytes)

    $directory = [System.IO.Path]::GetDirectoryName($Path)
    $null = [System.IO.Directory]::CreateDirectory($directory)
    $temporaryPath = Join-Path $directory ".$([System.IO.Path]::GetFileName($Path)).$([Guid]::NewGuid().ToString('N')).tmp"
    try {
        [System.IO.File]::WriteAllBytes($temporaryPath, $Bytes)
        [System.IO.File]::Move($temporaryPath, $Path, $true)
    }
    finally {
        if ([System.IO.File]::Exists($temporaryPath)) {
            [System.IO.File]::Delete($temporaryPath)
        }
    }
}

function Invoke-TypeSpecTextEdits {
    param([AllowEmptyString()][string]$Text, [AllowEmptyCollection()][object[]]$Edits)

    $builder = [System.Text.StringBuilder]::new()
    $position = 0
    foreach ($edit in ($Edits | Sort-Object Start, End)) {
        if ($edit.Start -lt $position -or $edit.End -lt $edit.Start -or $edit.End -gt $Text.Length) {
            throw "Overlapping or invalid TypeSpec transformation at offset $($edit.Start)."
        }
        $null = $builder.Append($Text, $position, $edit.Start - $position)
        $null = $builder.Append($edit.Text)
        $position = $edit.End
    }
    $null = $builder.Append($Text, $position, $Text.Length - $position)
    return $builder.ToString()
}
