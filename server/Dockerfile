# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0.102 AS restore

WORKDIR /workspace
COPY global.json ./
COPY server/Directory.Build.props server/Directory.Packages.props server/L2.Admin.slnx server/
COPY server/L2.Admin.Api/L2.Admin.Api.csproj server/L2.Admin.Api/
COPY server/L2.Admin.Api.Tests/L2.Admin.Api.Tests.csproj server/L2.Admin.Api.Tests/
COPY server/L2.Admin.Configurations/L2.Admin.Configurations.csproj server/L2.Admin.Configurations/
COPY server/L2.Admin.Configurations.Tests/L2.Admin.Configurations.Tests.csproj server/L2.Admin.Configurations.Tests/
COPY server/L2.Admin.Contracts/L2.Admin.Contracts.csproj server/L2.Admin.Contracts/
COPY server/L2.Admin.Exceptions/L2.Admin.Exceptions.csproj server/L2.Admin.Exceptions/
COPY server/L2.Admin.Repositories.Interfaces/L2.Admin.Repositories.Interfaces.csproj server/L2.Admin.Repositories.Interfaces/
COPY server/L2.Admin.Repositories/L2.Admin.Repositories.csproj server/L2.Admin.Repositories/
COPY server/L2.Admin.Repositories.Tests/L2.Admin.Repositories.Tests.csproj server/L2.Admin.Repositories.Tests/
RUN dotnet restore server/L2.Admin.slnx

FROM restore AS build

COPY server/ server/
RUN dotnet build server/L2.Admin.slnx --configuration Release --no-restore -m:1

FROM build AS publish

RUN dotnet publish server/L2.Admin.Api/L2.Admin.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    --property:UseAppHost=false

FROM publish AS validate

RUN dotnet test server/L2.Admin.slnx --configuration Release --no-build -m:1

FROM mcr.microsoft.com/dotnet/aspnet:10.0.2 AS production

WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
LABEL org.opencontainers.image.licenses="AGPL-3.0-only"
COPY --from=publish --chown=$APP_UID:$APP_UID /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "L2.Admin.Api.dll"]
