#!/bin/bash

# HRM System - Quick Start Script

echo " Starting HRM System Setup..."

# Install EF Core tools if not already installed
if ! command -v dotnet-ef &> /dev/null; then
    echo " Installing EF Core tools..."
    dotnet tool install --global dotnet-ef --version 6.0.33
fi

# Create migration
echo " Creating database migration..."
dotnet ef migrations add InitialCreate --project HRM.Infrastructure --startup-project HRM.Api

# Apply migration
echo "🗄️  Applying migrations..."
dotnet ef database update --project HRM.Infrastructure --startup-project HRM.Api

echo " Setup complete! Run 'dotnet run --project HRM.Api' to start the API"

