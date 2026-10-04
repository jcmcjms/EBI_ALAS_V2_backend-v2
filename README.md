# EBI.ALAS.V2 Backend

> **ALAS** — Automated Loan Application System, version 2

A banking-grade .NET 10 Web API that powers the full loan origination lifecycle for Enterprise Bank Inc. Built following strict modular monolith architecture with clean dependency boundaries.

## Architecture

This repository follows a **modular monolith** architecture organized by feature with mandatory dependency direction:

```
HTTP Endpoint
    ↓
Feature Service / Use Case
    ↓
Feature Persistence / External Boundary
    ↓
Infrastructure / External System
```

### Key Principles

- **No Common/Helpers/Utils/Managers** catch-all folders
- **No repository interfaces** solely for "clean architecture" compliance
- **No MediatR/CQRS** mediators solely for style
- **Async all the way** - no `.Result`, `.Wait()`, or `Task.Run` in request paths
- **CancellationToken propagation** at every async I/O boundary
- **Thin endpoints** - business logic in services
- **No EF entities** exposed directly from public API contracts
- **Classes under ~300 lines** unless strong cohesion reason
- **AsNoTracking()** for read-only EF Core queries
- **Projection only required fields** in queries

## Project Structure

```
src/EBI.ALAS.Api/
├── Program.cs                           # Composition root
├── GlobalUsings.cs                      # Global usings
├── Common/                              # Cross-cutting concerns
│   ├── Constants/                       # Roles, Permissions, Queues
│   ├── Extensions/                      # DI, middleware, auth, caching
│   ├── Middleware/                      # Correlation ID, logging, security, etc.
│   ├── Models/                          # ApiResponse, PagedResult, Result<T>
│   ├── Errors/                          # Domain exceptions
│   ├── Authorization/                   # Permission requirements
│   └── Time/                            # ITimeProvider, PhilippinesTimeProvider
├── Features/                            # Feature slices (vertical)
│   ├── Auth/                            # Login, refresh, logout, password
│   ├── Users/                           # Admin user management
│   ├── Roles/                           # Role & permission matrix
│   ├── Branches/                        # Branch registry
│   ├── Loans/                           # Core loan workflow
│   ├── ApprovalMatrix/                  # Approver routing
│   ├── AuditLogs/                       # Audit trail
│   ├── Notifications/                   # In-app + SignalR
│   ├── Presence/                        # Online directory
│   ├── Dashboard/                       # Aggregated metrics
│   ├── WebLoans/                        # Legacy read-only integration
│   ├── SystemSettings/                  # Runtime configuration
│   └── Account/                         # My account
├── Infrastructure/
│   ├── Data/                            # AppDbContext, WebLoanDbContext, DbInitializer
│   ├── Interceptors/                    # Audit, Read-only enforcement
│   ├── Messaging/                       # MassTransit, consumers, events
│   ├── Caching/                         # Garnet/Redis distributed cache
│   ├── Security/                        # Token revocation, validators
│   └── SignalR/                         # JWT user ID provider
└── Migrations/                          # EF Core migrations
```

## Tech Stack

| Layer | Technology |
|-------|------------|
| Runtime | .NET 10 |
| Web Framework | ASP.NET Core Minimal APIs |
| ORM | Entity Framework Core 10 |
| Database | Microsoft SQL Server |
| Message Bus | MassTransit + RabbitMQ |
| Real-Time | SignalR |
| Distributed Cache | Redis (StackExchange) / Garnet |
| Auth | JWT Bearer + Refresh Token Rotation |
| Validation | FluentValidation |
| Logging | Serilog + OpenTelemetry |
| Health Checks | AspNetCore.HealthChecks |
| API Docs | Scalar (OpenAPI) |

## Getting Started

### Prerequisites

- **.NET 10 SDK**
- **SQL Server** (LocalDB, Express, Developer, or full)
- **Redis** (optional, falls back to in-memory)
- **RabbitMQ** (optional, falls back to in-memory)

### 1. Clone & Restore

```powershell
git clone <repo-url> alas_v2_backend_v2
cd alas_v2_backend_v2
dotnet restore
```

### 2. Configure Secrets

```powershell
cd src/EBI.ALAS.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=...;Database=ALASv2_DB;..."
dotnet user-secrets set "ConnectionStrings:WebLoanConnection" "Server=...;Database=webloan;..."
dotnet user-secrets set "Jwt:SecretKey" "your-32+-character-secret-key"
```

Optional for production:
```powershell
dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
dotnet user-secrets set "ConnectionStrings:RabbitMQ" "amqp://guest:guest@localhost:5672"
```

### 3. Run

```powershell
dotnet run --project src/EBI.ALAS.Api
```

API starts on:
- **HTTPS:** `https://localhost:7220`
- **HTTP:** `http://localhost:5173`

On first run, `DbInitializer` will:
1. Run EF Core migrations
2. Seed 31 branches across the Philippines
3. Seed default admin user (`admin` / `admin123`)
4. Seed loan products, approval matrix, deviation catalog

**Scalar API Reference:** `https://localhost:7220/scalar/v1` (Development)

## Configuration

All settings in `appsettings.json`, override via `appsettings.{Environment}.json` or environment variables.

### Key Settings

