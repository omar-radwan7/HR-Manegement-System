## Quick Start (simple commands by OS)

### 1. Prerequisites (all OS)
- **.NET 6 SDK**
- **Docker** and **docker-compose**
- **Node.js 18+ and npm** (for the React frontend)

Start SQL Server (same command on all OS):

```bash
cd "HRM System"
docker-compose up -d sqlserver
```

---

### 2. Linux / macOS

```bash
cd "HRM System"

# One-time: install EF tools
dotnet tool install --global dotnet-ef --version 6.0.33

# Create / update database
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api

# Terminal 1: run API
dotnet run --project HRM.Api --urls "http://localhost:5000"

# Terminal 2: run React frontend
cd hrm-frontend
npm install
npm run dev
```

Open:
- API Swagger: `http://localhost:5000/swagger`
- Frontend: Vite URL (usually `http://localhost:5173`)

---

### 3. Windows (PowerShell)

```powershell
cd "HRM System"

# One-time: install EF tools
dotnet tool install --global dotnet-ef --version 6.0.33

# Create / update database
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api

# Terminal 1: run API
dotnet run --project HRM.Api --urls "http://localhost:5000"

# Terminal 2: run React frontend
cd hrm-frontend
npm install
npm run dev
```

---

### 4. Optional: helper script (Linux only)

```bash
cd "HRM System"
./restart-all.sh
```

This script: kills old dotnet processes, recreates the database, applies migrations, and starts the API on `http://localhost:5000`.

