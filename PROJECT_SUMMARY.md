# Vertex CRM — Backend API Summary

## What This Is

A **ASP.NET Core Web API** (.NET 10) serving as the backend for Vertex CRM. It handles authentication (register, login, token refresh, logout, forgot/reset password) using **JWT access tokens** + **HttpOnly cookie refresh tokens**. The database is **PostgreSQL** accessed via **Entity Framework Core**. The project is now multi-tenant — each workspace is isolated under a `Tenant`.

---

## Tech Stack

| Layer | Tech |
|---|---|
| Framework | ASP.NET Core (.NET 10) |
| Language | C# |
| Database | PostgreSQL |
| ORM | Entity Framework Core 10 + Npgsql |
| Auth | JWT Bearer + ASP.NET Core Identity (password hashing only) |
| Email | MailKit / MimeKit |
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
│   └── AuthController.cs          ← POST /api/auth/{register,login,refresh-token,logout,forgot-password,reset-password}
│
├── Services/
│   ├── AuthService.cs             ← All auth business logic
│   └── EmailService.cs            ← SMTP email sending via MailKit
│
├── Interfaces/
│   ├── IAuthService.cs            ← Service contract
│   └── IEmailService.cs           ← Email service contract
│
├── Entities/
│   ├── BaseEntity.cs              ← Id (int PK), ChangedBy, CreatedOn
│   ├── User.cs                    ← Users table (now with TenantId, RoleId)
│   ├── RefreshToken.cs            ← RefreshTokens table
│   ├── PasswordResetToken.cs      ← PasswordResetTokens table
│   ├── Tenant.cs                  ← Tenants table (multi-tenant workspaces)
│   ├── Role.cs                    ← Roles table (Platform or Tenant scoped)
│   ├── Company.cs                 ← Companies table (CRM core entity, per-tenant)
│   └── UserCompany.cs             ← Join table: users ↔ companies
│
├── Configurations/
│   ├── RoleConfiguration.cs       ← EF config for Roles + HasData seed (Member, Tenant Admin, Platform Admin)
│   ├── UserConfiguration.cs       ← EF config for Users (column sizes, unique email index, FK behaviors)
│   ├── TenantConfiguration.cs     ← EF config for Tenants (explicit nav wiring for CreatedByUser + Users)
│   ├── CompanyConfiguration.cs    ← EF config for Companies (indexes, FKs, soft delete)
│   └── TenantUserConfiguration.cs ← Stale — references removed TenantUser entity; should be deleted
│
├── Data/
│   └── ApplicationDbContext.cs    ← EF DbContext (Users, RefreshTokens, PasswordResetTokens, Tenants, Companies, UserCompanies, Roles)
│
├── DTOs/
│   ├── Auth/
│   │   ├── LoginRequestDTO.cs
│   │   ├── RegisterRequestDTO.cs
│   │   ├── AuthResponseDTO.cs
│   │   ├── ForgotPasswordRequestDTO.cs
│   │   ├── ResetPasswordRequestDTO.cs
│   │   ├── RefreshTokenRequestDTO.cs  ← (unused — cookie-based)
│   │   └── LogoutRequestDTO.cs        ← (unused — cookie-based)
│   └── Common/
│       └── CommonResponseDTO.cs   ← { status, message, data? }
│
├── Exceptions/
│   └── AppException.cs            ← Custom exception with HttpStatusCode + safe message
│
├── Middleware/
│   └── GlobalExceptionMiddleware.cs ← Catches all exceptions, returns consistent JSON
│
├── Constants/
│   ├── ResponseConstants.cs       ← "success" / "failed" string constants
│   ├── RoleConstants.cs           ← Seeded role names + stable IDs (Member=1, TenantAdmin=2, PlatformAdmin=3)
│   ├── UserStatus.cs              ← Pending=1, Active=2, Suspended=3, Deactivated=4
│   ├── TenantStatus.cs            ← Active=1, Suspended=2, Closed=3
│   └── RoleScope.cs               ← Platform=1, Tenant=2
│
└── Migrations/
    ├── 20260920070646_InitialMigration.cs              ← Creates Users table
    ├── 20261006062818_AddRefreshTokens.cs              ← Creates RefreshTokens table
    ├── 20261023053157_AddPasswordResetTokens.cs        ← Creates PasswordResetTokens table
    ├── 20261103191236_AddTenantFoundationAndCompanies.cs ← Creates Tenants, Companies, TenantUsers; adds TenantId/PlatformRole to Users
    └── 20261104080706_AddRolesAndSeedData.cs           ← Creates Roles (seeded), UserCompanies; adds RoleId FK; drops TenantUsers/PlatformRole; makes TenantId nullable
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
| Status | int | Enum: Pending=1, Active=2, Suspended=3, Deactivated=4 |
| PasswordHash | text | Hashed via ASP.NET Identity PasswordHasher |
| LastLoginAt | timestamptz | Nullable, updated on each login |
| IsActive | bool | Default true |
| TenantId | int? | FK → Tenants.Id (nullable — platform admins have no tenant) |
| RoleId | int | FK → Roles.Id |
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