| Section | Purpose |
|---------|---------|
| `ConnectionStrings:DefaultConnection` | Main DB (read/write) |
| `ConnectionStrings:WebLoanConnection` | Legacy WebLoan DB (read-only) |
| `ConnectionStrings:Redis` | Distributed cache + SignalR backplane |
| `ConnectionStrings:RabbitMQ` | Message bus |
| `Jwt:SecretKey` | HMAC-SHA256 signing key (min 32 chars) |
| `Jwt:ExpiryMinutes` | Access token lifetime (default 15) |
| `Workflow:RequireRecommendation` | Skip recommendation step |

## API Endpoints

All endpoints return `ApiResponse<T>` envelope:

```json
{
  "success": true,
  "message": "Operation completed",
  "data": { ... },
  "errors": null
}
```

### Authentication `/api/auth`

| Method | Path | Rate-Limited | Description |
|--------|------|--------------|-------------|
| `POST` | `/api/auth/login` | Yes (5/60s) | Login → access token + refresh cookie |
| `POST` | `/api/auth/refresh` | No | Silent token rotation |
| `POST` | `/api/auth/logout` | No | Revoke tokens, clear cookie |
| `POST` | `/api/auth/change-password` | No | Change password, revoke all sessions |

### Users `/api/users`

| Method | Path | Policy | Description |
|--------|------|--------|-------------|
| `GET` | `/api/users` | `CanViewUsers` | Paged, filterable user list |
| `GET` | `/api/users/{id}` | `CanViewUsers` | Get user details |
| `POST` | `/api/users` | `CanCreateUsers` | Create user |
| `PUT` | `/api/users/{id}` | `CanEditUsers` | Update user |
| `PATCH` | `/api/users/{id}/status` | `CanSuspendUsers` | Suspend/activate user |

### Roles `/api/roles`

| Method | Path | Policy | Description |
|--------|------|--------|-------------|
| `GET` | `/api/roles` | `CanViewRoles` | List all roles |
| `GET` | `/api/roles/matrix` | `CanViewRoles` | Role × permission matrix |

### Branches `/api/branches`

| Method | Path | Policy | Description |
|--------|------|--------|-------------|
| `GET` | `/api/branches` | `CanViewUsers` | Paged branch list |
| `GET` | `/api/branches/all` | `CanViewUsers` | All branches (no paging) |
| `GET` | `/api/branches/{id}` | `CanViewUsers` | Get by numeric ID |
| `GET` | `/api/branches/code/{code}` | `CanViewUsers` | Get by branch code |

### Loans `/api/loans`

| Method | Path | Policy | Description |
|--------|------|--------|-------------|
| `GET` | `/api/loans` | `CanViewLoan` | Paged, filterable loan list |
| `GET` | `/api/loans/{id}` | `CanViewLoan` | Full loan detail |
| `POST` | `/api/loans` | `CanCreateLoan` | Create draft application |
| `PUT` | `/api/loans/{id}/status` | Workflow role | Transition loan status |
| `POST` | `/api/loans/{id}/cancel` | `CanCreateLoan` | Cancel own loan |

### Health

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| `GET` | `/health` | Public | Detailed health check |

## Security

### Authentication Flow

```
POST /api/auth/login → { accessToken (15 min), Set-Cookie: refresh_token (HttpOnly, Secure, SameSite=Strict, 7 days) }

POST /api/auth/refresh (cookie sent automatically) → { new accessToken, new refresh_token cookie }
```

### Authorization

16 granular permissions mapped to 5 roles:

| Role | Display Name | Workflow Stage |
|------|--------------|----------------|
| Encoder | Encoder (AO/CAA) | Creates and revises applications |
| Recommender | Recommender (Branch Head) | Reviews and recommends |
| Evaluator | Evaluator (Credit Checker) | Evaluates creditworthiness |
| Approver | Approver (Area Head) | Approves, rejects, requests revision |
| Admin | Administrator | Full access |

### Defense in Depth

- HTTPS enforced in non-Development
- CORS whitelist with credentials
- Security headers (CSP, X-Frame-Options, etc.)
- Rate limiting (login-specific + global)
- IP allowlisting for admin endpoints
- JWT Bearer authentication
- CSRF validation middleware
- Policy-based authorization
- Workflow validation (state machine)
- FluentValidation on every input
- Global exception handler (no stack traces in prod)
- Audit interceptor (auto-timestamps)
- Read-only interceptor (blocks WebLoan writes)
- Idempotency middleware
- Request body size limits (10MB)
- Kestrel hardening

## Loan Workflow

```
Draft → ForRecommendation* → ForChecking → ForApproval → Approved → ForDisbursement → Disbursed → OnGoing
               ↓              ↓             ↓           ↓            ↓
            Recommender   Evaluator      Approver     Admin        Admin
               ↓              ↓             ↓
            ForRevision ← ForRevision ← ForRevision
               ↓
            Encoder (revises)
```

* Skippable via `Workflow.RequireRecommendation`

## Development

```powershell
# Build
dotnet build

# Run
dotnet run --project src/EBI.ALAS.Api

# Hot reload
dotnet watch run --project src/EBI.ALAS.Api

# EF Core migrations
dotnet ef migrations add MyChange --project src/EBI.ALAS.Api
dotnet ef database update --project src/EBI.ALAS.Api

# Run tests
dotnet test
```

## License

Internal use only - Enterprise Bank Inc.