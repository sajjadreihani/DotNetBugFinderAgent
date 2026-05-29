# BugFinderAgent

BugFinderAgent is a sample Blazor WebAssembly/Server (Blazor) project demonstrating an AI-driven code analysis pipeline. The app orchestrates multiple agent-like services (planner, reviewer, debugger, verifier, Ollama client) to analyze and verify code changes and produce structured pipeline results.

This repository is ideal as a portfolio piece for developers who want to show experience with modern .NET web development, simple agent orchestration, and integration with AI components.

## Key features

- Blazor-based UI components for a lightweight frontend
- Structured service layer with agents: PlannerAgent, CodeReviewAgent, CodeDebuggerAgent, CodeVerifierAgent, OllamaAgent
- Central orchestration via Services/AgentOrchestrator.cs
- Models for typed AI responses and pipeline results
- Config-driven (appsettings.json) for environment-specific behavior

## Tech stack

- .NET 10
- Blazor (component-based UI)
- C# 12
- Simple local service architecture (no external database required)

## Project structure (important files)

- Program.cs – app startup and DI registration
- Services/AgentOrchestrator.cs – orchestrates the analysis pipeline
- Services/*.cs – implementations for planner, reviewer, debugger, verifier, and Ollama client
- Models/*.cs – typed request / response models used across services
- Components/Pages/*.razor – UI pages
- wwwroot/app.css – styling

## Getting started

Prerequisites
- .NET 10 SDK installed
- Visual Studio 2022/2026 or `dotnet` CLI

Run locally
1. Open the solution in Visual Studio and press F5, or from the repo root run:
   dotnet run --project BugFinderAgent.csproj
2. Open the browser to the URL printed in the console (default Kestrel port).

Configuration
- appsettings.json and appsettings.Development.json contain environment configuration values. Review and update any AI endpoint or API configuration before running.

## Usage notes
- The app is designed as a demo/sample. The AI-related services are implemented as local classes under Services and AITools; if any API integration (Ollama or others) is used, confirm credentials and endpoints are configured.
- The orchestrator executes a pipeline that returns a PipelineResult model. Use that model to render or log results.

## Suggestions to make this repo stronger for a resume/GitHub
- Add a concise architecture diagram or sequence diagram (README or docs/)
- Add unit tests for core services (PlannerAgent, CodeReviewAgent, CodeVerifierAgent, AgentOrchestrator) to show TDD/QA skills
- Add a demo GIF or short video in the README showing the UI and pipeline run
- Add CI workflow (GitHub Actions) to build and run tests on push
- Document any third-party API keys or how to mock them for local runs
- Add license and contributing guidelines

## Suggested resume bullets
- Built a Blazor web application (NET 10) that orchestrates modular AI "agents" (planner, reviewer, debugger, verifier) to analyze and verify code changes
- Implemented a typed service layer and pipeline orchestration with dependency injection and clear separation of concerns
- Integrated a local Ollama client and simulated AI responses with strongly-typed models for reliability in development
- Improved developer experience by providing configuration-based environment support and component-driven UI

## License
Add a LICENSE file to specify terms (MIT recommended for public samples).

## Contact / Next steps
- Add unit tests and CI, include a demo video, and optionally publish to GitHub Pages or Azure for a live demo to make this repo presentation-ready for recruiters.
