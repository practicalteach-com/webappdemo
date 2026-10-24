# WebApp — Contact Manager

A contact management web application built with **ASP.NET Core 10 (Razor Pages)** and **Entity Framework Core**, featuring full CRUD operations for contacts and a complete **Azure DevOps CI/CD pipeline** that deploys to Azure App Service with automated database migrations. This simple web application is used to illustrate how to adopt git branching strategy which video is available in https://www.youtube.com/watch?v=xBK46CtMzjQ.

## Features

- **Contact CRUD** — list, create, edit, view details, and delete contact records
  - List: `/ContactPages`
  - Create / Edit / Details / Delete: under `/ContactPages` (linked from the list page)
- **Server-side validation** via data annotations on the `Contact` model:

| Field | Rules |
|---|---|
| `Name` | max 100 characters |
| `Phone` | must match `^0[0-9]+`, max 20 characters |
| `Email` | valid email format, max 100 characters |
| `Instagram` | max 200 characters |
| `Address` | max 255 characters |

- **Database migrations** managed with EF Core (migrations are committed to the repo and applied automatically in CI/CD)
- **CI/CD pipeline** — build, package, deploy to Test and Production environments on Azure App Service

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime / SDK | .NET 10 (`net10.0`) |
| Web framework | ASP.NET Core Razor Pages |
| ORM | Entity Framework Core 10.x (SQL Server provider) |
| Database | SQL Server (LocalDB for local development, Azure SQL in CI/CD) |
| Frontend | Bootstrap 5, jQuery, jquery-validation (bundled locally in `wwwroot/lib`) |
| CI/CD | Azure DevOps Pipelines (`azure-pipelines.yaml`) |
| Hosting | Azure App Service (`webappdemotest` / `webappdemoprod`) |

## Project Structure

```
.
├── azure-pipelines.yaml            # CI/CD pipeline (build → package → deploy)
└── WebApp/
    ├── Program.cs                  # App entry point: DI setup, request pipeline
    ├── ContactDbContext.cs         # EF Core DbContext (single DbSet<Contact>)
    ├── Model/
    │   └── Contact.cs              # Contact entity with validation attributes
    ├── Migrations/                 # Committed EF Core migrations
    ├── Pages/
    │   ├── Index.cshtml            # Home page
    │   ├── Privacy.cshtml          # Privacy page
    │   ├── Error.cshtml            # Error handler (non-development)
    │   ├── Shared/                 # Layout, validation scripts partial
    │   └── ContactPages/           # CRUD pages: Index, Create, Edit, Delete, Details
    ├── Properties/
    │   └── launchSettings.json     # Local URLs (http :5215 / https :7277)
    ├── appsettings.json            # Connection string + logging config
    ├── appsettings.Development.json
    └── wwwroot/lib/                # Bootstrap, jQuery, validation libraries
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/latest)
- SQL Server or LocalDB (for local development — the default connection string targets `localdb\mssqllocaldb`)
- [EF Core CLI tools](https://learn.microsoft.com/ef/core/cli/index) (`dotnet ef`) for managing migrations

## Getting Started

### 1. Clone and restore

```bash
git clone <repo-url>
cd WebApp
dotnet restore
```

### 2. Configure the database connection (if needed)

The default development connection string in `WebApp/appsettings.json` points to LocalDB:

```json
"ConnectionStrings": {
    "ContactDbContext": "Server=(localdb)\\mssqllocaldb;Database=ContactDbContext;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Update it if you use a different SQL Server instance.

### 3. Apply database migrations

```bash
dotnet ef database update --project WebApp/WebApp.csproj
```

### 4. Run the app

```bash
cd WebApp
dotnet run
```

Then open:

- **HTTP:** http://localhost:5215
- **HTTPS:** https://localhost:7277

## Working with Migrations

Migrations are committed to `WebApp/Migrations/`. To create a new one after changing the model:

```bash
dotnet ef migrations add <MigrationName> --project WebApp/WebApp.csproj
dotnet ef database update --project WebApp/WebApp.csproj
```

## CI/CD Pipeline (`azure-pipelines.yaml`)

The pipeline runs on an `ubuntu-latest` agent and is triggered by pushes to `main` and `release/*`.

| Stage | What it does | Runs when |
|---|---|---|
| **Build** | `dotnet restore` + `dotnet build` (Release) | Every trigger |
| **Package** | `dotnet publish`, then generates an EF Core **migration bundle** (`efbundle`) for `linux-x64` and publishes the artifact | Not on pull requests |
| **Deploy_Test** | Deploys to Azure App Service `webappdemotest`: runs network diagnostics, executes the migration bundle against the test database (via Workload Identity Federation), sets app settings & connection strings from the `WebAppVarGroup` variable group (`TestDbConnectionString`) | Merges to `main` |
| **Deploy_Production** | Same deployment flow to `webappdemoprod`, using `ProdDbConnectionString` | Branches matching `release/*` |

### Pipeline variables / secrets (Azure DevOps)

- Variable group: `WebAppVarGroup` — provides `TestDbConnectionString` and `ProdDbConnectionString`
- Service connection: `WebAppServiceConn` — Azure subscription with a Workload Identity Federation-enabled service principal (used to run the EF migration bundle via `az account get-access-token`)

## Configuration

| Setting | Location | Notes |
|---|---|---|
| Connection string (`ContactDbContext`) | `appsettings.json` / app settings in Azure | Overridden per environment by the pipeline |
| Logging levels | `appsettings.json` → `Logging:LogLevel` | Default `Information`, ASP.NET Core `Warning` |
| Detailed errors (dev only) | `appsettings.Development.json` | Enabled in Development |
| Local URLs | `Properties/launchSettings.json` | http :5215, https :7277 |

## Notes

- In non-development environments the app uses a global exception handler (`/Error`) and HSTS.
- The pipeline's Package stage skips on pull requests; Test deployment is skipped for PRs as well (build validation only).