### `PasswordResetTokens`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| UserId | int | FK → Users.Id |
| Token | text | 64 random bytes, Base64 encoded |
| ExpiresAt | timestamptz | 15 minutes from creation |
| IsUsed | bool | Marked true once consumed |
| CreatedOn | timestamptz | Nullable |

### `Tenants`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | varchar(200) | Required |
| Slug | varchar(100) | Required, unique index |
| Status | int | Enum: Active=1, Suspended=2, Closed=3 |
| CreatedByUserId | int | FK → Users.Id (restrict) |
| UpdatedOn | timestamptz | Updated on changes |
| ChangedBy | text | Nullable |
| CreatedOn | timestamptz | Nullable |

### `Roles`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| Name | varchar | Required |
| Description | text | Nullable |
| Scope | int | Enum: Platform=1, Tenant=2 |

### `Companies`
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| TenantId | int | FK → Tenants.Id (restrict) |
| Name | varchar(200) | Required |
| LegalName | varchar(250) | Nullable |
| Website | varchar(500) | Nullable |
| Industry | varchar(100) | Nullable |
| PhoneNumber | varchar(30) | Nullable |
| Email | varchar(254) | Nullable |
| Description | text | Nullable |
| AddressLine1 | varchar(250) | Nullable |
| AddressLine2 | varchar(250) | Nullable |
| City | varchar(100) | Nullable |
| StateOrProvince | varchar(100) | Nullable |
| PostalCode | varchar(20) | Nullable |
| CountryCode | varchar(2) | Nullable (ISO 3166-1 alpha-2) |
| CreatedByUserId | int | FK → Users.Id (restrict) |
| UpdatedByUserId | int? | FK → Users.Id (restrict) |
| UpdatedOn | timestamptz | Updated on changes |
| IsDeleted | bool | Default false (soft delete) |
| DeletedOn | timestamptz | Nullable |
| DeletedByUserId | int? | FK → Users.Id (restrict) |

Indexes: `(TenantId, IsDeleted, Name)` and `(TenantId, CreatedOn)`.

### `UserCompanies` (join table)
| Column | Type | Notes |
|---|---|---|
| Id | int | PK |
| UserId | int | FK → Users.Id |
| CompanyId | int | FK → Companies.Id |
| IsActive | bool | Default true |
| AssignedOn | timestamptz | Set on assignment |
| AssignedByUserId | int | Who assigned this user |

---

## API Endpoints

All auth endpoints are under `/api/auth`.

### `POST /api/auth/register`
- Body: `{ firstName, lastName, email, password }`
- Creates a new user (status: Pending), hashes password
- Returns `201` with user info (no tokens — user must log in separately)
- Throws `409 Conflict` if email already exists

### `POST /api/auth/login`
- Body: `{ email, password }`
- Verifies password hash, checks `IsActive`
- Issues a **15-minute JWT access token** (includes `tenant_id` claim)
- Issues a **7-day refresh token**, stored as an **HttpOnly cookie** (`refreshToken`, path `/api/auth`)
- Returns `200` with `{ userId, firstName, lastName, email, accessToken }`

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

### `POST /api/auth/forgot-password`
- Body: `{ email }`
- Always returns `200` (no email enumeration leak)
- Invalidates any existing unused reset tokens for the user
- Generates a new 15-minute reset token and sends it via email

