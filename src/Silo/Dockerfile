FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
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

EXPOSE 11111
EXPOSE 30000

ENTRYPOINT ["dotnet", "SiloHost.dll"]