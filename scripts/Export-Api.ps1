<#
.SYNOPSIS
    Generates the public API surface for the OpenAI .NET library using GenAPI.

.DESCRIPTION
    This script invokes the MSBuild GenerateApi target to produce C# source files
    representing the public API contract of the OpenAI library. The output files
    are placed in the 'api' folder at the repository root.

.EXAMPLE
    .\Export-Api.ps1
    Generates API for all target frameworks defined in
    ClientTargetFrameworks (Directory.Build.props) using the Release configuration.

.NOTES
    Outputs are written to api/OpenAI.<TargetFramework>.cs
#>

[CmdletBinding()]
param(
)

$ErrorActionPreference = "Stop"

$configuration = "Release"

# Resolve paths
$repoRootPath = Join-Path $PSScriptRoot ".." -Resolve
$projectPath = Join-Path $repoRootPath "src" "OpenAI.csproj"
$responsesProjectPath = Join-Path $repoRootPath "src" "Responses" "OpenAI.Responses.csproj"
$outputDirectory = Join-Path $repoRootPath "api"

# Get ClientTargetFrameworks from Directory.Build.props
$propsPath = Join-Path $repoRootPath "Directory.Build.props"
$clientTargetFrameworks = ""
if (Test-Path $propsPath) {
    $propsContent = Get-Content $propsPath -Raw
    if ($propsContent -match '<ClientTargetFrameworks>([^<]+)</ClientTargetFrameworks>') {
        $clientTargetFrameworks = $Matches[1]
    }
}

if (-not $clientTargetFrameworks) {
    Write-Error "Could not find ClientTargetFrameworks in Directory.Build.props"
    exit 1
}

Write-Host ""
Write-Host "Target Frameworks: $clientTargetFrameworks" -ForegroundColor Green
Write-Host "Configuration: $configuration"
Write-Host ""

# Ensure output directory exists and is clean
if (Test-Path $outputDirectory) {
    Write-Host "Cleaning existing output directory..." -ForegroundColor Cyan
    try {
        Get-ChildItem -Path $outputDirectory -Force | Remove-Item -Recurse -Force
    }
    catch {
        Write-Warning "Failed to clean some items in output directory: $_"
    }
} else {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
    Write-Host "Created output directory: $outputDirectory"
}

# Build the dotnet command arguments
$buildArgs = @(
    "build"
    $projectPath
    "-t:ExportApi"
    "-c:$configuration"
    "-p:ExportingApi=true"
)

Write-Host "Output Directory: $outputDirectory"
Write-Host ""
Write-Host "Running GenAPI for OpenAI..." -ForegroundColor Cyan
Write-Host ""

# Run a single build command - the MSBuild target handles all frameworks
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "GenAPI failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

# Build OpenAI.Responses
$responsesBuildArgs = @(
    "build"
    $responsesProjectPath
    "-t:ExportApi"
    "-c:$configuration"
    "-p:ExportingApi=true"
)

Write-Host ""
Write-Host "Running GenAPI for OpenAI.Responses..." -ForegroundColor Cyan
Write-Host ""

& dotnet @responsesBuildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "GenAPI failed for OpenAI.Responses with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Cleaning up generated files..." -ForegroundColor Cyan

# Clean up each generated file
Get-ChildItem -Path $outputDirectory -Filter "OpenAI*.cs" | ForEach-Object {
    Write-Host "  Cleaning $($_.Name)..."
    
    $content = Get-Content $_.FullName -Raw

    # Normalize line breaks and whitespace.
    $content = $content -creplace '\r?\n\r?\n', "`n"
    $content = $content -creplace '\r?\n *{', " {"

    # Remove fully-qualified namespace prefixes.
    @(
        "Diagnostics\.CodeAnalysis\.",
        "System\.ComponentModel\.",
        "System\.ClientModel\.Primitives\.",
        "System\.ClientModel\.",
        "System\.Collections\.Generic\.",
        "System\.Collections\.",
        "System\.Threading\.Tasks\.",
        "System\.Threading\.",
        "System\.Text\.Json\.",
        "System\.Text\.",
        "System\.IO\.",
        "System\." # System must be last to avoid partial matches
    ) | ForEach-Object { $content = $content -creplace $_, "" }

    # Remove OpenAI sub-namespace prefixes.
    @(
        "Assistants",
        "Audio",
        "Batch",
        "Chat",
        "Common",
        "Containers",
        "Conversations",
        "Embeddings",
        "Evals",
        "Files",
        "FineTuning",
        "Graders",
        "Images",
        "Models",
        "Moderations",
        "Realtime",
        "Responses",
        "VectorStores",
        "Videos"
    ) | ForEach-Object { $content = $content -creplace "$_\.", "" }

    # Remove non-public APIs.
    $content = $content -creplace "  * internal.*`n", ""
    $content = $content -creplace ".*private.*dummy.*`n", ""

    # Remove Diagnostics.DebuggerStepThrough attribute.
    $content = $content -creplace ".*Diagnostics.DebuggerStepThrough.*\n", ""

    # Remove ModelReaderWriterBuildable attributes.
    $content = $content -creplace '\[ModelReaderWriterBuildable\(typeof\([^\)]+\)\)\]\s*', ''

    # Remove IJsonModel/IPersistableModel interface method entries.
    $content = $content -creplace "        .*(IJsonModel|IPersistableModel).*`n", ""

    # Other cosmetic simplifications.
    $content = $content -creplace "partial class", "class"
    $content = $content -creplace " { throw null; }", ";"
    $content = $content -creplace " { }", ";"

    Set-Content -Path $_.FullName -Value $content -NoNewline
}

Write-Host ""
Write-Host "API generation completed successfully." -ForegroundColor Green
Write-Host ""

# List generated files
Write-Host "Generated files:" -ForegroundColor Cyan
Get-ChildItem -Path $outputDirectory -Filter "OpenAI*.cs" | ForEach-Object {
    Write-Host "  - $($_.Name)"
}
Write-Host ""