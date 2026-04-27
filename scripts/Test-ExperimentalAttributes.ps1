<#
.SYNOPSIS
    Validates [Experimental] attribute decoration on public APIs in stable classes.

.DESCRIPTION
    Uses assembly reflection to verify that all public methods and properties in stable classes
    are either listed in the stable sets (from api/ga-apis.yaml) or decorated
    with [Experimental]. This catches custom (hand-written) code that is not processed by the
    code generator's visitor.

.PARAMETER Configuration
    Build configuration. Defaults to Release.

.PARAMETER TargetFramework
    Target framework to build and validate. Defaults to the first framework in ClientTargetFrameworks.

.EXAMPLE
    .\Test-ExperimentalAttributes.ps1
    Validates using default settings.

.EXAMPLE
    .\Test-ExperimentalAttributes.ps1 -Configuration Debug -TargetFramework net8.0
    Validates using Debug configuration targeting net8.0.
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$Configuration = "Release",

    [Parameter(Mandatory = $false)]
    [string]$TargetFramework
)

$ErrorActionPreference = "Stop"

$repoRootPath = Join-Path $PSScriptRoot ".." -Resolve
$projectPath = Join-Path $repoRootPath "OpenAI" "src" "OpenAI.csproj"
$yamlPath = Join-Path $repoRootPath "api" "ga-apis.yaml"

# Auto-detect target framework: pick the highest TFM that is compatible with the
# current PowerShell/.NET runtime so that the assembly can be loaded for reflection.
if (-not $TargetFramework) {
    $runtimeMajor = [System.Environment]::Version.Major
    $propsPath = Join-Path $repoRootPath "Directory.Build.props"
    if (Test-Path $propsPath) {
        $propsContent = Get-Content $propsPath -Raw
        if ($propsContent -match '<ClientTargetFrameworks>([^<]+)</ClientTargetFrameworks>') {
            $allFrameworks = ($Matches[1] -split ';') | ForEach-Object { $_.Trim() }

            # Filter to netX.0 frameworks whose major version <= current runtime
            $compatible = $allFrameworks | Where-Object {
                $_ -match '^net(\d+)\.0$' -and [int]$Matches[1] -le $runtimeMajor
            } | Sort-Object { if ($_ -match '(\d+)') { [int]$Matches[1] } } -Descending

            $TargetFramework = $compatible | Select-Object -First 1
        }
    }
    if (-not $TargetFramework) {
        $TargetFramework = "net8.0"
    }
}

Write-Host ""
Write-Host "Experimental Attribute Validation" -ForegroundColor Cyan
Write-Host "  Configuration:    $Configuration" -ForegroundColor DarkGray
Write-Host "  Target Framework: $TargetFramework" -ForegroundColor DarkGray
Write-Host ""

# ---------------------------------------------------------------------------
# Step 1: Parse stable sets from api/ga-apis.yaml
# ---------------------------------------------------------------------------

Write-Host "Parsing stable sets from api/ga-apis.yaml..." -ForegroundColor Cyan

if (-not (Test-Path $yamlPath)) {
    Write-Error "ga-apis.yaml not found at: $yamlPath"
    exit 1
}

$yamlLines = Get-Content $yamlPath

$stableClasses = @()
$stableProperties = @()
$stableMethods = @()

$currentSet = $null
foreach ($line in $yamlLines) {
    $trimmed = $line.Trim()
    if ($trimmed.Length -eq 0 -or $trimmed.StartsWith('#')) { continue }

    if ($trimmed -eq 'stableClasses:') { $currentSet = 'classes'; continue }
    if ($trimmed -eq 'stableProperties:') { $currentSet = 'properties'; continue }
    if ($trimmed -eq 'stableMethods:') { $currentSet = 'methods'; continue }

    if ($trimmed.StartsWith('- ') -and $currentSet) {
        $value = $trimmed.Substring(2).Trim()
        switch ($currentSet) {
            'classes'    { $stableClasses += $value }
            'properties' { $stableProperties += $value }
            'methods'    { $stableMethods += $value }
        }
    }
}

Write-Host "  Stable classes:    $($stableClasses.Count)" -ForegroundColor DarkGray
Write-Host "  Stable properties: $($stableProperties.Count)" -ForegroundColor DarkGray
Write-Host "  Stable methods:    $($stableMethods.Count)" -ForegroundColor DarkGray
Write-Host ""

# ---------------------------------------------------------------------------
# Step 2: Publish the project to a temp directory so all deps are co-located
# ---------------------------------------------------------------------------

