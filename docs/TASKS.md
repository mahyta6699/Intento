# Intento — Project Tasks

This document tracks the planned development work for Intento, from initial project setup through the first usable Windows release.

## 🎯 Goal

Build a Windows application that lets a user choose an activity such as **Job**, **Learning**, **Research**, or **Workout**, then automatically opens the applications, websites, and folders associated with that activity.

---

## Phase 1 — Project Setup

- [ ] Create the .NET solution
- [ ] Create the WinUI 3 desktop application
- [ ] Configure Windows App SDK
- [ ] Set up the initial project structure
- [ ] Configure Debug and Release builds
- [ ] Add `.gitignore`
- [ ] Add basic application metadata
- [ ] Configure NuGet package management
- [x] Add the test project
- [ ] Verify the application builds and launches

## Phase 2 — Core Domain Model

- [x] Create the `Activity` model
- [ ] Create the `LaunchItem` model
- [x] Define launch item types
  - [x] Application
  - [x] URL
  - [x] Folder
- [ ] Define activity properties
  - [x] Name
  - [x] Icon
  - [x] Launch items
- [ ] Add validation for activity names
- [ ] Add validation for launch targets
- [ ] Define interfaces for core services

## Phase 3 — Configuration & Persistence

- [x] Create the configuration schema
- [x] Implement JSON serialization
- [x] Implement JSON deserialization
- [x] Create the local AppData storage location
- [x] Implement configuration loading
- [x] Implement configuration saving
- [x] Handle missing configuration files
- [x] Handle invalid/corrupted configuration files
- [ ] Add configuration versioning support
- [x] Add tests for persistence

## Phase 4 — Launch System

### Application Launcher

- [ ] Implement application launching
- [ ] Support executable paths
- [ ] Support application arguments where required
- [ ] Detect missing application paths
- [ ] Return launch success/failure information

### URL Launcher

- [ ] Implement URL launching
- [ ] Use the user's default browser
- [ ] Validate URLs
- [ ] Handle invalid URLs gracefully

### Folder Launcher

- [ ] Implement folder launching
- [ ] Open folders in Windows Explorer
- [ ] Detect missing folders
- [ ] Handle inaccessible folders gracefully

### Unified Launcher

- [ ] Create a common launcher interface
- [ ] Route launch items to the correct launcher
- [ ] Support launching multiple items
- [ ] Decide whether items launch sequentially or concurrently
- [ ] Capture launch errors without stopping other valid launches
- [ ] Add launcher tests

## Phase 5 — Activity Management

- [ ] Implement activity creation
- [ ] Implement activity editing
- [ ] Implement activity deletion
- [ ] Implement activity selection
- [ ] Implement activity duplication
- [x] Add default activities
  - [x] Job
  - [x] Learning
  - [x] Research
  - [x] Workout
- [ ] Allow custom activity names
- [ ] Allow custom activity icons
- [ ] Add activity validation

## Phase 6 — User Interface

### Main Activity Screen

- [ ] Design the main Intento window
- [x] Display available activities
- [x] Add activity selection controls
- [x] Add visual activity icons
- [ ] Add empty-state UI

### Activity Editor

- [ ] Create activity editor screen
- [ ] Add activity name input
- [ ] Add launch item list
- [ ] Add application picker
- [ ] Add URL input
- [ ] Add folder picker
- [ ] Add remove-item functionality
- [ ] Add drag-and-drop ordering if useful
- [ ] Add save/cancel actions

### Launch Experience

- [ ] Show selected activity
- [ ] Start launching items
- [ ] Show launch progress where useful
- [ ] Show failed launch items
- [ ] Allow the user to continue after partial failures
- [ ] Add a completion state

### Settings

- [ ] Create settings screen
- [ ] Add Windows startup setting
- [ ] Add default activity setting if needed
- [ ] Add configuration reset option
- [ ] Add application information/about section

## Phase 7 — Windows Startup Integration

- [ ] Research the recommended Windows startup mechanism
- [ ] Implement startup registration
- [ ] Implement startup removal
- [ ] Add startup preference to Settings
- [ ] Test startup behavior after Windows login
- [ ] Handle startup failures gracefully
- [ ] Verify application permissions and packaging requirements

