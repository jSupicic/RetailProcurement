# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["src/Retail.Api/Retail.Api.csproj", "src/Retail.Api/"]
COPY ["src/Retail.Application/Retail.Application.csproj", "src/Retail.Application/"]
COPY ["src/Retail.Infrastructure/Retail.Infrastructure.csproj", "src/Retail.Infrastructure/"]
COPY ["src/Retail.Tests/Retail.Tests.csproj", "src/Retail.Tests/"]

RUN dotnet restore "src/Retail.Api/Retail.Api.csproj"

# Copy everything and build
COPY . .

# Build and publish the API
WORKDIR /src/src/Retail.Api
RUN dotnet publish "Retail.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Run tests (fail build if tests fail)
WORKDIR /src/src/Retail.Tests
RUN dotnet test --no-restore --no-build --logger:"trx;LogFileName=test_results.trx"

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Retail.Api.dll"]