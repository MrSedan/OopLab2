---
name: architecture-overview
description: Operational map for running, modifying, and safely changing the LabBack API.
type: architecture
date: 2026-05-29
scope: [runtime, api, persistence]
refs:
  - LabBack.csproj
  - LabBack.sln
  - Program.cs
  - docker-compose.yml
  - appsettings.json
  - Controllers/ComputerController.cs
  - Services/ComputerLabService.cs
  - Data/AppDbContext.cs
  - Models/Computer.cs
  - Models/PC.cs
  - Models/Server.cs
  - Contracts/CreatePcRequest.cs
  - Contracts/CreateServerRequest.cs
  - Contracts/Responses/ComputerResponses.cs
  - Swagger/ComputerApiOperationFilter.cs
---

## Run/Test/Build

- Build the application as a single Web SDK project targeting `net10.0`; use `dotnet build LabBack.sln` when the .NET 10 SDK is available. (LabBack.csproj)
- Run the API from the repository root with `dotnet run --project LabBack.csproj`; `Program.cs` builds the web app, maps `/`, maps controllers, and calls `app.Run()`. (Program.cs)
- Start the local PostgreSQL dependency with `docker compose up db`; the compose service publishes Postgres on `localhost:5432` with database/user/password all set to `postgres`. (docker-compose.yml)
- The app expects a connection string named `Postgres`, and the checked-in default points to the same local Postgres credentials as compose. (appsettings.json)
- Swagger UI is available only when the ASP.NET environment is Development; production-style runs still map controllers but skip `UseSwagger()` and `UseSwaggerUI()`. (Program.cs)
- There is no separate test project in the solution, so `dotnet test LabBack.sln` is only a build-oriented smoke check until a test project is added. (LabBack.sln)

## Key Entry Points

- Web host and dependency injection bootstrap. (Program.cs)
- Computer API route surface under `api/computers`. (Controllers/ComputerController.cs)
- Persistence service for CRUD and explicit save operations. (Services/ComputerLabService.cs)
- EF Core database schema mapping. (Data/AppDbContext.cs)
- Shared computer base model. (Models/Computer.cs)
- PC domain behavior. (Models/PC.cs)
- Server domain behavior. (Models/Server.cs)
- Request DTOs for PC creation and updates. (Contracts/CreatePcRequest.cs)
- Request DTOs for server creation and updates. (Contracts/CreateServerRequest.cs)
- API response DTOs. (Contracts/Responses/ComputerResponses.cs)
- Swagger operation tags and examples. (Swagger/ComputerApiOperationFilter.cs)

## Module Map

- `.agents/` — project-local agent configuration, rules, knowledge, and skills.
- `.claude/` — not explored.
- `.gitignore` — not explored.
- `.omx/` — OMX runtime state; not product code.
- `AGENTS.md` — root agent instruction handoff file.
- `Contracts/` — request and response DTOs consumed by `ComputerController`.
- `Controllers/` — HTTP controller layer for PCs and servers.
- `Data/` — EF Core `DbContext` and table/column mapping.
- `LabBack.csproj` — .NET Web SDK project and NuGet package references.
- `LabBack.sln` — Visual Studio solution containing the single `LabBack` project.
- `Models/` — domain models for `Computer`, `PC`, and `Server` behavior.
- `Program.cs` — ASP.NET Core startup, Swagger setup, EF registration, database creation, and route mapping.
- `Services/` — application service abstraction and EF-backed implementation.
- `Swagger/` — Swashbuckle operation filter for Russian tags and JSON examples.
- `appsettings.Development.json` — development connection string and logging levels.
- `appsettings.json` — default connection string, logging levels, and allowed hosts.
- `docker-compose.yml` — local PostgreSQL and pgAdmin services with named volumes.
- `report/` — report PDF/TeX and screenshots; not part of the API runtime.

## Hidden Contracts

- Persistence uses table-per-type inheritance: `Computer` maps to `computers`, while `PC` and `Server` map to `pcs` and `servers`, so model hierarchy changes must be reflected in `OnModelCreating`. (Data/AppDbContext.cs)
- Startup calls `Database.EnsureCreated()` before mapping requests; this project currently relies on direct schema creation rather than an EF migrations workflow. (Program.cs)
- `CurrentConnections` has a private setter, so server updates must go through `Server.Update()` or domain methods rather than object-initializer assignment. (Models/Server.cs)
- Creating or updating a server rejects negative connection counts and rejects `CurrentConnections > MaxConnections` before saving to the database. (Controllers/ComputerController.cs)
- Creating or updating a PC normalizes blank `UserShell` to `XFCE` and blank `Os` to `Linux` at the controller boundary. (Controllers/ComputerController.cs)
- Swagger examples are matched by controller method names; renaming action methods without updating the switch removes or misassigns examples. (Swagger/ComputerApiOperationFilter.cs)

## What Will Break

- If Postgres is not reachable through the configured `Postgres` connection string, startup fails while resolving `AppDbContext` and calling database creation. (Program.cs)
- If XML documentation output is disabled or missing, Swagger setup still passes the computed assembly XML path into `IncludeXmlComments`. (Program.cs)
- If a server connection is accepted and `SaveChangesAsync` is not called after the domain mutation, the incremented connection count is not persisted. (Controllers/ComputerController.cs)
- If new controller responses bypass `Contracts/Responses`, Swagger response metadata and the existing response shape conventions drift from the current API surface. (Controllers/ComputerController.cs)
