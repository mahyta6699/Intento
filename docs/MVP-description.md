

### What I added

**1. Project structure**
I created a .NET/WinUI 3 solution:

```text
Intento.sln
src/
├── Intento.App/
├── Intento.Core/
└── Intento.Infrastructure/
```

The idea is to keep:

* `App` → Windows UI
* `Core` → Intento's domain models
* `Infrastructure` → persistence and Windows launching

---

**2. Activity model**

I added the basic concept of an Intento activity:

```text
Activity
 ├── Id
 ├── Name
 ├── Icon
 └── Items
```

Each activity can contain:

```text
Application
URL
Folder
```

So eventually you could have:

```text
Job
 ├── VS Code
 ├── Chrome → GitHub
 ├── Chrome → Jira
 └── C:\Projects\MyProject

Learning
 ├── Chrome → YouTube
 ├── Obsidian
 └── C:\Notes
```

---

**3. JSON persistence**

I added an `ActivityStore`.

Activities are saved locally under:

```text
%LOCALAPPDATA%\Intento\activities.json
```

So the user's configuration doesn't need a database for the MVP.

---

**4. Workspace launcher**

I added `WorkspaceLauncher`.

When an activity is selected, Intento can launch:

* Windows applications
* URLs
* folders

It uses Windows' normal shell handling, so things like a URL or folder can be opened by the associated Windows application.

---

**5. First WinUI interface**

I created the initial Intento window.

The UI asks:

> **What do you intend to do today?**

It currently presents:

* **Job**
* **Learning**
* **Research**
* **Workout**

There are also **Reload** and **Exit** actions.

---

**6. Default activities**

If there isn't an existing configuration, Intento creates those four activities automatically.

They are currently **empty** because I haven't added an Activity Editor yet.

The intended flow is:

```text
Start Intento
     ↓
"What do you intend to do today?"
     ↓
Choose Job
     ↓
Intento loads Job configuration
     ↓
Launch apps / URLs / folders
```

---

**7. Windows application setup**

I added the basic WinUI 3 / Windows App SDK project configuration, application manifest, solution configuration, and project references.

The project targets **.NET 10** and Windows.

---

**8. README**

I also updated the README to explain:

* What Intento does
* The MVP
* Architecture
* Project structure
* Requirements
* Current status
* Roadmap

---

### What I deliberately did **not** build yet

The MVP is intentionally the smallest end-to-end version. These are still pending:

```text
❌ Activity Editor
❌ Add/remove applications
❌ Add/remove URLs
❌ Add/remove folders
❌ Browse for .exe files
❌ Folder picker
❌ Startup with Windows
❌ First-run onboarding
❌ Error reporting UI
❌ Automated tests
❌ MSIX release packaging
```

So right now, **the foundation and launch flow exist, but there isn't yet a UI for configuring what each activity launches.**

One important caveat: I **did not have a Windows/Visual Studio environment available to actually compile and run the WinUI application**, so the code has not been verified with a real Windows build yet.

### In simple terms

Before my changes, you had the **idea/planning** for Intento.

After my changes, you have the beginning of the **actual application**:

**Intent → Activity → Configuration → Launch**

The most logical next piece is the **Activity Editor**, because that turns the current hard-coded/default activities into something you can actually configure yourself.

