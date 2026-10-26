# Vertex CRM — Backend API Summary

## What This Is

A **ASP.NET Core Web API** (.NET 10) serving as the backend for Vertex CRM. It handles authentication (register, login, token refresh, logout) using **JWT access tokens** + **HttpOnly cookie refresh tokens**. The database is **PostgreSQL** accessed via **Entity Framework Core**.

---

## Tech Stack

| Layer | Tech |
|---|---|
| Framework | ASP.NET Core (.NET 10) |
| Language | C# |
| Database | PostgreSQL |
| ORM | Entity Framework Core 10 + Npgsql |
| Auth | JWT Bearer + ASP.NET Core Identity (password hashing only) |
| API Docs | Swagger / OpenAPI (Swashbuckle 10) |

---

## Project Structure

```
saas-crm-api/
├── Program.cs                     ← App bootstrap, DI, JWT config, middleware pipeline
├── VertexCRM.csproj               ← NuGet packages, targets net10.0
├── appsettings.json               ← DB connection string, JWT settings
│
├── Controllers/
│   └── AuthController.cs          ← POST /api/auth/{register,login,refresh-token,logout}
│
├── Services/
│   └── AuthService.cs             ← All auth business logic
│
├── Interfaces/
│   └── IAuthService.cs            ← Service contract (RegisterUser, LoginUser, RefreshToken, LogoutUser)
│
├── Entities/
│   ├── BaseEntity.cs              ← Id (int PK), ChangedBy, CreatedOn
│   ├── User.cs                    ← Users table
│   └── RefreshToken.cs            ← RefreshTokens table
│
├── Data/
│   └── ApplicationDbContext.cs    ← EF DbContext (Users, RefreshTokens)
│
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequestDTO.cs     ← { Email, Password }
│   │   ├── RegisterRequestDTO.cs  ← { FirstName, LastName, Email, Password }
│   │   ├── AuthResponseDTO.cs     ← { UserId, FirstName, LastName, Email, AccessToken, RefreshToken }
│   │   ├── RefreshTokenRequestDTO.cs  ← (unused — refresh token now comes from cookie)
│   │   └── LogoutRequestDTO.cs    ← (unused — logout token now comes from cookie)
│   └── Common/
│       └── CommonResponseDTO.cs   ← { status, message, data? }
│
├── Exceptions/
│   └── AppException.cs            ← Custom exception with HttpStatusCode + safe message
│
├── Middleware/
│   └── GlobalExceptionMiddleware.cs ← Catches all exceptions, returns JSON error responses
│
├── Constants/
│   └── ResponseConstants.cs       ← "success" / "failed" string constants
│
└── Migrations/
    ├── 20260920070646_InitialMigration.cs   ← Creates Users table
    └── 20261006062818_AddRefreshTokens.cs   ← Creates RefreshTokens table
```

---

## Database Schema

### `Users`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, auto-increment |
| FirstName | varchar(200) | Required |
| LastName | varchar(200) | Required |
| Email | varchar(200) | Required |
| PhoneNumber | varchar(20) | Nullable |
| UserStatus | int | Enum: Pending=0, Active=1, Inactive=2 |
| PasswordHash | text | Hashed via ASP.NET Identity PasswordHasher |
| LastLoginAt | timestamptz | Nullable, updated on each login |
| IsActive | bool | Default true |
| ChangedBy | text | Nullable (from BaseEntity) |
| CreatedOn | timestamptz | Nullable (from BaseEntity) |

### `RefreshTokens`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK, auto-increment |
| UserId | int | FK → Users.Id (cascade delete) |
| Token | varchar(512) | 64 cryptographically random bytes, Base64 encoded |
| ExpiresAt | timestamptz | 7 days from creation |
| RevokedAt | timestamptz | Nullable — set on revoke/logout |
| ChangedBy | text | Nullable |
| CreatedOn | timestamptz | Nullable |

`IsRevoked` is a computed property (`RevokedAt.HasValue`), not a DB column.

---

## API Endpoints

All endpoints are under `/api/auth`.

### `POST /api/auth/register`
- Body: `{ firstName, lastName, email, password }`
- Creates a new user (status: Pending), hashes password
- Returns `201` with user info (no tokens — user must log in separately)
- Throws `409 Conflict` if email already exists

