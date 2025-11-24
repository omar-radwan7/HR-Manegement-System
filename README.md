# HR Management System

A production-ready Human Resources Management (HRM) web application built with ASP.NET Core and Microsoft SQL Server. This system provides comprehensive HR functionality including employee management, leave tracking, attendance monitoring, document management, and approval workflows.

## Project Overview

The HR Management System is designed to handle the complete employee lifecycle within an organization. It supports multi-branch operations with branch-scoped access control, ensuring that users can only access data relevant to their assigned branches. The system includes an approval engine that supports complex workflows with branch, department, and global-level rules.

### Key Features

**Employee Management**
- Complete employee profiles with personal information, contracts, and position assignments
- Branch and department organization
- Role-based access control with field-level masking for sensitive data

**Leave Management**
- Leave request creation and tracking
- Configurable approval workflows
- Leave balance tracking and validation
- Support for multiple leave types
- CSV import functionality with dry-run mode

**Attendance Tracking**
- Check-in and check-out functionality
- Server-side time validation
- Attendance record management
- CSV import with per-row error reporting

**Document Management**
- Document upload and storage
- PII tagging for compliance
- Retention policies and auto-purge
- Virus scanning hooks

**Approval Engine**
- Configurable approval rules at branch, department, and global levels
- Sequential and parallel approval steps
- Delegation and out-of-office substitution
- SLA monitoring with automatic escalation

**Reporting and Analytics**
- Headcount reports
- Attrition analysis
- Leave balance reports
- Absence tracking
- CSV export functionality

**Audit and Compliance**
- Comprehensive audit logging for all data changes
- Immutable audit trail with before/after snapshots
- Query and export capabilities

**Settings Management**
- Versioned settings with effective dates
- Environment-specific overrides

## Architecture

The application follows a clean layered architecture:

- **HRM.Api** - ASP.NET Core Web API layer containing controllers, middleware, and API configuration
- **HRM.Application** - Application layer with business services, DTOs, and application logic
- **HRM.Domain** - Domain layer with entities, enums, and domain models
- **HRM.Infrastructure** - Infrastructure layer with EF Core, Identity, Hangfire, Redis, and logging
- **HRM.Web** - Blazor Server frontend application

## Technology Stack

