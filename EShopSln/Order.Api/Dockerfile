# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Healthcheck için curl ekleyelim
RUN apt-get update \
 && apt-get install -y --no-install-recommends curl ca-certificates \
 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
# Order.Api dışarıya 3434 verecek
EXPOSE 3434

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Proje dosyalarını KÖK dizine göre kopyala (EShopSln/ öneki YOK)
COPY ["Order.Api/Order.Api.csproj", "Order.Api/"]
COPY ["Order.Application/Order.Application.csproj", "Order.Application/"]
COPY ["Order.Domain/Order.Domain.csproj", "Order.Domain/"]
COPY ["Order.Domain.Core/Order.Domain.Core.csproj", "Order.Domain.Core/"]
COPY ["Order.Infrastructure/Order.Infrastructure.csproj", "Order.Infrastructure/"]
COPY ["EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]

RUN dotnet restore "Order.Api/Order.Api.csproj"

# Tüm kaynakları kopyala ve publish et
COPY . .
WORKDIR /src/Order.Api
RUN dotnet publish "Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
# Container içinde 0.0.0.0:3434 dinle
ENV ASPNETCORE_URLS=http://0.0.0.0:3434
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Order.Api.dll"]
