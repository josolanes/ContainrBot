using System.Text.Json;
using System.Text.Json.Serialization;
using GameServerApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IDockerService, DockerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "GameServerApi";
    config.Title = "GameServerApi";
    config.Version = "v1";
});

builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.Logger.LogInformation("Logging is working");

string gamesRaw = builder.Configuration.GetValue<string>("GAME_SERVER_LIST") ?? throw new InvalidOperationException("Environment variable not set: GAME_SERVER_LIST");
Dictionary<string, string> games = JsonSerializer.Deserialize<List<Game>>(gamesRaw)?.ToDictionary(k => k.FriendlyName, v => v.ContainerName) ?? [];

app.Logger.LogInformation("Retrieved GAME_SERVER_LIST");

var dockerService = app.Services.GetRequiredService<IDockerService>();

app.MapGet("/", () => "Hello World!");

app.MapGet("/start/{game}", (string game) =>
{
    try
    {
        var gameNames = GetGameKeys();

        if (!gameNames.Contains(game))
        {
            return Results.BadRequest($"The game '{game}' is not a valid game.");
        }

        string containerId = dockerService.GetContainerId(games[game]);

        if (String.IsNullOrEmpty(containerId))
        {
            return Results.BadRequest($"Container doesn't exist for '{game}'.");
        }

        dockerService.StartContainer(containerId);

        return Results.Ok("Success");
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
});

app.MapGet("/stop/{game}", (string game) =>
{
    try
    {
        var gameNames = GetGameKeys();

        if (!gameNames.Contains(game))
        {
            return Results.BadRequest($"The game '{game}' is not a valid game.");
        }

        string containerId = dockerService.GetContainerId(games[game]);

        if (String.IsNullOrEmpty(containerId))
        {
            return Results.BadRequest($"Container doesn't exist for '{game}'.");
        }

        dockerService.StopContainer(containerId);

        return Results.Ok("Success");
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
});

app.MapGet("/list", () =>
{
    try
    {
        var gameNames = GetGameKeys();

        return Results.Ok(gameNames);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.Logger.LogInformation("Mapped endpoints");

app.Run();

IEnumerable<string> GetGameKeys()
{
    return games.Keys.Select(k => k);
}

struct Game
{
    public string FriendlyName { get; set; }

    public string ContainerName { get; set; }
}