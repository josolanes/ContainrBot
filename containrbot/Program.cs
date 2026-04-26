using ContainrBot.Library;
using ContainrBot.Services;
using ContainrBot.Services.Chatbots;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = WebApplication.CreateBuilder(args);

// Fixed services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IContainrBotApiService, ContainrBotApiService>();

// Get required environment variables (will error if not set)
var chatbot = Helpers.GetRequiredEnvironmentVariable(builder, "CHATBOT");
Helpers.GetRequiredEnvironmentVariable(builder, "CONTAINRBOTAPI_BASEURL");
var token = GetBotToken();

// Conditional services
switch (chatbot.ToLowerInvariant())
{
	case "discord":
		builder.Services.AddSingleton<IChatbot, DiscordChatbot>();

		builder.Services
			.AddDiscordGateway(options => { options.Token = token; })
			.AddGatewayHandlers(typeof(Program).Assembly)
			.AddApplicationCommands();
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
	default:
		throw new InvalidOperationException();
}

// Start application
await host.RunAsync();

// private methods
string GetBotToken()
{
	const string botTokenSecretPathDocker = "/run/secrets/bot-token";
	const string botTokenSecretPathKubernetes = "/run/secrets/bot-token/bot-token";
	
	var token = "";
	if (File.Exists(botTokenSecretPathDocker))
	{
		token = File.ReadAllText(botTokenSecretPathDocker).Trim();
	}
	else if (File.Exists($"{botTokenSecretPathKubernetes}/bot-token"))
	{
		token = File.ReadAllText($"{botTokenSecretPathKubernetes}/bot-token").Trim();
	}
	else
	{
		throw new InvalidOperationException($"Secret file not set: bot-token");
	}

	return token;
}