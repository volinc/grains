FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 11111
EXPOSE 30000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SiloHost/SiloHost.csproj", "SiloHost/"]
COPY ["HelloWorld.Interfaces/HelloWorld.Interfaces.csproj", "HelloWorld.Interfaces/"]
COPY ["HelloWorld.Grains/HelloWorld.Grains.csproj", "HelloWorld.Grains/"]

RUN dotnet restore "SiloHost/SiloHost.csproj"
COPY . .
WORKDIR "/src/SiloHost"
RUN dotnet build "SiloHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SiloHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SiloHost.dll"]