# --- RUNTIME IMAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Healthcheck'ler için curl ekle
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl ca-certificates \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# Basket.Api dışarıya 5151 verecek
EXPOSE 5151

# --- BUILD IMAGE ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# .csproj dosyalarını KÖK dizine göre kopyala
COPY ["Basket.Api/Basket.Api.csproj", "Basket.Api/"]
COPY ["Basket.Application/Basket.Application.csproj", "Basket.Application/"]
COPY ["Basket.Persistence/Basket.Persistence.csproj", "Basket.Persistence/"]
COPY ["Basket.Infrastructure/Basket.Infrastructure.csproj", "Basket.Infrastructure/"]
COPY ["Basket.Domain/Basket.Domain.csproj", "Basket.Domain/"]
COPY ["EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]
# Eğer referans varsa aç:
# COPY ["Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
# COPY ["Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]

RUN dotnet restore "Basket.Api/Basket.Api.csproj"

# Tüm kaynakları kopyala ve publish et
COPY . .
WORKDIR /src/Basket.Api
RUN dotnet publish "Basket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
# Container içinde 0.0.0.0:5151 dinle
ENV ASPNETCORE_URLS=http://0.0.0.0:5151
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Basket.Api.dll"]
