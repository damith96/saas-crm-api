# VertexCRM API

ASP.NET Core Web API backend for the Vertex CRM platform.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (see `VertexCRM.csproj` for target framework)

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Run in development
dotnet run
```

The API will be available at the URL shown in the console (configured in `Properties/launchSettings.json`).

## Project Structure

```
saas-crm-api/
├── Properties/
│   └── launchSettings.json   # Launch profiles (ports, env)
├── appsettings.json           # Base configuration
├── appsettings.Development.json
├── Program.cs                 # App entry point & middleware setup
└── VertexCRM.csproj           # Project file
```

## Configuration

App settings are managed via `appsettings.json`. Sensitive values (connection strings, API keys) should be stored using [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) in development:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "your-connection-string"
```

## Testing the API

Use the included `.http` file (`VertexCRM.http`) with the VS Code REST Client extension or Visual Studio's built-in HTTP client.
