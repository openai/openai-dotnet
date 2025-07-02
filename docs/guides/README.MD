# OpenAI with .NET 10 - Getting Started Guide
This readme shows you how to run each OpenAI based sample (.cs) file in this folder directly without a project or additional setup using the latest .NET 10 Preview features. 

## Prerequisites

### 1. Install .NET 10 Preview


#### Option A: Using Windows Package Manager (winget) - Recommended
```powershell
# Install .NET 10 SDK Preview
winget install Microsoft.DotNet.SDK.Preview
```

#### Option B: Manual Download
1. Visit the [.NET 10 Download Page](https://dotnet.microsoft.com/download/dotnet/10.0)
2. Download and install: **.NET SDK 10.0 Preview** (required for development and `dotnet run`)

### 2. Verify Installation

After installation, verify you have the correct versions:

```powershell
# Check installed SDKs
dotnet --list-sdks

# Check version from the guides directory (should show 10.x)
cd docs/guides
dotnet --version
```

You should see output similar to:
```
10.0.100-preview.5.25277.114
```

## Setup

### 1. Clone the Repository
```powershell
git clone https://github.com/openai/openai-dotnet.git
cd openai-dotnet
```

### 2. Set Your OpenAI API Key

You need an OpenAI API key to run the samples. Get one from [OpenAI's API platform](https://platform.openai.com/api-keys).

#### Temporary (Current Session Only)
```powershell
$env:OPENAI_API_KEY = "your-api-key-here"
```

#### Permanent Options

**Option A: Using System Properties (GUI)**
1. Press `Win + R`, type `sysdm.cpl`, press Enter
2. Click "Environment Variables"
3. Under "User variables", click "New"
4. Variable name: `OPENAI_API_KEY`
5. Variable value: Your API key

**Option B: Using PowerShell (Permanent)**
```powershell
[Environment]::SetEnvironmentVariable("OPENAI_API_KEY", "your-api-key-here", "User")
```

**Option C: Using Command Prompt as Administrator**
```cmd
setx OPENAI_API_KEY "your-api-key-here"
```

### 3. Verify Environment Variable
```powershell
echo $env:OPENAI_API_KEY
```

## Running the Samples

The samples use .NET 10's new single-file application feature. Each `.cs` file in the guides folder is a standalone application.

### Navigate to the Guides Directory
```powershell
cd docs/guides
```

### Run a Sample
```powershell
# Example: Run the simple chat prompt sample
dotnet run text/chat/chat_simpleprompt.cs

# Run other samples
dotnet run text/chat/chat_instructions.cs
dotnet run text/chat/chat_roles.cs
```

### Expected Output
When you run `chat_simpleprompt.cs`, you should see output similar to:
```
Under a velvet-purple sky, a gentle unicorn named Luna sprinkled stardust over the dreaming forest, filling every heart with peaceful, magical dreams.
```

## Sample File Structure

The samples are organized as follows:
```
docs/
├── guides/
│   ├── global.json          # Specifies .NET 10 preview SDK
│   ├── README.MD            # Basic usage instructions
│   └── text/
│       ├── chat/
│       │   ├── chat_simpleprompt.cs    # Basic chat completion
│       │   ├── chat_instructions.cs    # Chat with system instructions
│       │   └── chat_roles.cs          # Chat with different roles
│       └── responses/
│           └── ...          # Response handling samples
```

## Understanding the Single-File Format

Each sample file contains special directives at the top:

```csharp
// SAMPLE: Description of what this sample does
#:package OpenAI@2.2.*-*        // NuGet package reference
#:property PublishAot false      // Build properties

using OpenAI.Chat;              // Regular C# code follows

// Your application code here...
```

## Troubleshooting

### Problem: "No package found matching input criteria"
- **Solution**: The .NET 10 preview packages might not be available yet. Try installing from the official Microsoft download page instead.

### Problem: `dotnet --version` shows 9.x instead of 10.x
- **Solution**: You need to install the .NET 10 **SDK** (not just the runtime). The `global.json` file in the guides directory requires the SDK.

### Problem: "Couldn't find a project to run"
- **Solution**: Make sure you're running the command from the `docs/guides` directory and providing the correct path to the `.cs` file.

### Problem: "The property directive needs to have two parts"
- **Solution**: The property directive format should be `#:property PropertyName PropertyValue` (space-separated, not equals sign).

### Problem: API errors
- **Solution**: 
  - Verify your `OPENAI_API_KEY` environment variable is set correctly
  - Check that your API key is valid and has sufficient credits
  - Ensure you're using a valid model name (e.g., "gpt-4", "gpt-3.5-turbo")

### Problem: Build errors about missing packages
- **Solution**: The package directives should automatically download dependencies. If not, try:
  ```powershell
  dotnet restore
  ```

## Additional Resources

- [OpenAI .NET SDK Documentation](https://github.com/openai/openai-dotnet)
- [.NET 10 Preview Documentation](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [Single-File Applications in .NET 10](https://devblogs.microsoft.com/dotnet/announcing-dotnet-run-app/)

## Next Steps

Once you have the basic samples working, you can:

1. **Explore other samples** in the `text/` directory
2. **Modify the prompts** in the sample files to experiment with different outputs
3. **Create your own samples** following the same single-file format
4. **Integrate the OpenAI SDK** into your own .NET applications

Happy coding with OpenAI and .NET 10! 🚀
