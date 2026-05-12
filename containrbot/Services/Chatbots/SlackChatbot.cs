using System.Text.Json;

using SlackNet.Interaction;
using SlackNet.WebApi;

namespace ContainrBot.Services.Chatbots;

public class SlackChatbot(
	IContainrBotApiService containrBotApiService) : ISlashCommandHandler, IChatbot
{
	public async Task<SlashCommandResponse> Handle(SlashCommand command)
	{
		var commandParts = command.Text.Split(' ',  StringSplitOptions.RemoveEmptyEntries);
		var commandName = commandParts[0];
		var containerName = commandParts.Length >= 2 ? commandParts[1] : string.Empty;

		return new SlashCommandResponse
		{
			Message = new Message
			{
				Text = commandName.ToLower() switch
				{
					IChatbot.ListName => await List(),
					IChatbot.StartName => await Start(containerName),
					IChatbot.StopName => await Stop(containerName),
					IChatbot.RestartName => await Restart(containerName),
					_ => "Invalid command provided"
				},
				Channel = command.ChannelName
			}
		};
	}
	
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