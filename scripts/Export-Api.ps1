<#
.SYNOPSIS
    Cross-platform API export script for OpenAI .NET SDK.

.DESCRIPTION
    This script supports Windows, macOS, and Linux environments. It automatically detects the current platform
    and uses the appropriate paths for .NET and NuGet packages.
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
    $systemClientModelPath = Join-Path $nugetPackagesPath "system.clientmodel\1.4.2"
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
    $systemMemoryDataPath = Join-Path $nugetPackagesPath "system.memory.data\6.0.1"
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
        $systemDiagnosticsDiagnosticSourcePath = Join-Path $nugetPackagesPath "system.diagnostics.diagnosticsource\6.0.1"
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
        $microsoftBclAsyncInterfacesPath = Join-Path $nugetPackagesPath "microsoft.bcl.asyncinterfaces\1.1.0"
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

    Write-Output "  NOTE: If any of the above are empty, tool output may be inaccurate."
    Write-Output ""

    # Build genapi command arguments, excluding null references
    $genapiArgs = @(
        "--assembly", $AssemblyPath
        "--output-path", $Destination
    )
    
    if ($netRef) { $genapiArgs += @("--assembly-reference", $netRef) }
    if ($systemClientModelRef) { $genapiArgs += @("--assembly-reference", $systemClientModelRef) }
    if ($microsoftExtensionsLoggingAbstractionsRef) { $genapiArgs += @("--assembly-reference", $microsoftExtensionsLoggingAbstractionsRef) }
    if ($microsoftExtensionsDependencyInjectionAbstractionsRef) { $genapiArgs += @("--assembly-reference", $microsoftExtensionsDependencyInjectionAbstractionsRef) }
    if ($systemMemoryDataRef) { $genapiArgs += @("--assembly-reference", $systemMemoryDataRef) }
    if ($systemDiagnosticsDiagnosticSourceRef) { $genapiArgs += @("--assembly-reference", $systemDiagnosticsDiagnosticSourceRef) }
    if ($microsoftBclAsyncInterfacesRef) { $genapiArgs += @("--assembly-reference", $microsoftBclAsyncInterfacesRef) }

    & genapi @genapiArgs

    Write-Output "Cleaning up $($Destination)..."
    Write-Output ""

    $content = Get-Content $Destination -Raw

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

    # Remove Diagnostics.DebuggerStepThrough attribute.
    $content = $content -creplace ".*Diagnostics.DebuggerStepThrough.*\n", ""

    # Remove ModelReaderWriterBuildable attributes.
    $content = $content -creplace '\[ModelReaderWriterBuildable\(typeof\([^\)]+\)\)\]\s*', ''

    # Remove internal APIs.
    $content = $content -creplace "  * internal.*`n", ""

    # Remove IJsonModel/IPersistableModel interface method entries.
    $content = $content -creplace "        .*(IJsonModel|IPersistableModel).*`n", ""
    # $content = $content -creplace "        protected (virtual|override) .* (Json|Persistable)Model(Create|Write)Core.*`n", ""

    # Other cosmetic simplifications.
    $content = $content -creplace "partial class", "class"
    $content = $content -creplace ".*private.*dummy.*`n", ""
    $content = $content -creplace " { throw null; }", ";"
    $content = $content -creplace " { }", ";"
    $content = $content -creplace "Diagnostics.CodeAnalysis.Experimental", "Experimental"
    $content = $content -creplace "Diagnostics.CodeAnalysis.SetsRequiredMembers", "SetsRequiredMembers"

    Set-Content -Path $Destination -Value $content -NoNewline
}

$repoRootPath = Join-Path $PSScriptRoot .. -Resolve
$projectPath = Join-Path $repoRootPath "src\OpenAI.csproj"

Invoke-DotNetBuild -ProjectPath $projectPath

$targetFramework = "netstandard2.0"
$assemblyPath = Join-Path $repoRootPath "src\bin\Debug\$($targetFramework)\OpenAI.dll"
$destination = Join-Path $repoRootPath "api\OpenAI.$($targetFramework).cs"
Invoke-GenAPI -TargetFramework $targetFramework -AssemblyPath $assemblyPath -Destination $destination

$targetFramework = "net8.0"
$assemblyPath = Join-Path $repoRootPath "src\bin\Debug\$($targetFramework)\OpenAI.dll"
$destination = Join-Path $repoRootPath "api\OpenAI.$($targetFramework).cs"
Invoke-GenAPI -TargetFramework $targetFramework -AssemblyPath $assemblyPath -Destination $destination