### `POST /api/auth/login`
- Body: `{ email, password }`
- Verifies password hash, checks `IsActive`
- Issues a **15-minute JWT access token**
- Issues a **7-day refresh token**, stored as an **HttpOnly cookie** (`refreshToken`, path `/api/auth`)
- Returns `200` with `{ userId, firstName, lastName, email, accessToken }` (refresh token is NOT in the body)

### `POST /api/auth/refresh-token`
- No body — reads `refreshToken` from the HttpOnly cookie
- Validates: token exists in DB, not revoked, not expired, user is active
- **Rotates** the refresh token: revokes the old one, issues a new one
- Returns a new `accessToken` + sets a new `refreshToken` cookie

### `POST /api/auth/logout`
- No body — reads `refreshToken` cookie
- Revokes the token in DB (sets `RevokedAt`)
- Deletes the `refreshToken` cookie
- Idempotent: already-revoked or missing tokens cause no error

---

## Auth Flow Details

- **Access token lifetime**: 15 minutes, JWT (HS256)
- **Refresh token lifetime**: 7 days, stored in DB
- **Refresh token rotation**: every `/refresh-token` call revokes the old token and issues a brand new one
- **Token expiry signal**: when a JWT is expired, the server sets the `X-Token-Expired: true` response header — the frontend uses this to trigger a silent refresh
- **Refresh token transport**: HttpOnly cookie (not accessible to JS), scoped to `/api/auth` path only
- **Password hashing**: ASP.NET Identity `PasswordHasher<User>` (PBKDF2)
- **JWT claims**: `sub` (userId), `email`, `name` (full name)

---

## Error Handling

All errors go through `GlobalExceptionMiddleware`:

- `AppException` → returns the status code and message directly to the client (these are intentional, safe errors)
- Any other exception → returns `500` with a generic message (internal details never leak)
- Response shape is always `CommonResponseDTO`: `{ status: "failed", message: "...", data: null }`

---

## Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=vertexcrm;Username=postgres;Password=123456"
  },
  "Jwt": {
    "Key": "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET_KEY",
    "Issuer": "VertexCRM.Api",
    "Audience": "VertexCRM.Client"
  }
}
```

> ⚠️ The JWT key in `appsettings.json` is a placeholder. It must be replaced with a strong secret (32+ chars) before any real use. In production this should come from an environment variable or secrets manager, NOT committed to source control.

---

## What Has Been Built

- ✅ Full auth module: register, login, refresh token, logout
- ✅ JWT access tokens (15-min expiry) + rotating HttpOnly cookie refresh tokens
- ✅ Password hashing (PBKDF2 via ASP.NET Identity)
- ✅ `X-Token-Expired` header signal for silent token refresh
- ✅ `AppException` pattern for clean, typed error handling
- ✅ `GlobalExceptionMiddleware` — all errors return consistent JSON, internals never leak
- ✅ EF Core with PostgreSQL, 2 migrations applied (Users + RefreshTokens)
- ✅ Swagger UI available in development (`/swagger`)
- ✅ `CommonResponseDTO` — consistent `{ status, message, data }` wrapper on all responses

---

## What Is NOT Done Yet

- ❌ No CORS configuration — the frontend (`localhost:3000`) will be blocked in the browser
- ❌ No `forgot-password` / `reset-password` endpoints (the frontend already has those UI pages)
- ❌ No CRM entities at all: Companies, Contacts, Leads, Deals, Tasks are not modeled yet
- ❌ No user profile endpoint (`GET /api/users/me` or similar)
- ❌ No `[Authorize]` on any endpoint yet — all routes are currently publicly accessible
- ❌ `UserStatus` is always set to `Pending` on register and never updated to `Active`
- ❌ `RefreshTokenRequestDTO` and `LogoutRequestDTO` exist but are unused (the controller reads from cookies instead — these can be deleted)
- ❌ No rate limiting on auth endpoints
- ❌ No email sending (forgot password flow has no backend support)
- ❌ JWT key is a hardcoded placeholder — needs a real secret before any deployment

---

## Mismatch with Frontend

One known integration issue between the frontend and this API:

- The register page in the frontend calls `data.token` on the response, but this API returns `data.data.accessToken` — and actually returns **no token at all** from `/register` (only from `/login`). The register flow needs to be fixed on the frontend side.
