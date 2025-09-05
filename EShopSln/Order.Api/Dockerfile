# --- RUNTIME ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# $APP_UID tanımlı olmayınca build kırılmaması için USER satırını yorumladım
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Bağımlı projeleri kopyala
COPY ["EShopSln/Order.Api/Order.Api.csproj", "Order.Api/"]
COPY ["EShopSln/Order.Application/Order.Application.csproj", "Order.Application/"]
COPY ["EShopSln/Order.Domain/Order.Domain.csproj", "Order.Domain/"]
COPY ["EShopSln/Order.Domain.Core/Order.Domain.Core.csproj", "Order.Domain.Core/"]
COPY ["EShopSln/Order.Infrastructure/Order.Infrastructure.csproj", "Order.Infrastructure/"]
COPY ["EShopSln/EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]

RUN dotnet restore "Order.Api/Order.Api.csproj"

# Kaynak kodların tamamını kopyala
COPY . .

# Build et
WORKDIR /src/EShopSln/Order.Api
RUN dotnet build "Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- PUBLISH ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/EShopSln/Order.Api
RUN dotnet publish "Order.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- FINAL ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.Api.dll"]
