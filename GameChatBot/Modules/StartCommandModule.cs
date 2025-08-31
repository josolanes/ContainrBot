using NetCord.Services.ApplicationCommands;

namespace GameChatBot.Modules;

[SlashCommand("game", "Game commands!")]
public class GameCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SubSlashCommand("list", "List existing games")]
    public string Games() => $"Games: {Context.Guild!.Channels.Count}";

    [SubSlashCommand("start", "Start game")]
    public string StartGame(string name)
    {
        return $"Started: {name}";
    }

    [SubSlashCommand("stop", "Start game")]
    public string StopGame(string name)
    {
        return $"Stopped: {name}";
    }
}
