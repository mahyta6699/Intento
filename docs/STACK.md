# Intento — Technology Stack

Intento is planned as a native Windows desktop productivity application. The stack is chosen to provide a modern Windows UI, reliable operating-system integration, simple local configuration, and a maintainable architecture.

## 🧱 Core Stack

| Layer | Technology | Purpose |
|---|---|---|
| Language | **C#** | Primary application language |
| Runtime | **.NET 10** | Application runtime and modern .NET APIs |
| UI | **WinUI 3** | Native Windows user interface |
| Windows Platform | **Windows App SDK** | Windows integration and desktop application APIs |
| Architecture | **MVVM** | Separation of UI, application logic, and state |
| Configuration | **JSON** | Store activities and launch targets |
| Local Storage | **Windows AppData** | Persist user configuration locally |
| Testing | **xUnit** | Unit and application-level testing |
| Dependencies | **NuGet** | .NET package management |
| Version Control | **Git + GitHub** | Source control and collaboration |
| CI | **GitHub Actions** | Automated build and test workflows |
| Packaging | **MSIX** | Windows application packaging and distribution |

## 🖥️ Application Architecture

Intento will use a layered architecture with MVVM for the presentation layer.

```text
┌─────────────────────────────────────┐
│             Intento UI              │
│          WinUI 3 / MVVM             │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│        Intento Application          │
│                                     │
│  Activity Manager                   │
│  Workspace Manager                  │
│  Configuration Manager              │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│           Launch Services           │
│                                     │
│  Application Launcher               │
│  URL Launcher                       │
│  Folder Launcher                    │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│             Windows OS              │
│  Applications │ Browser │ Explorer │
└─────────────────────────────────────┘
```

## 🎨 UI Layer

### WinUI 3

WinUI 3 will be used to build the desktop interface because Intento is designed specifically for Windows.

Responsibilities include:

- Activity selection screen
- Activity creation and editing
- Settings
- Launch progress and status feedback
- Native Windows look and feel

### MVVM

The UI will follow the Model-View-ViewModel pattern.

```text
View
 │
 ▼
ViewModel
 │
 ▼
Application Services
 │
 ▼
Models / Storage / Launchers
```

This keeps UI code separate from business logic and makes the application easier to test and maintain.

## ⚙️ Application Layer

The application layer contains the core Intento functionality.

### Activity Manager

Responsible for:

- Creating activities
- Updating activities
- Deleting activities
- Selecting an activity
- Managing activity metadata

Example activities:

- Job
- Learning
- Research
- Workout

### Workspace Manager

Responsible for managing the resources associated with an activity.

An activity can contain:

- Applications
- Websites
- Folders

Example:

```text
Job
├── Microsoft Excel
├── Browser → Company Portal
├── Browser → Project Management
└── Folder → C:\Projects\Work
```

### Configuration Manager

Responsible for loading and saving the user's activities and launch targets.

The initial implementation can use JSON stored in the user's local AppData directory.

## 🚀 Launch Services

Intento needs to launch different types of resources. These should be separated into dedicated services.

### Application Launcher

Uses .NET process APIs and Windows integration to launch installed desktop applications.

### URL Launcher

Opens websites using the user's default browser.

### Folder Launcher

Opens directories using Windows Explorer.

A common launcher abstraction can keep these implementations consistent:

```text
ILauncher
   │
   ├── ApplicationLauncher
   ├── UrlLauncher
   └── FolderLauncher
```

## 🪟 Windows Integration

Intento will use Windows APIs and the Windows App SDK for operating-system integration such as:

- Windows startup behavior
- Desktop application launching
- Opening folders in Explorer
- Opening URLs with the default browser
- Windows application lifecycle
- Native Windows packaging

## 💾 Data Storage

The first version should keep configuration simple and local.

### JSON

Example configuration:

```json
{
  "activities": [
    {
      "name": "Job",
      "items": [
        {
          "type": "application",
          "target": "C:\\Program Files\\Microsoft Office\\root\\Office16\\EXCEL.EXE"
        },
        {
          "type": "url",
          "target": "https://example.com"
        },
        {
          "type": "folder",
          "target": "C:\\Projects\\Work"
        }
      ]
    }
  ]
}
```

The exact schema can evolve as the application develops.

## 🧪 Testing

**xUnit** will be used for automated tests.

Priority areas for testing:

- Activity creation and editing
- Configuration serialization/deserialization
- Activity selection
- Launcher behavior
- Invalid or missing paths
- Invalid URLs
- Configuration recovery

The UI itself can be tested separately where appropriate, while most business logic should remain independently testable.

## 🔧 Development Tools

Recommended development environment:

- **Visual Studio 2022** with Windows desktop development tooling
- **.NET SDK**
- **Windows 10/11** development environment
- **Git**
- **GitHub**

## 🔄 CI/CD

GitHub Actions will be used for continuous integration.

A basic CI workflow should:

1. Restore NuGet dependencies.
2. Build the solution.
3. Run automated tests.
4. Report build/test failures.

Later, the workflow can be extended to create packaged releases.

## 📦 Packaging

Intento is intended to be distributed as a Windows desktop application.

**MSIX** is the planned packaging format because it provides a standardized Windows application package and integrates with the Windows application ecosystem.

## 📁 Suggested Project Structure

```text
Intento/
├── src/
│   ├── Intento.App/
│   │   ├── Views/
│   │   ├── ViewModels/
│   │   ├── Assets/
│   │   └── App.xaml
│   │
│   ├── Intento.Core/
│   │   ├── Models/
│   │   ├── Interfaces/
│   │   └── Services/
│   │
│   └── Intento.Infrastructure/
│       ├── Storage/
│       ├── Launchers/
│       └── Windows/
│
├── tests/
│   └── Intento.Tests/
│
├── docs/
├── README.md
├── STACK.md
└── Intento.sln
```

## 🗺️ Development Direction

The stack should remain intentionally lightweight during the early stages of Intento.

The initial implementation should focus on:

1. Creating activity groups.
2. Adding applications, URLs, and folders.
3. Saving configuration locally.
4. Selecting an activity.
5. Launching all associated resources.
6. Integrating with Windows startup.

Additional infrastructure such as a database, cloud synchronization, or an external backend should only be introduced if the application's requirements eventually justify it.

> **Note:** This is the proposed technology stack for Intento. The project is currently in the planning / early-development stage, so individual technologies and architectural decisions may change during implementation.
