FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BikeRental.Api/BikeRental.Api.csproj", "BikeRental.Api/"]
COPY ["BikeRental.Application/BikeRental.Application.csproj", "BikeRental.Application/"]
COPY ["BikeRental.Domain/BikeRental.Domain.csproj", "BikeRental.Domain/"]
COPY ["BikeRental.Infrastructure/BikeRental.Infrastructure.csproj", "BikeRental.Infrastructure/"]
RUN dotnet restore "./BikeRental.Api/BikeRental.Api.csproj"
COPY . .
WORKDIR "/src/BikeRental.Api"
RUN dotnet build "./BikeRental.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BikeRental.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BikeRental.Api.dll"]