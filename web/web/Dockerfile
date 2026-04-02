#dotnet setup
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web/web/web.csproj", "web/web/"]
COPY ["DashLib/DashLib.csproj", "DashLib/"]
COPY ["DashComponents/DashComponents.csproj", "DashComponents/"]
COPY ["web/web.Client/web.Client.csproj", "web/web.Client/"]
RUN dotnet restore "./web/web/web.csproj"
COPY . .
WORKDIR "/src/web/web"
RUN dotnet build "./web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

USER $APP_UID
FROM base AS final
WORKDIR /app

EXPOSE 8080
EXPOSE 8443

USER $APP_UID
COPY --from=publish /app/publish .
COPY web/web/docker_entry.sh /docker_entry.sh

USER root
RUN mkdir -p ./data ./data/certs
RUN apt-get update -y
RUN apt-get install -y iputils-ping
RUN chmod 0777 ./appsettings.json
RUN chmod 0777 ./wwwroot
RUN chmod +x /docker_entry.sh
RUN chmod 0777 ./data ./data/certs

#finish publish
USER $APP_UID
ENTRYPOINT ["/docker_entry.sh"]