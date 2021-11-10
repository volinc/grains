#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["OrleansHost/OrleansHost.csproj", "OrleansHost/"]
COPY ["Grains.Interfaces/Grains.Interfaces.csproj", "Grains.Interfaces/"]
COPY ["Grains/Grains.csproj", "Grains/"]
RUN dotnet restore "OrleansHost/OrleansHost.csproj"
COPY . .
WORKDIR "/src/OrleansHost"
RUN dotnet build "OrleansHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrleansHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrleansHost.dll"]