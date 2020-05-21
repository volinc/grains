FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["docker-compose/docker_compose.csproj", "docker-compose/"]

RUN dotnet restore "docker-compose/docker_compose.csproj"
COPY . .
WORKDIR "/src/docker-compose"
RUN dotnet build "docker_compose.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "docker_compose.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "docker_compose.dll"]