$publishDir = $null
$violations = @()

$validatorCode = @'
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

public class ExperimentalAttributeValidator
{
    private class ValidatorLoadContext : AssemblyLoadContext
    {
        private readonly string _basePath;

        public ValidatorLoadContext(string basePath) : base("ExperimentalValidator", isCollectible: true)
        {
            _basePath = basePath;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string path = Path.Combine(_basePath, assemblyName.Name + ".dll");
            if (File.Exists(path))
                return LoadFromAssemblyPath(path);
            return null;
        }
    }

    private static readonly Dictionary<string, string> TypeAliases = new(StringComparer.Ordinal)
    {
        { "Boolean", "bool" },
        { "Byte", "byte" },
        { "Char", "char" },
        { "Decimal", "decimal" },
        { "Double", "double" },
        { "Int16", "short" },
        { "Int32", "int" },
        { "Int64", "long" },
        { "Object", "object" },
        { "SByte", "sbyte" },
        { "Single", "float" },
        { "String", "string" },
        { "UInt16", "ushort" },
        { "UInt32", "uint" },
        { "UInt64", "ulong" },
        { "Void", "void" },
    };

    public static string GetFriendlyTypeName(Type type)
    {
        if (type.IsByRef)
            return GetFriendlyTypeName(type.GetElementType());

        if (type.IsArray)
            return GetFriendlyTypeName(type.GetElementType()) + "[]";

        if (type.IsGenericType)
        {
            Type underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                return GetFriendlyTypeName(underlying) + "?";

            string baseName = type.Name.Substring(0, type.Name.IndexOf('`'));
            string args = string.Join(", ", type.GetGenericArguments().Select(GetFriendlyTypeName));
            return baseName + "<" + args + ">";
        }

        if (TypeAliases.TryGetValue(type.Name, out var alias))
            return alias;

        return type.Name;
    }

    public static string GetMethodLookupName(MethodInfo method, string typeName)
    {
        var paramTypes = method.GetParameters()
            .Select(p => GetFriendlyTypeName(p.ParameterType))
            .ToArray();

        string name;
        switch (method.Name)
        {
            case "op_Implicit":
                name = "operator implicit " + typeName;
                break;
            case "op_Explicit":
                name = "operator explicit " + typeName;
                break;
            case "op_Equality":
                name = "operator ==";
                break;
            case "op_Inequality":
                name = "operator !=";
                break;
            case "op_GreaterThan":
                name = "operator >";
                break;
            case "op_LessThan":
                name = "operator <";
                break;
            case "op_GreaterThanOrEqual":
                name = "operator >=";
                break;
            case "op_LessThanOrEqual":
                name = "operator <=";
                break;
            case "op_Addition":
                name = "operator +";
                break;
            case "op_Subtraction":
                name = "operator -";
                break;
            default:
                name = method.Name;
                break;
        }

        if (paramTypes.Length > 0)
            return typeName + "." + name + "|" + string.Join("|", paramTypes);
        return typeName + "." + name;
    }

    public static bool HasExperimentalAttribute(MemberInfo member)
    {
        try
        {
            return member.GetCustomAttributesData()
                .Any(a => a.AttributeType.Name == "ExperimentalAttribute");
        }
        catch
        {
            return true;
        }
    }

