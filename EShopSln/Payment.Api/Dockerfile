# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER $APP_UID  # Default yoksa hata verir, istersen açabilirsin
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Bağımlı projeleri kopyala
COPY ["EShopSln/Payment.Api/Payment.Api.csproj", "Payment.Api/"]
COPY ["EShopSln/Payment.Application/Payment.Application.csproj", "Payment.Application/"]
COPY ["EShopSln/EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]

RUN dotnet restore "Payment.Api/Payment.Api.csproj"

# Tüm kaynakları kopyala
COPY . .

# Build et
WORKDIR /src/EShopSln/Payment.Api
RUN dotnet build "Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- PUBLISH ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/EShopSln/Payment.Api
RUN dotnet publish "Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Api.dll"]
