# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece csproj'leri kopyala + restore (cache dostu)
COPY Payment.Api/Payment.Api.csproj Payment.Api/
COPY Payment.Application/Payment.Application.csproj Payment.Application/
COPY Payment.Persistence/Payment.Persistence.csproj Payment.Persistence/
COPY EShop.Shared/EShop.Shared.csproj EShop.Shared/
RUN dotnet restore Payment.Api/Payment.Api.csproj

# ÖNEMLİ: restore’dan SONRA tüm kaynakları kopyala
COPY . .

WORKDIR /src/Payment.Api
RUN dotnet publish Payment.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","Payment.Api.dll"]
