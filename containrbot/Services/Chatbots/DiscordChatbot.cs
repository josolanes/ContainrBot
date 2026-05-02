using System.Text.Json;

using NetCord.Services.ApplicationCommands;

namespace ContainrBot.Services.Chatbots;

[SlashCommand(IChatbot.CommandName, IChatbot.CommandDescription)]
public class DiscordChatbot(
	IContainrBotApiService containrBotApiService) : ApplicationCommandModule<ApplicationCommandContext>, IChatbot
{
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