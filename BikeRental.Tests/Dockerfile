FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BikeRental.Tests/BikeRental.Tests.csproj", "BikeRental.Tests/"]
COPY ["BikeRental.Api/BikeRental.Api.csproj", "BikeRental.Api/"]
COPY ["BikeRental.Application/BikeRental.Application.csproj", "BikeRental.Application/"]
COPY ["BikeRental.Domain/BikeRental.Domain.csproj", "BikeRental.Domain/"]
COPY ["BikeRental.Infrastructure/BikeRental.Infrastructure.csproj", "BikeRental.Infrastructure/"]

RUN dotnet restore "BikeRental.Tests/BikeRental.Tests.csproj"

COPY . .

RUN dotnet build "BikeRental.Tests/BikeRental.Tests.csproj" -c Debug -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "test", "BikeRental.Tests.dll", "--logger:console;verbosity=detailed"]