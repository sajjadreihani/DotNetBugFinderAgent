# BugFinderAgent

> **AI-powered code analysis pipeline built with .NET 10, Blazor, Microsoft Agent Stack, and Ollama local LLMs.**

BugFinderAgent is a portfolio-grade Blazor application that demonstrates a multi-agent AI pipeline for automated code review, debugging, and verification — all running locally with no cloud dependency required.

---

## What It Does

The app orchestrates a structured pipeline of specialized AI agents — each powered by a local LLM via Ollama — to analyze code changes end-to-end:

1. **PlannerAgent** — breaks down the analysis task into actionable steps
2. **CodeReviewAgent** — reviews code for quality, style, and potential issues
3. **CodeDebuggerAgent** — identifies bugs, edge cases, and runtime risks
4. **CodeVerifierAgent** — validates fixes and confirms correctness
5. **AgentOrchestrator** — coordinates the full pipeline and aggregates results into a typed `PipelineResult`

All agent communication is handled through **Microsoft Agent Stack** (Microsoft.Extensions.AI abstractions), with **Ollama** providing local LLM inference — no OpenAI API key or internet connection needed for core functionality.

---

## Key Features

- **Local-first AI** — runs entirely on your machine via Ollama; no API keys, no data leaving your network
- **Microsoft Agent Stack integration** — uses Microsoft's official agent abstractions (`Microsoft.Extensions.AI`) for structured, interoperable agent communication
- **Blazor UI** — component-driven frontend with clean separation from the service layer
- **Typed pipeline results** — strongly-typed `PipelineResult` and response models throughout; no stringly-typed magic
- **Clean Architecture** — dependency injection, clear service boundaries, and an orchestrator pattern you can extend or swap out
- **Config-driven** — `appsettings.json` / `appsettings.Development.json` for environment-specific Ollama endpoints and model selection

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 10, C# 13 |
| Frontend | Blazor (WebAssembly/Server) |
| Agent abstractions | Microsoft Agent Stack (`Microsoft.Extensions.AI`) |
| Local LLM runtime | Ollama |
| Architecture | Clean Architecture, Dependency Injection |
| Config | `appsettings.json` |

---

## Project Structure

```
BugFinderAgent/
├── Program.cs                        # App startup and DI registration
├── Services/
│   ├── AgentOrchestrator.cs          # Pipeline coordinator
│   ├── PlannerAgent.cs
│   ├── CodeReviewAgent.cs
│   ├── CodeDebuggerAgent.cs
│   ├── CodeVerifierAgent.cs
│   └── OllamaAgent.cs                # Ollama client wrapper
├── Models/                           # Typed request/response/pipeline models
├── Components/Pages/                 # Blazor UI pages
├── AITools/                          # Tool definitions for agent use
└── wwwroot/app.css                   # Global styles
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Ollama](https://ollama.com/) installed and running locally
- A compatible model pulled in Ollama, e.g.:
  ```bash
  ollama pull llama3
  # or any model your hardware supports
  ```

### Run Locally

```bash
# Clone the repo
git clone https://github.com/your-username/BugFinderAgent.git
cd BugFinderAgent

# Run the app
dotnet run --project BugFinderAgent.csproj
```

Then open the URL printed in the console (default Kestrel port).

Or open the solution in Visual Studio 2022/2026 and press **F5**.

### Configuration

Edit `appsettings.json` (or `appsettings.Development.json`) to point at your Ollama instance and select your preferred model:

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "llama3"
  }
}
```

No external API keys are required for default operation.

---

## How the Pipeline Works

```
User Input (code snippet / diff)
        │
        ▼
  CodeReviewAgent       → Flags quality and style issues
        │
        ▼
  PlannerAgent          → Decomposes the task
        │
        ▼
  CodeDebuggerAgent     → Identifies bugs and edge cases
        │
        ▼
  CodeVerifierAgent     → Confirms fixes and validates output
        │
        ▼
  AgentOrchestrator     → Assembles PipelineResult
        │
        ▼
  Blazor UI             → Renders structured results
```

Each agent communicates via Microsoft Agent Stack abstractions, making it straightforward to swap Ollama for any `IChatClient`-compatible provider (OpenAI, Azure OpenAI, etc.) with a one-line config change.

---

## Making This Production-Ready (Suggestions)

- **Add unit tests** for `PlannerAgent`, `CodeReviewAgent`, `CodeVerifierAgent`, and `AgentOrchestrator` to demonstrate TDD discipline
- **Add a GitHub Actions CI workflow** to build and run tests on push
- **Add a demo GIF or short video** to the README showing the UI and a real pipeline run
- **Publish a live demo** to GitHub Pages, Azure Static Web Apps, or Azure Container Apps
- **Add an architecture/sequence diagram** to `docs/` for recruiters and collaborators
- **Add a LICENSE file** (MIT recommended for public portfolio projects)
- **Add contributing guidelines** if you want OSS contributions

---

## Resume Bullets

- Built a Blazor web application (.NET 10) orchestrating a multi-agent AI pipeline (planner → reviewer → debugger → verifier) for automated code analysis using **Microsoft Agent Stack** (`Microsoft.Extensions.AI`)
- Integrated **Ollama** for fully local LLM inference, eliminating cloud dependency and demonstrating on-device AI architecture
- Implemented a typed service layer with clean separation of concerns, dependency injection, and a central `AgentOrchestrator` producing structured `PipelineResult` models
- Designed a config-driven, provider-agnostic agent layer — swappable from local Ollama to OpenAI/Azure OpenAI without code changes

---

## License

Add a `LICENSE` file to specify terms. MIT is recommended for public portfolio samples.