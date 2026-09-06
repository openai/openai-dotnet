# API Listings

The `.cs` files in the target framework subdirectories are **auto-generated** API listings and must not be modified manually.

To regenerate them, run:

```powershell
./scripts/Export-Api.ps1
```

The most common reason to regenerate the API listings is to reflect a change in the public API surface (such as adding, removing, or renaming public types or members). In that case, modify the relevant source files first and then re-run the script above.

Note that the API listings under `api/released/` reflect the latest released public API surface. See `api-version.txt` for the corresponding release version.

API listings are split by target framework and namespace. For example, the `net10.0` listings are written to `api/in-progress/net10.0/` as files such as `OpenAI.net10.0.cs`, `OpenAI.Chat.net10.0.cs`, and `OpenAI.Responses.net10.0.cs`.
