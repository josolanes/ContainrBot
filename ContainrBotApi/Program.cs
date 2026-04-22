using Microsoft.AspNetCore.JsonPatch;
using System.Text.Json;
using k8s;
using k8s.Models;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ContainrBotApi";
    config.Title = "ContainrBotApi";
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

string containersRaw = builder.Configuration.GetValue<string>("CONTAINER_LIST") ?? throw new InvalidOperationException("Environment variable not set: CONTAINER_LIST");
List<Container> containers = JsonSerializer.Deserialize<List<Container>>(containersRaw) ?? [];

app.Logger.LogInformation("Retrieved CONTAINER_LIST");

var k8sConfig = KubernetesClientConfiguration.InClusterConfig();
var client =  new Kubernetes(k8sConfig);

app.MapGet("/", () => "Hello World!");

app.MapGet("/start/{container}", (string container) =>
{
    try
    {
        if (!containers.Exists(e => e.FriendlyName == container))
        {
            return Results.BadRequest($"The container '{container}' is not a valid container.");
        }

        var currentContainer = containers.FirstOrDefault(g => g.FriendlyName == container);

        var deployments = client.ListNamespacedDeployment(currentContainer.Namespace);

        if (deployments?.Items.Count == 0)
        {
            return Results.BadRequest($"'{container}' doesn't seem to be deployed.");
        }

        var patch = new JsonPatchDocument<V1Scale>();
        patch.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        patch.Replace(e => e.Spec.Replicas, 1);
        
        var jsonPatchString = Newtonsoft.Json.JsonConvert.SerializeObject(patch);

        var v1Patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);

        client.AppsV1.PatchNamespacedDeploymentScale(
            v1Patch,
            currentContainer.DeployName,
            currentContainer.Namespace);

        return Results.Ok($"Successfully started {container}!");
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
});

app.MapGet("/stop/{container}", (string container) =>
{
    try
    {
        if (!containers.Exists(e => e.FriendlyName == container))
        {
            return Results.BadRequest($"The container '{container}' is not a valid container.");
        }
        
        var currentContainer = containers.FirstOrDefault(g => g.FriendlyName == container);

        var deployments = client.ListNamespacedDeployment(currentContainer.Namespace);

        if (deployments?.Items.Count == 0)
        {
            return Results.BadRequest($"'{container}' doesn't seem to be deployed.");
        }
        
        var patch = new JsonPatchDocument<V1Scale>();
        patch.ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        patch.Replace(e => e.Spec.Replicas, 0);
        
        var jsonPatchString = Newtonsoft.Json.JsonConvert.SerializeObject(patch);

        var v1Patch = new V1Patch(jsonPatchString, V1Patch.PatchType.JsonPatch);

        client.AppsV1.PatchNamespacedDeploymentScale(
            v1Patch,
            currentContainer.DeployName,
            currentContainer.Namespace);

        return Results.Ok($"Successfully stopped {container}!");
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
        
        for (int i = 0; i < containers.Count(); i++)
        {
            try
            {
                var scale = client.ReadNamespacedDeploymentScale(containers[i].DeployName, containers[i].Namespace);
                var isRunning = scale.Spec.Replicas is > 0;
                
                output.Add($"{containers[i].FriendlyName} is {(isRunning ? "running" : "not running")}");
            }
            catch
            {
                output.Add($"{containers[i].FriendlyName} is not deployed");
            }
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

struct Container
{
    public string FriendlyName { get; set; }

    public string DeployName { get; set; }
    
    public string Namespace { get; set; }
}