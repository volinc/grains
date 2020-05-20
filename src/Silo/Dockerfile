#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 11111
EXPOSE 30000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Silo/Silo.csproj", "src/Silo/"]
COPY ["src/Grains.Interfaces/Grains.Interfaces.csproj", "src/Grains.Interfaces/"]
COPY ["src/Grains/Grains.csproj", "src/Grains/"]
RUN dotnet restore "src/Silo/Silo.csproj"
COPY . .
WORKDIR "/src/src/Silo"
RUN dotnet build "Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Silo.dll"]