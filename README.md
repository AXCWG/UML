# UML — Universal Minecraft Launcher

A native desktop Minecraft launcher built with **.NET 10 + Photino.NET + SolidJS**. No Electron. No bloat. Just a WebView window and a C# backend.

## Stack

| Layer | Technology |
|-------|------------|
| **Desktop shell** | [Photino.NET](https://github.com/tryphotino/photino.NET) v4 — lightweight cross-platform WebView host |
| **Backend** | C# / .NET 10 preview |
| **Frontend** | [SolidJS](https://solidjs.com) v1.9 + TypeScript |
| **CSS** | [TailwindCSS](https://tailwindcss.com) v4 + [DaisyUI](https://daisyui.com) v5 |
| **Minecraft launch** | [CmlLib.Core](https://github.com/CmlLib/CmlLib.Core) v4 — pure C# Minecraft launcher library |
| **IPC** | JSON messages over Photino's `SendWebMessage`/`ReceiveWebMessage` bridge |
| **Build** | Vite v8 + pnpm |

## Project structure

```
Minecraft/
├── Minecraft/                         # Backend (.NET project)
│   ├── Program.cs                     # Entry point — PhotinoWindow + message dispatch
│   ├── Configuration.cs               # State persistence, StateSnapshot, validation
│   ├── Minecraft.cs                   # CmlLib.Core game launcher wrapper
│   ├── ResourceExtractor.cs           # Extracts embedded wwwroot.zip at runtime
│   ├── LauncherLogger.cs              # File + console logger
│   ├── PhotinoExtension.cs            # Extensions for PhotinoWindow
│   ├── Request/                       # IPC message types (frontend → backend)
│   └── Response/                      # IPC response types (backend → frontend)
│
└── Frontend/                          # Frontend (SolidJS + Vite)
    ├── src/
    │   ├── index.tsx                  # App entry — HashRouter + routes
    │   ├── Shell.tsx                  # Tab layout (Home / Profiles / Settings)
    │   ├── Home.tsx                   # Main page — online/offline toggle, play button
    │   ├── ProfilesPage.tsx           # Profile & version management
    │   ├── BackendDelegator.ts        # IPC bridge — typed request/response with promises
    │   ├── Request.ts                 # Frontend request type definitions
    │   └── Response.ts                # Frontend response type definitions
    ├── vite.config.ts                 # Vite config (outputs to ../Minecraft/wwwroot)
    └── package.json
```

## Architecture

```
┌──────────────────────────────────────────┐
│  Photino Desktop Window (WebView2)       │
│  ┌────────────────────────────────────┐  │
│  │  SolidJS SPA (Vite build)          │  │
│  │  ┌──────────┐  ┌────────────────┐  │  │
│  │  │ Shell    │  │ BackendDelega- │  │  │
│  │  │ (tabs)   │  │ tor.ts (IPC)   │──┼──┼── JSON over
│  │  │          │  │                │  │  │   window.external
│  │  │ Home     │  │ Promise-based  │  │  │   .sendMessage()
│  │  │ Profiles │  │ polling API    │  │  │
│  │  │ Settings │  │                │  │  │
│  │  └──────────┘  └────────────────┘  │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
                    ↕ IPC (JSON)
┌──────────────────────────────────────────┐
│  Program.cs — Message dispatch switch    │
│  ┌──────────┐  ┌──────────────────────┐  │
│  │ State    │  │  Minecraft.cs        │  │
│  │ (JSON)   │  │  (CmlLib.Core)       │  │
│  │ persis-  │  │  Launch game process │  │
│  │ tence    │  │                      │  │
│  └──────────┘  └──────────────────────┘  │
└──────────────────────────────────────────┘
```

### IPC protocol

Every message is a flat JSON object with `{ id, type, ...payload }`. The frontend uses a promise-based polling API in `BackendDelegator.ts`. The backend deserializes to the base `Request` record, switches on `Type`, then re-deserializes to the typed subclass.

| Type | Direction | Purpose |
|------|-----------|---------|
| `getConfig` | FE → BE | Get full state snapshot from disk |
| `setConfig` | FE → BE | Persist updated state to disk |
| `play` | FE → BE | Launch Minecraft with current config |
| `getVersions` | FE → BE | Query installed versions via CmlLib |
| `message` | FE → BE | Echo/chat test |
| `addition` | FE → BE | Test: add two numbers |
| `error` | BE → FE | Error response for any failed request |

### Build pipeline

```
dotnet publish -c Release
    │
    ├─ 1. BuildFrontend: pnpm run build → Minecraft/wwwroot/
    ├─ 2. PrepareUiZip: zip wwwroot/ → obj/ui.zip
    ├─ 3. EmbeddedResource: ui.zip embedded in assembly (LogicalName: ui.wwwroot.zip)
    └─ 4. Publish: single self-contained executable
```

At runtime, `ResourceExtractor.Extract()` decompresses the zip to `%TEMP%\MinecraftLauncher\<guid>\`, loads `index.html`, and cleans up on close.

## Getting started

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0) (preview, roll-forward to latest — see `global.json`)
- [pnpm](https://pnpm.io/installation)
- Node.js (for Vite)
- Windows 10+ with [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/) (included in Windows 11 22H2+)

### Development

```bash
# 1. Install frontend dependencies
cd Frontend
pnpm install

# 2. Start Vite dev server (hot-reload on http://localhost:3000)
pnpm run dev

# 3. In another terminal, run the backend in debug mode
dotnet run --project Minecraft -c Debug
```

The debug build loads from `http://localhost:3000` — Vite provides hot module replacement.

### Release build

```bash
dotnet run --project Minecraft -c Release
```

Or publish a self-contained executable:

```bash
dotnet publish Minecraft -c Release -o out
```

The `.csproj` automatically runs `pnpm run build` before packaging. No need to build the frontend manually.

### State & logs

| What | Where |
|------|-------|
| **State file** | `%LocalAppData%\AXCWG\UML\state.json` |
| **Logs** | `%LocalAppData%\AXCWG\UML\log\latest.log` |
| **Minecraft** | `%AppData%\.minecraft` (standard Mojang path) |

## Key dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Photino.NET` | 4.0.16 | Native WebView desktop window |
| `CmlLib.Core` | 4.0.6 | Minecraft installation & launching |
| `AXExpansion` | 1.8.0 | C# extension methods |
| `solid-js` | 1.9.9 | Reactive UI framework |
| `@solidjs/router` | 0.16.2 | Client-side routing |
| `tailwindcss` | 4.1.13 | Utility-first CSS |
| `daisyui` | 5.6.18 | Tailwind component library |
| `vite` | 8.1.5 | Frontend build tool |

## License

MIT
