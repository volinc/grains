FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 11112
EXPOSE 30002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CoHosted/CoHosted.csproj", "CoHosted/"]
COPY ["Grains.Interfaces/Grains.Interfaces.csproj", "Grains.Interfaces/"]
RUN dotnet restore "CoHosted/CoHosted.csproj"
COPY . .
WORKDIR "/src/CoHosted"
RUN dotnet build "CoHosted.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoHosted.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoHosted.dll"]