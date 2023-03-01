FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY Server.csproj .
RUN dotnet restore "Server.csproj"
COPY . .
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV JWT_EXPIRE="8" DB_HOST="" DB_PASSWORD="yourSecurePassword"

ENTRYPOINT ["dotnet", "Server.dll"]
