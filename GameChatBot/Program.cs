using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = WebApplication.CreateBuilder(args);

var token = builder.Configuration.GetValue<string>("BOT_TOKEN") ?? throw new InvalidOperationException("Environment variable not set: BOT_TOKEN");

builder.Services
    .AddDiscordGateway(new Action<GatewayClientOptions>(options =>
    {
        options.Token = token;
    }))
    .AddApplicationCommands();

var host = builder.Build();

host.AddModules(typeof(Program).Assembly);

await host.RunAsync();