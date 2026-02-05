
FROM mcr.microsoft.com/dotnet/aspnet:8.0 as base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY tesis /src
WORKDIR /src
RUN ls
RUN dotnet restore
RUN dotnet build "tesis.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "tesis.csproj" -c Debug -o /app/publish /p:EnvironmentName=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "tesis.dll"]
