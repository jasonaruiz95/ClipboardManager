# ClipboardManager

A cross-platform desktop clipboard history manager built with [Avalonia UI](https://avaloniaui.net/) and [FluentAvalonia](https://github.com/amwx/FluentAvalonia). Clipboard entries are automatically captured and persisted to a local SQLite database, so your history survives app restarts.

---

## Features

- Automatically monitors the system clipboard and captures new text entries
- Persists clipboard history to a local SQLite database via Entity Framework Core
- Restore any saved entry back to the clipboard with one click
- Delete individual entries from history
- Cross-platform: runs on Windows, Linux, and macOS
- Supports swapping to a remote database backend (e.g. PostgreSQL)

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/jasonaruiz95/ClipboardManager.git
cd ClipboardManager
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Run the app

```bash
dotnet run
```

The app will create a `clipboard.db` SQLite file in the working directory on first launch.

---

## Project Structure

```
ClipboardManager/
├── Assets/                         # App icons and static assets
├── Data/
│   └── ClipboardDbContext.cs       # Entity Framework Core DbContext
├── Models/
│   └── ClipboardEntry.cs           # Database entity model
├── Services/
│   ├── Interfaces/
│   │   ├── IClipboardMonitorService.cs
│   │   ├── IClipboardRepository.cs
│   │   └── IClipboardService.cs
│   ├── ClipboardMonitorService.cs  # Polls clipboard every 500ms
│   ├── ClipboardService.cs         # Reads/writes system clipboard
│   ├── DatabaseConfig.cs           # Database backend configuration
│   └── SqliteClipboardRepository.cs # EF Core SQLite implementation
├── ViewModels/
│   └── MainWindowViewModel.cs      # Main window logic and commands
├── Views/
│   └── MainWindow.axaml            # Main window UI
├── App.axaml                       # Application entry point and DI setup
└── Program.cs                      # Process entry point
```

---

## Database

By default the app uses a local SQLite database (`clipboard.db`) stored in the app's working directory. The database and schema are created automatically on first launch — no migrations need to be run manually.

### Switching to a remote database

To use a remote PostgreSQL database instead, update the `DatabaseConfig` in `App.axaml.cs`:

```csharp
var dbConfig = new DatabaseConfig
{
    Backend = DatabaseBackend.Remote,
    RemoteConnectionString = "Host=your-host;Database=clipboard;Username=user;Password=pass"
};
```

---

## Publishing

Build a self-contained executable for your target platform from the project directory:

| Platform | Command |
|---|---|
| Windows | `dotnet publish -c Release -r win-x64 --self-contained true` |
| Linux | `dotnet publish -c Release -r linux-x64 --self-contained true` |
| macOS (Intel) | `dotnet publish -c Release -r osx-x64 --self-contained true` |
| macOS (Apple Silicon) | `dotnet publish -c Release -r osx-arm64 --self-contained true` |

Output is placed in `bin/Release/net10.0/<rid>/publish/`. Use the `-o` flag to specify a custom output directory:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -o ./dist/windows
```

> **Linux note:** mark the binary as executable after publishing:
> ```bash
> chmod +x ./bin/Release/net10.0/linux-x64/publish/ClipboardManager
> ```

---

## Dependencies

| Package | Purpose |
|---|---|
| [Avalonia](https://avaloniaui.net/) | Cross-platform UI framework |
| [FluentAvalonia](https://github.com/amwx/FluentAvalonia) | Fluent Design controls (NavigationView, AppWindow) |
| [Microsoft.EntityFrameworkCore.Sqlite](https://learn.microsoft.com/en-us/ef/core/) | SQLite persistence via EF Core |
| [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/) | `IAsyncRelayCommand` and MVVM helpers |
| [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) | DI container |

---

## License

MIT
