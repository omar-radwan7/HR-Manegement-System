# HRM System

A production-ready Human Resources Management (HRM) web application built with ASP.NET Core and Microsoft SQL Server.

## Architecture

The application follows a layered architecture:

- **HRM.Api** - ASP.NET Core Web API project (controllers, middleware, startup)
- **HRM.Application** - Application layer (services, DTOs, interfaces, validators)
- **HRM.Domain** - Domain layer (entities, value objects, domain services, enums)
- **HRM.Infrastructure** - Infrastructure layer (EF Core, repositories, external services, Hangfire, Redis, Serilog)
- **HRM.Web** - Blazor Server project (pages, components, services)

## Features

### Core Modules
- **Employee Management** - Create, read, update, delete employees with branch-scoped access
- **Leave Management** - Leave requests with approval workflow, balance tracking, and CSV import
- **Attendance Tracking** - Check-in/out functionality with server-side validation and CSV import
- **Document Management** - Document upload with PII tagging and retention policies
- **Approval Engine** - Configurable approval workflows with branch/department-specific rules
- **Reports** - Headcount, attrition, leave balances, and absence reports with CSV export
- **Audit Logging** - Comprehensive audit trail for all data changes
- **Settings Management** - Versioned settings with environment overrides

### Security & Authorization
- ASP.NET Identity with JWT and cookie authentication
- Role-based access control (Admin, HR Manager, Manager, Employee)
- Branch-scoped access control via `X-Branch-Id` header
- Field-level masking for PII and salary information
- Audit logging for all create/update/delete operations

### Background Jobs
- Hangfire for SLA monitoring and approval escalation
- Document retention and auto-purge jobs

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server 2019 or later (or use Docker)
- Redis (optional, for caching)
- Docker and Docker Compose (for containerized deployment)

## Quick Start

### One-Time Setup
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef --version 6.0.33

# Setup database (creates migration and applies it)
./run.sh
```

### Run the Application

**Run API:**
```bash
dotnet run --project HRM.Api
```
Access Swagger at: http://localhost:5000/swagger

**Run Web App:**
```bash
dotnet run --project HRM.Web
```

**Or use Docker (all services):**
```bash
docker-compose up
```

## Detailed Setup

### Option 1: Using Docker Compose (Recommended)

1. Update connection strings in `docker-compose.yml` if needed
2. Run:
   ```bash
   docker-compose up -d
   ```
3. Access:
   - API: `http://localhost:5000`
   - Web: `http://localhost:5001`
   - Swagger: `http://localhost:5000/swagger`
   - Hangfire: `http://localhost:5000/hangfire`

### Option 2: Manual Setup

1. **Database Setup**
   - Create a SQL Server database named `HRMDB`
   - Update connection string in `HRM.Api/appsettings.json`

2. **Run Migrations**
   ```bash
   dotnet ef migrations add InitialCreate --project HRM.Infrastructure --startup-project HRM.Api
   dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api
   ```

3. **Run the API**
   ```bash
   dotnet run --project HRM.Api
   ```

4. **Run the Web Application** (in another terminal)
   ```bash
   dotnet run --project HRM.Web
   ```

## Default Credentials

- **Username**: admin@hrm.com
- **Password**: Admin@123

## API Documentation

### Authentication

All API endpoints (except authentication endpoints) require:
1. JWT Bearer token in the `Authorization` header
2. `X-Branch-Id` header with a valid branch GUID

### Key Endpoints

#### Employees
- `GET /api/employees` - List employees
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create employee (Admin/HR Manager only)
- `PUT /api/employees/{id}` - Update employee (Admin/HR Manager only)
- `DELETE /api/employees/{id}` - Delete employee (Admin/HR Manager only)

#### Leave Requests
- `GET /api/leaverequests` - List leave requests
- `POST /api/leaverequests` - Create leave request
- `POST /api/leaverequests/{id}/approve` - Approve/reject leave request
- `GET /api/leaverequests/balance/{employeeId}/{leaveTypeId}` - Get leave balance

#### Attendance
- `POST /api/attendance/checkin` - Check in
- `POST /api/attendance/checkout` - Check out
- `GET /api/attendance` - Get attendance records

#### Reports
- `GET /api/reports/headcount` - Headcount report
- `GET /api/reports/attrition` - Attrition report
- `GET /api/reports/leave-balances` - Leave balances report
- `GET /api/reports/absence` - Absence report

#### Audit Logs
- `GET /api/audit-logs` - Query audit logs
- `GET /api/audit-logs/export` - Export audit logs

## Database Migrations

To create a new migration:
```bash
dotnet ef migrations add MigrationName --project HRM.Infrastructure --startup-project HRM.Api
```

To generate SQL script:
```bash
dotnet ef migrations script --project HRM.Infrastructure --startup-project HRM.Api
```

To apply migrations:
```bash
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api
```

## Configuration

### appsettings.json

Key configuration sections:
- `ConnectionStrings:DefaultConnection` - SQL Server connection string
- `JwtSettings` - JWT token configuration
- `Redis:ConnectionString` - Redis connection string (optional)

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to `Development`, `Staging`, or `Production`
- `ConnectionStrings__DefaultConnection` - Override connection string

## Testing

### Unit Tests
```bash
dotnet test HRM.Tests.Unit
```

### E2E Tests
```bash
dotnet test HRM.Tests.E2E
```

## Background Jobs

Hangfire dashboard is available at `/hangfire` (configure authorization in production).

Recurring jobs:
- Approval escalation (runs every hour)
- Document retention (runs daily)

## Security Considerations

1. **Production Deployment**:
   - Change default admin password
   - Use strong JWT secret key
   - Enable HTTPS
   - Configure CORS properly
   - Secure Hangfire dashboard
   - Use environment-specific connection strings

2. **PII Protection**:
   - Field-level masking is enforced in DTOs
   - Audit logs exclude sensitive data
   - Document PII tagging for compliance

## Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string format
- Ensure SQL Server allows TCP/IP connections

### Migration Issues
- Ensure database exists
- Check user permissions
- Review migration history in `__EFMigrationsHistory` table

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository.

