# Quick Start Commands

## One-Time Setup

```bash
# Install EF Core tools (only needed once)
dotnet tool install --global dotnet-ef --version 6.0.33

# Setup database
./run.sh
```

## Run the Application

### Option 1: Run API Only
```bash
dotnet run --project HRM.Api
```
Access: http://localhost:5000/swagger

### Option 2: Run Web App Only
```bash
dotnet run --project HRM.Web
```

### Option 3: Run Both (in separate terminals)
```bash
# Terminal 1
dotnet run --project HRM.Api

# Terminal 2
dotnet run --project HRM.Web
```

## Docker (All-in-One)
```bash
docker-compose up
```

## Default Login
- Email: `admin@hrm.com`
- Password: `Admin@123`

