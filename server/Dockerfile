FROM mcr.microsoft.com/dotnet/sdk:10.0.102 AS publish

ARG PROJECT

WORKDIR /workspace
COPY . .
RUN dotnet publish "$PROJECT" --configuration Release --output /app/publish --property:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet"]
