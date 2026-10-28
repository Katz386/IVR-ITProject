FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore \
    src/backend/TasksService/TasksService.WebApi/TasksService.WebApi.csproj

RUN dotnet publish \
    src/backend/TasksService/TasksService.WebApi/TasksService.WebApi.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8082
ENV ASPNETCORE_URLS=http://0.0.0.0:8082

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TasksService.WebApi.dll"]