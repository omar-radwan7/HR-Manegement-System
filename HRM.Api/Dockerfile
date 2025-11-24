FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HRM.Api/HRM.Api.csproj", "HRM.Api/"]
COPY ["HRM.Application/HRM.Application.csproj", "HRM.Application/"]
COPY ["HRM.Domain/HRM.Domain.csproj", "HRM.Domain/"]
COPY ["HRM.Infrastructure/HRM.Infrastructure.csproj", "HRM.Infrastructure/"]
RUN dotnet restore "HRM.Api/HRM.Api.csproj"
COPY . .
WORKDIR "/src/HRM.Api"
RUN dotnet build "HRM.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HRM.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRM.Api.dll"]

