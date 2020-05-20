FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["TestWebAPI/TestWebAPI.csproj", "TestWebAPI/"]
COPY ["HelloWorld.Interfaces/HelloWorld.Interfaces.csproj", "HelloWorld.Interfaces/"]

RUN dotnet restore "TestWebAPI/TestWebAPI.csproj"
COPY . .
WORKDIR "/src/TestWebAPI"
RUN dotnet build "TestWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "TestWebAPI.dll"]