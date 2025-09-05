FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Catalog.Apii/Catalog.Apii.csproj", "Catalog.Apii/"]
COPY ["Catalog.Application/Catalog.Application.csproj", "Catalog.Application/"]
COPY ["Catalog.Domain/Catalog.Domain.csproj", "Catalog.Domain/"]
COPY ["EShop.Shared/EShop.Shared.csproj", "EShop.Shared/"]
COPY ["Catalog.Persistence/Catalog.Persistence.csproj", "Catalog.Persistence/"]
RUN dotnet restore "Catalog.Apii/Catalog.Apii.csproj"
COPY . .
WORKDIR "/src/Catalog.Apii"
RUN dotnet build "Catalog.Apii.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Catalog.Apii.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Apii.dll"]
