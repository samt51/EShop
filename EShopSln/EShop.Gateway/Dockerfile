# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# curl (ve kök sertifikalar) yükle
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl ca-certificates \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY EShop.Gateway/*.csproj EShop.Gateway/
RUN dotnet restore EShop.Gateway/EShop.Gateway.csproj

COPY . .
WORKDIR /src/EShop.Gateway
RUN dotnet publish EShop.Gateway.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EShop.Gateway.dll"]
