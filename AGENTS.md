# Repository Guidelines

## Project Structure & Module Organization
This repository is a single ASP.NET Core Web API project (`LabBack.csproj`, `Program.cs`). Keep production code grouped by responsibility:

- `Controllers/` — HTTP endpoints and request validation
- `Services/` — business logic and database operations
- `Data/` — `AppDbContext` and persistence setup
- `Models/` — domain entities such as `PC` and `Server`
- `Contracts/` — request DTOs for API input
- `appsettings*.json` — environment-specific configuration

`docker-compose.yml` starts local PostgreSQL and pgAdmin for development.

## Build, Test, and Development Commands
Use the .NET CLI from the repository root:

- `dotnet restore` — download NuGet packages
- `dotnet build` — compile the API
- `dotnet run` — start the web server locally
- `dotnet watch run` — run with hot reload during development
- `docker compose up -d db pgadmin` — start the local database stack

The default connection string points to `localhost:5432` with `postgres/postgres` credentials.

## Coding Style & Naming Conventions
Follow standard C# conventions: 4-space indentation, `PascalCase` for types and public members, `camelCase` for local variables and parameters, and `I`-prefixed interfaces (for example, `IComputerLabService`). Keep controllers thin; move logic into services. Prefer nullable reference types and explicit validation for request DTOs.

## Testing Guidelines
There is no test project in the repository yet. When adding tests, create a separate test project at the repo root (for example, `LabBack.Tests/`) and name test classes after the unit under test, such as `ComputerLabServiceTests`. Run the full suite with `dotnet test`.

## Commit & Pull Request Guidelines
Recent commits are very short (`Initial`, `Done`), so use concise, descriptive commit subjects instead of vague messages. Prefer imperative style, e.g. `Add server connection validation`. Pull requests should include a short summary, setup/run notes, and any API or database changes. Attach sample requests or screenshots only when they help demonstrate behavior.

## Security & Configuration Tips
Do not commit real secrets. Keep local overrides in `appsettings.Development.json` or environment variables, and update `ConnectionStrings:Postgres` if your database host or credentials differ.
