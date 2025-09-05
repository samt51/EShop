# --- RUNTIME IMAGE ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER satırını kaldırıyoruz; $APP_UID tanımlı değilse build kırılır
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD IMAGE ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ÇÖZÜM: Tüm .csproj kopyaları repo köküne göre (context = .) ve EShopSln/ önekiyle
COPY ["EShopSln/Basket.Api/Basket.Api.csproj", "Basket.Api/"]
COPY ["EShopSln/Basket.Persistence/Basket.Persistence.csproj", "Basket.Persistence/"]
COPY ["EShopSln/Basket.Application/Basket.Application.csproj", "Basket.Application/"]
COPY ["EShopSln/Basket.Domain/Basket.Domain.csproj", "Basket.Domain/"]
COPY ["EShopSln/EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]
COPY ["EShopSln/Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["EShopSln/Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
# Gerekirse diğer projeler de eklenebilir

RUN dotnet restore "Basket.Api/Basket.Api.csproj"

# Tüm kaynakları al
COPY . .

# Basket.Api klasörüne geç ve build et
WORKDIR /src/EShopSln/Basket.Api
RUN dotnet build "Basket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- PUBLISH ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/EShopSln/Basket.Api
RUN dotnet publish "Basket.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.Api.dll"]
