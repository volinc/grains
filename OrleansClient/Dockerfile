#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["OrleansClient/OrleansClient.csproj", "OrleansClient/"]
COPY ["Grains.Interfaces/Grains.Interfaces.csproj", "Grains.Interfaces/"]
RUN dotnet restore "OrleansClient/OrleansClient.csproj"
COPY . .
WORKDIR "/src/OrleansClient"
RUN dotnet build "OrleansClient.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrleansClient.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrleansClient.dll"]