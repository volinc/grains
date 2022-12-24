FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
EXPOSE 11111
EXPOSE 30001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SelfHosted/SelfHosted.csproj", "SelfHosted/"]
COPY ["Grains.Interfaces/Grains.Interfaces.csproj", "Grains.Interfaces/"]
COPY ["Grains/Grains.csproj", "Grains/"]
RUN dotnet restore "SelfHosted/SelfHosted.csproj"
COPY . .
WORKDIR "/src/SelfHosted"
RUN dotnet build "SelfHosted.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SelfHosted.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SelfHosted.dll"]