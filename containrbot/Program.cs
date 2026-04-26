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
const string botTokenSecretPath = "/run/secrets/bot_token";

var chatbot = Helpers.GetRequiredEnvironmentVariable(builder, "CHATBOT");
var token = File.Exists(botTokenSecretPath) ? File.ReadAllText(botTokenSecretPath).Trim() : throw new InvalidOperationException($"Secret file not set: bot_token.txt");
Helpers.GetRequiredEnvironmentVariable(builder, "CONTAINRBOTAPI_BASEURL");

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