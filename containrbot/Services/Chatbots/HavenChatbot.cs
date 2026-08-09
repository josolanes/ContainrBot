using System.Text.Json;

using Haven.DotNet.Attributes;
using Haven.DotNet.Handlers;

namespace ContainrBot.Services.Chatbots;

public class HavenChatbot(
	IContainrBotApiService containrBotApiService) : IHavenDotNetHandler, IChatbot
{
	public async Task<string> Handle(string command, string args)
	{
		var commandParts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var commandName = commandParts[0];
		var containerName = commandParts.Length >= 2 ? commandParts[1] : string.Empty;

		return commandName.ToLower() switch
		{
			IChatbot.ListName => await List(),
			IChatbot.StartName => await Start(containerName),
			IChatbot.StopName => await Stop(containerName),
			IChatbot.RestartName => await Restart(containerName),
			_ => "Invalid command provided"
		};
	}
	
	[SubSlashCommand(IChatbot.ListName, IChatbot.ListDescription)]
	public async Task<string> List()
	{
		try
		{
			var containers = await containrBotApiService.ListContainers();

			return string.Join("\n", JsonSerializer.Deserialize<List<string>>(containers) ?? []);
		}
		catch (Exception ex)
		{
			return $"Unable to list containers: {ex.Message}";
		}
	}

	[SubSlashCommand(IChatbot.StartName, IChatbot.StartDescription)]
	public async Task<string> Start(string name)
	{
		try
		{
			var message = await containrBotApiService.StartContainers(name);

			return message.Trim('"');
		}
		catch (Exception ex)
		{
			return $"Unable to start container {name}: {ex.Message}";
		}
	}

	[SubSlashCommand(IChatbot.StopName, IChatbot.StopDescription)]
	public async Task<string> Stop(string name)
	{
		try
		{
			var message = await containrBotApiService.StopContainers(name);

			return message.Trim('"');
		}
		catch (Exception ex)
		{
			return $"Unable to stop container {name}: {ex.Message}";
		}
	}

	[SubSlashCommand(IChatbot.RestartName, IChatbot.RestartDescription)]
	public async Task<string> Restart(string name)
	{
		try
		{
			var message = await containrBotApiService.Restart(name);

			return message.Trim('"');
		}
		catch (Exception ex)
		{
			return $"Unable to restart container {name}: {ex.Message}";
		}
	}
}