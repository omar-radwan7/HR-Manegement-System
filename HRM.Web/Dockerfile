FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HRM.Web/HRM.Web.csproj", "HRM.Web/"]
COPY ["HRM.Application/HRM.Application.csproj", "HRM.Application/"]
COPY ["HRM.Domain/HRM.Domain.csproj", "HRM.Domain/"]
COPY ["HRM.Infrastructure/HRM.Infrastructure.csproj", "HRM.Infrastructure/"]
RUN dotnet restore "HRM.Web/HRM.Web.csproj"
COPY . .
WORKDIR "/src/HRM.Web"
RUN dotnet build "HRM.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HRM.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HRM.Web.dll"]

