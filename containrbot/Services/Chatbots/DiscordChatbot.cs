using System.Text.Json;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ContainrBot.Services.Chatbots;

[SlashCommand(IChatbot.CommandName, IChatbot.CommandDescription)]
public class DiscordChatbot(
	IContainrBotApiService containrBotApiService) : ApplicationCommandModule<ApplicationCommandContext>, IChatbot
{
	[SubSlashCommand(IChatbot.ListName, IChatbot.ListDescription)]
	public async Task<string> List()
	{
		await Context.Interaction.SendResponseAsync(InteractionCallback.Message(IChatbot.InProgressMessage));

		var output = "";

		try
		{
			output = await containrBotApiService.ListContainers();

			var successMessage = string.Join("\n", JsonSerializer.Deserialize<List<string>>(output) ?? []);

			await Context.Interaction.SendFollowupMessageAsync(successMessage);

			return successMessage;
		}
		catch
		{
			var failedMessage = $"Unable to retrieve the containers list: {output}";

			await Context.Interaction.SendFollowupMessageAsync(failedMessage);

			return failedMessage;
		}
	}

	[SubSlashCommand(IChatbot.StartName, IChatbot.StartDescription)]
	public async Task<string> Start(string name)
	{
		await Context.Interaction.SendResponseAsync(InteractionCallback.Message(IChatbot.InProgressMessage));

		var message = await containrBotApiService.StartContainers(name);
		message = message.Trim('"');

		await Context.Interaction.SendFollowupMessageAsync(message);

		return message;
	}

	[SubSlashCommand(IChatbot.StopName, IChatbot.StopDescription)]
	public async Task<string> Stop(string name)
	{
		await Context.Interaction.SendResponseAsync(InteractionCallback.Message(IChatbot.InProgressMessage));

		var message = await containrBotApiService.StopContainers(name);
		message = message.Trim('"');

		await Context.Interaction.SendFollowupMessageAsync(message);

		return message;
	}
	
	[SubSlashCommand(IChatbot.RestartName, IChatbot.RestartDescription)]
	public async Task<string> Restart(string name)
	{
		await Context.Interaction.SendResponseAsync(InteractionCallback.Message(IChatbot.InProgressMessage));

		var message = await containrBotApiService.Restart(name);
		message = message.Trim('"');

		await Context.Interaction.SendFollowupMessageAsync(message);

		return message;
	}
}