using ContainrBot.Library;

using ContainrBotApi;
using ContainrBotApi.Models;
using ContainrBotApi.Models.Internal;
using ContainrBotApi.Orchestrators;

using JsonSerializer = System.Text.Json.JsonSerializer;

var builder = WebApplication.CreateBuilder(args);

var orchestratorVariable = Helpers.GetRequiredEnvironmentVariable(builder, "ORCHESTRATOR")?.ToLowerInvariant();
var containersRaw = Helpers.GetRequiredEnvironmentVariable(builder, "CONTAINER_LIST");
var containers = JsonSerializer.Deserialize<List<Container>>(containersRaw) ?? [];

var orchestrators = AppDomain.CurrentDomain.GetAssemblies()
	.SelectMany(s => s.GetTypes())
	.Where(p => typeof(IOrchestrator).IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false })
	.Select(o => o.Name.Replace("Orchestrator", string.Empty).ToLower());

builder.Services.AddValidation();
builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.WriteIndented = true; });

// Expose API endpoints with OpenApi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
	config.DocumentName = "ContainrBotApi";
	config.Title = "ContainrBotApi";
	config.Version = "v1";
});

builder.Services.AddScoped<IOrchestrator>(sp =>
	orchestratorVariable?.ToLower() switch
	{
		"kubernetes" => new KubernetesOrchestrator(),
		"docker" => new DockerOrchestrator(),
		_ => throw new InvalidOperationException(
			"Environment variable `ORCHESTRATOR` is invalid. Valid values are: `kubernetes`, `docker`")
	});

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

app.MapGet("/health", () => Results.Ok(new
{
	status = "pass"
}));

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
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.IsValidContainer(invocationContext, next, containers));

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
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.IsValidContainer(invocationContext, next, containers));

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
		await PreRequestMethods.RequestedContainerInEnvVar(invocationContext, next, containers))
	.AddEndpointFilter(async (invocationContext, next) =>
		await PreRequestMethods.IsValidContainer(invocationContext, next, containers));

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
			CurrentOrchestrator = orchestratorVariable ?? "***NOT SET***",
			SupportedOrchestrators = [.. orchestrators],
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