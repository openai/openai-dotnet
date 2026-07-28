# API Listings

The `.cs` files in the target framework subdirectories are **auto-generated** and must not be modified manually.

To regenerate them, run:

```powershell
./scripts/Export-Api.ps1
```

The most common reason to regenerate these files is a change to the public API surface (adding, removing, or renaming public types or members). In that case, modify the relevant source files first and then re-run the script above.

API listings are split by target framework and namespace. For example, the `net10.0` listings are written to `api/net10.0/` as files such as `OpenAI.net10.0.cs`, `OpenAI.Assistants.net10.0.cs`, and `OpenAI.Chat.net10.0.cs`.
