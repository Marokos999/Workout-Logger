# Workout-Logger

Android workout tracking app built with **.NET MAUI** and **ASP.NET Core 10**. Log exercises, track sets, and see progress over time — including personal records and volume history per exercise.

## Features

- Registration and login with JWT authentication and refresh token rotation (30-day tokens, one-time use)
- Create and name workout sessions; add sets with exercise, reps, and weight; end the session to see a summary
- Workout summary: total duration, number of sets, total volume (kg), and a breakdown per exercise with max weight
- Exercise catalogue with 50+ pre-seeded exercises across muscle groups
- Per-exercise progress: full history of max weight per session and personal record highlighted
- Automatic token refresh — the app silently renews expired tokens in the background; on final failure it logs out cleanly
- Platform-specific API base URL via compiler directives (Android emulator `10.0.2.2`, iOS/Windows `localhost`)

## Tech stack

| Area | Technology |
| --- | --- |
| Mobile | .NET MAUI 10, CommunityToolkit.Mvvm (ObservableProperty, RelayCommand) |
| Backend | ASP.NET Core 10, Minimal API, JWT Bearer, Rate limiting |
| Data | EF Core 10 + Npgsql, PostgreSQL |
| Auth | JWT (access token, 60 min) + refresh token rotation (30 days, one-time use) |
| Dev orchestration | .NET Aspire (AppHost + ServiceDefaults) |
| CI | GitHub Actions — Android release APK build on push to `main` |

## Architecture

```text
MAUI App
  └── JwtHandler (DelegatingHandler)
        │  attaches Bearer token
        │  on 401 → RefreshAsync → retry
        │  on second 401 → Logout + navigate to //login
        ▼
  Services (AuthService, WorkoutService, ExerciseService, ProgressService)
        ▼
  ASP.NET Core Minimal API
        │  JWT authentication
        │  Rate limiting: auth endpoints 10 req/min per IP
        │  UseExceptionHandler → JSON error response
        ▼
  EF Core → PostgreSQL
        │
  MigrationHostedService — runs migrations + seeds exercises on startup
```

- **Refresh token rotation.** Each refresh issues a new pair of tokens and marks the old refresh token as used. A reused token returns 401. Tokens are stored in `SecureStorage` (Keystore on Android, Keychain on iOS).
- **Request cloning for retry.** `HttpRequestMessage` is not reusable after it is sent. `JwtHandler` clones the request (copies method, URI, headers, and body bytes) before the retry send.
- **Aspire orchestration.** `WorkoutLogger.AppHost` adds a PostgreSQL container with pgAdmin and wires it to the API project with `.WaitFor(db)`. Running the AppHost is enough to start everything locally.

## Project structure

```text
src/
├── WorkoutLogger.API/         Minimal API, Features (Auth, Workouts, Exercises, Progress), Infrastructure
├── WorkoutLogger.Contracts/   Shared request/response records (used by API and mobile)
├── WorkoutLogger.AppHost/     .NET Aspire orchestration (Postgres + API)
└── WorkoutLogger.ServiceDefaults/  Aspire service defaults (OpenTelemetry, health checks)
frontend/
└── WorkoutLogger.Mobile/      .NET MAUI 10 Android/iOS/Windows app
tests/
└── WorkoutLogger.Tests/       xUnit integration tests (WebApplicationFactory + InMemoryDatabase)
```

## Run locally with Aspire

Requirements: .NET 10 SDK, .NET Aspire workload, Docker (for the Postgres container).

```bash
dotnet run --project src/WorkoutLogger.AppHost
```

The AppHost starts PostgreSQL, applies migrations, seeds exercises, and runs the API. Aspire dashboard: `http://localhost:15888`.

## Run API without Aspire

Requirements: .NET 10 SDK, PostgreSQL on `localhost:5432`.

Set the connection string and JWT key in `src/WorkoutLogger.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "workoutdb": "Host=localhost;Database=workoutlogger;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-chars",
    "Issuer": "WorkoutLogger",
    "Audience": "WorkoutLogger",
    "ExpiresInMinutes": 60
  }
}
```

```bash
dotnet run --project src/WorkoutLogger.API
```

API: `http://localhost:5107`. Interactive docs: `http://localhost:5107/scalar`.

## Run with Docker

```bash
docker build -f src/WorkoutLogger.API/Dockerfile -t workoutlogger-api .
docker run -p 5107:8080 \
  -e ConnectionStrings__workoutdb="Host=host.docker.internal;Database=workoutlogger;Username=postgres;Password=postgres" \
  -e Jwt__Key="your-secret-key-at-least-32-chars" \
  -e Jwt__Issuer="WorkoutLogger" \
  -e Jwt__Audience="WorkoutLogger" \
  workoutlogger-api
```

Build context must be the repo root (`.`) because the API references `WorkoutLogger.Contracts` and `WorkoutLogger.ServiceDefaults`.

A pre-built image is published to GitHub Container Registry on every push to `main`:

```bash
docker pull ghcr.io/marokos999/workoutlogger-api:latest
```

## Run mobile app

Open `WorkoutLogger.slnx` in Visual Studio 2022 17.13+, select `WorkoutLogger.Mobile` as startup project, choose `Android Emulator` or a physical device.

The base URL is selected at compile time: `http://10.0.2.2:5107/` for Android emulator, `http://localhost:5107/` for Windows/iOS.

## Tests

```bash
dotnet test tests/WorkoutLogger.Tests
```

Integration tests use `WebApplicationFactory` with an in-memory SQLite database. Covers: registration, login, refresh token rotation (valid token → 200, reused token → 401), duplicate email conflict, invalid input validation, workout session and set endpoints, and exercise endpoints.

## Configuration

| Setting | Description |
| --- | --- |
| `ConnectionStrings__workoutdb` | PostgreSQL connection string |
| `Jwt__Key` | Signing key (minimum 32 characters) |
| `Jwt__Issuer` | JWT issuer (must match `ValidIssuer` in token validation) |
| `Jwt__Audience` | JWT audience |
| `Jwt__ExpiresInMinutes` | Access token lifetime in minutes (default 60) |
