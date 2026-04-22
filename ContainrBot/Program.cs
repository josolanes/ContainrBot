using ContainrBot.Services;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IContainrBotApiService, ContainrBotApiService>();

builder.Services.AddHttpClient();

var token = builder.Configuration.GetValue<string>("BOT_TOKEN") ?? throw new InvalidOperationException("Environment variable not set: BOT_TOKEN");
var baseUrl = builder.Configuration.GetValue<string>("GAMESERVERAPI_BASEURL") ?? throw new InvalidOperationException("Environment variable not set: GAMESERVERAPI_BASEURL");

builder.Services
    .AddDiscordGateway(new Action<GatewayClientOptions>(options =>
    {
        options.Token = token;
    }))
    .AddGatewayHandlers(typeof(Program).Assembly)
    .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);

await host.RunAsync();