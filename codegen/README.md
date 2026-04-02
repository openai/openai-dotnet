# Debugging the generator

To configure VS Code for debugging the generator, specifically visitors, add the following to your `launch.json` in the root of the workspace

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Debug OpenAI Library Plugin",
            "type": "coreclr",
            "request": "launch",
            "program": "dotnet",
            "args": [
                "${workspaceFolder}/codegen/dist/generator/Microsoft.TypeSpec.Generator.dll",
                "${workspaceFolder}",
                "-g",
                "OpenAILibraryGenerator"
            ],
        }
    ]
}
```