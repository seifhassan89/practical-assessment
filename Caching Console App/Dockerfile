FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CachingConsoleApp.sln", "."]
COPY ["src/CacheImplementation.Core/CacheImplementation.Core.csproj", "src/CacheImplementation.Core/"]
COPY ["src/CacheImplementation.Infrastructure/CacheImplementation.Infrastructure.csproj", "src/CacheImplementation.Infrastructure/"]
COPY ["src/CacheImplementation.Console/CacheImplementation.Console.csproj", "src/CacheImplementation.Console/"]
RUN dotnet restore "CachingConsoleApp.sln"

COPY . .
RUN dotnet publish "src/CacheImplementation.Console/CacheImplementation.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

ENV DOTNET_EnableDiagnostics=0

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CacheImplementation.Console.dll"]
