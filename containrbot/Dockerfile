FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY . .
RUN dotnet restore "containrbot.library/ContainrBot.Library.csproj"
RUN dotnet restore "containrbot/ContainrBot.csproj"
RUN dotnet restore "containrbot.tests/ContainrBot.Tests.csproj"
COPY . .
RUN dotnet build "containrbot.library/ContainrBot.Library.csproj" -c $configuration -o /app/build
RUN dotnet build "containrbot/ContainrBot.csproj" -c $configuration -o /app/build
COPY . .
RUN dotnet test "containrbot.tests/ContainrBot.Tests.csproj" --configuration Release --no-restore
COPY . .

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "containrbot/ContainrBot.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContainrBot.dll"]
