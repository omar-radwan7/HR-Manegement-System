#!/bin/bash
set -e

echo "=== HRM System Complete Restart ==="
echo

# 1. Kill all dotnet processes
echo "1. Stopping all running processes..."
pkill -9 -f "dotnet" 2>/dev/null || true
pkill -9 -f "HRM" 2>/dev/null || true
sleep 3

# 2. Clean build artifacts
echo "2. Cleaning build artifacts..."
cd "/home/oradwan/Desktop/AIC Internship Projects/HRM System"
find . -name "bin" -o -name "obj" | xargs rm -rf 2>/dev/null || true

# 3. Drop and recreate database
echo "3. Recreating database..."
docker-compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "DROP DATABASE IF EXISTS HRMDB;" -C || true
docker-compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong@Passw0rd' -Q "CREATE DATABASE HRMDB;" -C || true

# 4. Rebuild solution
echo "4. Building solution..."
dotnet build

# 5. Apply migrations
echo "5. Applying EF Core migrations..."
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api --no-build

# 6. Start API
echo "6. Starting API on http://localhost:5000..."
cd HRM.Api
dotnet run --urls "http://localhost:5000" --no-build > /tmp/hrm-api.log 2>&1 &
echo "API PID: $!"
cd ..

echo
echo "=== Done! ==="
echo "API is starting at http://localhost:5000"
echo "Swagger UI: http://localhost:5000/swagger"
echo "API logs: tail -f /tmp/hrm-api.log"
echo
echo "To start the React frontend:"
echo "  cd hrm-frontend"
echo "  npm run dev"

