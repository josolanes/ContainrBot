FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update && \
    apt-get install -y curl && \
    curl https://get.docker.com/builds/Linux/x86_64/docker-latest.tgz | tar xvz -C /tmp/ && \
    mv /tmp/docker/docker /usr/bin/docker
WORKDIR /app
EXPOSE 8080

ARG USER_UID=0
ARG USER_GID=0

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["GameServerApi/GameServerApi.csproj", "GameServerApi/"]
RUN dotnet restore "GameServerApi/GameServerApi.csproj"
COPY . .
WORKDIR "/src/GameServerApi"
RUN dotnet build "GameServerApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "GameServerApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameServerApi.dll"]
