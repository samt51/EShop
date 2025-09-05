# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# $APP_UID tanımlı olmadığında build kırılmasın diye USER satırını kullanmıyoruz
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Proje dosyalarını EShopSln/ önekiyle (context: .) kopyala
COPY ["EShopSln/Catalog.Apii/Catalog.Apii.csproj", "Catalog.Apii/"]
COPY ["EShopSln/Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["EShopSln/Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["EShopSln/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "Catalog.Infrastructure/"]
COPY ["EShopSln/Catalog.Persistence/Catalog.Persistence.csproj", "Catalog.Persistence/"]
COPY ["EShopSln/EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]

RUN dotnet restore "Catalog.Apii/Catalog.Apii.csproj"

# Tüm kaynakları kopyala
COPY . .

# API klasörüne gir ve build et
WORKDIR /src/EShopSln/Catalog.Apii
RUN dotnet build "Catalog.Apii.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- PUBLISH ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/EShopSln/Catalog.Apii
RUN dotnet publish "Catalog.Apii.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Apii.dll"]