    public static string[] Validate(
        string dllPath,
        string[] stableClasses,
        string[] stableProperties,
        string[] stableMethods)
    {
        var stableClassSet = new HashSet<string>(stableClasses, StringComparer.OrdinalIgnoreCase);
        var stablePropSet = new HashSet<string>(stableProperties, StringComparer.OrdinalIgnoreCase);
        var stableMethodSet = new HashSet<string>(stableMethods, StringComparer.OrdinalIgnoreCase);

        var violations = new List<string>();
        var basePath = Path.GetDirectoryName(Path.GetFullPath(dllPath));

        var ctx = new ValidatorLoadContext(basePath);
        Assembly assembly = null;
        Type[] types = Array.Empty<Type>();
        try
        {
            try
            {
                assembly = ctx.LoadFromAssemblyPath(Path.GetFullPath(dllPath));
            }
            catch (Exception ex)
            {
                return new[] { "ERROR|Assembly load failed|" + ex.Message + "|" };
            }

            var bindingFlags = BindingFlags.Public | BindingFlags.Instance |
                               BindingFlags.Static | BindingFlags.DeclaredOnly;

            try
            {
                types = assembly.GetExportedTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var type in types)
            {
                string fullName = type.Namespace + "." + type.Name;
                if (!stableClassSet.Contains(fullName))
                    continue;

                string typeName = type.Name;

                // Validate properties
                try
                {
                    foreach (var prop in type.GetProperties(bindingFlags))
                    {
                        string lookupName = typeName + "." + prop.Name;
                        if (!stablePropSet.Contains(lookupName) && !HasExperimentalAttribute(prop))
                        {
                            violations.Add("PROPERTY|" + fullName + "|" + prop.Name + "|" + lookupName);
                        }
                    }
                }
                catch { /* skip types whose properties can't be reflected */ }

                // Validate methods (skip property/event accessors)
                try
                {
                    foreach (var method in type.GetMethods(bindingFlags))
                    {
                        if (method.IsSpecialName &&
                            (method.Name.StartsWith("get_") || method.Name.StartsWith("set_") ||
                             method.Name.StartsWith("add_") || method.Name.StartsWith("remove_")))
                            continue;

                        string lookupName = GetMethodLookupName(method, typeName);
                        if (!stableMethodSet.Contains(lookupName) && !HasExperimentalAttribute(method))
                        {
                            var paramDesc = string.Join(", ",
                                method.GetParameters().Select(p => GetFriendlyTypeName(p.ParameterType)));
                            violations.Add("METHOD|" + fullName + "|" +
                                method.Name + "(" + paramDesc + ")|" + lookupName);
                        }
                    }
                }
                catch { /* skip types whose methods can't be reflected */ }
            }

            return violations.ToArray();
        }
        finally
        {
            types = Array.Empty<Type>();
            assembly = null;
            ctx.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
'@

try {
    $publishDir = Join-Path ([System.IO.Path]::GetTempPath()) "openai-experimental-validation-$([System.IO.Path]::GetRandomFileName())"

    Write-Host "Publishing project ($TargetFramework)..." -ForegroundColor Cyan

    & dotnet publish $projectPath -c $Configuration -f $TargetFramework -o $publishDir --nologo -v quiet 2>&1 | Out-Null

    if ($LASTEXITCODE -ne 0) {
        & dotnet publish $projectPath -c $Configuration -f $TargetFramework -o $publishDir --nologo
        throw "Build/publish failed with exit code $LASTEXITCODE"
    }

    $dllPath = Join-Path $publishDir "OpenAI.dll"
    if (-not (Test-Path $dllPath)) {
        throw "Expected assembly not found at: $dllPath"
    }

    Write-Host ""

    # ---------------------------------------------------------------------------
    # Step 3: Compile and run the reflection-based validator
    # ---------------------------------------------------------------------------

    Write-Host "Loading assembly and validating..." -ForegroundColor Cyan

    $validatorType = [AppDomain]::CurrentDomain.GetAssemblies() |
        ForEach-Object { $_.GetType('ExperimentalAttributeValidator', $false, $false) } |
        Where-Object { $null -ne $_ } |
        Select-Object -First 1

    if ($null -eq $validatorType) {
        Add-Type -TypeDefinition $validatorCode -WarningAction SilentlyContinue
    }

    # ---------------------------------------------------------------------------
    # Step 4: Run validation
    # ---------------------------------------------------------------------------

    $violations = [ExperimentalAttributeValidator]::Validate(
            $dllPath,
            [string[]]$stableClasses,
            [string[]]$stableProperties,
            [string[]]$stableMethods
        )
}
catch {
    Write-Error $_
    exit 1
}
finally {
    if ($publishDir -and (Test-Path $publishDir)) {
        Remove-Item -Path $publishDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

# ---------------------------------------------------------------------------
# Step 5: Report results
# ---------------------------------------------------------------------------

if ($violations.Count -eq 0) {
    Write-Host ""
    Write-Host "All public APIs in stable classes are correctly attributed." -ForegroundColor Green
    Write-Host ""
    exit 0
}

Write-Host ""
Write-Host "Found $($violations.Count) violation(s) - public members in stable classes missing [Experimental] attribute:" -ForegroundColor Red
Write-Host ""

foreach ($v in $violations) {
    $parts = $v -split '\|', 4
    $kind = $parts[0]
    $class = $parts[1]
    $member = $parts[2]
    $lookup = $parts[3]

    Write-Host "  [$kind] $class :: $member" -ForegroundColor Yellow
    Write-Host "    Lookup key: $lookup" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "To fix: add [Experimental(""OPENAI001"")] to these members in custom code," -ForegroundColor Red
Write-Host "or add them to the stable sets in api/ga-apis.yaml." -ForegroundColor Red
Write-Host ""
exit 1
