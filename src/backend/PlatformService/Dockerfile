FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore \
    src/backend/PlatformService/PlatformService.WebApi/PlatformService.WebApi.csproj

RUN dotnet publish \
    src/backend/PlatformService/PlatformService.WebApi/PlatformService.WebApi.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8085
ENV ASPNETCORE_URLS=http://0.0.0.0:8085

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PlatformService.WebApi.dll"]