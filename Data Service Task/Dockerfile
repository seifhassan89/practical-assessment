# ── Stage 1: build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore dependencies first (layer-cached unless .csproj files change).
COPY ["DataService.Api/DataService.Api.csproj", "DataService.Api/"]
COPY ["Directory.Build.props", "."]
RUN dotnet restore "DataService.Api/DataService.Api.csproj"

# Copy remaining source and build in Release mode.
COPY . .
WORKDIR "/src/DataService.Api"
RUN dotnet build "DataService.Api.csproj" -c Release --no-restore -o /app/build

# ── Stage 2: publish ──────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "DataService.Api.csproj" -c Release --no-restore -o /app/publish /p:UseAppHost=false

# ── Stage 3: runtime image ────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=publish /app/publish .
RUN mkdir -p /app/logs && chown -R $APP_UID:$APP_UID /app

# Run as the built-in non-root user provided by the .NET runtime image.
USER $APP_UID

ENTRYPOINT ["dotnet", "DataService.Api.dll"]