- **Backend**: ASP.NET Core 6.0 (C#)
- **Database**: Microsoft SQL Server
- **ORM**: Entity Framework Core with code-first migrations
- **Authentication**: ASP.NET Identity with JWT and cookie support
- **Frontend**: Blazor Server
- **Background Jobs**: Hangfire
- **Caching**: Redis (optional)
- **Logging**: Serilog with structured logging

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later (or Docker for containerized SQL Server)
- Docker and Docker Compose (optional, for containerized deployment)

## Quick Start

### One-Time Setup

1. Install EF Core tools (if not already installed):
```bash
dotnet tool install --global dotnet-ef --version 6.0.33
```

2. Start SQL Server using Docker:
```bash
docker-compose up sqlserver -d
```

3. Setup the database (creates and applies migrations):
```bash
./run.sh
```

### Running the Application

**Option 1: Run API Only**

Start the API server:
```bash
dotnet run --project HRM.Api
```

The API will be available at:
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

**Option 2: Run Web Application Only**

Start the Blazor Server application:
```bash
dotnet run --project HRM.Web
```

The web application will be available at http://localhost:5001

**Option 3: Run Both (Recommended for Development)**

Open two terminal windows:

Terminal 1 - API:
```bash
dotnet run --project HRM.Api
```

Terminal 2 - Web:
```bash
dotnet run --project HRM.Web
```

**Option 4: Docker Compose (All Services)**

Run all services including SQL Server, Redis, API, and Web:
```bash
docker-compose up
```

This will start:
- SQL Server on port 1433
- Redis on port 6379
- API on http://localhost:5000
- Web on http://localhost:5001

## Default Credentials

After running the initial setup, you can log in with:
- Email: `admin@hrm.com`
- Password: `Admin@123`

**Important**: Change the default password in production environments.

## API Usage

### Authentication

Most API endpoints require authentication. Include a JWT Bearer token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Branch Scope

All API requests must include the `X-Branch-Id` header with a valid branch GUID. The system validates that the user has access to the specified branch.

```
X-Branch-Id: <branch-guid>
```

### Key Endpoints

**Employees**
- `GET /api/employees` - List employees (branch-scoped)
- `GET /api/employees/{id}` - Get employee details
- `POST /api/employees` - Create new employee (Admin/HR Manager only)
- `PUT /api/employees/{id}` - Update employee (Admin/HR Manager only)
- `DELETE /api/employees/{id}` - Delete employee (Admin only)

**Leave Requests**
- `GET /api/leaverequests` - List leave requests
- `POST /api/leaverequests` - Create leave request
- `POST /api/leaverequests/{id}/approve` - Approve or reject leave request
- `GET /api/leaverequests/balance/{employeeId}/{leaveTypeId}` - Get leave balance

**Attendance**
- `POST /api/attendance/checkin` - Employee check-in
- `POST /api/attendance/checkout` - Employee check-out
- `GET /api/attendance` - Get attendance records

**Reports**
- `GET /api/reports/headcount` - Headcount report
- `GET /api/reports/attrition` - Attrition report
- `GET /api/reports/leave-balances` - Leave balances report
- `GET /api/reports/absence` - Absence report

**Audit Logs**
- `GET /api/audit-logs` - Query audit logs with filters
- `GET /api/audit-logs/export` - Export audit logs as CSV

## Database Migrations

To create a new migration:
```bash
dotnet ef migrations add MigrationName --project HRM.Infrastructure --startup-project HRM.Api
```

To apply migrations to the database:
```bash
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api
```

To generate a SQL script from migrations:
```bash
dotnet ef migrations script --project HRM.Infrastructure --startup-project HRM.Api
```

## Configuration

### Connection Strings

Update the connection string in `HRM.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=HRMDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"
  }
}
```

### JWT Settings

Configure JWT settings in `HRM.Api/appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here-minimum-32-characters",
    "Issuer": "HRM.Api",
    "Audience": "HRM.Web",
    "ExpirationMinutes": 60
  }
}
```

## Security Features

- Role-based access control (Admin, HR Manager, Manager, Employee)
- Branch-scoped data access via X-Branch-Id header validation
- Field-level masking for PII and salary information
- JWT token-based authentication
- Comprehensive audit logging
- No PII data in application logs

## Background Jobs

Hangfire dashboard is available at `/hangfire` when the API is running.

Scheduled jobs:
- Approval escalation (runs hourly)
- Document retention cleanup (runs daily)

## Testing

Run unit tests:
```bash
dotnet test HRM.Tests.Unit
```

## Troubleshooting

**Database Connection Issues**
- Ensure SQL Server is running and accessible
- Verify the connection string in appsettings.json
- Check that SQL Server allows TCP/IP connections
- For Docker: ensure the container is running with `docker ps`

**Migration Issues**
- Ensure the database exists
- Check user permissions
- Review migration history in the `__EFMigrationsHistory` table

**Build Errors**
- Run `dotnet restore` to restore NuGet packages
- Ensure .NET 6.0 SDK is installed
- Check that all project references are correct

## Project Structure

```
HRM System/
├── HRM.Api/              # API layer
├── HRM.Application/      # Application layer
├── HRM.Domain/           # Domain layer
├── HRM.Infrastructure/   # Infrastructure layer
├── HRM.Web/              # Blazor Server frontend
├── HRM.Tests.Unit/       # Unit tests
├── docker-compose.yml    # Docker orchestration
└── run.sh                # Database setup script
```

## License

This project is licensed under the MIT License.
