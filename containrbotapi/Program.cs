using ContainrBot.Library;
using ContainrBotApi;
using ContainrBotApi.Models;
using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;
using JsonSerializer = System.Text.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);

var orchestratorVariable = Helpers.GetRequiredEnvironmentVariable(builder, "ORCHESTRATOR");
var containersRaw = Helpers.GetRequiredEnvironmentVariable(builder, "CONTAINER_LIST");
var containers = JsonSerializer.Deserialize<List<Container>>(containersRaw) ?? [];

builder.Services.AddValidation();

// Expose API endpoints with OpenApi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
	config.DocumentName = "ContainrBotApi";
	config.Title = "ContainrBotApi";
	config.Version = "v1";
});

switch (orchestratorVariable.ToLowerInvariant())
{
	case "kubernetes":
		builder.Services.AddScoped<IOrchestrator, KubernetesOrchestrator>();
		break;
	default:
		throw new InvalidOperationException();
}

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi(config =>
{
	config.DocumentTitle = "ContainrBotApi";
	config.Path = "/swagger";
	config.DocExpansion = "list";
});

app.UseDeveloperExceptionPage();

app.MapGet("/start/{container}", async (IOrchestrator orchestrator, string container) =>
	{
		try
		{
			if (!containers.Exists(e => e.FriendlyName == container))
			{
				return Results.BadRequest($"The container '{container}' is not a valid container.");
			}

			var currentContainer = containers.FirstOrDefault(g => g.FriendlyName == container);

			if (!await orchestrator.Exists(currentContainer))
			{
				return Results.BadRequest($"'{container}' doesn't seem to be deployed.");
			}

			await orchestrator.Start(currentContainer);

			return Results.Ok($"Successfully started {container}!");
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(ex.Message);
		}
	})
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.CanConnectToOrchestrator(invocationContext, next))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers));

app.MapGet("/stop/{container}", async (IOrchestrator orchestrator, string container) =>
	{
		try
		{
			if (!containers.Exists(e => e.FriendlyName == container))
			{
				return Results.BadRequest($"The container '{container}' is not a valid container.");
			}

			var currentContainer = containers.FirstOrDefault(g => g.FriendlyName == container);

			if (!await orchestrator.Exists(currentContainer))
			{
				return Results.BadRequest($"'{container}' doesn't seem to be deployed.");
			}

			await orchestrator.Stop(currentContainer);

			return Results.Ok($"Successfully stopped {container}!");
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(ex.Message);
		}
	})
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.CanConnectToOrchestrator(invocationContext, next))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers));

app.MapGet("/restart/{container}", async (IOrchestrator orchestrator, string container) =>
	{
		try
		{
			if (!containers.Exists(e => e.FriendlyName == container))
			{
				return Results.BadRequest($"The container '{container}' is not a valid container.");
			}

			var currentContainer = containers.FirstOrDefault(g => g.FriendlyName == container);

			if (!await orchestrator.Exists(currentContainer))
			{
				return Results.BadRequest($"'{container}' doesn't seem to be deployed.");
			}

			await orchestrator.Restart(currentContainer);

			return Results.Ok($"Successfully restarted {container}!");
		}
		catch (Exception ex)
		{
			return Results.InternalServerError(ex.Message);
		}
	})
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.CanConnectToOrchestrator(invocationContext, next))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers));

app.MapGet("/list", async (IOrchestrator orchestrator) =>
{
	try
	{
		var output = await orchestrator.List(containers);

		return Results.Ok(output);
	}
	catch (Exception ex)
	{
		return Results.InternalServerError(ex);
	}
}).AddEndpointFilter(async (invocationContext, next) =>
	await PreRequestMethods.CanConnectToOrchestrator(invocationContext, next));

app.MapGet("/debug", async (IOrchestrator orchestrator) =>
{
	try
	{
		var canConnect = false;
		Exception? connectionError = null;

		try
		{
			canConnect = await orchestrator.CanConnect();
		}
		catch (Exception ex)
		{
			connectionError = ex;
		}

		var output = new DebugResponse
		{
			IsOrchestratorAccessible = canConnect,
			OrchestratorConnectionError = connectionError?.Message,
			Containers = containers,
			ContainersStatus = await orchestrator.List(containers)
		};

		return Results.Ok(output);
	}
	catch (Exception ex)
	{
		return Results.InternalServerError(ex);
	}
});

app.Run();