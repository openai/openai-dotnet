<#
.SYNOPSIS
    Generates the public API surface for the OpenAI .NET library using GenAPI.

.DESCRIPTION
    This script invokes the MSBuild GenerateApi target to produce C# source files
    representing the public API contract of the OpenAI library. The output files
    are placed in the 'api' folder at the repository root.

    By default, API files are generated for all target frameworks defined in
    ClientTargetFrameworks (Directory.Build.props). You can optionally specify
    a single target framework using the -TargetFramework parameter.

.PARAMETER TargetFramework
    Optional. The target framework to generate API for (e.g., net10.0, net8.0, netstandard2.0).
    Must be a valid framework from ClientTargetFrameworks in Directory.Build.props.
    If not specified, generates API for all target frameworks.

.PARAMETER Configuration
    Optional. The build configuration to use. Defaults to 'Release'.

.EXAMPLE
    .\Generate-Api.ps1
    Generates API for all target frameworks using Release configuration.

.EXAMPLE
    .\Generate-Api.ps1 -TargetFramework net8.0
    Generates API for net8.0 only.

.EXAMPLE
    .\Generate-Api.ps1 -Configuration Debug
    Generates API for all target frameworks using Debug configuration.

.NOTES
    Target frameworks are determined by ClientTargetFrameworks in Directory.Build.props.
    Outputs are written to api/OpenAI.<TargetFramework>.cs
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$TargetFramework,

    [Parameter(Mandatory = $false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

# Resolve paths
$repoRoot = Join-Path $PSScriptRoot ".." -Resolve
$projectPath = Join-Path $repoRoot "src" "OpenAI.csproj"
$apiOutputDir = Join-Path $repoRoot "api"

# Get ClientTargetFrameworks from Directory.Build.props
$propsPath = Join-Path $repoRoot "Directory.Build.props"
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

$supportedFrameworks = $clientTargetFrameworks -split ";"

Write-Host "OpenAI .NET API Generator" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

# Validate TargetFramework if provided
if ($TargetFramework) {
    if ($TargetFramework -notin $supportedFrameworks) {
        Write-Error "TargetFramework '$TargetFramework' is not valid. Supported frameworks: $clientTargetFrameworks"
        exit 1
    }
}

# Ensure api output directory exists
if (-not (Test-Path $apiOutputDir)) {
    New-Item -ItemType Directory -Path $apiOutputDir -Force | Out-Null
    Write-Host "Created output directory: $apiOutputDir"
}

# Build the dotnet command arguments
$buildArgs = @(
    "build"
    $projectPath
    "-t:ExportApi"
    "-c:$Configuration"
    "-p:ExportingApi=true"
)

if ($TargetFramework) {
    Write-Host "Target Framework: $TargetFramework" -ForegroundColor Yellow
    Write-Host "Note: Generating for a single TFM will produce partial API output." -ForegroundColor Yellow
    Write-Host ""
    $buildArgs += "-p:TargetFramework=$TargetFramework"
} else {
    Write-Host "Target Frameworks: $clientTargetFrameworks" -ForegroundColor Green
    Write-Host ""
}

Write-Host "Configuration: $Configuration"
Write-Host "Output Directory: $apiOutputDir"
Write-Host ""
Write-Host "Running GenAPI..." -ForegroundColor Cyan
Write-Host ""

# Execute the build
& dotnet @buildArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "GenAPI failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Cleaning up generated files..." -ForegroundColor Cyan

# Clean up each generated file
Get-ChildItem -Path $apiOutputDir -Filter "OpenAI.*.cs" | ForEach-Object {
    Write-Host "  Cleaning $($_.Name)..."
    
    $content = Get-Content $_.FullName -Raw

    # Remove empty lines.
    $content = $content -creplace '//.*\r?\n', ''
    $content = $content -creplace '\r?\n\r?\n', "`n"
    $content = $content -creplace '\r?\n *{', " {"

    # Remove fully-qualified names.
    $content = $content -creplace "System\.ComponentModel\.", ""
    $content = $content -creplace "System\.ClientModel.Primitives\.", ""
    $content = $content -creplace "System\.ClientModel\.", ""
    $content = $content -creplace "System\.Collections\.Generic\.", ""
    $content = $content -creplace "System\.Collections\.", ""
    $content = $content -creplace "System\.Threading.Tasks\.", ""
    $content = $content -creplace "System\.Threading\.", ""
    $content = $content -creplace "System\.Text.Json\.", ""
    $content = $content -creplace "System\.Text\.", ""
    $content = $content -creplace "System\.IO\.", ""
    $content = $content -creplace "System\.", ""
    $content = $content -creplace "Assistants\.", ""
    $content = $content -creplace "Audio\.", ""
    $content = $content -creplace "Batch\.", ""
    $content = $content -creplace "Chat\.", ""
    $content = $content -creplace "Common\.", ""
    $content = $content -creplace "Containers\.", ""
    $content = $content -creplace "Conversations\.", ""
    $content = $content -creplace "Embeddings\.", ""
    $content = $content -creplace "Evals\.", ""
    $content = $content -creplace "Files\.", ""
    $content = $content -creplace "FineTuning\.", ""
    $content = $content -creplace "Graders\.", ""
    $content = $content -creplace "Images\.", ""
    $content = $content -creplace "Models\.", ""
    $content = $content -creplace "Moderations\.", ""
    $content = $content -creplace "Realtime\.", ""
    $content = $content -creplace "Responses\.", ""
    $content = $content -creplace "VectorStores\.", ""
    $content = $content -creplace "Videos\.", ""

    # Remove Diagnostics.DebuggerStepThrough attribute.
    $content = $content -creplace ".*Diagnostics.DebuggerStepThrough.*\n", ""

    # Remove ModelReaderWriterBuildable attributes.
    $content = $content -creplace '\[ModelReaderWriterBuildable\(typeof\([^\)]+\)\)\]\s*', ''

    # Remove internal APIs.
    $content = $content -creplace "  * internal.*`n", ""

    # Remove IJsonModel/IPersistableModel interface method entries.
    $content = $content -creplace "        .*(IJsonModel|IPersistableModel).*`n", ""

    # Other cosmetic simplifications.
    $content = $content -creplace "partial class", "class"
    $content = $content -creplace ".*private.*dummy.*`n", ""
    $content = $content -creplace " { throw null; }", ";"
    $content = $content -creplace " { }", ";"
    $content = $content -creplace "Diagnostics.CodeAnalysis.Experimental", "Experimental"
    $content = $content -creplace "Diagnostics.CodeAnalysis.SetsRequiredMembers", "SetsRequiredMembers"

    Set-Content -Path $_.FullName -Value $content -NoNewline
}

Write-Host ""
Write-Host "API generation completed successfully." -ForegroundColor Green
Write-Host ""

# List generated files
Write-Host "Generated files:" -ForegroundColor Cyan
Get-ChildItem -Path $apiOutputDir -Filter "OpenAI.*.cs" | ForEach-Object {
    Write-Host "  - $($_.Name)"
}
