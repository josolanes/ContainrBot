using System.Text.Json;
using System.Text.Json.Serialization;
using ContainrBot.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace ContainrBot.Modules;

[SlashCommand("containr", "Containr commands!")]
public class ContainrCommandModule(
    IContainrBotApiService containrBotApiService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("list", "List of available container")]
    public async Task<string> ListContainers()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        var output = containrBotApiService.ListContainers(out bool status);

        string message = status ? string.Join("\n", JsonSerializer.Deserialize<List<string>>(output) ?? []) : $"Unable to retrieve the containers list: {output}";

        await Context.Interaction.SendFollowupMessageAsync(message);

        return message;
    }

    [SubSlashCommand("start", "Start a container")]
    public async Task<string> StartContainer(string name)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        string message = containrBotApiService.StartContainers(name, out bool _).Trim('"');

        await Context.Interaction.SendFollowupMessageAsync(message);

        return message;
    }

    [SubSlashCommand("stop", "Stop a container")]
    public async Task<string> StopContainer(string name)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        string message = containrBotApiService.StopContainers(name, out bool _).Trim('"');

        await Context.Interaction.SendFollowupMessageAsync(message);

        return message;
    }
}
