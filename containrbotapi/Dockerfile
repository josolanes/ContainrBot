FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY --from=node:slim /usr/local/bin /usr/local/bin
COPY --from=node:slim /usr/local/lib/node_modules /usr/local/lib/node_modules
COPY . .
RUN dotnet restore "containrbot.library/ContainrBot.Library.csproj"
RUN dotnet restore "containrbotapi/ContainrBotApi.csproj"
RUN dotnet restore "containrbotapi.tests/ContainrBotApi.Tests.csproj"
COPY . .
RUN dotnet build "containrbot.library/ContainrBot.Library.csproj" -c $configuration -o /app/build
RUN dotnet build "containrbotapi/ContainrBotApi.csproj" -c $configuration -o /app/build
COPY . .
RUN dotnet test "containrbotapi.tests/ContainrBotApi.Tests.csproj" --configuration Release --no-restore
COPY . .

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "containrbotapi/ContainrBotApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContainrBotApi.dll"]
