namespace GameChatBot.Services;

public interface IGameServerApiService
{
    public string StartGame(string name, out bool success);

    public string StopGame(string name, out bool success);

    public string ListGames(out bool success);
}
