# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Healthcheck için curl ekleyelim
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl ca-certificates \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# Catalog.Apii dışarıya 5018 verecek
EXPOSE 5018

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Proje dosyalarını KÖK dizine göre kopyala (EShopSln/ öneki yok)
COPY ["Catalog.Apii/Catalog.Apii.csproj", "Catalog.Apii/"]
COPY ["Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog.Infrastructure/"]
COPY ["Catalog.Persistence/Catalog.Persistence.csproj", "Catalog.Persistence/"]
COPY ["EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]

RUN dotnet restore "Catalog.Apii/Catalog.Apii.csproj"

# Tüm kaynakları kopyala ve publish et
COPY . .
WORKDIR /src/Catalog.Apii
RUN dotnet publish "Catalog.Apii.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5018
EXPOSE 5018
ENTRYPOINT ["dotnet", "Catalog.Apii.dll"]
