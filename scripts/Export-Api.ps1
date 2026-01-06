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
Write-Host "Running GenAPI for all target frameworks..." -ForegroundColor Cyan
Write-Host ""

# Run a single build command - the MSBuild target handles all frameworks
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "GenAPI failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Cleaning up generated files..." -ForegroundColor Cyan

# Clean up each generated file
Get-ChildItem -Path $outputDirectory -Filter "OpenAI.*.cs" | ForEach-Object {
    Write-Host "  Cleaning $($_.Name)..."
    
    $content = Get-Content $_.FullName -Raw

    if ($isWindowsPlatform) {
        $dotnetPacksPath = Join-Path $env:ProgramFiles "dotnet\packs\Microsoft.NETCore.App.Ref"
        $nugetPackagesPath = Join-Path $env:UserProfile ".nuget\packages"
    } elseif ($isMacOSPlatform) {
        $dotnetPacksPath = "/usr/local/share/dotnet/packs/Microsoft.NETCore.App.Ref"
        $nugetPackagesPath = Join-Path $env:HOME ".nuget/packages"
    } else {
        # Linux or other Unix-like systems
        $dotnetPacksPath = "/usr/share/dotnet/packs/Microsoft.NETCore.App.Ref"
        $nugetPackagesPath = Join-Path $env:HOME ".nuget/packages"
    }

    Write-Output "  * .NET packs:"
    Write-Output "    $($dotnetPacksPath)"
    Write-Output ""
    Write-Output "  * NuGet packages:"
    Write-Output "    $($nugetPackagesPath)"
    Write-Output ""

    Write-Output "  Assembly reference paths:"
    Write-Output ""

    # .NET
    $netRef = $null
    if (Test-Path $dotnetPacksPath) {
        $netRef = Get-ChildItem -Recurse `
            -Path $dotnetPacksPath `
            -Include "net8.0" | Select-Object -Last 1
    }
    
    # If not found in primary location, try alternative locations
    if (-not $netRef) {
        $alternativePaths = @()
        if ($isWindowsPlatform) {
            $alternativePaths += "${env:ProgramFiles(x86)}\dotnet\packs\Microsoft.NETCore.App.Ref"
        } elseif ($isMacOSPlatform) {
            $alternativePaths += "/usr/local/share/dotnet/packs/Microsoft.NETCore.App.Ref"
            $alternativePaths += "/Library/Frameworks/Microsoft.NETCore.App.Ref"
        } else {
            $alternativePaths += "/usr/lib/dotnet/packs/Microsoft.NETCore.App.Ref"
            $alternativePaths += "/opt/dotnet/packs/Microsoft.NETCore.App.Ref"
        }
        
        foreach ($altPath in $alternativePaths) {
            if (Test-Path $altPath) {
                $netRef = Get-ChildItem -Recurse -Path $altPath -Include "net8.0" | Select-Object -Last 1
                if ($netRef) { break }
            }
        }
    }

    Write-Output "  * .NET:"
    Write-Output "    $($netRef)"
    Write-Output ""

    # System.ClientModel
    $systemClientModelPath = Join-Path $nugetPackagesPath "system.clientmodel\1.8.1"
    $systemClientModelRef = $null
    if (Test-Path $systemClientModelPath) {
        $systemClientModelRef = Get-ChildItem `
            -Path $systemClientModelPath `
            -Include $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net8.0") `
            -Recurse |
                Select-Object -Last 1
    }

    Write-Output "  * System.ClientModel:"
    Write-Output "    $($systemClientModelRef)"
    Write-Output ""

    # System.Net.ServerSentEvents
    $systemNetServerSentEventsPath = Join-Path $nugetPackagesPath "system.net.serversentevents\9.0.9"
    $systemNetServerSentEventsRef = $null
    if (Test-Path $systemNetServerSentEventsPath) {
        $systemNetServerSentEventsRef = Get-ChildItem `
            -Path $systemNetServerSentEventsPath `
            -Include $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net8.0") `
            -Recurse |
                Select-Object -Last 1
    }

    Write-Output "  * System.Net.ServerSentEvents:"
    Write-Output "    $($systemNetServerSentEventsRef)"
    Write-Output ""

    # Microsoft.Extensions.Logging.Abstractions
    $microsoftExtensionsLoggingAbstractionsPath = Join-Path $nugetPackagesPath "microsoft.extensions.logging.abstractions\8.0.3"
    $microsoftExtensionsLoggingAbstractionsRef = $null
    if (Test-Path $microsoftExtensionsLoggingAbstractionsPath) {
        $microsoftExtensionsLoggingAbstractionsRef = Get-ChildItem `
            -Path $microsoftExtensionsLoggingAbstractionsPath `
            -Include  $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net8.0") `
            -Recurse |
                Select-Object -Last 1
    }

    Write-Output "  * Microsoft.Extensions.Logging.Abstractions:"
    Write-Output "    $($microsoftExtensionsLoggingAbstractionsRef)"
    Write-Output ""

    # Microsoft.Extensions.DependencyInjection.Abstractions
    $microsoftExtensionsDependencyInjectionAbstractionsPath = Join-Path $nugetPackagesPath "microsoft.extensions.dependencyinjection.abstractions\8.0.2"
    $microsoftExtensionsDependencyInjectionAbstractionsRef = $null
    if (Test-Path $microsoftExtensionsDependencyInjectionAbstractionsPath) {
        $microsoftExtensionsDependencyInjectionAbstractionsRef = Get-ChildItem `
            -Path $microsoftExtensionsDependencyInjectionAbstractionsPath `
            -Include  $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net8.0") `
            -Recurse |
                Select-Object -Last 1
    }

    Write-Output "  * Microsoft.Extensions.DependencyInjection.Abstractions:"
    Write-Output "    $($microsoftExtensionsDependencyInjectionAbstractionsRef)"
    Write-Output ""

    # System.Memory.Data
    $systemMemoryDataPath = Join-Path $nugetPackagesPath "system.memory.data\8.0.1"
    $systemMemoryDataRef = $null
    if (Test-Path $systemMemoryDataPath) {
        $systemMemoryDataRef = Get-ChildItem `
            -Path $systemMemoryDataPath `
            -Include  $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net6.0") `
            -Recurse |
                Select-Object -Last 1
    }

    Write-Output "  * System.Memory.Data:"
    Write-Output "    $($systemMemoryDataRef)"
    Write-Output ""

    if ($TargetFramework -eq "netstandard2.0") {
        # System.Diagnostics.DiagnosticSource
        $systemDiagnosticsDiagnosticSourcePath = Join-Path $nugetPackagesPath "system.diagnostics.diagnosticsource\8.0.1"
        $systemDiagnosticsDiagnosticSourceRef = $null
        if (Test-Path $systemDiagnosticsDiagnosticSourcePath) {
            $systemDiagnosticsDiagnosticSourceRef = Get-ChildItem `
                -Path $systemDiagnosticsDiagnosticSourcePath `
                -Include $(($TargetFramework -eq "netstandard2.0") ? "netstandard2.0" : "net5.0") `
                -Recurse |
                    Select-Object -Last 1
        }

        Write-Output "  * System.Diagnostics.DiagnosticSource:"
        Write-Output "    $($systemDiagnosticsDiagnosticSourceRef)"
        Write-Output ""

        # Microsoft.Bcl.AsyncInterfaces
        $microsoftBclAsyncInterfacesPath = Join-Path $nugetPackagesPath "microsoft.bcl.asyncinterfaces\8.0.0"
        $microsoftBclAsyncInterfacesRef = $null
        if (Test-Path $microsoftBclAsyncInterfacesPath) {
            $microsoftBclAsyncInterfacesRef = Get-ChildItem `
                -Path $microsoftBclAsyncInterfacesPath `
                -Include "netstandard2.0" `
                -Recurse |
                    Select-Object -Last 1
        }

        Write-Output "  * Microsoft.Bcl.AsyncInterfaces:"
        Write-Output "    $($microsoftBclAsyncInterfacesRef)"
        Write-Output ""
    }

    # Internal project references (e.g. OpenAI.Shared)
    $assemblyDir = Split-Path -Parent $AssemblyPath
    $openAiSharedPath = Join-Path $assemblyDir "OpenAI.Shared.dll"
    if (Test-Path $openAiSharedPath) {
        Write-Output "  * OpenAI.Shared:"
        Write-Output "    $($openAiSharedPath)"
        Write-Output ""
    }
    
    $openAiPath = Join-Path $assemblyDir "OpenAI.dll"
    if (Test-Path $openAiPath) {
        Write-Output "  * OpenAI:"
        Write-Output "    $($openAiPath)"
        Write-Output ""
    }

    Write-Output "  NOTE: If any of the above are empty, tool output may be inaccurate."
    Write-Output ""

    # Build genapi command arguments, excluding null references
    $genapiArgs = @(
        "--assembly", $AssemblyPath
        "--output-path", $Destination
    )
    
    if ($netRef) { $genapiArgs += @("--assembly-reference", $netRef) }
    if ($systemClientModelRef) { $genapiArgs += @("--assembly-reference", $systemClientModelRef) }
    if ($systemNetServerSentEventsRef) { $genapiArgs += @("--assembly-reference", $systemNetServerSentEventsRef) }
    if ($microsoftExtensionsLoggingAbstractionsRef) { $genapiArgs += @("--assembly-reference", $microsoftExtensionsLoggingAbstractionsRef) }
    if ($microsoftExtensionsDependencyInjectionAbstractionsRef) { $genapiArgs += @("--assembly-reference", $microsoftExtensionsDependencyInjectionAbstractionsRef) }
    if ($systemMemoryDataRef) { $genapiArgs += @("--assembly-reference", $systemMemoryDataRef) }
    if ($systemDiagnosticsDiagnosticSourceRef) { $genapiArgs += @("--assembly-reference", $systemDiagnosticsDiagnosticSourceRef) }
    if ($microsoftBclAsyncInterfacesRef) { $genapiArgs += @("--assembly-reference", $microsoftBclAsyncInterfacesRef) }
    
    # Add sibling assemblies as references if they differ from the main assembly
    if ((Test-Path $openAiSharedPath) -and ($AssemblyPath -ne $openAiSharedPath)) { 
        $genapiArgs += @("--assembly-reference", $openAiSharedPath) 
    }
    if ((Test-Path $openAiPath) -and ($AssemblyPath -ne $openAiPath)) { 
        $genapiArgs += @("--assembly-reference", $openAiPath) 
    }

    & genapi @genapiArgs

    Write-Output "Cleaning up $($Destination)..."
    Write-Output ""

    $content = Get-Content $Destination -Raw

    # Remove empty lines.
    $content = $content -creplace '//.*\r?\n', ''
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
    $content = $content -creplace "new\[\];", "new Type[0]"
    $content = $content -creplace "Diagnostics.CodeAnalysis.Experimental", "Experimental"
    $content = $content -creplace "Diagnostics.CodeAnalysis.SetsRequiredMembers", "SetsRequiredMembers"

    # Manually back-fill OpenAIClientOptions from OpenAI.Shared if we are generating OpenAI
    if ($AssemblyPath -match "OpenAI.dll$") {
        $sharedApiPath = $Destination -replace "OpenAI\.","OpenAI.Shared."
        if (Test-Path $sharedApiPath) {
             $content = $content -replace '\[assembly: Runtime.CompilerServices.TypeForwardedTo\(typeof\(OpenAI.OpenAIClientOptions\)\)\]\r?\n?', ''
             $sharedContent = Get-Content $sharedApiPath -Raw
             if ($sharedContent -match '(?ms)(    public class OpenAIClientOptions : ClientPipelineOptions \{.*?\n    \})') {
                 $optionsClass = $matches[1]
                 $content = $content -replace 'namespace OpenAI \{', "namespace OpenAI {`n$optionsClass"
             }
        }
    }

    Set-Content -Path $Destination -Value $content -NoNewline
}

$repoRootPath = Join-Path $PSScriptRoot .. -Resolve
$solutionPath = Join-Path $repoRootPath "OpenAI.sln"

Invoke-DotNetBuild -ProjectPath $solutionPath

$projects = @(
    @{
        Name = "OpenAI.Shared"
        AssemblyPath = "src\Shared\bin\Debug"
    },
    @{
        Name = "OpenAI"
        AssemblyPath = "src\bin\Debug"
    },
    @{
        Name = "OpenAI.Responses"
        AssemblyPath = "src\Responses\bin\Debug"
    }
)

foreach ($project in $projects) {
    foreach ($targetFramework in @("netstandard2.0", "net8.0")) {
        $assemblyPath = Join-Path $repoRootPath "$($project.AssemblyPath)\$($targetFramework)\$($project.Name).dll"
        $destination = Join-Path $repoRootPath "api\$($project.Name).$($targetFramework).cs"
        
        if (Test-Path $assemblyPath) {
            Invoke-GenAPI -TargetFramework $targetFramework -AssemblyPath $assemblyPath -Destination $destination
        } else {
            Write-Warning "Assembly not found: $assemblyPath"
        }
    }
}
