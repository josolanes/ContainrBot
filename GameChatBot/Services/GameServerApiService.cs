namespace GameChatBot.Services;

public class GameServerApiService(
    IConfiguration configuration,
    HttpClient httpClient) : IGameServerApiService
{
    public string ListGames(out bool success)
    {
        var baseUrl = configuration.GetValue<string>("GAMESERVERAPI_BASEURL");
        var url = $"{baseUrl}/list";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }

    public string StartGame(string name, out bool success)
    {
        var baseUrl = configuration.GetValue<string>("GAMESERVERAPI_BASEURL");
        var url = $"{baseUrl}/start/{name}";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }

    public string StopGame(string name, out bool success)
    {
        var baseUrl = configuration.GetValue<string>("GAMESERVERAPI_BASEURL");
        var url = $"{baseUrl}/stop/{name}";

        httpClient.Timeout = TimeSpan.FromSeconds(60);

        HttpResponseMessage response = httpClient.GetAsync(url).Result;

        success = response.IsSuccessStatusCode;

        return response.Content.ReadAsStringAsync().Result;
    }
}
