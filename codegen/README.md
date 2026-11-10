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

## Extensible model hierarchies

`ExtensibleModelVisitor` exposes public, get-only string-backed extensible discriminators and
protected-internal initialization constructors in explicitly opted-in namespaces. Namespace opt-in
is separate from `ProtocolModelVisitor`, and explicit visibility customizations retain precedence.

A model can fix an inherited discriminator and still introduce an open discriminator for a nested
hierarchy. For example, a message base can fix an item's kind to `message` while accepting
consumer-defined roles. The visitor exposes the new role without changing the fixed item kind,
serialization constructors, or concrete leaves. Discriminator lookup checks each declaring model's
properties and constructor metadata before searching its ancestors, including when a handwritten
property suppresses the generated member.
Constructor forwarding targets that discriminator's parameter, not the first discriminator in a
multi-level serialization signature, and replaces existing inherited arguments instead of duplicating them.