## Phase 8 — First-Run Experience

- [ ] Detect first application launch
- [ ] Create default configuration
- [ ] Show a short introduction
- [ ] Explain activity-based workspaces
- [ ] Let the user create their first activity
- [ ] Allow skipping onboarding

## Phase 9 — Error Handling & Reliability

- [ ] Add centralized error handling
- [ ] Add user-friendly error messages
- [ ] Handle missing executables
- [ ] Handle deleted folders
- [ ] Handle invalid URLs
- [x] Handle inaccessible persistence files gracefully
- [ ] Prevent one failed launch from blocking the remaining items
- [ ] Add diagnostic logging
- [ ] Define safe logging rules that do not expose sensitive user data

## Phase 10 — Testing

### Unit Tests

- [ ] Test activity creation
- [ ] Test activity editing
- [ ] Test activity deletion
- [ ] Test activity validation
- [x] Test configuration serialization
- [x] Test configuration deserialization
- [x] Test configuration recovery
- [ ] Test launcher selection
- [ ] Test URL validation

### Integration Tests

- [x] Test configuration persistence with isolated temporary files
- [ ] Test application launching
- [ ] Test URL launching
- [ ] Test folder launching
- [ ] Test startup integration

### Manual Testing

- [ ] Test clean installation
- [ ] Test upgrade installation
- [ ] Test first launch
- [ ] Test normal activity launch
- [ ] Test missing launch targets
- [ ] Test corrupted configuration
- [ ] Test Windows restart/login behavior
- [ ] Test uninstall behavior

## Phase 11 — Performance & UX Polish

- [ ] Measure application startup time
- [ ] Reduce unnecessary startup work
- [ ] Avoid blocking the UI while launching resources
- [ ] Improve activity selection responsiveness
- [ ] Add keyboard navigation
- [ ] Add accessible labels and controls
- [ ] Ensure appropriate DPI/scaling behavior
- [ ] Review Windows light/dark theme behavior
- [ ] Add polished icons and visual assets

## Phase 12 — Packaging & Release

- [ ] Configure MSIX packaging
- [ ] Add application icon
- [ ] Configure package identity
- [ ] Configure application metadata
- [ ] Test packaged installation
- [ ] Test uninstall
- [ ] Test upgrade from an earlier version
- [ ] Create GitHub Actions build workflow
- [ ] Run automated tests in CI
- [ ] Build release artifacts
- [ ] Create the first GitHub release
- [ ] Document installation instructions

## Phase 13 — Documentation

- [x] Create `README.md`
- [x] Create `STACK.md`
- [x] Create `TASKS.md`
- [ ] Create `ARCHITECTURE.md`
- [x] Document configuration format
- [x] Document persistence and recovery behavior
- [ ] Document development setup
- [ ] Document build instructions
- [ ] Document release process
- [ ] Add screenshots when the UI is ready
- [ ] Add contribution guidelines
- [ ] Add license when a project license is selected

---

## 🚀 MVP Definition

The first usable version of Intento should be able to:

- [ ] Run as a native Windows desktop application
- [ ] Display a list of activities
- [ ] Create and edit activities
- [ ] Add applications to activities
- [ ] Add URLs to activities
- [ ] Add folders to activities
- [x] Save activities locally with safe recovery behavior
- [x] Select an activity
- [x] Launch all valid items associated with the activity
- [ ] Report failed launches without crashing
- [ ] Optionally start with Windows

Once these tasks are complete, Intento should provide the core experience described in the README.

## 🔮 Future / Optional Tasks

These should be considered after the MVP rather than requirements for the first release.

- [ ] Cloud synchronization
- [ ] Multiple device synchronization
- [ ] Activity usage statistics
- [ ] Recently used activities
- [ ] Scheduled activity launches
- [ ] Time-based activity suggestions
- [ ] Automatic workspace restoration
- [ ] Window positioning and layout management
- [ ] Import/export activities
- [ ] Backup and restore configuration
- [ ] Search for activities
- [ ] Global keyboard shortcut
- [ ] System tray mode
- [ ] Windows notifications

> **Note:** This task list is a development plan rather than a fixed specification. Tasks and priorities can change as Intento is implemented and tested.
