using System.Text.Json;
using System.Text.Json.Serialization;
using GameChatBot.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace GameChatBot.Modules;

[SlashCommand("game", "Game commands!")]
public class GameCommandModule(
    IGameServerApiService gameServerApiService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("list", "List of available games")]
    public async Task<string> ListGames()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        var output = gameServerApiService.ListGames(out bool status);

        string message = status ? string.Join("\n", JsonSerializer.Deserialize<List<string>>(output) ?? []) : $"Unable to retrieve the games list: {output}";

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(message));

        return message;
    }

    [SubSlashCommand("start", "Start a game")]
    public async Task<string> StartGame(string name)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        string message = gameServerApiService.StartGame(name, out bool _).Trim('"');

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(message));

        return message;
    }

    [SubSlashCommand("stop", "Stop a game")]
    public async Task<string> StopGame(string name)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("Working on it..."));

        string message = gameServerApiService.StopGame(name, out bool _).Trim('"');

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(message));

        return message;
    }
}
