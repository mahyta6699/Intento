# Intento

> **What do you intend to do today?**

Intento is a Windows productivity application that prepares your workspace around your intention. Choose an activity and Intento opens its configured applications, websites, and folders together.

## MVP

The first runnable MVP is now scaffolded as a **.NET 10 + WinUI 3** desktop application.

### Current MVP flow

1. Start Intento.
2. See the question **"What do you intend to do today?"**.
3. Choose **Job**, **Learning**, **Research**, or **Workout**.
4. Intento launches every configured item for that activity.
5. Activities are stored as JSON under the user's local application data.

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
   ├── ActivityStore (JSON)
   └── WorkspaceLauncher (Windows shell)
```

## Project Structure

```text
Intento/
├── Intento.sln
├── src/
│   ├── Intento.App/
│   │   ├── App.xaml
│   │   ├── App.xaml.cs
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   └── Intento.App.csproj
│   ├── Intento.Core/
│   │   ├── Models/Activity.cs
│   │   └── Intento.Core.csproj
│   └── Intento.Infrastructure/
│       ├── ActivityStore.cs
│       ├── WorkspaceLauncher.cs
│       └── Intento.Infrastructure.csproj
├── STACK.md
└── TASKS.md
```

## Example Activity Configuration

The persisted JSON model supports application paths, URLs, and folders:

```json
[
  {
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

## Project Status

**MVP scaffold implemented.** The core selection, persistence, and workspace-launching path is in place. Activity editing, Windows startup integration, richer error reporting, packaging, and automated tests remain on the roadmap.

See [`TASKS.md`](TASKS.md) for the implementation plan and [`STACK.md`](STACK.md) for the technology choices.

## Roadmap

- [x] Create initial activity model
- [x] Persist activities as JSON
- [x] Select an activity from the WinUI interface
- [x] Launch applications, URLs, and folders
- [ ] Add activity editor
- [ ] Add item picker / file and folder selection
- [ ] Add Windows startup integration
- [ ] Add launch result/error reporting
- [ ] Add automated tests
- [ ] Package the application with MSIX

## License

A license has not yet been specified for this project.

---

**Intento — Start with your intention, not your setup.**
