using ContainrBot.Library;
using ContainrBot.Services;
using ContainrBot.Services.Chatbots;

using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

using SlackNet.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Get required environment variables (will error if not set)
var chatbot = Helpers.GetRequiredEnvironmentVariable(builder, "CHATBOT");
var containrbotapiUrl = Helpers.GetRequiredEnvironmentVariable(builder, "CONTAINRBOTAPI_BASEURL");
var token = GetBotToken();

builder.Services.AddHttpClient<IContainrBotApiService, ContainrBotApiService>("containrbotapi",
	client => client.BaseAddress = new Uri(containrbotapiUrl));

// Conditional services
switch (chatbot.ToLowerInvariant())
{
	case "discord":
		builder.Services.AddScoped<IChatbot, DiscordChatbot>();

		builder.Services
			.AddDiscordGateway(options => options.Token = token)
			.AddGatewayHandlers(typeof(Program).Assembly)
			.AddApplicationCommands();
		break;
	case "slack":
		builder.Services.AddSlackNet(options => options
			.UseAppLevelToken(token)
			.RegisterSlashCommandHandler<SlackChatbot>($"/{IChatbot.CommandName}"));
		break;
	default:
		throw new InvalidOperationException();
}

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddEventSourceLogger();

// Build application
var host = builder.Build();

// Custom services
switch (chatbot.ToLowerInvariant())
{
	case "discord":
		host.AddModules(typeof(Program).Assembly);
		break;
	case "slack":
		var client = host.Services.SlackServices().GetSocketModeClient();
		await client.Connect();
		break;
	default:
		throw new InvalidOperationException();
}

// Start application
await host.RunAsync();

// private methods
static string GetBotToken()
{
	const string botTokenSecretPathDocker = "/run/secrets/bot-token";
	const string botTokenSecretPathKubernetes = "/run/secrets/bot-token/bot-token";

	var botToken = "";
	if (File.Exists(botTokenSecretPathDocker))
	{
		botToken = File.ReadAllText(botTokenSecretPathDocker).Trim();
	}
	else if (File.Exists(botTokenSecretPathKubernetes))
	{
		botToken = File.ReadAllText(botTokenSecretPathKubernetes).Trim();
	}
	else
	{
		throw new InvalidOperationException("Secret file not set: bot-token");
	}

	return botToken;
}