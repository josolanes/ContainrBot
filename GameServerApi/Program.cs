using Microsoft.AspNetCore.JsonPatch;
using System.Text.Json;
using k8s;
using k8s.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

app.UseDeveloperExceptionPage();

app.Logger.LogInformation("Logging is working");

string gamesRaw = builder.Configuration.GetValue<string>("GAME_SERVER_LIST") ?? throw new InvalidOperationException("Environment variable not set: GAME_SERVER_LIST");
List<Game> games = JsonSerializer.Deserialize<List<Game>>(gamesRaw) ?? [];

app.Logger.LogInformation("Retrieved GAME_SERVER_LIST");

var k8sConfig = KubernetesClientConfiguration.InClusterConfig();
var client =  new Kubernetes(k8sConfig);

app.MapGet("/", () => "Hello World!");

app.MapGet("/start/{game}", (string game) =>
{
    try
    {
        if (!games.Exists(e => e.FriendlyName == game))
        {
            return Results.BadRequest($"The game '{game}' is not a valid game.");
        }

        var deployments = client.ListNamespacedDeployment(game);

        if (deployments?.Items.Count == 0)
        {
            return Results.BadRequest($"'{game}' doesn't seem to be deployed.");
        }

        var patch = new JsonPatchDocument<V1Scale>();
        patch = patch.Replace(e => e.Spec.Replicas, 1);

        var v1Patch = new V1Patch(patch, V1Patch.PatchType.JsonPatch);

        client.PatchNamespacedDeploymentScale(
            v1Patch,
            game,
            game);

        return Results.Ok($"Successfully started {game}!");
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
        if (!games.Exists(e => e.FriendlyName == game))
        {
            return Results.BadRequest($"The game '{game}' is not a valid game.");
        }

        var deployments = client.ListNamespacedDeployment(game);

        if (deployments?.Items.Count == 0)
        {
            return Results.BadRequest($"'{game}' doesn't seem to be deployed.");
        }

        var patch = new JsonPatchDocument<V1Scale>();
        patch = patch.Replace(e => e.Spec.Replicas, 0);

        var v1Patch = new V1Patch(patch, V1Patch.PatchType.JsonPatch);

        client.PatchNamespacedDeploymentScale(
            v1Patch,
            game,
            game);

        return Results.Ok($"Successfully stopped {game}!");
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
        List<string> output = [];
        
        for (int i = 0; i < games.Count(); i++)
        {
            var scale = client.ReadNamespacedDeploymentScale(games[i].DeployName, games[i].Namespace);
            var isRunning = scale.Spec.Replicas is > 1;
            
            output.Add($"{games[i].FriendlyName} is {(isRunning ? "running" : "not running")}");
        }

        return Results.Ok(output);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.Logger.LogInformation("Mapped endpoints");

app.Run();

struct Game
{
    public string FriendlyName { get; set; }

    public string DeployName { get; set; }
    
    public string Namespace { get; set; }
}