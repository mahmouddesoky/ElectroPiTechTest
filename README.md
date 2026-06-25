# Project Task Management API

Backend .NET Developer technical assessment by Mahmoud Mostafa Desoky Ahmed.

## Overview

This solution implements a small project and task management backend using ASP.NET Core Web API, .NET 9, Clean Architecture, Entity Framework Core, SQL Server, JWT authentication, API versioning, validation, global exception handling, MediatR/CQRS, role-based authorization, Swagger, and unit tests.

## Architecture

- `ProjectTaskManagement.Domain`: core entities and enums.
- `ProjectTaskManagement.Application`: DTOs, contracts, CQRS commands/queries, validators, response wrapper, and business rules.
- `ProjectTaskManagement.Infrastructure`: EF Core DbContext, Identity, JWT generation, SQL Server configuration, and migrations.
- `ProjectTaskManagement.Api`: controllers, middleware, authentication, authorization, API versioning, and Swagger.
- `ProjectTaskManagement.Tests`: focused unit tests for handlers and validation.

Business rules are kept in the Application layer. Infrastructure implements persistence and identity details. The API layer only handles HTTP concerns and delegates work through MediatR.

## Prerequisites

- .NET SDK 9.0.303 or later compatible .NET 9 SDK.
- SQL Server instance available at `MDESOKY`.
- Windows Integrated Security enabled for the current user.

The repository includes `global.json` pinned to SDK `9.0.303`.

## Configuration

The default connection string is stored in:

- `src/ProjectTaskManagement.Api/appsettings.json`
- `src/ProjectTaskManagement.Api/appsettings.Development.json`

Current local value:

```json
"DefaultConnection": "Data Source=MDESOKY;Initial Catalog=ProjectTaskManagementDb;Integrated Security=True;TrustServerCertificate=True"
```

To change it later, edit the config files or override it with an environment variable:

```powershell
$env:ConnectionStrings__DefaultConnection="Data Source=YOUR_SERVER;Initial Catalog=ProjectTaskManagementDb;Integrated Security=True;TrustServerCertificate=True"
```

JWT settings are under the `Jwt` section. Replace the development secret before production use.

## Setup

Restore and build:

```powershell
dotnet restore
dotnet build
```

Apply migrations:

```powershell
dotnet ef database update --project src/ProjectTaskManagement.Infrastructure --startup-project src/ProjectTaskManagement.Api
```

Run tests:

```powershell
dotnet test
```

Run the API:

```powershell
dotnet run --project src/ProjectTaskManagement.Api --launch-profile http
```

Swagger:

```text
http://localhost:5113/swagger
```

In Development, the API also applies pending migrations and seeds the `User` and `Admin` roles at startup.

## Authentication Flow

Register:

```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "fullName": "Mahmoud Mostafa Desoky Ahmed",
  "email": "mahmoud@example.com",
  "password": "Password1"
}
```

Login:

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "mahmoud@example.com",
  "password": "Password1"
}
```

Use the returned JWT token in Swagger or HTTP clients:

```text
Authorization: Bearer <token>
```

Registered users receive the `User` role. Project and task endpoints require `User` or `Admin`.

## Main Endpoints

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/projects`
- `GET /api/v1/projects`
- `GET /api/v1/projects/{id}`
- `PUT /api/v1/projects/{id}`
- `DELETE /api/v1/projects/{id}`
- `POST /api/v1/projects/{projectId}/tasks`
- `GET /api/v1/projects/{projectId}/tasks`
- `PATCH /api/v1/tasks/{id}/status`
- `DELETE /api/v1/tasks/{id}`

## Notes

- Users can only access projects and tasks they own.
- API responses use a consistent generic wrapper.
- Validation is handled by FluentValidation through a MediatR pipeline.
- Unhandled errors, validation failures, not found cases, and authorization failures are returned through global exception handling.
- Enum values are serialized as readable strings in JSON and Swagger.
