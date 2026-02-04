<#
.SYNOPSIS
    Cross-platform API export script for OpenAI .NET SDK.

.DESCRIPTION
    This script supports Windows, macOS, and Linux environments. It automatically detects the current platform
    and uses the appropriate paths for .NET and NuGet packages.
    
    Generates API files for both OpenAI and OpenAI.Responses assemblies.
#>

function Invoke-DotNetBuild {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )

    Write-Output "Building $($ProjectPath)..."
    Write-Output ""
    & dotnet build $ProjectPath
    Write-Output ""
}

function Invoke-GenAPI {
    param (
        [Parameter(Mandatory = $true)]
        [string]$TargetFramework,

        [Parameter(Mandatory = $true)]
        [string]$AssemblyPath,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    Write-Output "Generating $($Destination)..."
    Write-Output ""

    Write-Output "  Detected platform paths:"
    Write-Output ""

    # Set platform-specific paths using PowerShell automatic variables (PowerShell 6.0+)
    # Fall back to manual detection for older PowerShell versions
    $isWindowsPlatform = $false
    $isMacOSPlatform = $false
    
    if (Get-Variable -Name "IsWindows" -ErrorAction SilentlyContinue) {
        $isWindowsPlatform = $IsWindows
        $isMacOSPlatform = $IsMacOS
    } else {
        # Fallback for Windows PowerShell 5.1 and earlier
        $isWindowsPlatform = [System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT
        $isMacOSPlatform = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform([System.Runtime.InteropServices.OSPlatform]::OSX)
    }

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

    # Determine target framework folder name for reference assemblies
    $netRefFolder = if ($TargetFramework -eq "net10.0") { "net10.0" } elseif ($TargetFramework -eq "net8.0") { "net8.0" } else { "net8.0" }

    # .NET
    $netRef = $null
    if (Test-Path $dotnetPacksPath) {
        $netRef = Get-ChildItem -Recurse `
            -Path $dotnetPacksPath `
            -Include $netRefFolder | Select-Object -Last 1
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
                $netRef = Get-ChildItem -Recurse -Path $altPath -Include $netRefFolder | Select-Object -Last 1
                if ($netRef) { break }
            }
        }
    }

    Write-Output "  * .NET:"
    Write-Output "    $($netRef)"
    Write-Output ""

    # Determine NuGet package target framework folder
    $nugetTfm = if ($TargetFramework -eq "netstandard2.0") { "netstandard2.0" } else { "net8.0" }

    # System.ClientModel
    $systemClientModelPath = Join-Path $nugetPackagesPath "system.clientmodel"
    $systemClientModelRef = $null
    if (Test-Path $systemClientModelPath) {
        $latestVersion = Get-ChildItem -Path $systemClientModelPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestVersion) {
            $systemClientModelRef = Get-ChildItem `
                -Path $latestVersion.FullName `
                -Include $nugetTfm `
                -Recurse |
                    Select-Object -Last 1
        }
    }

    Write-Output "  * System.ClientModel:"
    Write-Output "    $($systemClientModelRef)"
    Write-Output ""

    # System.Net.ServerSentEvents
    $systemNetServerSentEventsPath = Join-Path $nugetPackagesPath "system.net.serversentevents"
    $systemNetServerSentEventsRef = $null
    if (Test-Path $systemNetServerSentEventsPath) {
        $latestVersion = Get-ChildItem -Path $systemNetServerSentEventsPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestVersion) {
            $systemNetServerSentEventsRef = Get-ChildItem `
                -Path $latestVersion.FullName `
                -Include $nugetTfm `
                -Recurse |
                    Select-Object -Last 1
        }
    }

    Write-Output "  * System.Net.ServerSentEvents:"
    Write-Output "    $($systemNetServerSentEventsRef)"
    Write-Output ""

    # Microsoft.Extensions.Logging.Abstractions
    $microsoftExtensionsLoggingAbstractionsPath = Join-Path $nugetPackagesPath "microsoft.extensions.logging.abstractions"
    $microsoftExtensionsLoggingAbstractionsRef = $null
    if (Test-Path $microsoftExtensionsLoggingAbstractionsPath) {
        $latestVersion = Get-ChildItem -Path $microsoftExtensionsLoggingAbstractionsPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestVersion) {
            $microsoftExtensionsLoggingAbstractionsRef = Get-ChildItem `
                -Path $latestVersion.FullName `
                -Include $nugetTfm `
                -Recurse |
                    Select-Object -Last 1
        }
    }

    Write-Output "  * Microsoft.Extensions.Logging.Abstractions:"
    Write-Output "    $($microsoftExtensionsLoggingAbstractionsRef)"
    Write-Output ""

    # Microsoft.Extensions.DependencyInjection.Abstractions
    $microsoftExtensionsDependencyInjectionAbstractionsPath = Join-Path $nugetPackagesPath "microsoft.extensions.dependencyinjection.abstractions"
    $microsoftExtensionsDependencyInjectionAbstractionsRef = $null
    if (Test-Path $microsoftExtensionsDependencyInjectionAbstractionsPath) {
        $latestVersion = Get-ChildItem -Path $microsoftExtensionsDependencyInjectionAbstractionsPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestVersion) {
            $microsoftExtensionsDependencyInjectionAbstractionsRef = Get-ChildItem `
                -Path $latestVersion.FullName `
                -Include $nugetTfm `
                -Recurse |
                    Select-Object -Last 1
        }
    }

    Write-Output "  * Microsoft.Extensions.DependencyInjection.Abstractions:"
    Write-Output "    $($microsoftExtensionsDependencyInjectionAbstractionsRef)"
    Write-Output ""

    # System.Memory.Data
    $systemMemoryDataPath = Join-Path $nugetPackagesPath "system.memory.data"
    $systemMemoryDataRef = $null
    if (Test-Path $systemMemoryDataPath) {
        $latestVersion = Get-ChildItem -Path $systemMemoryDataPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($latestVersion) {
            $memoryDataTfm = if ($TargetFramework -eq "netstandard2.0") { "netstandard2.0" } else { "net6.0" }
            $systemMemoryDataRef = Get-ChildItem `
                -Path $latestVersion.FullName `
                -Include $memoryDataTfm `
                -Recurse |
                    Select-Object -Last 1
        }
    }

    Write-Output "  * System.Memory.Data:"
    Write-Output "    $($systemMemoryDataRef)"
    Write-Output ""

    $systemDiagnosticsDiagnosticSourceRef = $null
    $microsoftBclAsyncInterfacesRef = $null

    if ($TargetFramework -eq "netstandard2.0") {
        # System.Diagnostics.DiagnosticSource
        $systemDiagnosticsDiagnosticSourcePath = Join-Path $nugetPackagesPath "system.diagnostics.diagnosticsource"
        if (Test-Path $systemDiagnosticsDiagnosticSourcePath) {
            $latestVersion = Get-ChildItem -Path $systemDiagnosticsDiagnosticSourcePath -Directory | Sort-Object Name -Descending | Select-Object -First 1
            if ($latestVersion) {
                $systemDiagnosticsDiagnosticSourceRef = Get-ChildItem `
                    -Path $latestVersion.FullName `
                    -Include "netstandard2.0" `
                    -Recurse |
                        Select-Object -Last 1
            }
        }

        Write-Output "  * System.Diagnostics.DiagnosticSource:"
        Write-Output "    $($systemDiagnosticsDiagnosticSourceRef)"
        Write-Output ""

        # Microsoft.Bcl.AsyncInterfaces
        $microsoftBclAsyncInterfacesPath = Join-Path $nugetPackagesPath "microsoft.bcl.asyncinterfaces"
        if (Test-Path $microsoftBclAsyncInterfacesPath) {
            $latestVersion = Get-ChildItem -Path $microsoftBclAsyncInterfacesPath -Directory | Sort-Object Name -Descending | Select-Object -First 1
            if ($latestVersion) {
                $microsoftBclAsyncInterfacesRef = Get-ChildItem `
                    -Path $latestVersion.FullName `
                    -Include "netstandard2.0" `
                    -Recurse |
                        Select-Object -Last 1
            }
        }

        Write-Output "  * Microsoft.Bcl.AsyncInterfaces:"
        Write-Output "    $($microsoftBclAsyncInterfacesRef)"
        Write-Output ""
    }

    # Internal project references (sibling assemblies)
    $assemblyDir = Split-Path -Parent $AssemblyPath
    
    $openAiPath = Join-Path $assemblyDir "OpenAI.dll"
    if (Test-Path $openAiPath) {
        Write-Output "  * OpenAI:"
        Write-Output "    $($openAiPath)"
        Write-Output ""
    }

    $openAiResponsesPath = Join-Path $assemblyDir "OpenAI.Responses.dll"
    if (Test-Path $openAiResponsesPath) {
        Write-Output "  * OpenAI.Responses:"
        Write-Output "    $($openAiResponsesPath)"
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
    if ((Test-Path $openAiPath) -and ($AssemblyPath -ne $openAiPath)) { 
        $genapiArgs += @("--assembly-reference", $openAiPath) 
    }
    if ((Test-Path $openAiResponsesPath) -and ($AssemblyPath -ne $openAiResponsesPath)) { 
        $genapiArgs += @("--assembly-reference", $openAiResponsesPath) 
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

    Set-Content -Path $Destination -Value $content -NoNewline
}

# Main execution
$repoRootPath = Join-Path $PSScriptRoot .. -Resolve

# Build the solution
$solutionPath = Join-Path $repoRootPath "OpenAI.slnx"
Invoke-DotNetBuild -ProjectPath $solutionPath

# Define projects and their assembly paths
$projects = @(
    @{
        Name = "OpenAI"
        AssemblyPath = "src/bin/Debug"
    },
    @{
        Name = "OpenAI.Responses"
        AssemblyPath = "src/Responses/bin/Debug"
    }
)

# Target frameworks should match ClientTargetFrameworks from Directory.Build.props
$targetFrameworks = @("netstandard2.0", "net8.0", "net10.0")

foreach ($project in $projects) {
    foreach ($targetFramework in $targetFrameworks) {
        $assemblyPath = Join-Path $repoRootPath "$($project.AssemblyPath)/$($targetFramework)/$($project.Name).dll"
        $destination = Join-Path $repoRootPath "api/$($project.Name).$($targetFramework).cs"
        
        if (Test-Path $assemblyPath) {
            Invoke-GenAPI -TargetFramework $targetFramework -AssemblyPath $assemblyPath -Destination $destination
        } else {
            Write-Warning "Assembly not found: $assemblyPath"
        }
    }
}

Write-Output ""
Write-Output "API export complete!"