### `POST /api/auth/reset-password`
- Body: `{ token, password }`
- Validates the reset token (not used, not expired)
- Updates the password hash
- Marks the reset token as used
- Revokes all active refresh tokens for the user (terminates all sessions)

---

## Auth Flow Details

- **Access token lifetime**: 15 minutes, JWT (HS256)
- **JWT claims**: `sub` (userId), `email`, `name` (full name), `tenant_id`
- **Refresh token lifetime**: 7 days, stored in DB
- **Refresh token rotation**: every `/refresh-token` call revokes the old token and issues a brand new one
- **Token expiry signal**: expired JWT → server sets `X-Token-Expired: true` response header for the frontend to trigger a silent refresh
- **Refresh token transport**: HttpOnly cookie, scoped to `/api/auth` path only
- **Password hashing**: ASP.NET Identity `PasswordHasher<User>` (PBKDF2)
- **Password reset**: 15-min expiring token sent via email; resets password + revokes all sessions

---

## Error Handling

All errors go through `GlobalExceptionMiddleware`:

- `AppException` → returns the status code and message directly to the client (intentional, safe errors)
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

> ⚠️ The JWT key in `appsettings.json` is a placeholder. Replace it with a strong 32+ char secret. In production use environment variables or a secrets manager — never commit the real key.

---

## What Has Been Built

- ✅ Full auth module: register, login, refresh token, logout, forgot password, reset password
- ✅ JWT access tokens (15-min, HS256) with `tenant_id` claim + rotating HttpOnly cookie refresh tokens
- ✅ Password hashing (PBKDF2 via ASP.NET Identity)
- ✅ `X-Token-Expired` header signal for silent token refresh
- ✅ Forgot/reset password flow with secure 15-min expiring tokens + email delivery (MailKit)
- ✅ Password reset revokes all active sessions
- ✅ `AppException` pattern for clean, typed error handling
- ✅ `GlobalExceptionMiddleware` — all errors return consistent JSON, internals never leak
- ✅ EF Core with PostgreSQL, 5 migrations applied
- ✅ Swagger UI available in development (`/swagger`)
- ✅ `CommonResponseDTO` — consistent `{ status, message, data }` wrapper on all responses
- ✅ Multi-tenant foundation: `Tenant`, `Role`, `RoleScope` entities modeled and migrated
- ✅ `Roles` table seeded with 3 default roles: Member (id=1), Tenant Admin (id=2), Platform Admin (id=3)
- ✅ New registrations assigned `RoleId = 1` (Member) automatically — FK violation fixed
- ✅ `RoleConstants.cs` — stable IDs and name constants; no magic strings
- ✅ `Company` entity with full address, soft-delete, audit fields, and tenant isolation
- ✅ `UserCompany` join table for assigning users to companies
- ✅ EF Fluent configurations for `Role`, `User`, `Tenant`, `Company`

---

## What Is NOT Done Yet

- ❌ No CORS configuration — the frontend (`localhost:3000`) will be blocked in the browser
- ❌ No Company CRUD endpoints — the entity and migration exist but no controller/service yet
- ❌ No Tenant provisioning endpoint — tenants can't be created via the API yet
- ❌ No user profile endpoint (`GET /api/users/me` or similar)
- ❌ No `[Authorize]` on any endpoint yet — all routes are currently publicly accessible
- ❌ `UserStatus` is always set to `Pending` on register and never updated to `Active`
- ❌ `RefreshTokenRequestDTO` and `LogoutRequestDTO` exist but are unused (cookie-based) — can be deleted
- ❌ No rate limiting on auth endpoints
- ❌ JWT key is a hardcoded placeholder — needs a real secret before any deployment
- ❌ `TenantUserConfiguration` references a removed `TenantUser` entity — this file should be deleted

---

## Known Issues / Mismatches

- **`TenantUserConfiguration`** — references `TenantUser` entity and `Tenant.Members` which no longer exist. The `TenantUsers` table was replaced by `UserCompanies`. This configuration file is dead code and should be removed to avoid confusion.
- **Frontend integration**: The register page calls `data.token` on the response, but `/register` returns no token — the frontend needs to redirect to `/login` after registration.
