#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Silo.Client/Silo.Client.csproj", "src/Silo.Client/"]
COPY ["src/Grains.Interfaces/Grains.Interfaces.csproj", "src/Grains.Interfaces/"]
RUN dotnet restore "src/Silo.Client/Silo.Client.csproj"
COPY . .
WORKDIR "/src/src/Silo.Client"
RUN dotnet build "Silo.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Silo.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Silo.Client.dll"]