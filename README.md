# Intento

> **What do you intend to do today?**

Intento is a Windows productivity application that prepares your workspace around your intention. Choose an activity and Intento opens its configured applications, websites, and folders together.

## Product documentation

- [Persian product idea and MVP specification](docs/PRODUCT_SPEC_FA.md)
- [Technology stack and rationale](docs/STACK.md)
- [MVP backlog and delivery plan](docs/MVP_BACKLOG_FA.md)
- [Activity persistence and JSON format](docs/PERSISTENCE.md)

## MVP

The first runnable MVP is scaffolded as a **.NET 10 + WinUI 3** desktop application.

### Current MVP flow

1. Start Intento.
2. See the question **"What do you intend to do today?"**.
3. Choose **Job**, **Learning**, **Research**, or **Workout**.
4. Intento launches every configured item for that activity.
5. Activities are stored as JSON at `%LocalAppData%/Intento/activities.json`.

The default activities are intentionally empty so the MVP can be used without assuming which applications, URLs, or folders exist on a user's PC.

## Architecture

```text
Intento.App (WinUI 3)
        │
        ▼
Intento.Core (domain models)
        │
        ▼
Intento.Infrastructure
   ├── ActivityStore (safe JSON persistence)
   ├── IActivityFileSystem (testable file boundary)
   └── WorkspaceLauncher (Windows shell)
```

## Project Structure

```text
Intento/
├── Intento.sln
├── src/
│   ├── Intento.App/
│   ├── Intento.Core/
│   │   └── Models/Activity.cs
│   └── Intento.Infrastructure/
│       ├── ActivityStore.cs
│       ├── ActivityStoreException.cs
│       ├── IActivityFileSystem.cs
│       ├── PhysicalActivityFileSystem.cs
│       └── WorkspaceLauncher.cs
├── tests/
│   └── Intento.Infrastructure.Tests/
└── docs/
    └── PERSISTENCE.md
```

## Example Activity Configuration

The persisted JSON model supports application paths, URLs, and folders:

```json
[
  {
    "Id": "job-1",
    "Name": "Job",
    "Icon": "💼",
    "Items": [
      { "Name": "Excel", "Type": 0, "Target": "C:\\Program Files\\Microsoft Office\\root\\Office16\\EXCEL.EXE" },
      { "Name": "Company", "Type": 1, "Target": "https://example.com" },
      { "Name": "Project", "Type": 2, "Target": "C:\\Projects\\MyProject" }
    ]
  }
]
```

`Type` values are `0 = Application`, `1 = Url`, and `2 = Folder`.

## Development

Requirements:

- Windows 10 version 1809 (build 17763) or later
- Visual Studio 2022 with Windows App SDK / WinUI 3 development support
- .NET 10 SDK

Open `Intento.sln`, select the `x64` platform, and run the `Intento.App` project.

Run persistence tests with:

```powershell
dotnet test tests/Intento.Infrastructure.Tests/Intento.Infrastructure.Tests.csproj
```

The infrastructure and test projects target plain `net10.0`, so persistence tests do not require launching the WinUI app or touching the current user's configuration.

## Project Status

**MVP scaffold implemented.** Activity persistence now uses Local AppData, safe temporary-file replacement, controlled storage errors, and an injectable file-system boundary. Activity editing, Windows startup integration, richer launch reporting, packaging, and broader automated tests remain on the roadmap.

See [`TASKS.md`](docs/TASKS.md) for the implementation plan, [`STACK.md`](docs/STACK.md) for technology choices, and [`PERSISTENCE.md`](docs/PERSISTENCE.md) for storage behavior.

## Roadmap

- [x] Create initial activity model
- [x] Persist activities as JSON
- [x] Store JSON under Local AppData with safe replacement
- [x] Handle missing/corrupt files and storage errors
- [x] Add automated persistence tests
- [x] Select an activity from the WinUI interface
- [x] Launch applications, URLs, and folders
- [ ] Add activity editor
- [ ] Add item picker / file and folder selection
- [ ] Add Windows startup integration
- [ ] Add launch result/error reporting
- [ ] Package the application with MSIX

## License

A license has not yet been specified for this project.

---

**Intento — Start with your intention, not your setup.**
